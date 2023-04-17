namespace med_game.src.Core.IService
{
    public interface IGameLobbyService
    {
        Task InvokeAsync(HttpContext context, string? enemyEmail = null);
    }
}
