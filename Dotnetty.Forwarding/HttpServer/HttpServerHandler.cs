using DotNetty.Codecs.Http;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dotnetty.Forwarding.HttpServer
{
    public class HttpServerHandler : SimpleChannelInboundHandler<IFullHttpRequest>
    {
        public Action<IChannelHandlerContext, IFullHttpRequest> httpAction;
        public HttpServerHandler(Action<IChannelHandlerContext,IFullHttpRequest> action)
        {
            httpAction = action;
        }
        protected async override void ChannelRead0(IChannelHandlerContext ctx, IFullHttpRequest msg)
        {
            await Task.Run(()=>httpAction(ctx,msg));
        }
    }
}
