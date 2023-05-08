using WorkerService;
using SharedLib;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSharedLib();
        services.AddSystemd();
        services.AddWindowsService();
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
