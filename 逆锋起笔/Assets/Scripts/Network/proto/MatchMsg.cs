using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network.proto
{
    public class MsgStartMatch : MsgBase
    {
        /// <summary>
        /// 申请匹配类，不需要携带其他信息
        /// </summary>
        public MsgStartMatch()
        {
            protoName = "MsgStartMatch";
        }
    }

    /// <summary>
    /// 玩家信息类
    /// </summary>
    public class PlayerInfo
    {
        public int playerId = 0;//默认为0，根据进入房间的顺序分配：1、2、3
        public string playerName = "";

        public PlayerInfo(int id, string name)
        {
            playerId = id;
            playerName = name;
        }
    }

    /// <summary>
    /// 房间信息类
    /// 包括本地玩家信息、其他玩家信息的列表
    /// 通过判断remotePlayers的长度，可以知道是否满员，即是否可以开局
    /// </summary>
    public class MsgRoomInfo : MsgBase
    {
        public MsgRoomInfo()
        {
            protoName = "MsgRoomInfo";
        }

        public PlayerInfo localPlayer;
        public List<PlayerInfo> remotePlayers = new List<PlayerInfo>();
    }

    /// <summary>
    /// 下一局选择类
    /// </summary>
    /// </summary>
    public class MsgNextBattle : MsgBase
    {
        public MsgNextBattle()
        {
            protoName = "MsgNextBattle";
        }

        public int playerID;
        public bool choice;//true表示继续下一局
    }
}
