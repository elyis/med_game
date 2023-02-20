using med_game.src.Entities;
using med_game.src.Models;

namespace med_game.src.Core.IRepository
{
    public interface ILecternRepository : IDisposable
    {
        Task<Lectern?> CreateAsync(LecternBody lecternBody);
        Task<IEnumerable<Lectern>> CreateRangeAsync(IEnumerable<LecternBody> lecternBodies);
        Task<bool> RemoveAsync(int id);
        Task<bool> RemoveAsync(string name);
        Task<Lectern?> GetAsync(int id);
        Task<Lectern?> GetAsync(string name);
        Task<Lectern?> GetWithModulesAsync(int id);
        Task<Lectern?> GetWithModulesAsync(string name);

        IEnumerable<Lectern> GetAll();
        IEnumerable<Lectern> GetAll(string pattern);
    }
}
