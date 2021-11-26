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

            //判断是否可以进入游戏
            if (room.GetPlayerCount() == Room.MAX_PLAYER)
            {
                room.StartGame();
            }
        }

        /// <summary>
        /// 玩家取消匹配
        /// </summary>
        /// <param name="c"></param>
        /// <param name="msgBase"></param>
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

        /// <summary>
        /// 收到选牌消息
        /// </summary>
        /// <param name="c"></param>
        /// <param name="msgBase"></param>
        public static void MsgChooseCard(ClientState c,MsgBase msgBase)
        {
            Console.WriteLine("MsgChooseCard");
            MsgChooseCard msg = (MsgChooseCard)msgBase;
            Room room = c.player.room;
            room.ChooseCard(msg);
        }

        /// <summary>
        /// 下一局选择消息
        /// </summary>
        /// <param name="c"></param>
        /// <param name="msgBase"></param>
        public static void MsgNextBattle(ClientState c,MsgBase msgBase)
        {
            Console.WriteLine("MsgNextBattle");
            MsgNextBattle msg = (MsgNextBattle)msgBase;
            Room room = c.player.room;
            room.ChooseNextBattle(msg);
        }
    }
}
