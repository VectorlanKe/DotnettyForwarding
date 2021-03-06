﻿using DotNetty.Codecs.Http;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Libuv;
using IDotnetty.Forwarding;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Dotnetty.Forwarding.HttpServer
{
    public class HttpServer : IServer
    {
        private IEventLoopGroup group, workGroup;
        private ServerBootstrap serverBootstrap;
        public HttpServer(Action<IChannelHandlerContext, IFullHttpRequest> action)
        {
            var dispatcher = new DispatcherEventLoopGroup();
            group = dispatcher;
            workGroup = new WorkerEventLoopGroup(dispatcher);
            serverBootstrap = new ServerBootstrap()
                            .Group(group, workGroup)
                            .Channel<TcpServerChannel>()
                            .Option(ChannelOption.SoBacklog, 8192)
                            .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                            {
                                IChannelPipeline pipeline = channel.Pipeline;
                                pipeline.AddLast(new HttpRequestDecoder(4096, 8192, 8192, false));
                                pipeline.AddLast(new HttpObjectAggregator(1048576));
                                //pipeline.AddLast(new HttpContentCompressor());//压缩
                                pipeline.AddLast(new HttpResponseEncoder());
                                pipeline.AddLast(new HttpServerHandler(action));
                            }));
        }
        public Task<IChannel> RunBindAsync(IPAddress inetHost, int inetPort)
        {
            return serverBootstrap.BindAsync(inetHost, inetPort);
        }

        public async Task ShutdownGracefullyAsync()
        {
            await workGroup.ShutdownGracefullyAsync();
            await group.ShutdownGracefullyAsync();
        }
        public async Task ShutdownGracefullyAsync(TimeSpan quietPeriod, TimeSpan shutdownTimeout)
        {
            await workGroup.ShutdownGracefullyAsync(quietPeriod, shutdownTimeout);
            await group.ShutdownGracefullyAsync(quietPeriod, shutdownTimeout);
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~HttpServer() {
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
