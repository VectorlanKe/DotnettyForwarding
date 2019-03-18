using DotNetty.Buffers;
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
            //DefaultFullHttpResponse respon = new DefaultFullHttpResponse(DotNetty.Codecs.Http.HttpVersion.Http11, HttpResponseStatus.OK, Unpooled.WrappedBuffer(Encoding.UTF8.GetBytes("hello")));
            //await ctx.WriteAndFlushAsync(respon);
            //await ctx.CloseAsync();
            await Task.Run(()=>httpAction(ctx,msg));
        }
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception) => context.CloseAsync();

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();
    }
}
