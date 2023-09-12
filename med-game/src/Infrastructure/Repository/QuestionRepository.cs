using med_game.src.Domain.Entities.Shared;
using med_game.src.Domain.Enums;
using med_game.src.Domain.IRepository;
using med_game.src.Domain.Models;
using med_game.src.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace med_game.src.Infrastructure.Repository
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly AppDbContext _context;
        public QuestionRepository(AppDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<QuestionModel?> AddAsync(
            QuestionBody questionBody, 
            ModuleModel module, 
            IEnumerable<AnswerModel> answers, 
            long rightAnswerId)
        {
            QuestionProperties questionProperties = new()
            {
                Description = questionBody.description,
                Image = questionBody.image,
                Text = questionBody.text,
                Type = questionBody.type
            };

            var question = await GetAsync(questionProperties, module);
            if (question != null)
                return null;

            QuestionModel model = new()
            {
                Description = questionBody.description,
                Image = questionBody.image,
                Text = questionBody.text,
                Type = Enum.GetName(typeof(TypeQuestion), questionBody.type)!,
                TimeSeconds = questionBody.timeSeconds,
                CountPointsPerAnswer = questionBody.numOfPointsPerAnswer,   
                CorrectAnswerIndex = rightAnswerId
            };

            var result = await _context.Questions.AddAsync(model);
            model.Answers = answers.ToList();
            model.Module = module;

            await _context.SaveChangesAsync();
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

        public async Task<bool> DeleteAsync(QuestionProperties questionProperties, ModuleModel module)
        {
            var question = await GetAsync(questionProperties, module);
            if (question == null)
                return false;

            _context.Questions.Remove(question);
            _context.SaveChanges();
            return true;
        }


        public List<QuestionModel>? GenerateRandomQuestions(int lecternId, int? moduleId, int countQuestions)
        {

            var result = new List<QuestionModel>();
            if (moduleId != null)
            {
                var questionsByModule = _context.Questions
                    .Include(q => q.Answers)
                    .Where(q => q.Module.Id == moduleId)
                    .Take(countQuestions)
                    .ToList();
                while (result.Count < countQuestions)
                {
                    var questions = questionsByModule
                        .OrderBy(q => Guid.NewGuid())
                        .Take(countQuestions - result.Count)
                        .ToList();

                    if (!questions.Any())
                        return null;
                    result.AddRange(questions);
                }
                return result;
            }

            var lectern = _context.Lecterns.Include(l => l.Modules).ThenInclude(l => l.Questions).ThenInclude(q => q.Answers).First(l => l.Id == lecternId);
            if(lectern.Modules.Count == 0)
                return null;

            int averageCountQuestionsFromModule = (int)Math.Ceiling((double)countQuestions / lectern.Modules.Count);

            while(result.Count < countQuestions)
            {
                foreach (var module in lectern.Modules)
                {
                    var randomQuestions = module.Questions.AsEnumerable().OrderBy(q => Guid.NewGuid()).Take(averageCountQuestionsFromModule);
                    result.AddRange(randomQuestions);

                    if (result.Count >= countQuestions)
                        break;
                }
            }
            return result;
        }

        public async Task<QuestionModel?> GetAsync(long id)
            => await _context.Questions.FindAsync(id);

        public async Task<QuestionModel?> GetAsync(QuestionProperties questionProperties, ModuleModel module)
            => await _context.Questions.FirstOrDefaultAsync(q => 
                    q.Module == module && 
                    q.Type == Enum.GetName(typeof(TypeQuestion), questionProperties.Type) &&
                    q.Image == questionProperties.Image && 
                    q.Text == questionProperties.Text &&
                    q.Description == questionProperties.Description
                );
    }
}