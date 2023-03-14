using System.Net;
using System.Net.Http.Headers;
using System.Security.Authentication;

namespace HttpClientRequests;

public class HttpClientSingleton
{
    private static readonly Lazy<HttpClient> _instance = new(() =>
    {
        var httpClientHandler = new HttpClientHandler
        {

            Proxy = new WebProxy("http://127.0.0.1:81")
            {
                Credentials = new NetworkCredential("admin",
                "$uper4dmin")
            },


            //SslProtocols = SslProtocols.Tls13,
            AutomaticDecompression = DecompressionMethods.All
        };

        var httpClient = new HttpClient(httpClientHandler)
        {
            DefaultRequestVersion = new Version(1, 0),
            //DefaultRequestHeaders = { ConnectionClose = true },
            MaxResponseContentBufferSize = int.MaxValue
        };
        //httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
        //httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
        //httpClient.DefaultRequestHeaders.Add("Connection", "Close");
        httpClient.DefaultRequestHeaders.ExpectContinue = false;
        //ServicePointManager.FindServicePoint(new Uri("https://localhost:7064"))
        //    .ConnectionLimit = int.MaxValue;
        //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;

        return httpClient;
    });

    private HttpClientSingleton()
    {
    }

    public static HttpClient Instance => _instance.Value;
}