using GameServer.net;
using GameServer.proto;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.handler
{
    public partial class MsgHandler
    {
        /// <summary>
        /// 匹配
        /// </summary>
        /// <param name="c"></param>
        /// <param name="msgBase"></param>
        public static void MsgMatch(ClientState c, MsgBase msgBase)
        {
            Console.WriteLine("MsgMatch");
            //如果有空闲房间,如果没有则新建房间
            Room room = RoomManager.GetMatchingRoom();
            
            Player player = c.player;
            //获取名字
            player.data.playerName = room.GetRandomName();
            //加入房间
            room.SetPlayer(player);
            //给房间的玩家广播消息
            List<Player> players = room.GetPlayers();
            foreach(var p in players)
            {
                MsgRoomInfo msg = room.GetRoomInfo(p.data.id);
                NetManager.Send(c, msg);
            }
        }


    }
}
