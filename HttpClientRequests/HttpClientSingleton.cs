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
                Credentials = new NetworkCredential("admin", "super4dmin"),
            },
            //SslProtocols = SslProtocols.None // or SslProtocols.Tls11, SslProtocols.Tls, etc.
            //AutomaticDecompression = DecompressionMethods.All
        };

        var httpClient = new HttpClient(httpClientHandler)
        {
            //DefaultRequestHeaders = { ConnectionClose = true },
            MaxResponseContentBufferSize = int.MaxValue,
        };

        //httpClient.DefaultRequestVersion = HttpVersion.Version20;
        //httpClient.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
        
        //httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
        //httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
        
        // httpClient.DefaultRequestHeaders.ExpectContinue = false;
        ServicePointManager.FindServicePoint(new Uri("https://localhost:7064"))
            .ConnectionLimit = int.MaxValue;

        return httpClient;
    });

    private HttpClientSingleton()
    {
    }

    public static HttpClient Instance => _instance.Value;
}