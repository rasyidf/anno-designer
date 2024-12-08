using System;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace AnnoDesigner.Core.Helper
{
    public static class ConnectivityHelper
    {
        private const string URL = @"https://www.github.com";
        private const string SECOND_URL = @"https://www.google.com";
        private const string REQUEST_METHOD_HEAD = "HEAD";

        public static async Task<bool> IsConnected()
        {
            var result = false;

            var isInternetAvailable = await IsUrlAvailable(URL);

            //service outage? try second url
            if (!isInternetAvailable)
            {
                isInternetAvailable = await IsUrlAvailable(SECOND_URL);
            }

            if (isInternetAvailable)
            {
                result = IsNetworkAvailable;
            }

            return result;
        }

        private static async Task<bool> IsUrlAvailable(string url)
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(5);
            try
            {
                var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        public static bool IsNetworkAvailable
        {
            get { return NetworkInterface.GetIsNetworkAvailable(); }
        }
    }
}
