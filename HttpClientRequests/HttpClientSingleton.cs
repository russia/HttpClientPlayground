using System.Net;
using System.Net.Http.Headers;

namespace HttpClientRequests;

public class HttpClientSingleton
{
    private static readonly Lazy<HttpClient> _instance = new(() =>
    {
        var httpClientHandler = new HttpClientHandler
        {
            //Proxy = new WebProxy("http://127.0.0.1:444")
            //{
            //    Credentials = new NetworkCredential("admin", "$uper4dmin")
            //}
        };

        var httpClient = new HttpClient(httpClientHandler)
        {
            DefaultRequestHeaders = {ConnectionClose = false},
            MaxResponseContentBufferSize = int.MaxValue
        };
        httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
        httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
        httpClient.DefaultRequestHeaders.ExpectContinue = false;
        ServicePointManager.FindServicePoint(new Uri("https://localhost:7064"))
            .ConnectionLimit = int.MaxValue;

        return httpClient;
    });

    private HttpClientSingleton()
    {
    }

    public static HttpClient Instance => _instance.Value;
}