using med_game.src.Domain.Entities.Shared;
using med_game.src.Domain.Models;

namespace med_game.src.Domain.IRepository
{
    public interface IAnswerRepository
    {
        Task<IEnumerable<AnswerModel>> AddRangeAsync(List<AnswerOption> answers);
        Task<AnswerModel?> GetAsync(long id);
        Task<bool> DeleteAsync(long id);
    }
}