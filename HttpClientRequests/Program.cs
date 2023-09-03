using System.Diagnostics;
using System.Net;
using System.Runtime.Intrinsics.Arm;
using System.Threading.Tasks;

namespace HttpClientRequests;

public static class Program
{
    private const int TEST_COUNT = 100_000_000;

    public static async Task Main()
    {
        var proxyUserTasks = new List<Task>();

        var success = 0;
        var total = 0;

        var globalStopWatch = new Stopwatch();
        double rps,  successRate = 0;
        double rpm = 0;
        globalStopWatch.Start();

        var client = HttpClientSingleton.Instance;

        var response = await client.GetAsync($"https://localhost:7064/test"); // letting the proxy server set it's authentication cache before stress test
        response.EnsureSuccessStatusCode();
        Console.WriteLine(await response.Content.ReadAsStringAsync());
        
        await Task.Delay(500);

        var tasks = new List<Task>();
        while (total < TEST_COUNT)
        {
            var x = Task.Run(async () =>
            {
                try
                {
                    var response = await client.GetAsync($"https://localhost:7064/test");
                    //response.EnsureSuccessStatusCode();
                    

                    //Console.WriteLine(await response.Content.ReadAsStringAsync());

                    if (response.IsSuccessStatusCode)
                        Interlocked.Increment(ref success);
                    else
                        Console.WriteLine(response.StatusCode);

                    //await Task.Delay(1000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + " " + ex.InnerException?.Message);
                }
                finally
                {
                    Interlocked.Increment(ref total);
                }
            });
            tasks.Add(x);

            if (tasks.Count >= 500)
            {
                rps = success / globalStopWatch.Elapsed.TotalSeconds;
                rpm = rps * 60;
                successRate = success / (float)total * 100.00;
                Console.Title = $"{rpm} {tasks.Count}";
                //Task.WaitAny(tasks.ToArray());
                tasks.RemoveAll(t => t.IsCompleted);
            }
        }

        Task.WhenAll(proxyUserTasks)
            .Wait();
        Console.ReadKey();
    }
}