using HobbyCom.Infrastructure.src.Databases;

namespace HobbyCom.Presenter.API.src.HostedServices
{
    public class DatabaseConnectionCheckService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public DatabaseConnectionCheckService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<SupabaseContext>();

            try
            {
                bool isConnected = await dbContext.Database.CanConnectAsync(cancellationToken);
                Console.WriteLine(isConnected
                    ? "✅ Successfully connected to database"
                    : "❌ Database connection failed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error connecting to database: {ex.Message}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}