using DataReederConsole.Models;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace DataReederConsole
{
    public interface IDataSaver{
        Task SaveOutput(string jsonResult);
        Task SaveMeta(MetaInfo metaInfo);
    }
    public class DataSaver : IDataSaver
    {
        private readonly IConfiguration _configuration;
        private int counter;
        private static readonly object locker = new object();
        string _directory;
        public DataSaver(IConfiguration configuration)
        {
            _configuration = configuration;
            _directory = GetDirectory();
        }
        public async Task SaveOutput(string json)
        {
            await Task.Delay(100);

            lock (locker)
            {
                CounterUpdate(_directory);

                string filePath = Path.ChangeExtension(Path.Combine(_directory, $"output{counter}"), ".json");

                using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false))
                using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8, bufferSize: 4096))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                }
            }
        }
        public async Task SaveMeta(MetaInfo metaInfo)
        {
            await Task.Delay(100);

            lock (locker)
            {
                string filePath = Path.Combine(_directory, "meta.log");
                string metaInfoText = $"parsed_files: {metaInfo.ParsedFiles}\n" +
                                      $"parsed_lines: {metaInfo.ParsedLines}\n" +
                                      $"found_errors: {metaInfo.FoundErrors}\n" +
                                      $"invalid_files: [{string.Join(", ", metaInfo.InvalidFiles)}]";
         
                File.WriteAllText(filePath, metaInfoText);
            }
        }
        private string GetDirectory()
        {
            string directory = Path.Combine(GetPathFolder() + "folder_b", DateTime.Now.ToString("yyyy-MM-dd"));

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return directory;
        }
        private void CounterUpdate(string directory)
        {
            FileInfo[] files = new DirectoryInfo(directory).GetFiles("output*.json");

            if (files.Length != 0)
            {
                counter = files.Max(f => int.Parse(Path.GetFileNameWithoutExtension(f.Name).Substring(6))) + 1;
            }
            else
            {
                counter = 1;
            }
        }
        private string GetPathFolder()
        {
            return _configuration.GetSection("PathConfig:FolderPathToSave").Value!;
        }
    }
}
