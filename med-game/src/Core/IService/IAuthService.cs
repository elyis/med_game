using med_game.src.Entities;
using med_game.src.Entities.Request;
using med_game.src.Models;

namespace med_game.src.Core.IService
{
    interface IAuthService
    {
        Task<TokenPair?> RegisterAsync(RegistrationBody user, Roles role = Roles.User);
        Task<TokenPair?> LoginAsync(Login login);
        Task<TokenPair?> UpdateTokenAsync(string refreshToken);
    }
}
