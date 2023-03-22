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
                Description = answerOption.text,
                Image = answerOption.image,
                Type = Enum.GetName(typeof(AnswerOption), answerOption.type)!
            };

            var result = await _context.Answers.AddAsync(model);
            _context.SaveChanges();
            return result.Entity;
        }

        public async Task<IEnumerable<Answer>> AddRange(List<AnswerOption> answerOptions)
        {
            List<Answer> answers = new List<Answer>();

            var answersInDb = await GetAllAsync(answerOptions);
            var temp = answersInDb.Select(x => x.ToAnswerOption());

            var answersNotInDb = answerOptions.Where(a => !temp.Contains(a))
                .ToList();
            
            foreach(var answer in answersNotInDb)
            {
                Answer model = new()
                {
                    Description = answer.text,
                    Image = answer.image,
                    Type = Enum.GetName(typeof(TypeAnswer), answer.type)!
                };

                answers.Add(model);
            }

            _context.Answers.AddRange(answers);
            _context.SaveChanges();
            answers.AddRange(answersInDb);

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
                    a.Description == answer.text && 
                    a.Type == Enum.GetName(typeof(TypeAnswer), answer.type) && 
                    a.Image == answer.image);
    }

}
