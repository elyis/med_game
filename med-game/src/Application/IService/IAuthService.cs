using med_game.src.Domain.Entities.Request;
using med_game.src.Domain.Entities.Shared;
using med_game.src.Domain.Enums;
using med_game.src.Domain.Models;

namespace med_game.src.Application.IService
{
    public interface IAuthService
    {
        Task<TokenPair?> RegisterAsync(SignUpBody user, Roles role = Roles.User);
        Task<TokenPair?> LoginAsync(UserModel user);
        Task<TokenPair?> UpdateTokenAsync(string refreshToken);
    }
}