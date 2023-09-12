using med_game.src.Domain.Entities.Shared;
using med_game.src.Domain.Models;

namespace med_game.src.Domain.IRepository
{
    public interface IQuestionRepository
    {
        Task<QuestionModel?> AddAsync(QuestionBody questionBody, ModuleModel module, IEnumerable<AnswerModel> answers, long rightAnswerIndex);
        Task<QuestionModel?> GetAsync(long id);
        Task<QuestionModel?> GetAsync(QuestionProperties questionProperties, ModuleModel module);
        Task<bool> DeleteAsync(long id);    
        Task<bool> DeleteAsync(QuestionProperties questionProperties, ModuleModel module);
        List<QuestionModel>? GenerateRandomQuestions(int lecternId, int? moduleId, int countQuestions);
    }
}