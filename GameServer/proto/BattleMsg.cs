using GameServer.net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.proto
{
    /// <summary>
    /// 卡牌花色
    /// </summary>
    [Serializable]
    public enum CardColor
    {
        ZhiWu,//植物
        JianZhu,//建筑
        ShanShui,//山水
    }

    /// <summary>
    /// 卡牌类
    /// </summary>
    [Serializable]
    public class CardInfo
    {
        public CardColor cardColor;//花色
        public int num;//点数

        public CardInfo()
        {
            cardColor = CardColor.JianZhu;
            num = 0;
        }
        public CardInfo(CardColor color, int n)
        {
            cardColor = color;
            num = n;
        }
    }

    /// <summary>
    /// 初始卡牌类
    /// </summary>
    [Serializable]
    public class MsgInitCards : MsgBase
    {
        public MsgInitCards()
        {
            protoName = "MsgInitCards";
        }

        public Dictionary<int, List<CardInfo>> Cards = new Dictionary<int, List<CardInfo>>();//<playerID, 卡牌列表>
    }

    /// <summary>
    /// 下回合发牌类
    /// </summary>
    [Serializable]
    public class MsgRoundCards : MsgBase
    {
        public MsgRoundCards()
        {
            protoName = "MsgRoundCards";
        }

        public Dictionary<int, List<CardInfo>> Cards = new Dictionary<int, List<CardInfo>>();//<playerID, 卡牌列表>
    }

    /// <summary>
    /// 玩家选牌类
    /// </summary>
    [Serializable]
    public class MsgChooseCard : MsgBase
    {
        public MsgChooseCard()
        {
            protoName = "MsgChooseCard";
        }

        public int playerID;
        public CardInfo card;
    }

    [Serializable]
    public enum CardsType
    {
        ZhaDan,//炸弹
        TongHuaShun,//同花顺
        TongShuZi,//同数字
        ShunZi,//顺子
        TongHua,//同花
        TongSeYiDui,//同色一对
        YiSeYiDui,//异色一对
        DanZhang,//单张
    }

    /// <summary>
    /// 回合结果
    /// </summary>
    [Serializable]
    public class RoundResult
    {
        public int rank;//排名
        public CardsType cardsType;//牌型
    }

    /// <summary>
    /// 回合对比结果类
    /// </summary>
    [Serializable]
    public class MsgRoundResult : MsgBase
    {
        public MsgRoundResult()
        {
            protoName = "MsgRoundResult";
        }

        public Dictionary<int, RoundResult> result = new Dictionary<int, RoundResult>();
    }

    /// <summary>
    /// 胡牌类型
    /// None代表没胡牌
    /// </summary>
    [Serializable]
    public enum WinType
    {
        None,//没胡牌
        ZhiWu,//植物
        JianZhu,//建筑
        ShanShui,//山水
        BaoDi,//保底
        XiaoHe,//小和
    }

    /// <summary>
    /// 对局结果类
    /// </summary>
    [Serializable]
    public class MsgBattleResult : MsgBase
    {
        public MsgBattleResult()
        {
            protoName = "MsgBattleResult";
        }

        public Dictionary<int, WinType> result = new Dictionary<int, WinType>();//<playerID, 番型>
    }
}
