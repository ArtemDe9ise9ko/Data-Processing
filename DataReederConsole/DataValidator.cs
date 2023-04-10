using DataReederConsole.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DataReederConsole
{
    public interface IDataValidator{
        bool ValidateFile(string filePath);
        Task ValidateData(List<InputModel> inputModels);
    }
    public class DataValidator : IDataValidator
    {
        public bool ValidateFile(string filePath)
        {
            string fileExtension = Path.GetExtension(filePath).ToLower();

            return fileExtension == ".txt" || fileExtension == ".csv";
        }
        public async Task ValidateData(List<InputModel> inputModels)
        {
            await Task.Run(() =>
            {
                inputModels.RemoveAll(model =>
                    !IsValidName(model.Firstname!) ||
                    !IsValidName(model.LastName!) ||
                    !IsValidCity(model.City!) ||
                    !IsValidPayment(model.Payment!) ||
                    !IsValidDate(model.Date!) ||
                    !IsValidAccountId(model.AccountId!) ||
                    !IsValidName(model.ServiceName!));
            });
        }
        private static bool IsValidName(string name)
        {
            return !string.IsNullOrWhiteSpace(name) && name.Length < 50 && isAlphabetic(name);
        }
        private static bool IsValidCity(string city)
        {
            return !string.IsNullOrWhiteSpace(city) && city.Length < 50 && isCity(city);
        }
        private static bool IsValidPayment(string payment)
        {
            return !string.IsNullOrWhiteSpace(payment) && payment.Length < 20 && isNumericWithDot(payment) ;
        }
        private static bool IsValidDate(string date)
        {
            return !string.IsNullOrWhiteSpace(date) && date.Length == 10 && isDate(date);
        }
        private static bool IsValidAccountId(string accountId)
        {
            return !string.IsNullOrWhiteSpace(accountId) && accountId.Length < 20 && isNumeric(accountId) ;
        }
        private static bool isAlphabetic(string value)
        {
            return new Regex(@"^([A-Z][a-z]+)$").IsMatch(value);
        }
        private static bool isCity(string city)
        {
            return new Regex(@"^[A-Z][a-z]*(\s[A-Z][a-z]*)*$").IsMatch(city);
        }
        private static bool isNumeric(string value)
        {
            return new Regex(@"^\d+$").IsMatch(value);
        }

        private static bool isNumericWithDot(string value)
        {
            return new Regex(@"^\d+\.\d+$").IsMatch(value);
        }

        private static bool isDate(string value)
        {
            return DateTime.TryParseExact(value, "yyyy-dd-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result);
        }
    }
}
