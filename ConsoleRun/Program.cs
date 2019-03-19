using Dotnetty.Forwarding.HttpClient;
using Dotnetty.Forwarding.HttpServer;
using DotNetty.Buffers;
using DotNetty.Codecs.Http;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Libuv;
using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HttpVersion = DotNetty.Codecs.Http.HttpVersion;

namespace ConsoleRun
{
    class Program
    {
        static void Main(string[] args) => RunServer().Wait();

        public static async Task RunServer()
        {
            HttpServer httpServer = null;
            Random random = new Random();
            HttpClient[] httpClients = new HttpClient[] {
                new HttpClient(),
                new HttpClient(),
                new HttpClient(),
                new HttpClient(),
                new HttpClient(),
                new HttpClient(),
            };
            try
            {
                httpServer = new HttpServer(
                async (context, request) => 
                {
                    try
                    {
                        //Console.WriteLine(request.Uri);
                        string[] urlStrs = Regex.Split(request.Uri, "(/\\d+)|\\?");
                        Uri uri = new Uri($"http://127.0.0.1:5000/api{request.Uri}");
                        DefaultFullHttpRequest requestClient = new DefaultFullHttpRequest(HttpVersion.Http11, HttpMethod.Get, uri.ToString(), request.Content.Copy(),request.Headers, request.Headers);
                        HttpHeaders headers = requestClient.Headers;
                        headers.Set(HttpHeaderNames.Host, uri.Authority);
                        await httpClients[random.Next(httpClients.Length)].SendAsync(new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port), requestClient,
                        async (contextClient, response) =>
                        {
                            //Console.WriteLine(response.Status);
                            //string showData = response.Content.ToString(Encoding.UTF8);
                            //Console.WriteLine(showData);
                            //Console.WriteLine();
                            //Unpooled.WrappedBuffer(Encoding.UTF8.GetBytes("hello"))
                            DefaultFullHttpResponse respon = new DefaultFullHttpResponse(response.ProtocolVersion, response.Status, response.Content.Copy(), response.Headers, response.Headers);
                            response.SafeRelease();
                            await contextClient.Channel.CloseAsync();
                            if (context.Channel.IsWritable)
                            {
                                await contextClient.CloseAsync();
                                await context.WriteAndFlushAsync(respon);
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        //if (context.Channel.IsWritable)
                        //{
                        //    var response = new DefaultFullHttpResponse(HttpVersion.Http11, HttpResponseStatus.NotFound, Unpooled.Empty, false);
                        //    await context.WriteAndFlushAsync(response);
                        //}
                        //await context.CloseAsync();
                    }
                });
                await httpServer.RunBindAsync(IPAddress.IPv6Any, 5001);
                Console.WriteLine("服务启动成功！");
                Console.ReadKey();
            }
            finally
            {
                //httpClient.Dispose();
                httpServer.Dispose();
            }
        }
    }
}
