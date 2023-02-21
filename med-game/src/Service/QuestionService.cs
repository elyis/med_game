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
                Description = questionBody.Description, 
                Image = questionBody.Image, 
                Text = questionBody.Text, 
                Type = questionBody.Type
            };

            var questionIsExist = await _questionRepository.GetAsync(questionProperties, module);
            if (questionIsExist != null)
                return null;

            var answers = await _answerRepository.AddRange(questionBody.Answers);
            List<Answer> answerList = answers.ToList();
            var rightAnswerIndex = answerList.FindIndex(a => a.ToAnswerOption() == questionBody.RightAnswer);
            var question = await _questionRepository.AddAsync(questionBody, module, answerList, rightAnswerIndex);
            return question;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
