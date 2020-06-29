using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using ScibuAPIConnector.Models;

namespace ScibuAPIConnector.Services
{
    public class AuthorizationService
    {
        public string grantType = "password";
        public string device = "iOS";
        public string url = "https://api.scibu.com/token";
     //  public string url = "http://localhost:3366/token";

        public Token GetToken(string userName, string password, string clientId, string clientSecret)
        {
            var client = new HttpClient();
            var pairs = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>( "grant_type", grantType ),
                        new KeyValuePair<string, string>( "username", userName ),
                        new KeyValuePair<string, string>( "password", password ),
                        new KeyValuePair<string, string>( "client_id", clientId ),
                        new KeyValuePair<string, string>( "client_secret", clientSecret ),
                        new KeyValuePair<string, string>( "device", device ),
                    };

            var content = new FormUrlEncodedContent(pairs);
            var response = client.PostAsync(url, content).Result;
            var result = response.Content.ReadAsStringAsync().Result;
            var tokenDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);

            return new Token(tokenDictionary["access_token"], tokenDictionary["token_type"], Int32.Parse(tokenDictionary["expires_in"]), tokenDictionary["refresh_token"],
                tokenDictionary["fullname"], tokenDictionary["as:client_id"], tokenDictionary[".issued"], tokenDictionary[".expires"]);
        }
    }
}
