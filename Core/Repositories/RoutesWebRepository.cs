using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Core.Extensions;
using Core.Model;
using Newtonsoft.Json;

namespace Core.Repositories
{
    public class RoutesWebRepository
    {
        private HttpClient _httpClient;

        public RoutesWebRepository()
        {
            var userRepository = new UserRepository();
            var token = userRepository.GetUserData().Token;

            _httpClient = new HttpClient { BaseAddress = new Uri("http://192.168.1.105:5000/routes/") };
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");


        }

        public async Task CreateRoute(Route route)
        {
            var json = JsonConvert.SerializeObject(route);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(string.Empty, content);

        }

        public async Task CreateRankingRecordAsync(RankingRecord currentTry, Guid routeId)
        {
            var json = JsonConvert.SerializeObject(currentTry);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{routeId}/ranking-record", content);
        }

        public async Task<IEnumerable<Route>> GetRoutes(RoutesFilterQuery query)
        {
            //public int RouteLengthFrom { get; set; }
            //public int RouteLengthTo { get; set; }

            //public int SurfacePavedPercentageFrom { get; set; }
            //public int SurfacePavedPercentageTo { get; set; }

            //public int SurfaceLevel { get; set; }
            //public int SearchRadius { get; set; }
            var httpClientQuery = HttpUtility.ParseQueryString(string.Empty);
            httpClientQuery[nameof(query.RouteLengthFrom)] = query.RouteLengthFrom.ToString();
            httpClientQuery[nameof(query.RouteLengthTo)] = query.RouteLengthTo.ToString();
            httpClientQuery[nameof(query.SearchRadiusInMeters)] = query.SearchRadiusInMeters.ToString();
            httpClientQuery[nameof(query.SurfaceLevel)] = query.SurfaceLevel.ToString();
            httpClientQuery[nameof(query.SurfacePavedPercentageFrom)] = query.SurfacePavedPercentageFrom.ToString();
            httpClientQuery[nameof(query.SurfacePavedPercentageTo)] = query.SurfacePavedPercentageTo.ToString();
            httpClientQuery[nameof(query.CurrentLatitude)] = query.CurrentLatitude.ToStringWithDot();
            httpClientQuery[nameof(query.CurrentLongitude)] = query.CurrentLongitude.ToStringWithDot();

            var responseJson = await _httpClient.GetStringAsync("?"+httpClientQuery);

            var result = JsonConvert.DeserializeObject<IEnumerable<Route>>(responseJson);
            return result;
        }
    }
}