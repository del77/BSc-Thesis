using System;
using System.Net.Http;

namespace Core.Repositories.Web
{
    public abstract class WebRepositoryBase
    {
        private const string ApiBaseAddress = "http://192.168.1.16:5000/";
        protected readonly HttpClient Client;

        protected WebRepositoryBase()
        {
            Client = new HttpClient { BaseAddress = new Uri(ApiBaseAddress) };
        }
    }
}