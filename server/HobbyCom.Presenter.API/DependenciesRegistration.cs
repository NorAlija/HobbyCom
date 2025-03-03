using HobbyCom.Application.src.IServices;
using HobbyCom.Application.src.Services;
using HobbyCom.Presenter.API.src.Services;
using HobbyCom.Domain.src.IRepositories;
using HobbyCom.Infrastructure.src.Databases;
using HobbyCom.Infrastructure.src.Repositories;
using HobbyCom.Presenter.API.src.HostedServices;
using Microsoft.EntityFrameworkCore;
using HobbyCom.Presenter.API.src.Middlewares;
using Supabase;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Net.Http.Headers;

namespace HobbyCom.Presenter.API
{
    public static class DependenciesRegistration
    {
        public static IServiceCollection AddProjectDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            RegisterDBContextAndRepos(services, configuration);

            RegisteredServices(services, configuration);

            RegisterAuthentication(services, configuration);

            RegisterMiddlewares(services);

            RegisterHttpClients(services, configuration);

            return services;
        }

        private static void RegisterHttpClients(IServiceCollection services, IConfiguration configuration)
        {
            // Configure named HttpClient for Supabase
            services.AddHttpClient("SupabaseClient", client =>
            {
                client.BaseAddress = new Uri(configuration["Supabase:Url"] ?? throw new ArgumentNullException("Supabase:Url"));
                client.DefaultRequestHeaders.Add("apikey", configuration["Supabase:Key"]);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            // Register IHttpClientFactory
            services.AddHttpClient();
        }

        private static void RegisterAuthentication(IServiceCollection services, IConfiguration configuration)
        {

            var jwtSecret = configuration["Authentication:JwtSecret"] ?? throw new ArgumentNullException(nameof(configuration), "JwtSecret cannot be null");
            var bytes = Encoding.UTF8.GetBytes(jwtSecret);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opts =>
            {
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(bytes),
                    ValidIssuer = configuration["Authentication:ValidIssuer2"],
                    ValidAudience = configuration["Authentication:ValidAudience"],
                };
            });
        }

        private static void RegisterMiddlewares(IServiceCollection services)
        {
            services.AddTransient<GlobalExceptionMiddleware>();


        }

        private static void RegisteredServices(IServiceCollection services, IConfiguration configuration)
        {

            // Hosted Services work in the background
            services.AddHostedService<DatabaseConnectionCheckService>();

            services.AddScoped<IAuthenticationService, AuthenticationService>();

            services.AddScoped<IUserService, UserService>();
        }

        private static void RegisterDBContextAndRepos(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<SupabaseContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("SessionConnection"),
                    npgsqlOptions =>
                    {
                        npgsqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                        npgsqlOptions.CommandTimeout(60);
                    }
                )
            );

            // Register Supabase Client to usse supabase capabilities such as Realtime, Auth, Storage, etc.
            services.AddSingleton<Client>((provider) =>
            {
                var url = configuration["Supabase:Url"] ?? throw new ArgumentNullException(nameof(configuration), "Supabase URL cannot be null");
                var key = configuration["Supabase:Key"];

                return new Client(
                    url,
                    key,
                    new SupabaseOptions
                    {
                        AutoRefreshToken = false,
                        AutoConnectRealtime = true
                    });
            });

            services.AddScoped<IUserRepository, UserRepository>();

        }
    }
}