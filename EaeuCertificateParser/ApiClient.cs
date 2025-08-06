using EaeuCertificateParser.Database;
using EaeuCertificateParser.Models;
using Newtonsoft.Json.Linq;

namespace EaeuCertificateParser
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;

        public ApiClient()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromMinutes(10);
        }

        private const string Url = "https://tech.eaeunion.org/spd/find?collection=kbdallread.service-prop-35_1-conformityDocDetailsType&limit=20&skip=0&sort=" +
            "{\"docStartDate\":1}&fields={\"docId\":1,\"unifiedCountryCode\":1,\"conformityDocKindName\":1,\"docStartDate\":1,\"docValidityDate\":1,\"docStatusDetails\":1," +
            "\"applicantDetails.businessEntityName\":1,\"technicalRegulationObjectDetails.manufacturerDetails\":1}";

        public async Task<JArray> GetCertificatesPageAsync(int skip, int maxRetries = 3)
        {
            var requestUrl = Url.Replace("skip=0", $"skip={skip}");
            var content = new StringContent("{}", System.Text.Encoding.UTF8, "text/plain");

            for (int retries = 0; retries < maxRetries; retries++)
            {
                try
                {
                    var response = await _httpClient.PostAsync(requestUrl, content);
                    response.EnsureSuccessStatusCode();

                    var jsonString = await response.Content.ReadAsStringAsync();
                    var jsonObj = JObject.Parse(jsonString);

                    var resultArray = jsonObj["result"] as JArray;

                    if (resultArray == null)
                    {
                        Console.WriteLine("Ошибка: поле 'result' не найдено или не является массивом");
                        return new JArray();
                    }

                    return resultArray;
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Попытка {retries + 1} не удалась: {ex.Message}");
                    await Task.Delay(2000 * (retries + 1));
                }
            }

            Console.WriteLine("Все попытки получить данные не удались.");
            return new JArray();
        }


        public static Certificate MapFromJson(JToken token)
        {
            var startDate = ParseEaeuDate(token, "docStartDate") ?? default;
            var validityDate = ParseEaeuDate(token, "docValidityDate");

            return new Certificate
            {
                DocId = token.Value<string>("docId"),
                CountryCode = token["unifiedCountryCode"]?.Value<string>("value"),
                StatusCode = token["docStatusDetails"]?.Value<string>("docStatusCode"),
                Applicant = token["applicantDetails"]?.Value<string>("businessEntityName"),
                Manufacturer = token["technicalRegulationObjectDetails"]?["manufacturerDetails"]?.FirstOrDefault()?["businessEntityName"]?.ToString(),
                DocKind = token.Value<string>("conformityDocKindName"),
                StartDate = startDate,
                ValidityDate = validityDate
            };
        }

        public static DateTime? ParseEaeuDate(JToken token, string fieldName)
        {
            var field = token[fieldName];
            if (field == null) return null;

            var dateToken = field["$date"];

            if (dateToken is JObject obj)
            {
                var numberLong = obj["$numberLong"];
                if (numberLong != null && long.TryParse(numberLong.ToString(), out var millis))
                {
                    return DateTimeOffset.FromUnixTimeMilliseconds(millis).UtcDateTime;
                }
            }

            if (dateToken != null && DateTime.TryParse(dateToken.ToString(), out var parsedDate))
            {
                return parsedDate;
            }

            return null;
        }




        public async Task ParseAndSaveAllPagesAsync(DbService dbService)
        {
            int skip = 0;
            int pageSize = 20;
            while (true)
            {
                var items = await GetCertificatesPageAsync(skip);
                if (items.Count == 0)
                    break;

                foreach (var item in items)
                {
                    var cert = MapFromJson(item);
                    dbService.InsertCertificate(cert);
                }

                skip += pageSize;
            }
        }

    }
}
