using Dotnetty.Forwarding.HttpClient;
using Dotnetty.Forwarding.HttpServer;
using DotNetty.Buffers;
using DotNetty.Codecs.Http;
using DotNetty.Common.Utilities;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRun
{
    class Program
    {
        static void Main(string[] args) => RunServer();
        public static void RunServer()
        {
            Task.Run(async () =>
            {
                //HttpClient httpClient = new HttpClient(
                //reques =>
                //{
                //    Console.WriteLine(reques.Status);
                //    Console.WriteLine(reques.Content.ToString(Encoding.UTF8));
                //    Console.WriteLine();
                //});
                //for (; ; )
                //{
                //    Console.WriteLine("按任意键继续...");
                //    Console.ReadKey(true);
                //    Uri uri = new Uri("http://127.0.0.1:5000/api/values");
                //    DefaultFullHttpRequest request = new DefaultFullHttpRequest(DotNetty.Codecs.Http.HttpVersion.Http11, HttpMethod.Get, uri.ToString());
                //    HttpHeaders headers = request.Headers;
                //    headers.Set(HttpHeaderNames.Host, uri.Authority);
                //    await httpClient.SendAsync(new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port), request);
                //}
                HttpClient httpClient = new HttpClient();
                HttpServer httpServer = new HttpServer(
                async (context, request) =>
                {
                    Console.WriteLine(request.Uri);
                    Uri uri = new Uri("http://127.0.0.1:5000/api/values");
                    DefaultFullHttpRequest requestClient = new DefaultFullHttpRequest(DotNetty.Codecs.Http.HttpVersion.Http11, HttpMethod.Get, uri.ToString());
                    HttpHeaders headers = requestClient.Headers;
                    headers.Set(HttpHeaderNames.Host, uri.Authority);
                    await httpClient.SendAsync(new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port), requestClient,
                    async(clientConet, response) => 
                    {
                        Console.WriteLine(response.Status);
                        string showData = response.Content.ToString(Encoding.UTF8);
                        Console.WriteLine(showData);
                        Console.WriteLine();
                        //IByteBuffer byteBuffer = Unpooled.WrappedBuffer(Encoding.UTF8.GetBytes(showData));
                        DefaultFullHttpResponse respon = new DefaultFullHttpResponse(DotNetty.Codecs.Http.HttpVersion.Http11, response.Status, response.Content.Copy(), response.Headers, response.Headers);
                        await context.WriteAndFlushAsync(respon);
                        //context.FireChannelRead(request);
                        await context.CloseAsync();
                    });
               
                });
            await httpServer.RunBindAsync(IPAddress.IPv6Any, 5001);
            Console.WriteLine("服务启动成功！");
            Console.ReadKey();
        }).Wait();

    }
}
}
