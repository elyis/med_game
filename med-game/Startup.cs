using med_game.src.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

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

            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(Configuration["ConnectionStrings:DefaultConnection"]));
            services.AddAuthentication("Bearer")
                .AddJwtBearer(
                    options => options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                    });

            services.AddAuthorization();

            services.AddControllers().AddNewtonsoftJson()
                .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressMapClientErrors = true;
            });

            services.AddDistributedMemoryCache();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Med-game Api",
                    Description = "Api for game of mediicine university",
                });

                options.EnableAnnotations();
            }
            );

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.Zero;
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStatusCodePages();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.MapControllers();
            app.UseWebSockets(new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromSeconds(5),
            });

            app.Run();
        }
    }
}

