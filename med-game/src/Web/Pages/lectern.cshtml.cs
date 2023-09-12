using med_game.src.Domain.IRepository;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace med_game.src.Pages
{
    public class lectern : PageModel
    {
        private readonly ILogger<lectern> _logger;
        private readonly ILecternRepository _lecternRepository;

        public lectern(ILogger<lectern> logger, ILecternRepository lecternRepository)
        {
            _logger = logger;
            _lecternRepository = lecternRepository;
        }

        public async Task OnGet()
        {
            
        }

        public async Task OnPost(string name, string? description){
            var res = name;
            
        }
    }
}