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
        var result = false;

        var isInternetAvailable = false;

        using (var httpClient = new HttpClient())
        {
            httpClient.Timeout = TimeSpan.FromSeconds(5);

            try
            {
                var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, URL)).ConfigureAwait(false);
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
                    var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, SECOND_URL)).ConfigureAwait(false);
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

    public static bool IsNetworkAvailable
    {
        get { return NetworkInterface.GetIsNetworkAvailable(); }
    }
}
