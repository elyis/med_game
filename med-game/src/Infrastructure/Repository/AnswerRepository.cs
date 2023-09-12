using med_game.src.Domain.Entities.Shared;
using med_game.src.Domain.IRepository;
using med_game.src.Domain.Models;
using med_game.src.Infrastructure.Data;

namespace med_game.src.Repository
{
    public class AnswerRepository : IAnswerRepository
    {
        private readonly AppDbContext _context;

        public AnswerRepository(AppDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<IEnumerable<AnswerModel>> AddRangeAsync(List<AnswerOption> answerOptions)
        {
            var answers = answerOptions.Select(answer => answer.ToAnswerModel());
            await _context.Answers.AddRangeAsync(answers);
            await _context.SaveChangesAsync();
            return answers;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var answer = await GetAsync(id);
            if (answer == null)
                return false;

            _context.Answers.Remove(answer);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<AnswerModel?> GetAsync(long id)
            => await _context.Answers
                .FindAsync(id);
    }

}
