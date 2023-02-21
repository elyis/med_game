using med_game.src.Entities;
using med_game.src.Models;

namespace med_game.src.Core.IRepository
{
    public interface IQuestionRepository : IDisposable
    {
        Task<Question?> AddAsync(QuestionBody questionBody, Module module, List<Answer> answers, long rightAnswerIndex);
        Task<Question?> GetAsync(long id);
        Task<Question?> GetAsync(QuestionProperties questionProperties, Module module);
        Task<bool> DeleteAsync(long id);    
        Task<bool> DeleteAsync(QuestionProperties questionProperties, Module module);
        IEnumerable<Question> GetAllAsync();
    }
}
