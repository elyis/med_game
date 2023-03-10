using System.Net.WebSockets;
using Xunit;

namespace test.Controller
{
    public class GameLobbyDistributorController
    {
        [Fact]
        public async Task LobbyDistributorTesting()
        {
            using var ws = new ClientWebSocket();
            Uri uri = new Uri("wss://localhost:7296/main");

            await ws.ConnectAsync(uri, CancellationToken.None);
        }
    }
}
