using PayPal.Api;
using System.Collections.Generic;

namespace GestioniDirette.Service.PayPal
{
    public static class Configuration
    {
        public readonly static string ClientId;
        public readonly static string ClientSecret;

        // Static constructor for setting the readonly static members.
        static Configuration()
        {
            var config = GetConfig();
            ClientId = config["clientId"];
            ClientSecret = config["clientSecret"];
        }

        // Create the configuration map that contains mode and other optional configuration details.
        public static Dictionary<string, string> GetConfig()
        {
            return ConfigManager.Instance.GetProperties();
        }

        /// <summary>
        /// Metodo statico GetAccessToken() 
        /// </summary>
        /// <returns>accessToken</returns>
        private static string GetAccessToken()
        {
            // ###AccessToken
            // Retrieve the access token from
            // OAuthTokenCredential by passing in
            // ClientID and ClientSecret
            // It is not mandatory to generate Access Token on a per call basis.
            // Typically the access token can be generated once and
            // reused within the expiry window                
            string accessToken = new OAuthTokenCredential(ClientId, ClientSecret, GetConfig()).GetAccessToken();
            return accessToken;
        }

        // Returns APIContext object
        public static APIContext GetAPIContext(string accessToken = "ec-9dt36261vj5999711")
        {
            var apiContext = new APIContext(GetAccessToken()/*string.IsNullOrEmpty(accessToken) ? GetAccessToken() : accessToken*/);
            apiContext.Config = GetConfig();
            return apiContext;
        }

    }
}