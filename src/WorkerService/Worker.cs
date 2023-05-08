using SharedLib;
namespace WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IDbActions _dbActions;
        private readonly IUpdateManagedInstanceGroup _updateManagedInstanceGroup;

        public Worker(ILogger<Worker> logger, IDbActions dbActions, IUpdateManagedInstanceGroup updateManagedInstanceGroup)
        {
            _logger = logger;
            _dbActions = dbActions;
            _updateManagedInstanceGroup = updateManagedInstanceGroup;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                //TODO: Process needs to be done.
                await foreach (var item in await _dbActions.GetAll())
                {

                }

                //_updateManagedInstanceGroup.ExecuteAsync();

                await Task.Delay(TimeSpan.FromDays(7), stoppingToken);
            }
        }
    }
}