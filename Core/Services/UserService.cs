using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Core.Model;
using Core.Repositories;
using Newtonsoft.Json;

namespace Core.Services
{
    public class UserService
    {
        private const string CouldNotConnectCode = "ConnectionProblem";
        private HttpClient _httpClient;
        private readonly UserRepository _userRepository;

        public UserService()
        {
            _userRepository = new UserRepository();
            //_httpClient = new HttpClient { BaseAddress = new Uri("http://192.168.1.105:5000/users/") };
            _httpClient = new HttpClient { BaseAddress = new Uri("http://192.168.1.16:5000/users/") };

        }

        public async Task<string> RegisterAccount(string username, string password)
        {
            try
            {
                var content = BuildUserDataPayload(username, password);
                var response = await _httpClient.PostAsync("register", content);

                var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return message;
            }
            catch (Exception)
            {
                return CouldNotConnectCode;
            }
        }

        public async Task<Tuple<bool, string>> Login(string username, string password)
        {
            try
            {
                var content = BuildUserDataPayload(username, password);
                var response = await _httpClient.PostAsync("login", content);
                var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    _userRepository.CreateUserData(new UserData(username, message));
                    return new Tuple<bool, string>(true, string.Empty);
                }

                return new Tuple<bool, string>(false, message);
            }
            catch (Exception)
            {
                return new Tuple<bool, string>(false, CouldNotConnectCode);
            }
        }

        private StringContent BuildUserDataPayload(string username, string password)
        {
            var payload = new
            {
                username,
                password
            };
            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            return content;
        }
    }
}