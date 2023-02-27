using med_game.Models;
using med_game.src.Core.IRepository;
using med_game.src.Core.IService;
using med_game.src.Entities;

namespace med_game.src.Service
{
    public class AchievementService : IAchievementService
    {
        private readonly IAchievementRepository _achivementRepository;
        private readonly IUserRepository _userRepository;

        public AchievementService(IAchievementRepository achivementRepository, IUserRepository userRepository)
        {
            _achivementRepository = achivementRepository;
            _userRepository = userRepository;
        }

        public async Task<Achievement?> AddAsync(AchievementBody achievementBody)
        {
            var achievement = await _achivementRepository.AddAsync(achievementBody);
            if (achievement != null)
                await _userRepository.AddAchievementToEveryone(achievement);

            return achievement;
        }
    }
}
