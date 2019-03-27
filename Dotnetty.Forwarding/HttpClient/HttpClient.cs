using DotNetty.Codecs.Http;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using DotNetty.Transport.Libuv;
using IDotnetty.Forwarding;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Dotnetty.Forwarding.HttpClient
{
    public class HttpClient : IClient<DefaultFullHttpRequest>
    {
        private readonly IEventLoopGroup groupClient;
        private readonly Bootstrap bootstrapClient;
        private Dictionary<EndPoint, IChannel> channelDiction = new Dictionary<EndPoint, IChannel>();
        public HttpClient()
        {
            groupClient = new DispatcherEventLoopGroup();
            bootstrapClient = new Bootstrap()
                .Group(groupClient)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Handler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;
                    pipeline.AddLast(new HttpResponseDecoder(4096, 8192, 8192, false));
                    pipeline.AddLast(new HttpObjectAggregator(1024));
                    pipeline.AddLast(new HttpRequestEncoder());
                    pipeline.AddLast(new HttpContentDecompressor());//解压
                    //pipeline.AddLast(new HttpClientHandler(rollbackAction));
                }));
        }

        private async Task<IChannel> ConnectAsync(EndPoint endPoint)
        {
            IChannel channel=null;
            if (channelDiction.ContainsKey(endPoint))
            {
                channel = channelDiction.GetValueOrDefault(endPoint);
            }
            //ollbackAction = rollback;
            if (channel==null|| !channel.Open)
            {
                channel = await bootstrapClient.ConnectAsync(endPoint);
                channelDiction[endPoint]= channel;
            }
            return channel;
            //return await bootstrapClient.ConnectAsync(endPoint);
        }

        public async Task SendAsync(EndPoint endPoint, DefaultFullHttpRequest msg, Action<IChannelHandlerContext, IFullHttpResponse> rollbackAction)
        {
            IChannel channel = await ConnectAsync(endPoint);
            channel.Pipeline.AddLast(new HttpClientHandler(rollbackAction));
            await channel.WriteAndFlushAsync(msg);
            msg.SafeRelease();
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    channelDiction.Clear();
                    bootstrapClient.Clone();
                    groupClient.ShutdownGracefullyAsync();
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~HttpClient() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
