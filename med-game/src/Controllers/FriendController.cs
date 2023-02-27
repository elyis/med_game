using med_game.src.Core.IRepository;
using med_game.src.Data;
using med_game.src.Managers;
using med_game.src.Models;
using med_game.src.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace med_game.src.Controllers
{
    [Route("friendReq")]
    [ApiController]
    public class FriendController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly JwtManager _jwtManager;

        public FriendController()
        {
            _context = new AppDbContext();
            _userRepository = new UserRepository(_context);
            _jwtManager = new JwtManager();
        }

        [HttpPost("{email}")]
        [Authorize]
        public async Task CreateAuthor(string email)
        {
            string token = Request.Headers["Authorization"]!;
            token = token.Replace("Bearer ", string.Empty);

            string? subEmail = _jwtManager.GetClaimsFromJwt(token)
                .FirstOrDefault(e => e.Type == ClaimTypes.Email)
                ?.Value;

            if (subEmail == null)
                return;

            User? author = await _userRepository.GetAsync(email);
            if(author == null) 
                return ;

            User? subscriber = await _userRepository.GetAsync(subEmail);
            if(subscriber == null)
                return ;

            if(subscriber.Id == author.Id)
                return;

            Friend friend = new Friend { Subscriber = subscriber };
            author.Friends.Add(friend);
            _context.SaveChanges();

            User user= _context.Set<User>().Include(e => e.Friends).First(a => a.Id == author.Id);
            JsonSerializerOptions options = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.IgnoreCycles, WriteIndented = true };
            await Response.WriteAsJsonAsync(user, options);

        }

        [HttpGet("{email}")]
        public async Task Get(string email)
        {
            var user = _context.Users.Include(u => u.Friends).First(a => a.Email == email);
            var result = user?.Friends.ToList();
            JsonSerializerOptions options = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.IgnoreCycles, WriteIndented = true };
            await Response.WriteAsJsonAsync(result, options);
        }


    }
}
