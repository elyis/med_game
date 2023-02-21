using med_game.src.Entities;
using med_game.src.Models;

namespace med_game.src.Core.IRepository
{
    public interface IAnswerRepository : IDisposable
    {
        Task<Answer?> AddAsync(AnswerOption answer);
        Task<IEnumerable<Answer>> AddRange(List<AnswerOption> answers);
        Task<Answer?> GetAsync(long id);
        Task<Answer?> GetAsync(AnswerOption answer);
        Task<bool> DeleteAsync(long id);
        Task<bool> DeleteAsync(AnswerOption answer);
        Task<IEnumerable<Answer>> GetAllAsync(List<AnswerOption> answerOptions);
    }
}
