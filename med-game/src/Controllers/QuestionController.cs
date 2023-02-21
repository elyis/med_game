using med_game.src.Core.IRepository;
using med_game.src.Core.IService;
using med_game.src.Data;
using med_game.src.Entities.Request;
using med_game.src.Models;
using med_game.src.Repository;
using med_game.src.Service;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net;
using System.Runtime.CompilerServices;

namespace med_game.src.Controllers
{
    [Route("question")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        private readonly IModuleRepository _moduleRepository;
        private readonly IAnswerRepository _answerRepository;
        private readonly IQuestionRepository _questionRepository;

        public QuestionController()
        {
            AppDbContext context = new AppDbContext();

            _moduleRepository = new ModuleRepository(context);
            _answerRepository = new AnswerRepository(context);
            _questionRepository = new QuestionRepository(context);


            _questionService = new QuestionService(_answerRepository, _questionRepository);

        }

        [HttpPost]
        [ProducesResponseType(typeof(Question), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateQuestion(RequestedQuestionBody questionBody)
        {
            Module? module = await _moduleRepository.GetAsync(questionBody.LecternName, questionBody.ModuleName);
            if (module == null)
                return NotFound("Module not found");

            var result = await _questionService.AddAsync(questionBody.ToQuestionBody(), module);
            return result == null ? Conflict() : Ok();
        }
    }
}
