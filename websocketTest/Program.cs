using med_game.src.Controllers;
using med_game.src.Entities;
using med_game.src.Entities.Request;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;

Console.ReadLine();

Stopwatch stopwatch = new Stopwatch();
stopwatch.Start();
var gameLobby = new GameLobbyDistributorTesting(4);
await gameLobby.Execute();

stopwatch.Stop();
Console.WriteLine(stopwatch.Elapsed);

Console.ReadLine();



class GameLobbyDistributorTesting
{
    private readonly int _countPlayers;
    private readonly List<AuthController> _authControllers = new List<AuthController>();
    private readonly List<RegistrationBody> _registrationBodies = new List<RegistrationBody>();
    private readonly List<Login> _logins = new List<Login>();
    private readonly List<string> _accessTokens = new List<string>();
    private readonly List<(string email, string roomId)> _results = new List<(string email, string roomId)>();
    private readonly RoomSettingBody _roomSettingBody;
    private readonly Uri uri = new Uri("wss://localhost:7296/main");

    private readonly List<ClientWebSocket> _clients = new List<ClientWebSocket>();

    public GameLobbyDistributorTesting(int countUsers)
    {
        _countPlayers = countUsers;
        _roomSettingBody = new RoomSettingBody
        {
            LecternName = "Anatomy",
            ModuleName = "Остеология",
            Type = TypeBattle.Rating
        };

        Init();
    }

    

    public async Task Execute()
    {
        await GetTokens();
        await GenerateClients();
        string json = JsonConvert.SerializeObject(_roomSettingBody);
        await SendAll(json);
        await ReceiveRoomId();

        foreach (var result in _results)
        {
            Console.WriteLine(result);
        }
    }

    private void Init()
    {
        for (int i = 0; i < _countPlayers; i++)
        {
            var registrationBody = new RegistrationBody
            {
                Email = "testy@" + i,
                Nickname = "testy" + i,
                Password = "password"
            };

            var login = new Login
            {
                Email = registrationBody.Email,
                Password = registrationBody.Password
            };

            var authController = new AuthController();

            _authControllers.Add(authController);
            _logins.Add(login);
            _registrationBodies.Add(registrationBody);
        }
    }

    private async Task GetTokens()
    {
        foreach (var registationBody in _registrationBodies.Select((value, index) => new { value, index }))
        {
            await _authControllers[registationBody.index].SignUp(registationBody.value);
            var result = await _authControllers[registationBody.index].SignIn(_logins[registationBody.index]) as ObjectResult;
            TokenPair tokenPair = result.Value as TokenPair;
            _accessTokens.Add(tokenPair.AccessToken);
        }
    }

    private async Task GenerateClients()
    {
        for(int i = 0; i < _countPlayers; i++)
        {
            ClientWebSocket client = new ClientWebSocket();
            client.Options.SetRequestHeader("Authorization", "Bearer " + _accessTokens[i]);
            await client.ConnectAsync(uri, CancellationToken.None);
            _clients.Add(client);
        }
    }

    private async Task SendAll(string message)
    {
        Console.WriteLine(_clients.Count);  
        foreach(var client in _clients)
        {
            if(client.CloseStatus == null)
            {
                await client.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, CancellationToken.None);
                Console.WriteLine("send");
            }
        }
    }

    private async Task ReceiveRoomId()
    {
        foreach(var login in _logins.Select((value, index) => new {value, index}))
        {
            Console.WriteLine($"{login.value.Email} start receive");
            byte[] bytes = new byte[2048];
            await _clients[login.index].ReceiveAsync(bytes, CancellationToken.None);
            await _clients[login.index].CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);

            string roomId = Encoding.UTF8.GetString(bytes);
            _results.Add((login.value.Email, roomId));
        }
    }
}