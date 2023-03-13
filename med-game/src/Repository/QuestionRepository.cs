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

        public List<Question>? GenerateRandomQuestions(int lecternId, int? moduleId, int countQuestions)
        {

            List<Question> result = new List<Question>();
            if (moduleId != null)
            {
                while (result.Count < countQuestions)
                {
                    var questions = _context.Questions
                        .Where(q => q.Module.Id == moduleId)
                        .Include(q => q.Answers)
                        .OrderBy(q => EF.Functions.Random())
                        .Take(countQuestions - result.Count);

                    if (questions.Count() == 0)
                        return null;
                    result.AddRange(questions);
                }
                return result;
            }

            Lectern lectern = _context.Lecterns.Include(l => l.Modules).ThenInclude(l => l.Questions).ThenInclude(q => q.Answers).First(l => l.Id == lecternId);
            if(lectern.Modules.Count == 0)
                return null;

            int averageCountQuestionsFromModule = (int)Math.Ceiling((double)countQuestions / lectern.Modules.Count);

            while(result.Count < countQuestions)
            {
                foreach (var module in lectern.Modules)
                {
                    var randomQuestions = module.Questions.OrderBy(q => EF.Functions.Random()).Take(averageCountQuestionsFromModule);
                    result.AddRange(randomQuestions);

                    if (result.Count >= countQuestions)
                        break;
                }
            }
            return result;
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
