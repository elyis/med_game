using med_game.src.Core.IRepository;
using med_game.src.Data;
using med_game.src.Entities;
using med_game.src.Entities.Request;
using med_game.src.Models;
using med_game.src.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace med_game.src.Controllers
{
    [Route("question")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IModuleRepository _moduleRepository;
        private readonly IAnswerRepository _answerRepository;
        private readonly IQuestionRepository _questionRepository;

        private readonly ILogger _logger;

        public QuestionController(ILoggerFactory loggerFactory)
        {
            AppDbContext context = new AppDbContext();

            _moduleRepository = new ModuleRepository(context);
            _answerRepository = new AnswerRepository(context);
            _questionRepository = new QuestionRepository(context);
            _logger = loggerFactory.CreateLogger<QuestionController>();
        }



        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> CreateQuestion(RequestedQuestionBody questionBody)
        {
            Module? module = await _moduleRepository.GetAsync(questionBody.LecternName, questionBody.ModuleName);
            if (module == null)
                return NotFound();

            QuestionProperties questionProperties = questionBody.ToQuestionProperties();
            var questionIsExist = await _questionRepository.GetAsync(questionProperties, module);
            if (questionIsExist != null)
                return Conflict();

            if (questionBody.ListOfAnswers.FindIndex(q => q.Equals(questionBody.RightAnswer)) == -1)
                return BadRequest();

            var answers = await _answerRepository.AddRange(questionBody.ListOfAnswers);
            List<AnswerOption> answerList = answers.Select(answer => answer.ToAnswerOption()).ToList();

            var rightAnswerIndex = answerList.FindIndex(q => q.Equals(questionBody.RightAnswer));
            if (rightAnswerIndex == -1)
                return BadRequest();

            var question = await _questionRepository.AddAsync(questionBody.ToQuestionBody(), module, answers.ToList(), rightAnswerIndex);
            return question == null ? Conflict() : Ok();
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
