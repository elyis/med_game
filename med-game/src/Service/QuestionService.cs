using med_game.src.Core.IRepository;
using med_game.src.Core.IService;
using med_game.src.Entities;
using med_game.src.Models;

namespace med_game.src.Service
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IAnswerRepository _answerRepository;

        public QuestionService( IAnswerRepository answerRepository, 
                                IQuestionRepository questionRepository
            )
        {
            _answerRepository = answerRepository;
            _questionRepository = questionRepository;
        }

        public async Task<Question?> AddAsync(QuestionBody questionBody, Module module)
        {
            QuestionProperties questionProperties = new QuestionProperties 
            { 
                Description = questionBody.description, 
                Image = questionBody.image, 
                Text = questionBody.text, 
                Type = questionBody.type
            };

            var questionIsExist = await _questionRepository.GetAsync(questionProperties, module);
            if (questionIsExist != null)
                return null;

            var answers = await _answerRepository.AddRange(questionBody.answers);
            List<AnswerOption> answerList = answers.Select(answer => answer.ToAnswerOption()).ToList();

            var rightAnswerIndex = answerList.IndexOf(questionBody.rightAnswer);
            if(rightAnswerIndex == -1)
                return null;

            var question = await _questionRepository.AddAsync(questionBody, module, answers.ToList(), rightAnswerIndex);
            return question;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
