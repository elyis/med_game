namespace med_game.src.Application.IService
{
    public interface IGameLobbyService
    {
        Task InvokeAsync(HttpContext context, string? enemyEmail = null);
    }
}