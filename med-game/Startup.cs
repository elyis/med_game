using System.Text;
using med_game.src.Infrastructure.Data;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using med_game.src.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace med_game
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        
        public Startup(IConfiguration config)
        {
            Configuration = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var jwtSettings = Configuration.GetSection("JwtSettings");
            string secretKey = jwtSettings.GetValue<string>("Key")!;

            services.AddControllers().AddNewtonsoftJson()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressMapClientErrors = true;
                });

            services.AddEndpointsApiExplorer();
            
            services.AddRazorPages().AddRazorPagesOptions(options => {
                options.RootDirectory = "/src/Web/Pages";
            });

            services.AddDbContext<AppDbContext>();
            
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                });

            services.AddAuthorization();
            services.AddLogging(builder =>{
                builder.AddConsole();
            });

            
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Med-game Api",
                    Description = "Api for game of medicine university",
                });

                options.EnableAnnotations();
            });

            services.AddSingleton<JwtUtilities>();
            services.AddSingleton<FileUploader>();

            services.Scan(scan => {
                scan.FromCallingAssembly()
                    .AddClasses(classes => 
                        classes.Where(type => 
                            type.Name.EndsWith("Repository")))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime();
            });

            services.Scan(scan => {
                scan.FromCallingAssembly()
                    .AddClasses(classes => 
                        classes.Where(type => 
                            type.Name.EndsWith("Manager")))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime();
            });

            services.Scan(scan => {
                scan.FromCallingAssembly()
                    .AddClasses(classes => 
                        classes.Where(type => 
                            type.Name.EndsWith("Service")))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime();
            });
        }

        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpLogging();
            app.UseStaticFiles();
            app.UseRouting();
            app.MapRazorPages();

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.MapControllers();
            
            app.UseWebSockets(new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromSeconds(5),
            });

            app.Run();
        }
    }
}
