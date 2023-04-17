using med_game.src.Core.IRepository;
using med_game.src.Data;
using med_game.src.Entities;
using med_game.src.Entities.Request;
using med_game.src.Models;
using med_game.src.Repository;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
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

        public QuestionController(ILoggerFactory loggerFactory, AppDbContext context)
        {
            _moduleRepository = new ModuleRepository(context);
            _answerRepository = new AnswerRepository(context);
            _questionRepository = new QuestionRepository(context);
            _logger = loggerFactory.CreateLogger<QuestionController>();
        }



        [HttpPost]
        [SwaggerOperation("Create a new question")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succesfully created")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Answers do not contain the correct answer / ...")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Module not found")]
        [SwaggerResponse((int)HttpStatusCode.Conflict)]

        public async Task<IActionResult> CreateQuestion(List<RequestedQuestionBody> questionBodies)
        {
            foreach(var questionBody in questionBodies)
            {
                Module? module = await _moduleRepository.GetAsync(questionBody.LecternName, questionBody.ModuleName);
                if (module == null)
                    return NotFound();

                QuestionProperties questionProperties = questionBody.ToQuestionProperties();
                var questionIsExist = await _questionRepository.GetAsync(questionProperties, module);
                if (questionIsExist != null)
                    continue;

                if (questionBody.ListOfAnswer.FindIndex(q => q.Equals(questionBody.RightAnswer)) == -1)
                    return BadRequest();

                var answers = await _answerRepository.AddRange(questionBody.ListOfAnswer);
                List<AnswerOption> answerList = answers.Select(answer => answer.ToAnswerOption()).ToList();

                var rightAnswerIndex = answerList.FindIndex(q => q.Equals(questionBody.RightAnswer));
                if (rightAnswerIndex == -1)
                    return BadRequest();

                var question = await _questionRepository.AddAsync(questionBody.ToQuestionBody(), module, answers.ToList(), rightAnswerIndex);
            }
            return Ok();
        }


        [HttpDelete]
        [SwaggerOperation(Summary = "Remove question")]
        [SwaggerResponse((int)HttpStatusCode.NoContent, "Successfully removed")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Module not found")]

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
