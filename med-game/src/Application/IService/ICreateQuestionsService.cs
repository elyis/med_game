using med_game.src.Domain.Entities.Request;
using Microsoft.AspNetCore.Mvc;

namespace med_game.src.Application.IService
{
    public interface ICreateQuestionsService
    {
        Task<IActionResult> Invoke(List<RequestedQuestionBody> questionBodies);
    }
}