using System;
using System.Collections.Generic;
using System.Text;
using GameServer.net;

namespace GameServer.proto
{
    public class MsgPing : MsgBase
    {
        public MsgPing()
        {
            protoName = "MsgPing";
        }
    }

    public class MsgPong : MsgBase
    {
        public MsgPong()
        {
            protoName = "MsgPong";
        }
    }
}
