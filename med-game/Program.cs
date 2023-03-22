using med_game;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var startUp = new Startup(builder.Configuration);
startUp.ConfigureServices(builder.Services);

var app = builder.Build();

startUp.Configure(app, app.Environment);
