using WarehouseSystem.Data;

namespace WarehouseSystem.Services;

public class DemoResetService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TimeSpan _interval;
    private readonly ILogger<DemoResetService> _logger;

    public DemoResetService(IServiceScopeFactory scopeFactory, IConfiguration config, ILogger<DemoResetService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        var hours = config.GetValue<int>("DemoReset:IntervalHours", 3);
        _interval = TimeSpan.FromHours(hours);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_interval);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await DemoDataSeeder.ResetBusinessDataAsync(db);
                _logger.LogInformation("Demo data reset completed at {Time}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Demo data reset failed");
            }
        }
    }
}