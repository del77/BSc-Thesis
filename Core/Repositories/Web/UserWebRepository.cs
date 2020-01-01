using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Core.Model;
using Core.Repositories.Local;
using Newtonsoft.Json;

namespace Core.Repositories.Web
{
    public class UserWebRepository : WebRepositoryBase
    {
        private const string CouldNotConnectCode = "ConnectionProblem";
        private readonly UserLocalRepository _userLocalRepository;

        public UserWebRepository()
        {
            _userLocalRepository = new UserLocalRepository();

        }

        public async Task<string> RegisterAccount(string username, string password)
        {
            try
            {
                var content = BuildUserDataPayload(username, password);
                var response = await Client.PostAsync("users/register", content);

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
                var response = await Client.PostAsync("login", content);
                var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    _userLocalRepository.CreateUserData(new UserData(username, message));
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