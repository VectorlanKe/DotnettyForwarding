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
        
        Task SendAsync(EndPoint endPoint, T msg);
        Task ShutdownGracefullyAsync();
        Task ShutdownGracefullyAsync(TimeSpan quietPeriod, TimeSpan shutdownTimeout);
    }
}
