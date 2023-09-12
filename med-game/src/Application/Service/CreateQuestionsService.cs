using med_game.src.Application.IService;
using med_game.src.Domain.Entities.Request;
using med_game.src.Domain.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace med_game.src.Application.Service
{
    public class CreateQuestionsService : ICreateQuestionsService
    {
        private readonly IModuleRepository _moduleRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IAnswerRepository _answerRepository;

        public CreateQuestionsService(
            IModuleRepository moduleRepository, 
            IQuestionRepository questionRepository, 
            IAnswerRepository answerRepository
            )
        {
            _moduleRepository = moduleRepository;
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
        }

        public async Task<IActionResult> Invoke(List<RequestedQuestionBody> questionBodies)
        {
            foreach(var questionBody in questionBodies)
            {
                var module = await _moduleRepository.GetAsync(questionBody.LecternName, questionBody.ModuleName);
                if (module == null)
                    return new NotFoundResult();

                var questionIsExist = await _questionRepository.GetAsync(questionBody.ToQuestionProperties(), module);
                if (questionIsExist != null)
                    continue;

                var rightAnswerIndex = questionBody.ListOfAnswer.FindIndex(q => q.Equals(questionBody.RightAnswer));
                if (rightAnswerIndex == -1)
                    return new BadRequestResult();

                var answers = await _answerRepository.AddRangeAsync(questionBody.ListOfAnswer);
                var question = await _questionRepository.AddAsync(questionBody.ToQuestionBody(), module, answers.ToList(), rightAnswerIndex);
            }
            return new OkResult();
        }
    }
}