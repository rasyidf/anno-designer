using System;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace AnnoDesigner.Core.Helper;

public static class ConnectivityHelper
{
    private const string URL = @"https://www.github.com";
    private const string SECOND_URL = @"https://www.google.com";

    public static async Task<bool> IsConnected()
    {
        bool result = false;

        bool isInternetAvailable = false;

        using (HttpClient httpClient = new())
        {
            httpClient.Timeout = TimeSpan.FromSeconds(5);

            try
            {
                HttpResponseMessage response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, URL)).ConfigureAwait(false);
                isInternetAvailable = response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                isInternetAvailable = false;
            }

            //service outage? try second url
            if (!isInternetAvailable)
            {
                try
                {
                    HttpResponseMessage response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, SECOND_URL)).ConfigureAwait(false);
                    isInternetAvailable = response.IsSuccessStatusCode;
                }
                catch (HttpRequestException)
                {
                    isInternetAvailable = false;
                }
            }
        }

        if (isInternetAvailable)
        {
            result = IsNetworkAvailable;
        }

        return result;
    }

    public static bool IsNetworkAvailable => NetworkInterface.GetIsNetworkAvailable();
}
