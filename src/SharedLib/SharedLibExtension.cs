using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using SharedLib.Models;


namespace SharedLib
{
    public static class SharedLibExtension
    {
        public static IServiceCollection AddSharedLib(this IServiceCollection services, Action<SharedLibConfiguration>? options = null) 
        {
            if (options != null)
            {
                services.Configure<SharedLibConfiguration>(options);
                SharedLibConfiguration configuration = new();
                options?.Invoke(configuration);
                Options.Create(configuration);
            }         

            services.AddLogging(options => options.AddSerilog());
            services.AddSingleton<IConfigurationReader, ConfigurationReader>();
            services.AddTransient<ICommandExecutor, CommandExecutor>();
            services.AddSingleton<IAuthentication, Authentication>();
            services.AddScoped<ICompute, Compute>();
            services.AddTransient<IDbActions, DbActions>();
            services.AddScoped<IUpdateManagedInstanceGroup, UpdateManagedInstanceGroup>();
            services.AddTransient<IIAP, IAP>();

            ConfigurationReader configurationReader = new();
            
            var configs = configurationReader.ReadConfiguration();

            if (configs.Databases.Any() && configs.Databases.Count == 1)
            {
                if (configs.Databases.Where(d => d.Database == "sqlite").Any())
                {
                    services.AddDbContext<DatabaseConnection>(options =>
                    {
                        options.UseSqlite($"Data Source=database.sqlite");
                    });
                }
                if (configs.Databases.Where(d => d.Database == "mssql").Any())
                {
                    services.AddDbContext<DatabaseConnection>(options =>
                    {
                        options.UseSqlServer(
                            $"Server={configs.Databases.Where(d => d.Database == "mssql").First().Settings.Host};" +
                            $"Database={configs.Databases.Where(d => d.Database == "mssql").First().Settings.DatabaseName};" +
                            $"User Id={configs.Databases.Where(d => d.Database == "mssql").First().Settings.Username};" +
                            $"Password={configs.Databases.Where(d => d.Database == "mssql").First().Settings.Password};" +
                            "Encrypt=True;" +
                            "TrustServerCertificate=True;");
                    });
                }
                // TODO: Not working with MariaDB 10.11
                /*
                if (configs.Databases.Where(d => d.Database == "mysql").Any())
                {
                    services.AddDbContext<DatabaseConnection>(options =>
                    {
                        options.EnableDetailedErrors();
                        options.EnableSensitiveDataLogging();

                        string connStr = $"Server={configs.Databases.Where(d => d.Database == "mysql").First().Settings.Host};" +
                            $"Port={configs.Databases.Where(d => d.Database == "mysql").First().Settings.Port};" +
                            $"UserID={configs.Databases.Where(d => d.Database == "mysql").First().Settings.Username};" +
                            $"Password={configs.Databases.Where(d => d.Database == "mysql").First().Settings.Password};" +
                            $"Database={configs.Databases.Where(d => d.Database == "mysql").First().Settings.DatabaseName}";

                        options.UseMySql(ServerVersion.AutoDetect(connStr),
                            mysqlOptions =>
                                {
                                    mysqlOptions.EnableRetryOnFailure();
                            });
                    });
                }
                */
                if (configs.Databases.Where(d => d.Database == "postgresql").Any())
                {
                    services.AddDbContext<DatabaseConnection>(options =>
                    {
                        options.UseNpgsql(
                            $"Host={configs.Databases.Where(d => d.Database == "postgresql").First().Settings.Host};" +
                            $"Port={configs.Databases.Where(d => d.Database == "postgresql").First().Settings.Port};" +
                            $"Database={configs.Databases.Where(d => d.Database == "postgresql").First().Settings.DatabaseName};" +
                            $"Username={configs.Databases.Where(d => d.Database == "postgresql").First().Settings.Username};" +
                            $"Password={configs.Databases.Where(d => d.Database == "postgresql").First().Settings.Password}");
                    });
                }
            }
            else
            {
                Log.Error("Multiple databases configured");
                Environment.Exit(1);
            }
            return services;
        }
    }
}
