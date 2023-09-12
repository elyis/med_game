using med_game.src.Application.IService;
using med_game.src.Domain.Entities.Request;
using med_game.src.Domain.IRepository;
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
        private readonly IQuestionRepository _questionRepository;
        private readonly ICreateQuestionsService _createQuestionsService;
        private readonly ILogger<QuestionController> _logger;

        public QuestionController(
            IModuleRepository moduleRepository, 
            IQuestionRepository questionRepository,
            ICreateQuestionsService createQuestionsService,
            ILogger<QuestionController> logger)
        {
            _moduleRepository = moduleRepository;
            _questionRepository = questionRepository;
            _createQuestionsService = createQuestionsService;
            _logger = logger;
        }



        [HttpPost]
        [SwaggerOperation("Create a new question")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succesfully created")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Answers do not contain the correct answer / ...")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Module not found")]
        [SwaggerResponse((int)HttpStatusCode.Conflict)]

        public async Task<IActionResult> CreateQuestion(List<RequestedQuestionBody> questionBodies)
        {
            var result = await _createQuestionsService.Invoke(questionBodies);
            return result;
        }


        [HttpDelete]
        [SwaggerOperation(Summary = "Remove question")]
        [SwaggerResponse((int)HttpStatusCode.NoContent, "Successfully removed")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Module not found")]

        public async Task<IActionResult> RemoveQuestion(RemovableQuestionBody questionBody)
        {
            var module = await _moduleRepository.GetAsync(questionBody.LecternName, questionBody.ModuleName);
            if (module == null)
                return NotFound();

            var questionProperties = questionBody.ToQuestionProperties();
            var result = await _questionRepository.DeleteAsync(questionProperties, module);
            return result == false ? NotFound() : NoContent();
        }
    }
}
