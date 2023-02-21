using med_game.src.Core.IRepository;
using med_game.src.Data;
using med_game.src.Entities;
using med_game.src.Models;
using Microsoft.EntityFrameworkCore;

namespace med_game.src.Repository
{
    public class AnswerRepository : IAnswerRepository
    {
        private readonly AppDbContext _context;

        public AnswerRepository(AppDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<Answer?> AddAsync(AnswerOption answerOption)
        {
            Answer? answer = await GetAsync(answerOption);
            if (answer != null)
                return null;

            Answer model = new()
            {
                Description = answerOption.Text,
                Image = answerOption.Image,
                Type = Enum.GetName(typeof(AnswerOption), answerOption.Type)!
            };

            var result = await _context.Answers.AddAsync(model);
            _context.SaveChanges();
            return result.Entity;
        }

        public async Task<IEnumerable<Answer>> AddRange(List<AnswerOption> answerOptions)
        {
            List<Answer> answers = new List<Answer>();

            var answersInDb = await GetAllAsync(answerOptions);

            var answersNotInDb = answerOptions
                .Except(answersInDb.Select(a => a.ToAnswerOption()))
                .ToList();
            
            foreach(var answer in answersNotInDb)
            {
                Answer model = new()
                {
                    Description = answer.Text,
                    Image = answer.Image,
                    Type = Enum.GetName(typeof(TypeAnswer), answer.Type)!
                };

                answers.Add(model);
            }

            _context.Answers.AddRange(answers);
            answers.AddRange(answersInDb);
            _context.SaveChanges();

            return answers;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            Answer? answer = await GetAsync(id);
            if (answer == null)
                return false;

            _context.Answers.Remove(answer);
            _context.SaveChanges();
            return true;
        }

        public async Task<bool> DeleteAsync(AnswerOption answerOption)
        {
            Answer? answer = await GetAsync(answerOption);
            if (answer == null)
                return false;

            _context.Answers.Remove(answer);
            _context.SaveChanges();
            return true;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Answer>> GetAllAsync(List<AnswerOption> answerOptions)
        {
            List<Answer> answers = new List<Answer>();
            foreach(var answerOption in answerOptions)
            {
                Answer? answer = await GetAsync(answerOption);
                if(answer != null)
                    answers.Add(answer);
            }

            return answers;
        }

        public async Task<Answer?> GetAsync(long id)
            => await _context.Answers
                .FindAsync(id);

        public async Task<Answer?> GetAsync(AnswerOption answer)
            => await _context.Answers
                .FirstOrDefaultAsync(a => 
                    a.Description == answer.Text && 
                    a.Type == Enum.GetName(typeof(TypeAnswer), answer.Type) && 
                    a.Image == answer.Image);


    }
}
