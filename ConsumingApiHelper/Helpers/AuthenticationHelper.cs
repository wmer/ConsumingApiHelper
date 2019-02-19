using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace ConsumingApiHelper.Helpers {
    public class AuthenticationHelper {
        private HttpClient _client;
        private string _authenticationUri;

        public AuthenticationHelper(HttpClient client, string authenticationUri) {
            this._client = client;
            this._authenticationUri = authenticationUri;
        }

        public string Authenticate(string username, string password) {
            string token = null;
            var data = new Dictionary<string, string>() {
                    { "username", username },
                    { "password", password }
                };
            using (var requestContent = new FormUrlEncodedContent(data)) {
                var response = _client.PostAsync(_authenticationUri, requestContent).Result;
                if (response.IsSuccessStatusCode) {
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    var tokenData = JObject.Parse(responseContent);
                    token = (string)tokenData["token"];
                }
            }
            return token;
        }
    }
}
