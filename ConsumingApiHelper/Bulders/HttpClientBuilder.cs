using ConsumingApiHelper.Helpers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ConsumingApiHelper.Bulders {
    public class HttpClientBuilder {
        public static HttpClient Create(string appHost, string authenticationUri, string username, string password) {
            var client = new HttpClient() {
                BaseAddress = new Uri(appHost)
            };
            var mediaType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(mediaType);
            var auth = new AuthenticationHelper(client, authenticationUri);
            var token = auth.Authenticate(username, password);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        public static HttpClient Anonimous(string appHost) {
            var client = new HttpClient() {
                BaseAddress = new Uri(appHost)
            };
            var mediaType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(mediaType);
            return client;
        }
    }
}
