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
        public static void MsgStartMatch(ClientState c, MsgBase msgBase)
        {
            Console.WriteLine("MsgStartMatch");

            //如果已经在房间
            if (c.player != null )
            {
                return;
            }

            //新建玩家实例
            c.player = new Player(c);

            //如果有空闲房间,如果没有则新建房间
            Room room = RoomManager.GetMatchingRoom();
            
            Player player = c.player;
            //获取名字
            player.data.playerName = room.GetRandomName();
            //加入房间
            room.SetPlayer(player);
            //给房间的玩家广播消息
            room.SendRoomInfo();
        }

        public static void MsgCancelMatch(ClientState c,MsgBase msgBase)
        {
            Console.WriteLine("MsgCancelMatch");

            //从房间中移除该玩家
            Room room = c.player.room;
            room.RemovePlayer(c.player.data.id);
            //广播消息
            room.SendRoomInfo();
            //删除玩家的实例
            c.player = null;
        }
    }
}
