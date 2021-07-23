using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentServer.BlazorApp
{
    public class ApiClient : HttpClient
    {
        private ProtectedBrowserStorage _storage;
        private bool _useDefaultBaseUrl = true;

        public ApiClient(ProtectedBrowserStorage storage)
        {
            _storage = storage;
        }

        public ApiClient DisableDefaultBaseUrl() 
        {
            _useDefaultBaseUrl = false;
            return this;
        }
        
        public async Task<HttpResponseMessage> Get(string endpoint)
        {
            var fullUrl = _useDefaultBaseUrl ? Program.ApiBaseUrl + endpoint : endpoint;

            if (_storage is not null && 
                !DefaultRequestHeaders.Contains("Authorization"))
            {
                var token = await _storage.GetAsync<string>("token");

                if (token.Success)
                {
                    DefaultRequestHeaders.Add("Authorization", token.Value);
                }
            }

            return await GetAsync(fullUrl);
        }

        public async Task<HttpResponseMessage> Post(string endpoint, object payload)
        {
            var fullUrl = _useDefaultBaseUrl ? Program.ApiBaseUrl + endpoint : endpoint;

            if (_storage is not null &&
                !DefaultRequestHeaders.Contains("Authorization"))
            {
                var token = await _storage.GetAsync<string>("token");

                if (token.Success)
                {
                    DefaultRequestHeaders.Add("Authorization", token.Value);
                }
            }

            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            return await PostAsync(fullUrl, content);
        }
    }
}
