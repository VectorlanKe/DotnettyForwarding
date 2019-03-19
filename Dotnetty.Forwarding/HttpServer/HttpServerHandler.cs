using DotNetty.Buffers;
using DotNetty.Codecs.Http;
using DotNetty.Common.Utilities;
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
            :base(false)
        {
            httpAction = action;
        }
        protected override void ChannelRead0(IChannelHandlerContext ctx, IFullHttpRequest msg)
        {
            //byte[] test = Encoding.UTF8.GetBytes("hello");
            //DefaultFullHttpResponse respon = new DefaultFullHttpResponse(HttpVersion.Http11, HttpResponseStatus.OK, Unpooled.WrappedBuffer(test));
            //HttpHeaders headers = respon.Headers;
            //headers.Set(HttpHeaderNames.ContentType, AsciiString.Cached("text/plain"));
            //headers.Set(HttpHeaderNames.ContentLength, test.Length);
            //ctx.WriteAndFlushAsync(respon);
            //await ctx.CloseAsync();
            httpAction(ctx,msg);
        }
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception) => context.CloseAsync();

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();
    }
}
