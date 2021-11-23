using GameServer.net;
using GameServer.proto;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.handler
{
    public partial class MsgHandler
    {
        public static void MsgPing(ClientState c,MsgBase msgBase)
        {
            Console.WriteLine("MsgPing");
            c.lastPingTime = NetManager.GetTimeStamp();
            MsgPong msgPong = new MsgPong();
            NetManager.Send(c, msgPong);
        }
    }
}
