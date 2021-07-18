using p002.Models.Data;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace p002.Service
{
    public class CovidApiService
    {
        private static string BaseApiURL = "https://api.covid19api.com";
        private readonly HttpClient _httpClient;
        public CovidApiService()
        {
            _httpClient = new HttpClient();
        }
        public List<DailyByCountryApiResponse> DailyByCountryGet(DailyByCountryRequest request)
        {
            var response = new List<DailyByCountryApiResponse>();
            var stringBuilder = new StringBuilder();
            var dictionary = new Dictionary<string, string>()
            {
                { "country", request.CountryCode },
                { "status", "confirmed" }
            };
            var nameValueCollection = new NameValueCollection()
            {
                { "from", request.StartDate.ToString("yyyy-MM-ddTHH:mm:ssZ") },
                { "to", request.EndDate.ToString("yyyy-MM-ddTHH:mm:ssZ") },
            };
            stringBuilder.Append(BaseApiURL);
            stringBuilder.Append("/total");
            AppendEndpointSection(dictionary, stringBuilder);
            ToQueryString(nameValueCollection, stringBuilder);
            response = GetApiResponse<List<DailyByCountryApiResponse>>(stringBuilder.ToString());
            return response;
        }
        public List<SingleCountry> GetCountries()
        {
            var response = new List<SingleCountry>();
            var stringBuilder = new StringBuilder();
            var dictionary = new Dictionary<string, string>()
            {
                { "countries", string.Empty },
            };
            stringBuilder.Append(BaseApiURL);
            AppendEndpointSection(dictionary, stringBuilder);
            response = GetApiResponse<List<SingleCountry>>(stringBuilder.ToString());
            return response;
        }
        public List<GetLiveByCountryAllStatus> GetLiveByCountryAllStatuses(GetLiveByCountryAllStatusRequest request)
        {
            var response = new List<GetLiveByCountryAllStatus>();
            var stringBuilder = new StringBuilder();
            var dictionary = new Dictionary<string, string>()
            {
                { "country", request.CountryCode }
            };
            stringBuilder.Append(BaseApiURL);
            stringBuilder.Append("/live");
            AppendEndpointSection(dictionary, stringBuilder);
            response = GetApiResponse<List<GetLiveByCountryAllStatus>>(stringBuilder.ToString());
            return response;
        }

        private T GetApiResponse<T>(string url)
        {
            try
            {
                var response = Task.Run(async () => await _httpClient.GetAsync(url)).GetAwaiter().GetResult();
                var body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(body);
            }
            catch(Exception ex)
            {
                return default(T);
            }
        }

        private void AppendEndpointSection(Dictionary<string, string> dictionary, StringBuilder stringBuilder)
        {
            foreach(var keyValue in dictionary)
            {
                if(string.IsNullOrEmpty(keyValue.Value))
                {
                    stringBuilder.Append($"/{keyValue.Key}");
                }
                else
                {
                    stringBuilder.Append($"/{keyValue.Key}/{keyValue.Value}");
                }
            }
        }

        private void ToQueryString(NameValueCollection nvc, StringBuilder stringBuilder)
        {
            var array = (
                from key in nvc.AllKeys
                from value in nvc.GetValues(key)
                select string.Format(
            "{0}={1}",
            HttpUtility.UrlEncode(key),
            HttpUtility.UrlEncode(value))
                ).ToArray();
            stringBuilder.Append("?" + string.Join("&", array));
        }
    }
}