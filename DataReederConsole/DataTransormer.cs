using DataReederConsole.Models;
using Newtonsoft.Json;
using System.Globalization;

namespace DataReederConsole
{
    public interface IDataTransormer{
        Task<string> Transform(List<InputModel> inputModels);
    }
    public class DataTransormer : IDataTransormer
    {
        public async Task<string> Transform(List<InputModel> inputModels){ 

            return await ToJson(ToModel(inputModels));
        }
        private List<CityData> ToModel(List<InputModel> inputModels)
        {
            return inputModels.GroupBy(x => x.City)
                .Select(group => new CityData
                {
                    City = group.Key!,
                    Services = group.GroupBy(x => x.ServiceName)
                        .Select(serviceGroup => new Service
                        {
                            Name = serviceGroup.Key!,
                            Payers = serviceGroup.Select(inputModel => new Payer
                            {
                                Name = $"{inputModel.Firstname} {inputModel.LastName}",
                                Payment = Convert.ToDecimal(inputModel.Payment),
                                Date = DateTime.ParseExact(inputModel.Date!, "yyyy-dd-MM", CultureInfo.InvariantCulture),
                                AccountNumber = Convert.ToInt64(inputModel.AccountId)
                            }).ToList(),
                            Total = serviceGroup.Sum(x => Convert.ToDecimal(x.Payment))
                        }).ToList(),
                    Total = group.Sum(x => Convert.ToDecimal(x.Payment))
                }).ToList();
        }

    private async Task<string> ToJson(List<CityData> outputModel)
        {
            return await Task.Run(() => JsonConvert.SerializeObject(outputModel, Formatting.Indented));
        }

    }
}
