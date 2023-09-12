using med_game.src.Domain.IRepository;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace med_game.src.Web.Pages
{
    public class module : PageModel
    {
        private readonly ILogger<module> _logger;
        private readonly ILecternRepository _lecternRepository;
        private readonly IModuleRepository _moduleRepository;

        public module(
            ILogger<module> logger, 
            ILecternRepository lecternRepository, 
            IModuleRepository moduleRepository
            )
        {
            _logger = logger;
            _lecternRepository = lecternRepository;
            _moduleRepository = moduleRepository;
        }

        public async Task OnGet(){
            
        }

        
        public async Task OnPost(){

        }
    }
}