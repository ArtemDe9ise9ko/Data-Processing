using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;

namespace DataReederConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();

            var serviceProvider = FillServices(config);
            LogManager.Configuration = new XmlLoggingConfiguration("nlog.config");
            var logger = NLog.Web.NLogBuilder.ConfigureNLog(LogManager.Configuration).GetCurrentClassLogger();

            logger.Info("Start Watcher");
            serviceProvider.GetRequiredService<IWatcher>().Run();
            logger.Trace("end");
        }

        private static IServiceProvider FillServices(IConfiguration config)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton(config);
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddNLog(config);
            });

            services.AddTransient<IDataReader, DataReader>();
            services.AddTransient<IDataSaver, DataSaver>();
            services.AddTransient<IDataTransormer, DataTransormer>();
            services.AddTransient<IDataValidator, DataValidator>();
            services.AddTransient<IDataProcessor, DataProcessor>();
            services.AddSingleton<IWatcher, Watcher>();
            return services.BuildServiceProvider();
        }
    }

}