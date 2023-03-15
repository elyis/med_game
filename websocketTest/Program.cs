using med_game.src.Controllers;
using med_game.src.Entities;
using med_game.src.Entities.Request;
using med_game.src.Entities.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;

Console.ReadLine();

Stopwatch stopwatch = new Stopwatch();
stopwatch.Start();
var gameLobby = new GameLobbyDistributorTesting(2);
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
    private readonly ConcurrentDictionary<string, string> _accessTokens = new ();
    private readonly ConcurrentDictionary<string, string> _results = new ();

    private readonly RoomSettingBody _roomSettingBody;
    private readonly Uri uriToMain = new("wss://localhost:7296/main");

    private readonly List<Task<(string, string)>> _tasks = new();
    private readonly List<Task<(string, string)>> gameProcessTasks = new();
    private readonly Dictionary<string, ClientWebSocket> _clients = new ();

    public GameLobbyDistributorTesting(int countUsers)
    {
        _countPlayers = countUsers;
        _roomSettingBody = new RoomSettingBody
        {
            LecternName = "Анатомия",
            ModuleName = "Остеология",
            Type = TypeBattle.Rating
        };

        Init();
    }

    

    public async Task Execute()
    {
        try
        {
            await GetTokens();
            await SetupClients(uriToMain);
            string json = JsonConvert.SerializeObject(_roomSettingBody);
            await SendAll(json);
            await ReceiveRoomId();
            await ReceiveCloseFrame();
            _clients.Clear();

            foreach (var result in _results)
            {
                Console.WriteLine(result);
            }
            await SetupClients(uriToMain, false);
            await Gameplay();

            await ReceiveCloseFrame();
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
            _accessTokens.TryAdd( registationBody.value.Mail,tokenPair.AccessToken);
        }
    }

    private async Task SetupClients(Uri uri, bool toDistributor = true)
    {
        foreach(var accessToken in _accessTokens)
        {
            ClientWebSocket client = new ClientWebSocket();
            client.Options.SetRequestHeader("Authorization", "Bearer " + accessToken.Value);
            if (!toDistributor)
            {
                Uri url = new Uri($"{uri}/{_results[accessToken.Key]}");
                await client.ConnectAsync(url, CancellationToken.None); 
            }
            else
                await client.ConnectAsync(uri, CancellationToken.None);
            _clients.Add(accessToken.Key, client);
        }
    }

    private async Task SendAll(string message)
    {
        foreach(var client in _clients)
        {
            if(client.Value.State == WebSocketState.Open)
                await client.Value.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }

    private async Task<(string, string)> ReceiveRoomId(ClientWebSocket client, string email)
    {
        byte[] bytes = new byte[2048];
        await client.ReceiveAsync(bytes, CancellationToken.None);
        string message = Encoding.UTF8.GetString(bytes);
        return (email, message);
    }

    private async Task ReceiveRoomId()
    {
        foreach(var login in _logins)
        {
            _tasks.Add(ReceiveRoomId(_clients[login.Mail], login.Mail));
        }
        
        foreach(var task in _tasks)
        {
            _results.TryAdd(task.Result.Item1, task.Result.Item2);
        }
    }

    private async Task ReceiveCloseFrame()
    {
        foreach(var client in _clients)
        {
            await client.Value.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
        }
    }

    public async Task Gameplay()
    {
        foreach (var result in _results)
        {
            var task = ProccessLoop(_clients[result.Key], result.Key);
            gameProcessTasks.Add(task);
        }


        foreach(var task in gameProcessTasks)
        {
            var result = await task;
            Console.WriteLine(result);
        }

    }

    public async Task<(string, string)> ProccessLoop(ClientWebSocket client, string email)
    {
        StateGame stateGame = new StateGame();
        WebSocketReceiveResult? result = null;
        do
        {
            stateGame = await ReceiveJson<StateGame>(client);
            var questionBody = await ReceiveJson<QuestionBody>(client);

            var answer = questionBody.RightAnswer;
            await SendJson(answer, client);

        } while (result.MessageType != WebSocketMessageType.Close || stateGame.IsEndGame);

        return (email, stateGame.WinnerName!);
    }

    public async Task<T?> ReceiveJson<T>(ClientWebSocket client)
    {
        byte[] buffer = new byte[2048];

        WebSocketReceiveResult? result = null;
        MemoryStream memoryStream = new();

        do
        {
            result = await client.ReceiveAsync(buffer, CancellationToken.None);
            if (result.Count > 0 && result.MessageType == WebSocketMessageType.Text)
                memoryStream.Write(buffer.ToArray(), 0, result.Count);

        } while (!result.EndOfMessage && client.State == WebSocketState.Open);

        return JsonConvert.DeserializeObject<T?>(Encoding.UTF8.GetString(memoryStream.ToArray()));
    }

    public async Task SendJson<T> (T json, ClientWebSocket client)
    {
        string message = JsonConvert.SerializeObject(json);
        byte[] bytes = Encoding.UTF8.GetBytes(message);
        await client.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
    }

}