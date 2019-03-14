using DotNetty.Codecs.Http;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dotnetty.Forwarding
{
    public class HttpClientHandler : SimpleChannelInboundHandler<IFullHttpResponse>
    {
        public override bool IsSharable => true;
        protected override void ChannelRead0(IChannelHandlerContext ctx, IFullHttpResponse msg)
        {
            
        }
    }
}
