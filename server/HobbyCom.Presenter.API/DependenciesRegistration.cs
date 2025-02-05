using HobbyCom.Application.src.IServices;
using HobbyCom.Presenter.API.src.Services;
using HobbyCom.Domain.src.IRepositories;
using HobbyCom.Infrastructure.src.Databases;
using HobbyCom.Infrastructure.src.Repositories;
using HobbyCom.Presenter.API.src.HostedServices;
using Microsoft.EntityFrameworkCore;
using HobbyCom.Presenter.API.src.Middlewares;
using Supabase;

namespace HobbyCom.Presenter.API
{
    public static class DependenciesRegistration
    {
        public static IServiceCollection AddProjectDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            RegisterDBContextAndRepos(services, configuration);

            RegisteredServices(services, configuration);

            RegisterMiddlewares(services);

            return services;
        }

        private static void RegisterMiddlewares(IServiceCollection services)
        {
            services.AddTransient<GlobalExceptionMiddleware>();

        }

        private static void RegisteredServices(IServiceCollection services, IConfiguration configuration)
        {

            // Hosted Services work in the background
            services.AddHostedService<DatabaseConnectionCheckService>();

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
            services.AddScoped<Client>((provider) =>
            {
                var url = configuration["Supabase:Url"] ?? throw new ArgumentNullException(nameof(configuration), "Supabase URL cannot be null");
                var key = configuration["Supabase:Key"];

                return new Client(
                    url,
                    key,
                    new SupabaseOptions
                    {
                        AutoRefreshToken = true,
                        AutoConnectRealtime = true
                    });
            });

            services.AddScoped<IUserRepository, UserRepository>();

        }
    }
}