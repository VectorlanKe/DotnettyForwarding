using DotNetty.Codecs.Http;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IDotnetty.Forwarding
{
    public interface IClient<T>:IDisposable
    {
        
        Task SendAsync(EndPoint endPoint, T msg, Action<IChannelHandlerContext, IFullHttpResponse> rollbackAction);
        Task ShutdownGracefullyAsync();
        Task ShutdownGracefullyAsync(TimeSpan quietPeriod, TimeSpan shutdownTimeout);
    }
}
