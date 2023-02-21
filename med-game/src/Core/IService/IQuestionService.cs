using med_game.src.Entities;
using med_game.src.Models;

namespace med_game.src.Core.IService
{
    public interface IQuestionService : IDisposable
    {
        Task<Question?> AddAsync(QuestionBody questionBody, Module module);
    }
}
