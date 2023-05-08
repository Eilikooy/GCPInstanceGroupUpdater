using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SharedLib;
using SharedLib.Models;

try
{
    Parser.Default.ParseArguments<CommandLineArguments>(args)
        .WithParsed(RunOptions)
        .WithNotParsed(HandleParseError);
}
catch (Exception)
{
	throw;
}
finally
{
    Log.CloseAndFlush();
}

static void ConfigureServices(IServiceCollection services)
{
    services.AddSharedLib();
}

static void RunOptions(CommandLineArguments opts)
{
    Log.Logger = new LoggerConfiguration().Enrich.FromLogContext()
    .WriteTo.Console().CreateLogger();

    var serviceCollection = new ServiceCollection();
    ConfigureServices(serviceCollection);
    var serviceProvider = serviceCollection.BuildServiceProvider();
    serviceProvider.GetRequiredService<IUpdateManagedInstanceGroup>().ExecuteAsync(
        opts.FriendlyName, 
        opts.SshUsername, 
        opts.SshKeyFile, 
        opts.AutoContinue).Wait();
}
static void HandleParseError(IEnumerable<Error> errs)
{
    //handle errors
}