using dotnet_etcd;
using Dotnetty.Forwarding.Commonality;
using Dotnetty.Forwarding.HttpClient;
using Dotnetty.Forwarding.HttpServer;
using DotNetty.Buffers;
using DotNetty.Codecs.Http;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Libuv;
using System;
using System.Collections.Generic;
using System.Linq;
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
            IDictionary<ulong, HttpClient> httpClients = new Dictionary<ulong, HttpClient>();
            httpClients.Add(HashConsistent.Md5Hash("key1"), new HttpClient());
            httpClients.Add(HashConsistent.Md5Hash("key2"), new HttpClient());
            httpClients.Add(HashConsistent.Md5Hash("key3"), new HttpClient());
            httpClients.Add(HashConsistent.Md5Hash("key4"), new HttpClient());
            EtcdClient etcdClient = new EtcdClient("127.0.0.1", 2379);
            HttpClient httpClient = new HttpClient();
            try
            {
                httpServer = new HttpServer(
                async (context, request) => 
                {
                    try
                    {
                        //Console.WriteLine(request.Uri);
                        string urlStrs = Regex.Split(request.Uri, "(/\\d+)|\\?").FirstOrDefault();
                        //HashConsistent
                        var urls = etcdClient.GetRangeVal($"{urlStrs?.ToLower()}#")?.ToDictionary(f=>HashConsistent.Md5Hash(f.Key),v=>v.Value);
                        if (urls.Count<1)
                        {
                            if (context.Channel.IsWritable)
                            {
                                DefaultFullHttpResponse respon = new DefaultFullHttpResponse(HttpVersion.Http11, HttpResponseStatus.NotFound, Unpooled.Empty, false);
                                await context.WriteAndFlushAsync(respon);
                                respon.SafeRelease();
                                request.SafeRelease();
                                //await context.CloseAsync();
                            }
                            return;
                        }
                        string urlStr = HashConsistent.GetTargetValues(urls, request.Uri);
                        Uri uri = new Uri($"{urlStr}{request.Uri.Replace(urlStrs,string.Empty)}");
                        DefaultFullHttpRequest requestClient = new DefaultFullHttpRequest(HttpVersion.Http11, HttpMethod.Get, uri.ToString(), request.Content,request.Headers, request.Headers);
                        HttpHeaders headers = requestClient.Headers;
                        headers.Set(HttpHeaderNames.Host, uri.Authority);
                        //await httpClient.SendAsync(new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port), requestClient,
                        await HashConsistent.GetTargetValues(httpClients, urlStr).SendAsync(new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port), requestClient,
                        async (contextClient, response) =>
                        {
                            DefaultFullHttpResponse respon = new DefaultFullHttpResponse(response.ProtocolVersion, response.Status, response.Content, response.Headers, response.Headers);
                            await contextClient.Channel.CloseAsync();
                            if (context.Channel.IsWritable)
                            {
                                await contextClient.CloseAsync();
                                await context.WriteAndFlushAsync(respon);
                            }
                            respon.SafeRelease();
                            response.SafeRelease();
                        });
                        request.SafeRelease();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
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
