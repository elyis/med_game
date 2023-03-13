using med_game.src.Core.IRepository;
using med_game.src.Data;
using med_game.src.Models;
using med_game.src.Repository;
using System.Collections.Concurrent;
using System.Net.WebSockets;

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
        public int CurrentQuestion { get; private set; }
        public bool IsLobbyRunning { get; private set; }
        public readonly List<long> PlayerIds = new List<long>();

        public ConcurrentDictionary<long, WebSocket> Players { get; private set; } = new();
        private List<Question> Questions { get; set; }

        public bool AddPlayer(long userId, WebSocket ws)
            => Players.TryAdd(userId, ws);

        public void AddPlayerId(long userId)
            => PlayerIds.Add(userId);

        public bool GenerateQuestion()
        {
            AppDbContext context = new AppDbContext();
            IQuestionRepository questionRepository = new QuestionRepository(context);
            var questions = questionRepository.GenerateRandomQuestions(RoomSettings.LecternId, RoomSettings.ModuleId, CountQuestions);
            if(questions ==  null)
                return false;

            Questions = questions.ToList();
            return true;
        }
    }
}
