using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace GameServer.net
{
    public class ClientState
    {
        public Socket socket;
        public ByteArray readBuff = new ByteArray();
        //玩家数据
        public long lastPingTime = 0;
        //玩家
        public Player player;
    }
}
