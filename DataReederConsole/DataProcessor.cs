using DataReederConsole.Models;
using Microsoft.Extensions.Logging;

namespace DataReederConsole
{
    public interface IDataProcessor 
    {
        Task Run(string filePath);
    }
    public class DataProcessor : IDataProcessor
    {
        private readonly ILogger<DataProcessor> _logger;
        private readonly IDataReader _dataReader;
        private readonly IDataValidator _dataValidator;
        private readonly IDataTransormer _dataTransormer;
        private readonly IDataSaver _dataSaver;
        private MetaInfo metaInfo = new MetaInfo();
        public DataProcessor(ILogger<DataProcessor> logger, IDataReader dataReader, IDataValidator dataValidator, 
            IDataTransormer dataTransormer, IDataSaver dataSaver)
        {
            _logger = logger;
            _dataReader = dataReader;
            _dataValidator = dataValidator;
            _dataTransormer = dataTransormer;
            _dataSaver = dataSaver;
        }
        public async Task Run(string filePath)
        {
            if(!_dataValidator.ValidateFile(filePath))
            {
                _logger.LogWarning($"File: {filePath} - hasn't been processed due to file type");
                metaInfo.InvalidFiles.Add(filePath);
                await _dataSaver.SaveMeta(metaInfo);
                return;
            }

            _logger.LogInformation($"File: {filePath} - taken for processing");

            List<InputModel> inputModels = await _dataReader.Read(filePath, metaInfo);
            int preValidateCount = inputModels.ToList().Count;
            await _dataValidator.ValidateData(inputModels);
            string jsonResult = await _dataTransormer.Transform(inputModels);
            await _dataSaver.SaveOutput(jsonResult);
            int errorCounter = preValidateCount - inputModels.Count;
            metaInfo.FoundErrors += errorCounter;
            metaInfo.ParsedFiles++;
            await _dataSaver.SaveMeta(metaInfo);

            _logger.LogInformation($"File: {filePath} - finished processing");
            Console.WriteLine("Press 'q' to quit the sample.");
        }
    }
}
