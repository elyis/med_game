using med_game.src.Controllers;
using med_game.src.Data;
using med_game.src.Entities;
using med_game.src.Entities.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;

Console.ReadLine();

Stopwatch stopwatch = new Stopwatch();
stopwatch.Start();
var gameLobby = new GameLobbyDistributorTesting(400);
await gameLobby.Execute();

stopwatch.Stop();
Console.WriteLine(stopwatch.Elapsed);

Console.ReadLine();



class GameLobbyDistributorTesting
{
    private readonly string serverUrl = "192.168.101.95:5121";
    private readonly int _countPlayers;
    private readonly List<AuthController> _authControllers = new List<AuthController>();
    private readonly List<RegistrationBody> _registrationBodies = new List<RegistrationBody>();
    private readonly List<Login> _logins = new List<Login>();
    private readonly List<string> _accessTokens = new List<string>();
    private readonly List<(string email, string roomId)> _results = new List<(string email, string roomId)>();
    private readonly RoomSettingBody _roomSettingBody;
    private readonly Uri uriToDistributorLobby;
    private readonly List<Task<(string, string)>> _tasks = new ();

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

        uriToDistributorLobby = new Uri($"ws://{serverUrl}/main");

        Init();
    }

    

    public async Task Execute()
    {
        try
        {
            await GetTokens();
            await GenerateClients();
            string json = JsonConvert.SerializeObject(_roomSettingBody);
            await SendAll(json);
            await ReceiveRoomId();
            await ReceiveCloseFrame();

            Console.WriteLine(_results.DistinctBy(e => e.roomId).Count());

            foreach (var result in _results.DistinctBy(e => e.roomId))
            {
                Console.WriteLine(result);
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void Init()
    {
        for (int i = 0; i < _countPlayers; i++)
        {
            var registrationBody = new RegistrationBody
            {
                Mail = "testy@" + i,
                Nickname = "testy" + i,
                Password = "password"
            };

            var login = new Login
            {
                Mail = registrationBody.Mail,
                Password = registrationBody.Password
            };

            var authController = new AuthController(LoggerFactory.Create(builder => builder.AddConsole()), 
                                 new AppDbContext(new DbContextOptions<AppDbContext>()));

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
            _accessTokens.Add(tokenPair.access_token);
        }
    }

    private async Task GenerateClients()
    {
        for(int i = 0; i < _countPlayers; i++)
        {
            ClientWebSocket client = new ClientWebSocket();
            client.Options.SetRequestHeader("Authorization", "Bearer " + _accessTokens[i]);
            await client.ConnectAsync(uriToDistributorLobby, CancellationToken.None);
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
            }
        }
    }

    private async Task<(string, string)> ReceiveRoomId(ClientWebSocket client, string email)
    {
        byte[] bytes = new byte[2048];
        var result = await client.ReceiveAsync(bytes, CancellationToken.None);
        string message = Encoding.UTF8.GetString(bytes);
        return (email, message);
    }

    private async Task ReceiveRoomId()
    {
        foreach(var login in _logins.Select((value, index) => new {value, index}))
        {
            byte[] bytes = new byte[2048];

            _tasks.Add(ReceiveRoomId(_clients[login.index], login.value.Mail));
        }
        
        foreach(var task in _tasks)
        {
            _results.Add(task.Result);
        }
    }

    private async Task ReceiveCloseFrame()
    {
        foreach(var client in _clients)
        {
            await client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
        }
    }
}