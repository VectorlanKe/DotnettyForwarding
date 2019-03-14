using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IDotnetty.Forwarding
{
    public interface IServer :IDisposable
    {
        Task<IChannel> RunBindAsync(IPAddress inetHost, int inetPort);
        Task ShutdownGracefullyAsync();
        Task ShutdownGracefullyAsync(TimeSpan quietPeriod, TimeSpan shutdownTimeout);
    }
}
