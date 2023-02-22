using med_game.src.Core.IRepository;
using med_game.src.Core.IService;
using med_game.src.Data;
using med_game.src.Entities;
using med_game.src.Entities.Request;
using med_game.src.Models;
using med_game.src.Repository;
using med_game.src.Service;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> CreateQuestion(RequestedQuestionBody questionBody)
        {
            Module? module = await _moduleRepository.GetAsync(questionBody.LecternName, questionBody.ModuleName);
            if (module == null)
                return NotFound("Module not found");

            var result = await _questionService.AddAsync(questionBody.ToQuestionBody(), module);
            return result == null ? Conflict() : Ok();
        }


        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> RemoveQuestion(RemovableQuestionBody questionBody)
        {
            Module? module = await _moduleRepository.GetAsync(questionBody.LecternName, questionBody.ModuleName);
            if (module == null)
                return NotFound("Module not found");

            QuestionProperties questionProperties = new QuestionProperties
            {
                Description = questionBody.Description,
                Image = questionBody.Image,
                Text = questionBody.Text,
                Type = questionBody.TypeQuestion
            };

            var result = await _questionRepository.DeleteAsync(questionProperties, module);
            return result == false ? NotFound() : NoContent();
        }
    }
}
