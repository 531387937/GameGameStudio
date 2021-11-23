using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.net
{
    public class RoomManager
    {
        //房间列表
        public static List<Room> rooms = new List<Room>();

        /// <summary>
        /// 获取空闲的房间
        /// </summary>
        /// <returns></returns>
        public static Room GetMatchingRoom()
        {
            foreach(var r in rooms)
            {
                if (r.GetPlayerCount() < Room.MAX_PLAYER)
                {
                    return r;
                }
            }

            //没有合适的，新建房间
            Room room = new Room();
            return room;
        }

        /// <summary>
        /// 关闭房间
        /// </summary>
        /// <param name="room"></param>
        public static void CloseRoom(Room room)
        {
            rooms.Remove(room);

            
            foreach(var player in room.GetPlayers()){
                player.room = null;
            }
        }
    }
}
