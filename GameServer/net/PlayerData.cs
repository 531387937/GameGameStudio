using GameServer.proto;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.net
{
    public class PlayerData
    {
        //id
        public int id = 0;
        //名字
        public string playerName = "";
        //牌组
        public List<CardInfo> cards = new List<CardInfo>();
    }
}
