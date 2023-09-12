using med_game.src.Domain.Entities.Shared;
using med_game.src.Domain.Models;

namespace med_game.src.Domain.IRepository
{
    public interface ILecternRepository
    {
        Task<LecternModel?> CreateAsync(LecternBody lecternBody);
        Task AddRangeAsync(IEnumerable<LecternBody> lecternBodies);
        Task<bool> RemoveAsync(int id);
        Task<bool> RemoveAsync(string name);
        Task<LecternModel?> GetAsync(int id);
        Task<LecternModel?> GetAsync(string name);
        Task<LecternModel?> GetWithModulesAsync(int id);
        Task<LecternModel?> GetWithModulesAsync(string name);

        Task<IEnumerable<LecternModel>> GetAllAsync();
        Task<IEnumerable<LecternModel>> GetAllAsync(string pattern);
    }
}