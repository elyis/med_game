using med_game.src.Core.IRepository;
using med_game.src.Data;
using med_game.src.Entities.Request;
using med_game.src.Entities.Response;
using med_game.src.Models;
using med_game.src.Repository;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace med_game.src.Entities
{
    public class GamingLobby
    {
        public GamingLobby(RoomSettings roomSettings, int countQuestions = 10)
        {
            RoomSettings = roomSettings;
            CountQuestions = countQuestions;
        }

        public string Id { get; } = Guid.NewGuid().ToString();
        public RoomSettings RoomSettings { get; }
        public int CountQuestions { get; private set; }
        public int CurrentQuestionIndex { get; private set; }
        private int MaxPoints { get; set; }

        public int isLobbyRunning = 0;
        public int isEveryoneAnswer = 0;
        public int isAnsweredCorrectlyFirst = 0;
        public int isManaged = 0;
        private int countPointsForRightAnswer = 1;
        public int countAnswering = 0;


        public readonly Dictionary<long, GameStatisticInfo> PlayerInfo = new Dictionary<long, GameStatisticInfo>();
        public ConcurrentDictionary<long, GameSession> Players { get; private set; } = new();
        private List<Question> Questions { get; set; }

        public bool AddPlayer(long userId, WebSocket webSocket)
        {
            var gameStatistic = PlayerInfo[userId];
            if(gameStatistic != null)
            {
                GameSession session = new GameSession(webSocket, new GameStatistics(gameStatistic.Nickname, gameStatistic.Image));
                return Players.TryAdd(userId, session);
            }
            return false;
        }

        public void AddPlayerInfo(long userId, GameStatisticInfo statisticInfo)
            => PlayerInfo.Add(userId, statisticInfo);

        public bool GenerateQuestion()
        {
            IQuestionRepository questionRepository = new QuestionRepository(new AppDbContext());
            var questions = questionRepository.GenerateRandomQuestions(RoomSettings.LecternId, RoomSettings.ModuleId, CountQuestions);
            if(questions ==  null)
                return false;

            Questions = questions.ToList();
            return true;
        }

        public async Task ProcessLoop()
        {

        }

        public async Task Start()
        {
            MaxPoints = Questions.Select(e => e.CountPointsPerAnswer).Sum();
            foreach(var player in Players)
                player.Value.Statistics.MaxPoints = MaxPoints;

            await SendStateGameAndQuestionToEveryone();
            isLobbyRunning = 1;

        }

        public async Task SendStateGameAndQuestionToEveryone()
        {
            if(CurrentQuestionIndex == CountQuestions)
            {
                var winner = Players.MaxBy(player => player.Value.Statistics.CountPoints);
                await SendStateGameToEveryone(PlayerInfo[winner.Key].Nickname);
                await CloseAll(WebSocketCloseStatus.NormalClosure, $"Questions {CurrentQuestionIndex}/{CountQuestions}");
            }
            else
            {
                await SendStateGameToEveryone(null);
                await Task.Delay(2000);
                await SendQuestionToEveryone();

                foreach (var player in Players)
                {
                    Interlocked.CompareExchange(ref player.Value.IsPlayerAnswer, 0, 1);
                    player.Value.Statistics.PointGain = 0;
                }

                isAnsweredCorrectlyFirst = 0;
                isEveryoneAnswer = 0;
            }
        }

        public async Task<T?> ReceiveJson<T>(WebSocket webSocket)
        {
            byte[] buffer = new byte[2048];

            WebSocketReceiveResult? result = null;
            MemoryStream memoryStream = new MemoryStream();

            do
            {
                result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                if (result.Count > 0 && result.MessageType == WebSocketMessageType.Text)
                    memoryStream.Write(buffer.ToArray(), 0, result.Count);
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Close frame accepted", CancellationToken.None);
                    return default;
                }

            } while (!result.EndOfMessage && webSocket.State == WebSocketState.Open);

            return JsonConvert.DeserializeObject<T?>(Encoding.UTF8.GetString(memoryStream.ToArray()));
        }

        public async Task SendAll<T> (T jsonMessage)
        {
            string message = JsonConvert.SerializeObject(jsonMessage);
            foreach(var player in Players)
                await player.Value.WebSocket.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task SendQuestionToEveryone()
        {
            QuestionBody question = GetQuestionBody();  
            await SendAll(question);
        }

        public async Task SendStateGameToEveryone(string? winner)
        {
            StateGame stateGame = GetGameStatistics(winner);
            await SendAll(stateGame);
        }

        public async Task CloseAll(WebSocketCloseStatus status, string? description)
        {
            foreach(var player in Players)
            {
                if(player.Value.WebSocket.State == WebSocketState.Open || player.Value.WebSocket.State == WebSocketState.CloseReceived)
                    await player.Value.WebSocket.CloseOutputAsync(status, description, CancellationToken.None);
            }
        }

        public void PlayerAnswer(AnswerOption answer, long userId)
        {
            var question = Questions[CurrentQuestionIndex - 1];
            if (answer.Equals(question.Answers[(int)question.CorrectAnswerIndex].ToAnswerOption()))
            {
                Players[userId].Statistics.PointGain = question.CountPointsPerAnswer;
                if(Interlocked.CompareExchange(ref isAnsweredCorrectlyFirst, 1, 0) == 0)
                    Players[userId].Statistics.PointGain += countPointsForRightAnswer;

                Players[userId].Statistics.CountPoints += Players[userId].Statistics.PointGain;
            }

            Players[userId].IsPlayerAnswer = 1;
            countAnswering++;
        }

        public StateGame GetGameStatistics(string? winner)
        {
            return new StateGame
            {
                IsEndGame = winner != null,
                Statistics = Players.Select(player => player.Value.Statistics).ToList(),
                WinnerName = winner
            };
        }

        public QuestionBody GetQuestionBody()
        {
            var question = Questions[CurrentQuestionIndex];
            CurrentQuestionIndex++;
            return question.ToQuestionBody();
        }
    }
}
