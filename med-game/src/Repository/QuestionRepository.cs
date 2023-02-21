using med_game.src.Core.IRepository;
using med_game.src.Data;
using med_game.src.Entities;
using med_game.src.Models;
using Microsoft.EntityFrameworkCore;

namespace med_game.src.Repository
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly AppDbContext _context;
        public QuestionRepository(AppDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<Question?> AddAsync(QuestionBody questionBody, Module module, List<Answer> answers, long rightAnswerId)
        {
            QuestionProperties questionProperties = new()
            {
                Description = questionBody.Description,
                Image = questionBody.Image,
                Text = questionBody.Text,
                Type = questionBody.Type
            };

            var question = await GetAsync(questionProperties, module);
            if (question != null)
                return null;

            Question model = new()
            {
                Description = questionBody.Description,
                Image = questionBody.Image,
                Text = questionBody.Text,
                Type = Enum.GetName(typeof(TypeQuestion), questionBody.Type)!,
                TimeSeconds = questionBody.TimeSeconds,
                CountPointsPerAnswer = questionBody.NumOfPointsPerAnswer,   
                CorrectAnswerIndex = rightAnswerId
            };

            var result = await _context.Questions.AddAsync(model);
            model.Answers = answers;
            model.Module = module;

            _context.SaveChanges();
            return result.Entity;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var question = await GetAsync(id);
            if (question == null)
                return false;

            _context.Questions.Remove(question);
            _context.SaveChanges();
            return true;
        }

        public async Task<bool> DeleteAsync(QuestionProperties questionProperties, Module module)
        {
            var question = await GetAsync(questionProperties, module);
            if (question == null)
                return false;

            _context.Questions.Remove(question);
            _context.SaveChanges();
            return true;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Question> GetAllAsync()
            => _context.Questions;

        public async Task<Question?> GetAsync(long id)
            => await _context.Questions.FindAsync(id);

        public async Task<Question?> GetAsync(QuestionProperties questionProperties, Module module)
            => await _context.Questions.FirstOrDefaultAsync(q => 
                    q.Module == module && 
                    q.Type == Enum.GetName(typeof(TypeQuestion), questionProperties.Type) &&
                    q.Image == questionProperties.Image && 
                    q.Text == questionProperties.Text &&
                    q.Description == questionProperties.Description
                );
    }
}
