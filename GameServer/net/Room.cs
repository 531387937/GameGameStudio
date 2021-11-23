using GameServer.proto;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.net
{
    public class Room
    {
        public List<string> names = new List<string>()
        {
            "神蛊温皇",
            "俏如来",
            "雪山银燕",
            "神田京一",
            "宫本总司",
            "秋水浮萍任缥缈",
            "柳生鬼哭",
            "剑无极",
            "黑白郎君",
            "酆都月",
            "藏镜人"
        };
        //{
        //    "Naruto",
        //    "Sasuke",
        //    "Sakura",
        //    "Kakashi",
        //    "Shikamaru",
        //    "Hinata",
        //    "Jiraiya",
        //    "Orochimaru",
        //    "Tsunade"
        //};
        public Dictionary<int, Player> players = new Dictionary<int, Player>();
        public const int MAX_PLAYER = 3;

        /// <summary>
        /// 获取玩家个数
        /// </summary>
        /// <returns></returns>
        public int GetPlayerCount()
        {
            return players.Count;
        }

        /// <summary>
        /// 获取所有玩家
        /// </summary>
        /// <returns></returns>
        public List<Player> GetPlayers()
        {
            return new List<Player>(players.Values);
        }

        /// <summary>
        /// 随机获取不重复的名字
        /// </summary>
        /// <returns></returns>
        public string GetRandomName()
        {
            Random rd = new Random();
            int idx = -1;
            for(int i = 0; i < 10000&&idx==-1;i++)
            {
                idx = rd.Next(0,names.Count);
                foreach(var player in players.Values)
                {
                    if (names[idx] == player.data.playerName)
                    {
                        idx = -1;
                    }
                }
            }

            if (idx == -1)
                return (DateTime.Now.ToString());
            else
                return names[idx];
        }

        /// <summary>
        /// 添加或者修改玩家编号，从小排起
        /// </summary>
        /// <param name="player"></param>
        public void SetPlayer(Player player)
        {
            for(int i = 1; i <= MAX_PLAYER; i++)
            {
                if (!players.ContainsKey(i))
                {
                    players.Add(i, player);
                    player.room = this;
                }
            }
        }

        /// <summary>
        /// 删除玩家
        /// </summary>
        /// <param name="id"></param>
        public void RemovePlayer(int id)
        {
            if (players.ContainsKey(id))
            {
                players.Remove(id);

                //把剩余的往前移
                for (int i = 1; i < MAX_PLAYER; i++)
                {
                    if (players.ContainsKey(i))
                    {
                        players.Remove(i);
                        SetPlayer(players[i]);
                    }
                }
            }
        }

        /// <summary>
        /// 获取房间信息
        /// </summary>
        /// <param name="localPlayerId"></param>
        /// <returns></returns>
        public MsgRoomInfo GetRoomInfo(int localPlayerId)
        {
            MsgRoomInfo roomInfo = new MsgRoomInfo();
            roomInfo.localPlayer = new PlayerInfo(localPlayerId, players[localPlayerId].data.playerName);
            foreach(var i in players.Keys)
            {
                if (i != localPlayerId)
                {
                    roomInfo.remotePlayers.Add(new PlayerInfo(i, players[i].data.playerName));
                }
            }

            return roomInfo;
        }
    }
}
