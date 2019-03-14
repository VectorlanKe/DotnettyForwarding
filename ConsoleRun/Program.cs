using Dotnetty.Forwarding.HttpClient;
using Dotnetty.Forwarding.HttpServer;
using DotNetty.Codecs.Http;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ConsoleRun
{
    class Program
    {
        static void Main(string[] args) => RunServer();
        public static void RunServer()
        {
            using (HttpClient httpClient = new HttpClient(respon =>
            {
                var data = new DefaultFullHttpResponse(DotNetty.Codecs.Http.HttpVersion.Http11, respon.Status, respon.Content);
            }))
            {
                Uri uri = new Uri("http://127.0.0.1:50000/api/values");
                httpClient.SendAsync(
                    new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port),
                    new DefaultFullHttpRequest(DotNetty.Codecs.Http.HttpVersion.Http11, HttpMethod.Get, uri.ToString()));
                Console.ReadKey();

            }
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
        }
    }
}
