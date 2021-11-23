using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.net
{
    public class Player
    {
        //指向ClientState
        public ClientState state;
        //数据库数据
        public PlayerData data;
        //房间
        public Room room;

        //构造函数
        public Player(ClientState state)
        {
            this.state = state;
        }

        //发送消息
        public void Send(MsgBase msgBase)
        {
            NetManager.Send(state, msgBase);
        }
    }
}
