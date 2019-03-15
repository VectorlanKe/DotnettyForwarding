using Dotnetty.Forwarding.HttpClient;
using Dotnetty.Forwarding.HttpServer;
using DotNetty.Codecs.Http;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRun
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(async () => await RunServer());
            Console.ReadKey();
        }
        public async static Task RunServer()
        {
            //using (HttpServer httpServer = new HttpServer(async (context,reques) =>
            //{
            //    using (HttpClient httpClient = new HttpClient(respon =>
            //    {
            //        context.WriteAndFlushAsync(new DefaultFullHttpResponse(DotNetty.Codecs.Http.HttpVersion.Http11, respon.Status,respon.Content));
            //    }))
            //    {
            //        Uri uri = new Uri("http://127.0.0.1:50000/api/values");
            //        await httpClient.SendAsync(
            //            new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port),
            //            new DefaultFullHttpRequest(DotNetty.Codecs.Http.HttpVersion.Http11, reques.Method, uri.ToString()));
            //    }
            //}))
            //{
            //    httpServer.RunBindAsync(IPAddress.IPv6Any, 5001);
            //    Console.ReadKey();
            //}
            for (; ; )
            {
                try
                {
                    Console.WriteLine("按任意键继续...");
                    var der = Console.ReadLine();
                    bool isOk = false;
                    using (HttpClient httpClient = new HttpClient(
                    reques =>
                    {
                        Console.WriteLine(reques.Status);
                        Console.WriteLine(reques.Content.ToString(Encoding.UTF8));
                        isOk = true;
                    }))
                    {
                        Uri uri = new Uri("http://127.0.0.1:5000/api/values");
                        DefaultFullHttpRequest request = new DefaultFullHttpRequest(DotNetty.Codecs.Http.HttpVersion.Http11, HttpMethod.Get, uri.ToString());
                        HttpHeaders headers = request.Headers;
                        headers.Set(HttpHeaderNames.Host, uri.Authority);
                        await httpClient.SendAsync(new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port), request);
                        while (true)
                        {
                            if (isOk)
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

        }
    }
}
