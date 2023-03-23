using med_game.src.Core.IRepository;
using med_game.src.Data;
using med_game.src.Entities.Request;
using med_game.src.Entities.Response;
using med_game.src.Models;
using med_game.src.Repository;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Diagnostics.Eventing.Reader;
using System.Net.WebSockets;
using System.Text;

namespace med_game.src.Entities.Game
{
    public class GamingLobby
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger _logger;

        public GamingLobby(RoomSettings roomSettings, ILogger logger, int countQuestions = 3)
        {
            RoomSettings = roomSettings;
            CountQuestions = countQuestions;

            Random rnd = new();
            countPointsForWin = rnd.Next(8, 12);
            countPointsForLose = -rnd.Next(3, 6);

            _userRepository = new UserRepository(new AppDbContext());
            _logger = logger;
        }

        public string Id { get; } = Guid.NewGuid().ToString();
        public RoomSettings RoomSettings { get; }

        public int CountQuestions { get; private set; }
        public int CurrentQuestionIndex { get; private set; }
        private int MaxPointsByQuestions { get; set; }

        //flags
        public int isLobbyRunning = 0;
        private int isAnsweredCorrectlyFirst = 0;
        private int isManaged = 0;
        private int isResultSent = 0;
        private int countAnswering = 0;

        //Rewards for right answer, win, lose
        private readonly int countPointsForRightAnswer = 1;
        private readonly int countPointsForWin;
        private readonly int countPointsForLose;


        public readonly Dictionary<long, GameStatisticInfo> PlayerInfo = new();
        public ConcurrentDictionary<long, GameSession> Players { get; private set; } = new();
        private List<Question> Questions { get; set; }

        public bool AddPlayer(long userId, WebSocket webSocket)
        {
            var gameStatistic = PlayerInfo[userId];
            if (gameStatistic != null)
            {
                GameSession session = new(webSocket, new GameStatistics(gameStatistic.Nickname, gameStatistic.Image));
                return Players.TryAdd(userId, session);
            }
            return false;
        }

        public bool RemovePlayer(long userId)
            => Players.TryRemove(userId, out var _);

        public void AddPlayerInfo(long userId, GameStatisticInfo statisticInfo)
            => PlayerInfo.Add(userId, statisticInfo);

        public bool GenerateQuestion()
        {
            IQuestionRepository questionRepository = new QuestionRepository(new AppDbContext());
            var questions = questionRepository.GenerateRandomQuestions(RoomSettings.LecternId, RoomSettings.ModuleId, CountQuestions);
            if (questions == null)
                return false;

            Questions = questions.ToList();
            return true;
        }

        public async Task ProcessLoop(WebSocket webSocket, long userId)
        {
            if (Players.Count == PlayerInfo.Count)
            {
                if (Interlocked.CompareExchange(ref isManaged, 1, 0) == 0)
                    await Start();
            }


            try
            {
                while (isLobbyRunning == 0)
                    await Task.Delay(1000);

                try
                {
                    while (webSocket.State == WebSocketState.Open && isLobbyRunning == 1)
                    {
                        if (Players[userId].IsPlayerAnswer == 0)
                        {
                            var task = ReceiveJson<AnswerOption>(webSocket);
                            if(await Task.WhenAny(task, Task.Delay(Questions[CurrentQuestionIndex - 1].TimeSeconds * 1000)) == task){
                                PlayerAnswer(await task, userId);

                                if (countAnswering == Players.Count)
                                {
                                    if (Interlocked.CompareExchange(ref isManaged, 1, 0) == 0)
                                    {
                                        await SendStateGameAndQuestionToEveryone();
                                        countAnswering = 0;
                                        Interlocked.Exchange(ref isManaged, 0);
                                    }
                                    else
                                        await Task.Delay(2000);
                                }
                                else
                                    await Task.Delay(2000);
                            }
                            else
                            {
                                _logger.LogCritical($"{task.Status}");

                                _logger.LogCritical($"{Players[userId].Statistics.nickname}: operation cancelled");
                                Interlocked.Increment(ref countAnswering);
                                if (countAnswering == Players.Count)
                                {
                                    if (Interlocked.CompareExchange(ref isManaged, 1, 0) == 0)
                                    {
                                        await SendStateGameAndQuestionToEveryone();
                                        countAnswering = 0;
                                        Interlocked.Exchange(ref isManaged, 0);
                                    }
                                    else
                                        await Task.Delay(2000);
                                }
                                else
                                    await Task.Delay(2000);

                            }
                            
                        }
                        else
                            await Task.Delay(2000);
                    }

                }
                catch (WebSocketException e)
                {
                    await ConsiderDefeat(userId);
                    _logger.LogCritical($"\"{PlayerInfo[userId].Nickname}\" get {e.Message}");
                    _logger.LogCritical($"\"{PlayerInfo[userId].Nickname}\" get {e.WebSocketErrorCode}");
                }
                catch (Exception ex)
                {
                    await CloseAll(WebSocketCloseStatus.InternalServerError, ex.Message);
                }
            }

            catch (Exception ex)
            {
                await webSocket.CloseOutputAsync(WebSocketCloseStatus.InternalServerError, ex.Message, CancellationToken.None);

            }
            finally
            {
                if (Interlocked.CompareExchange(ref isResultSent, 1, 0) == 0)
                {
                    string winner = await GetWinner();
                    await SendStateGameToEveryone(winner);
                }
                else
                    await Task.Delay(500);

                RemovePlayer(userId);
                if (Players.Count <= 1)
                    GlobalVariables.GamingLobbies.Remove(Id, out _);
            }
        }

        private async Task Start()
        {
            MaxPointsByQuestions = Questions.Select(e => e.CountPointsPerAnswer).Sum();
            foreach (var player in Players)
                player.Value.Statistics.maxPointsGame = MaxPointsByQuestions;

            await SendStateGameAndQuestionToEveryone();
            isLobbyRunning = 1;
            isManaged = 0;
        }

        private async Task SendStateGameAndQuestionToEveryone()
        {
            if (CurrentQuestionIndex == CountQuestions)
            {
                string winner = await GetWinner();
                await SendStateGameToEveryone(winner);
            }
            else
            {
                await SendStateGameToEveryone(null);
                await Task.Delay(2000);
                await SendQuestionToEveryone();

                foreach (var player in Players)
                {
                    Interlocked.Exchange(ref player.Value.IsPlayerAnswer, 0);
                    player.Value.Statistics.pointGain = 0;
                }

                isAnsweredCorrectlyFirst = 0;
            }
        }

        private async Task<T?> ReceiveJson<T>(WebSocket webSocket)
        {
            byte[] buffer = new byte[2048];

            WebSocketReceiveResult? result = null;
            MemoryStream memoryStream = new();
            
            do
            {
                result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                if (result.Count > 0 && result.MessageType == WebSocketMessageType.Text)
                    memoryStream.Write(buffer.ToArray(), 0, result.Count);
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await CloseAll(WebSocketCloseStatus.NormalClosure, "Close frame accepter");
                    return default;
                }

            } while (!result.EndOfMessage && webSocket.State == WebSocketState.Open);

            return JsonConvert.DeserializeObject<T?>(Encoding.UTF8.GetString(memoryStream.ToArray()));
        }

        private async Task SendAll<T>(T jsonMessage)
        {
            string message = JsonConvert.SerializeObject(jsonMessage);
            foreach (var player in Players)
                await player.Value.WebSocket.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private async Task SendQuestionToEveryone()
        {
            var question = GetGameQuestion();
            await SendAll(question);
        }

        private async Task SendStateGameToEveryone(string? winner)
        {
            StateGame stateGame = GetGameStatistics(winner);
            await SendAll(stateGame);
            if (winner != null)
            {
                isResultSent = 1;
                await CloseAll(WebSocketCloseStatus.NormalClosure, $"Questions {CurrentQuestionIndex}/{CountQuestions}");
            }

        }

        private async Task CloseAll(WebSocketCloseStatus status, string? description)
        {
            foreach (var player in Players)
            {
                if (player.Value.WebSocket.State == WebSocketState.Open || player.Value.WebSocket.State == WebSocketState.CloseReceived)
                    await player.Value.WebSocket.CloseOutputAsync(status, description, CancellationToken.None);
            }
        }

        private void PlayerAnswer(AnswerOption answer, long userId)
        {
            var question = Questions[CurrentQuestionIndex - 1];
            if (answer.Equals(question.Answers[(int)question.CorrectAnswerIndex].ToAnswerOptionWithWebPath()))
            {
                Players[userId].Statistics.pointGain = question.CountPointsPerAnswer;
                if (Interlocked.CompareExchange(ref isAnsweredCorrectlyFirst, 1, 0) == 0)
                    Players[userId].Statistics.pointGain += countPointsForRightAnswer;

                Players[userId].Statistics.countOfPoints += Players[userId].Statistics.pointGain;
            }

            Players[userId].IsPlayerAnswer = 1;
            Interlocked.Increment(ref countAnswering);
        }

        private StateGame GetGameStatistics(string? winner)
        {
            return new StateGame
            {
                isEndGame = winner != null,
                rating = Players.Select(player => player.Value.Statistics).ToList(),
                nameWinner = winner
            };
        }

        private GameQuestion GetGameQuestion()
        {
            var question = Questions[CurrentQuestionIndex];
            CurrentQuestionIndex++;
            return question.ToGameQuestion();
        }

        /*
         * Рассчитывает очки для отключившихся игроков
         */

        private async Task ConsiderDefeat(long userId)
        {
            Players.Remove(userId, out _);
            await _userRepository.UpdateRating(userId, countPointsForLose);
        }


        /*
         * Рассчитывает очки для не отключившихся игроков
         */
        private async Task<string> GetWinner()
        {
            var pointsByWinner = Players.MaxBy(player => player.Value.Statistics.countOfPoints).Value.Statistics.countOfPoints;

            var winners = Players.Where(player => 
                    player.Value.Statistics.countOfPoints == pointsByWinner && 
                    player.Value.WebSocket.State != WebSocketState.Aborted
            );


            var losers = Players.Where(player =>
                    !winners.Contains(player) &&
                    player.Value.WebSocket.State != WebSocketState.Aborted
            );


            if (RoomSettings.Type == TypeBattle.Rating)
            {
                if (winners.ToHashSet().SetEquals(losers))
                {
                    foreach (var player in winners)
                        await _userRepository.UpdateRating(player.Key, countPointsForWin / 2);
                    foreach(var player in losers)
                        await _userRepository.UpdateRating(player.Key, countPointsForWin / 2);
                }
                else
                {
                    foreach (var player in winners)
                        await _userRepository.UpdateRating(player.Key,  countPointsForWin);
                    foreach (var player in losers)
                        await _userRepository.UpdateRating(player.Key, countPointsForLose);
                }
            }

            return winners.First().Value.Statistics.nickname;
        }
    }
}
