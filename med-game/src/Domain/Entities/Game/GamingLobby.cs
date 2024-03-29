using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using med_game.src.Domain.Entities.Response;
using med_game.src.Domain.Entities.Shared;
using med_game.src.Domain.Enums;
using med_game.src.Domain.IRepository;
using med_game.src.Domain.Models;
using med_game.src.Infrastructure.Data;
using med_game.src.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace med_game.src.Domain.Entities.Game
{
    public class GamingLobby
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger _logger;

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
        private List<QuestionModel> Questions { get; set; }

        public GamingLobby(RoomSettings roomSettings, ILogger logger, int countQuestions = 2)
        {
            RoomSettings = roomSettings;
            CountQuestions = countQuestions;

            Random rnd = new();
            countPointsForWin = rnd.Next(8, 12);
            countPointsForLose = -rnd.Next(3, 6);

            _userRepository = new UserRepository(new AppDbContext(new DbContextOptions<AppDbContext>()));
            _logger = logger;
        }

        public bool AddPlayer(long userId, WebSocket webSocket)
        {
            var gameStatistic = PlayerInfo[userId];
            if (gameStatistic != null)
            {
                var session = new GameSession(webSocket, new GameStatistics(gameStatistic.Nickname, gameStatistic.Image));
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
            IQuestionRepository questionRepository = new QuestionRepository(new AppDbContext(new DbContextOptions<AppDbContext>()));
            var questions = questionRepository.GenerateRandomQuestions(RoomSettings.LecternId, RoomSettings.ModuleId, CountQuestions);
            if (questions == null)
                return false;

            Questions = questions.ToList();
            return true;
        }

        public async Task ProcessLoop(WebSocket webSocket, long userId)
        {
            await ArePlayersJoined();

            while(isLobbyRunning == 0)
                await Task.Delay(1000);

            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    try
                    {
                        if (Players[userId].IsPlayerAnswer == 0)
                        {
                            var readAnswerTask = ReceiveJson<AnswerOption>(webSocket, CancellationToken.None);
                            var timeoutForReadAnswerTask = Task.Delay(Questions[CurrentQuestionIndex - 1].TimeSeconds * 1000);
                            var firstCompletedTask = await Task.WhenAny(readAnswerTask, timeoutForReadAnswerTask);
                            

                            if (readAnswerTask.IsFaulted)
                                throw new WebSocketException(WebSocketError.ConnectionClosedPrematurely);

                            if (firstCompletedTask == readAnswerTask)
                            {
                                PlayerAnswer(await readAnswerTask, userId);
                                await ArePlayersAnswered();
                            }
                            else
                            {
                                _logger.LogInformation($"Timeout for answering is expired");
                                PlayerAnswer(null, userId);
                                await ArePlayersAnswered();
                            }
                        }
                        else
                            await Task.Delay(2 * 1000);
                    }
                    catch (WebSocketException ex)
                    {
                        await ConsiderDefeat(userId);
                        _logger.LogCritical($"\"{PlayerInfo[userId].Nickname}\" get {ex.Message}");
                        _logger.LogCritical($"\"{PlayerInfo[userId].Nickname}\" get {ex.WebSocketErrorCode}");
                    }
                    catch(Exception ex)
                    {
                        await CloseAll(WebSocketCloseStatus.InternalServerError, ex.Message);
                    }
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
                    isLobbyRunning = 0;
                }
                else
                    await Task.Delay(1000);

                RemovePlayer(userId);
                if (Players.Count <= 1)
                    GlobalVariables.GamingLobbies.Remove(Id, out _);
            }
        }

        private async Task ArePlayersAnswered()
        {
            if(countAnswering == Players.Count && Interlocked.CompareExchange(ref isManaged, 1, 0) == 0)
            {
                await SendStateGameAndQuestionToEveryone();
                return;
            }
            await Task.Delay(2000);
        }

        private async Task ArePlayersJoined()
        {
            if (Players.Count == PlayerInfo.Count)
            {
                if (Interlocked.CompareExchange(ref isManaged, 1, 0) == 0)
                    await Start();
            }
        }

        private async Task Start()
        {
            MaxPointsByQuestions = Questions.Select(e => e.CountPointsPerAnswer).Sum() + CountQuestions;
            foreach (var player in Players)
                player.Value.Statistics.MaxPointsGame = MaxPointsByQuestions;

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
                await Task.Delay(2100);
                await SendQuestionToEveryone();

                foreach (var player in Players)
                {
                    Interlocked.Exchange(ref player.Value.IsPlayerAnswer, 0);
                    player.Value.Statistics.PointGain = 0;
                }

                isAnsweredCorrectlyFirst = 0;
            }
            countAnswering = 0;
            isManaged = 0;
        }

        private async Task<T> ReceiveJson<T>(WebSocket webSocket, CancellationToken token)
        {
            byte[] bytes = new byte[2048];
            WebSocketReceiveResult? result = null;
            MemoryStream stream = new MemoryStream();
            
            do
            {
                result = await webSocket.ReceiveAsync(bytes, token);
                if (result.Count > 0 && result.MessageType == WebSocketMessageType.Text)
                    stream.Write(bytes.ToArray(), 0, result.Count);
                else if (result.MessageType == WebSocketMessageType.Close)
                    await CloseAll(WebSocketCloseStatus.NormalClosure, "Close frame accepter");

            } while (!result.EndOfMessage && webSocket.State == WebSocketState.Open);

            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(stream.ToArray()));
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

        private void PlayerAnswer(AnswerOption? answer, long userId)
        {
            var question = Questions[CurrentQuestionIndex - 1];
            if (answer?.Equals(question.Answers[(int)question.CorrectAnswerIndex].ToAnswerOptionWithWebPath()) == true)
            {
                Players[userId].Statistics.PointGain = question.CountPointsPerAnswer;
                if (Interlocked.CompareExchange(ref isAnsweredCorrectlyFirst, 1, 0) == 0)
                    Players[userId].Statistics.PointGain += countPointsForRightAnswer;

                Players[userId].Statistics.CountOfPoints += Players[userId].Statistics.PointGain;
            }

            Players[userId].IsPlayerAnswer = 1;
            Interlocked.Increment(ref countAnswering);
        }

        private StateGame GetGameStatistics(string? winner)
        {
            return new StateGame
            {
                IsEndGame = winner != null,
                Rating = Players.Select(player => player.Value.Statistics).ToList(),
                NameWinner = winner
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
            var pointsByWinner = Players.MaxBy(player => player.Value.Statistics.CountOfPoints).Value.Statistics.CountOfPoints;

            var winners = Players.Where(player => 
                    player.Value.Statistics.CountOfPoints == pointsByWinner && 
                    player.Value.WebSocket.State != WebSocketState.Aborted
            );


            var losers = Players.Where(player =>
                    !winners.Contains(player)
            );


            if (RoomSettings.Type == TypeBattle.Rating)
            {
                if(!losers.Any())
                {
                    if(Players.Count == 2)
                    {
                        foreach (var player in winners)
                            await _userRepository.UpdateRating(player.Key, countPointsForWin / 2);
                    }
                    else
                    {
                        foreach (var player in winners)
                            await _userRepository.UpdateRating(player.Key, countPointsForWin);
                    }
                        
                }
                else
                {
                    foreach (var player in winners)
                        await _userRepository.UpdateRating(player.Key, countPointsForWin);
                    foreach (var player in losers)
                        await _userRepository.UpdateRating(player.Key, countPointsForLose);
                }
            }

            return winners.First().Value.Statistics.Nickname;
        }
    }
}