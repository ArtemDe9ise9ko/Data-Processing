using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DataReederConsole
{
    public interface IWatcher
    {
        void Run();
    }
    public class Watcher : IWatcher
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<Watcher> _logger;
        private readonly IDataProcessor _dataProcessor;
        public Watcher(IConfiguration configuration, ILogger<Watcher> logger, IDataProcessor dataProcessor)
        {
            _configuration = configuration;
            _logger = logger;
            _dataProcessor = dataProcessor;
        }

        public void Run(){
            string path = GetPathFolder();

            if (string.IsNullOrEmpty(path))
            {
                _logger.LogInformation("incorrect path");
                return;
            }

            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = path!;
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
            watcher.EnableRaisingEvents = true;
            watcher.Created += new FileSystemEventHandler(OnCreated);

            Console.WriteLine("Press 'q' to quit the sample.");
            while (Console.Read() != 'q') ;

             watcher.Dispose();
        }

        private string GetPathFolder()
        {
            return _configuration.GetSection("PathConfig:FolderPathToRead").Value!;
        }
        private async void OnCreated(object source, FileSystemEventArgs e)
        {
            Console.WriteLine($"File appeared in folder: {e.FullPath}");
            await _dataProcessor.Run(e.FullPath);
        }
    }
}
