﻿using Newtonsoft.Json;
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
        private string token;
        private string loginEndPoint;
        private object credentials;

        public ConsumingApi(string baseAdress) {
            _client = new HttpClient() {
                BaseAddress = new Uri(baseAdress)
            };
            var mediaType = new MediaTypeWithQualityHeaderValue("application/json");
            _client.DefaultRequestHeaders.Accept.Add(mediaType);
        }

        public void Authenticate(string endPoint, object obj) {
            loginEndPoint = endPoint;
            credentials = obj;
            var response = _client.PostAsync($"{_client.BaseAddress}{endPoint}", ObjectToHttpContent(credentials)).Result;
            if (response.IsSuccessStatusCode) {
                var responseContent = response.Content.ReadAsStringAsync().Result;
                var tokenData = JObject.Parse(responseContent);
                token = (string)tokenData["token"];
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public bool IsAuthenticated(string endPoint) {
            var result = Post<string, bool>($"{_client.BaseAddress}{endPoint}", token);
            bool isValidade = result.result;
            return isValidade;
        }

        public void ReAuthenticate() => Authenticate(loginEndPoint, credentials);

        public ConsumingApi CheckAndReAuthenticate(string endPoint) {
            var valid = IsAuthenticated(endPoint);
            if (!valid) {
                ReAuthenticate();
            }

            return this;
        }

        public (T result, string statusCode, string message) Get<T>(string endPoint) {
            return GetAssync<T>(endPoint).Result;
        }

        public async Task<(T result, string statusCode, string message)> GetAssync<T>(string endPoint) {
            var response = await _client.GetAsync($"{_client.BaseAddress}{endPoint}");
            return DeserializeResponse<T>(response);
        }

        public (TResult result, string statusCode, string message) Post<T, TResult>(string endPoint, T obj) {
            var response = _client.PostAsync($"{_client.BaseAddress}{endPoint}", ObjectToHttpContent(obj)).Result;
            return DeserializeResponse<TResult>(response);
        }

        public (TResult result, string statusCode, string message) Put<T, TResult>(string endPoint, T obj) {
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

        private (T result, string statusCode, string message) DeserializeResponse<T>(HttpResponseMessage response) {
            var responseContent = response.Content.ReadAsStringAsync().Result;
            var statusCode = response.StatusCode.ToString();
            if (response.IsSuccessStatusCode) {
                return (JsonConvert.DeserializeObject<T>(responseContent), statusCode, responseContent);
            } else {
                return (default(T), statusCode, responseContent);
            }
        }

        private HttpContent ObjectToHttpContent(object obj) {
            if (obj.GetType().IsPrimitive || obj.GetType() == typeof(string) || obj.GetType() == typeof(decimal)) {
                return new StringContent(obj.ToString());
            }
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}
