using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

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
                });

            services.AddAuthorization();
            services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressMapClientErrors = true;
            });

            services.AddDistributedMemoryCache();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
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
            app.Run();
        }
    }
}
