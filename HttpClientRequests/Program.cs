using System.Diagnostics;
using System.Text;

namespace HttpClientRequests;

public static class Program
{
    private const int TEST_COUNT = 100_000_000;
    public static async Task Main(string[] args)
    {
        var tasks = new List<Task>();
        var success = 0;
        var total = 0;

        var globalStopWatch = new Stopwatch();
        double rps, rpm, successRate = 0;

        globalStopWatch.Start();

        var client = HttpClientSingleton.Instance;

        //var timer = new Timer(state =>
        //{
        //    rps = success / globalStopWatch.Elapsed.TotalSeconds;
        //    rpm = rps * 60;
        //    successRate = success / (float)total * 100.00;

        //    //Note that this isn't the right CSV format.
        //    File.AppendAllText("C:\\Users\\user\\Desktop\\results.csv", $"{DateTime.Now};{rpm:N2}\n");
        //}, null, 50, 50);

        while (total < TEST_COUNT)
        {
            var x = Task.Run(async () =>
            {
                try
                {
                        //var stopWatch = new Stopwatch();
                        //stopWatch.Start();
                        var response = await client.GetAsync("https://cloudflare.com/cdn-cgi/trace");
                        //stopWatch.Stop();
                        //Console.WriteLine(stopWatch.ElapsedMilliseconds);
                        //Console.WriteLine(response.StatusCode);
                        //Console.WriteLine((await response.Content.ReadAsStringAsync()).Length);
                        //Console.WriteLine(await response.Content.ReadAsStringAsync());
                        response.EnsureSuccessStatusCode();
                        //await Task.Delay(150);
                        Interlocked.Increment(ref success);
                    
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    Interlocked.Increment(ref total);
                }
            });

            tasks.Add(x);
            //break;
            if (tasks.Count > 100)
            {
                rps = success / globalStopWatch.Elapsed.TotalSeconds;
                rpm = rps * 60;
                successRate = success / (float)total * 100.00;
                //await Task.Delay(2000);
                Console.Write($"Success: {success}\tTotal: {total}\tSuccess Rate: {successRate:N2}%\tRPS: {rps:N2}\tRPM: {rpm:N2}\t\n");

                tasks.RemoveAll(t => t.IsCompleted);
            }
        }

        Task.WhenAll(tasks)
            .Wait();
        Console.ReadKey();

        //await timer.DisposeAsync();
    }
}