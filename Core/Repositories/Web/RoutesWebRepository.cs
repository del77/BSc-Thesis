using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Core.Extensions;
using Core.Model;
using Core.Repositories.Local;
using Newtonsoft.Json;

namespace Core.Repositories.Web
{
    public class RoutesWebRepository : WebRepositoryBase
    {
        private readonly UserLocalRepository _userLocalRepository;
        private readonly string _routesUri = "routes";

        public RoutesWebRepository()
        {
            _userLocalRepository = new UserLocalRepository();
            var token = _userLocalRepository.GetUserData().Token;
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        public async Task<bool> CreateRoute(Route route)
        {
            var json = JsonConvert.SerializeObject(route);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            try
            {
                var response = await Client.PostAsync(_routesUri, content);
                return true;
            }
            catch (Exception)
            {
                var dataToSend = new DataToSend(json, _routesUri);
                _userLocalRepository.CreateDataToSend(dataToSend);

                return false;
            }
        }



        public async Task<bool> CreateRankingRecordAsync(RankingRecord currentTry, Guid routeId)
        {
            var json = JsonConvert.SerializeObject(currentTry);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var uri = $"{_routesUri}/{routeId}/ranking-record";

            try
            {
                var response = await Client.PostAsync(uri, content);
                return true;
            }
            catch (Exception)
            {
                var dataToSend = new DataToSend(json, uri);
                _userLocalRepository.CreateDataToSend(dataToSend);

                return false;
            }
        }

        public async Task<bool> SendJsonData(string json, string uri)
        {
            try
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await Client.PostAsync(uri, content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<Route>> GetRoutes(RoutesFilterQuery query)
        {
            var httpClientQuery = HttpUtility.ParseQueryString(string.Empty);
            httpClientQuery[nameof(query.RouteLengthFrom)] = query.RouteLengthFrom.ToString();
            httpClientQuery[nameof(query.RouteLengthTo)] = query.RouteLengthTo.ToString();
            httpClientQuery[nameof(query.SearchRadiusInMeters)] = query.SearchRadiusInMeters.ToString();
            httpClientQuery[nameof(query.SurfaceLevel)] = query.SurfaceLevel.ToString();
            httpClientQuery[nameof(query.SurfacePavedPercentageFrom)] = query.SurfacePavedPercentageFrom.ToString();
            httpClientQuery[nameof(query.SurfacePavedPercentageTo)] = query.SurfacePavedPercentageTo.ToString();
            httpClientQuery[nameof(query.CurrentLatitude)] = query.CurrentLatitude.ToStringWithDot();
            httpClientQuery[nameof(query.CurrentLongitude)] = query.CurrentLongitude.ToStringWithDot();

            var responseJson = await Client.GetStringAsync($"{_routesUri}?"+httpClientQuery);

            var result = JsonConvert.DeserializeObject<IEnumerable<Route>>(responseJson);
            return result;
        }
    }
}