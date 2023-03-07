using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using med_game.src.Data;
using med_game.src.Models;

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

            services.AddAuthentication("Bearer").AddJwtBearer(
                options => options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                }
            );

            services.AddAuthorization();

            services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressMapClientErrors = true;
            });

            services.AddDistributedMemoryCache();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Med-game Api",
                    Description = "Api for game of mediicine university", 
                }
                )
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
            app.UseHttpsRedirection();
            app.UseSession();
            app.MapControllers();
            app.UseWebSockets(new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromSeconds(20),
            });

            //InitDb();

            app.Run();
        }

        private void InitDb()
        {
            using (AppDbContext context =  new AppDbContext())
            {
                var admin = new User
                {
                    Email = "elyi7367@gmail.com",
                    Nickname = "root",
                    Password = "5c4769a51f45f8f0412a4db922d1f4237ce7d5551107d048e35c4e677527115633a4450ada2fb5b29d012db7f6a7273ca680af2895914cc706db45683f9fdc40",
                    RoleName = Enum.GetName(typeof(Roles), Roles.Admin)!
                };

                context.Users.Add(admin);
                context.SaveChanges();
            }
        }
    }
}
