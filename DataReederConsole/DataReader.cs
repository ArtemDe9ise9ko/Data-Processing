using DataReederConsole.Models;
using Microsoft.Extensions.Logging;

namespace DataReederConsole
{
    public interface IDataReader
    {
        Task<List<InputModel>> Read(string filePath, MetaInfo metaInfo);
    }
    public class DataReader : IDataReader
    {
        private readonly ILogger<DataReader> _logger;
        public DataReader(ILogger<DataReader> logger)
        {
            _logger = logger;
        }
        public async Task<List<InputModel>> Read(string filePath, MetaInfo metaInfo)
        {
            List<InputModel> inputModels = new List<InputModel>();
            bool isFirstLineCsv = true; 
            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    InputModel inputModel = new InputModel();

                    string line = await Task.Run(() => reader.ReadLine()!.Replace("“", "").Replace("”", "").Replace(" ", ""));

                    if (Path.GetExtension(filePath).ToLower() == ".csv" && isFirstLineCsv)
                    {
                        isFirstLineCsv = false;
                        continue;
                    }

                    string[] values = line.Split(',');

                    if (values.Length == 9)
                    {
                        inputModel.Firstname = values[0];
                        inputModel.LastName = values[1];
                        inputModel.City = values[2];
                        inputModel.Street = values[3];
                        inputModel.Number = values[4];
                        inputModel.Payment = values[5];
                        inputModel.Date = values[6];
                        inputModel.AccountId = values[7];
                        inputModel.ServiceName = values[8];

                        inputModels.Add(inputModel);

                        metaInfo.ParsedLines++;
                        _logger.LogInformation($"Line: {line} - taken for processing");
                    }
                    else{
                        metaInfo.FoundErrors += 1;
                        _logger.LogInformation($"Line: {line} - has incorrect values size");
                    }
                }
            }
            return inputModels;
        }
    }
}
