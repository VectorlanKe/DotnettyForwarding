using DotNetty.Codecs.Http;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dotnetty.Forwarding.HttpClient
{
    public class HttpClientHandler : SimpleChannelInboundHandler<IFullHttpResponse>
    {
        public Action<IChannelHandlerContext,IFullHttpResponse> rollback;
        public override bool IsSharable => true;
        protected override void ChannelRead0(IChannelHandlerContext ctx, IFullHttpResponse msg)
        {
            rollback(ctx,msg);
        }
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception) => context.CloseAsync();

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();
    }
}
