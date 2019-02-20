using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ConsumingApiHelper {
    public class ConsumingApi {
        private HttpClient _client;

        public ConsumingApi(string baseAdress) {
            _client = new HttpClient() {
                BaseAddress = new Uri(baseAdress)
            };
            var mediaType = new MediaTypeWithQualityHeaderValue("application/json");
            _client.DefaultRequestHeaders.Accept.Add(mediaType);
        }

        public void Authenticate(string endPoint, object obj) {
            var response = _client.PostAsync($"{_client.BaseAddress}{endPoint}", ObjectToHttpContent(obj)).Result;
            if (response.IsSuccessStatusCode) {
                var responseContent = response.Content.ReadAsStringAsync().Result;
                var tokenData = JObject.Parse(responseContent);
                var token = (string)tokenData["token"];
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public (T result, string message) Get<T>(string endPoint) {
            return GetAssync<T>(endPoint).Result;
        }

        public async Task<(T result, string message)> GetAssync<T>(string endPoint) {
            var response = await _client.GetAsync($"{_client.BaseAddress}{endPoint}");
            return DeserializeResponse<T>(response);
        }

        public (TResult result, string message) Post<T, TResult>(string endPoint, T obj) {
            var response = _client.PostAsync($"{_client.BaseAddress}{endPoint}", ObjectToHttpContent(obj)).Result;
            return DeserializeResponse<TResult>(response);
        }

        public (TResult result, string message) Put<T, TResult>(string endPoint, T obj) {
            var response = _client.PutAsync($"{_client.BaseAddress}{endPoint}", ObjectToHttpContent(obj)).Result;
            return DeserializeResponse<TResult>(response);
        }

        public (bool result, string message) Delete(string endPoint) {
            var response = _client.DeleteAsync($"{_client.BaseAddress}{endPoint}").Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;
            if (response.IsSuccessStatusCode) {
                return (true, responseContent);
            } else {
                return (false, responseContent);
            }
        }

        private (T result, string message) DeserializeResponse<T>(HttpResponseMessage response) {
            var responseContent = response.Content.ReadAsStringAsync().Result;
            if (response.IsSuccessStatusCode) {
                return (JsonConvert.DeserializeObject<T>(responseContent), responseContent);
            } else {
                return (default(T), responseContent);
            }
        }

        private HttpContent ObjectToHttpContent(object obj) {
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}
