using HobbyCom.Infrastructure.src.Databases;
using HobbyCom.Presenter.API.src.HostedServices;
using Microsoft.EntityFrameworkCore;
using Supabase;

namespace HobbyCom.Presenter.API
{
    public static class DependenciesRegistration
    {
        public static IServiceCollection AddProjectDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            // Register Infrastructure Services
            RegisterDBContextAndRepos(services, configuration);

            RegisteredServices(services, configuration);

            RegisterMiddlewares(services);

            return services;
        }

        private static void RegisteredServices(IServiceCollection services, IConfiguration configuration)
        {

            // Hosted Services work in the background
            services.AddHostedService<DatabaseConnectionCheckService>();
        }

        private static void RegisterMiddlewares(IServiceCollection services)
        {

        }

        private static void RegisterDBContextAndRepos(IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext
            services.AddDbContext<SupabaseContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // Register Supabase Client
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

        }
    }
}