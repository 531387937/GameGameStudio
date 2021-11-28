using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//花色
public enum CardColor
{
    plant,yard,moutain
}

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

public enum WinType
{
    None,//没胡牌
    ZhiWu,//植物
    JianZhu,//建筑
    ShanShui,//山水
    BaoDi,//保底
    XiaoHe,//小和
}
[System.Serializable]
public class Card
{
    private CardColor color;
    public Sprite tex;
    private int rank;
    public Card(CardColor type,int point,string tex)
    {
        color = type;
        rank = point;
        int num = 1;
        if (point < 3)
        {
            num = 1;
        }
        else if (point < 5)
        {
            num = 2;
        }
        else
            num = 3;
        this.tex = Resources.Load<Sprite>(tex + num.ToString());
    }

    public CardColor getCardColor()
    {
        return color;
    }
    public int getCardRank()
    {
        return rank;
    }
}

public struct PlayerGroundCard
{
    public bool used;       //用于检测是否已用于更新数据
    public int id;          //玩家ID

    public int plantNum;    //植物牌数
    public int plantSum;    //植物总点数

    public int moutainNum;  //山水牌数
    public int moutainSum;  //山水总点数

    public int yardNum;     //庭院牌数
    public int yardSum;     //庭院总点数

    public int pointSum { get { return (plantSum + moutainSum + yardSum); } }       //总点数
    public int numSum { get { return (plantNum + moutainNum + yardNum); } }         //总牌数
}

public class Player
{
    //玩家唯一标识符
    public readonly int id;
    public readonly string playerName;

    public PlayerGroundCard groundState;

    public Player(int id,string name)
    {
        this.id = id;
        playerName = name;
        handCards.Clear();
        groundCard.Clear();
        curCard.Clear();
        groundState = new PlayerGroundCard();
        groundState.used = false;
        groundState.id = id;
    }
    public CardsType curRoundCard;
    public WinType wintype = WinType.None;
    //现在的手牌
    public List<Card> handCards = new List<Card>();
    //现在留在场上的牌
    public List<Card> groundCard = new List<Card>();
    //本回合的出牌
    public List<Card> curCard = new List<Card>();
    //to do从牌库中取出六张牌
    public void DrawHandCard(Card card)
    {
        handCards.Add(card);
        Debug.Log("玩家" + id + "抽牌成功，抽到了" + card.getCardColor() + card.getCardRank()+"\n");
    }

    //选牌
    public Card ChooseOneCard(int i)
    {
        curCard.Add(handCards[i]);
        Card card = handCards[i];
        handCards.RemoveAt(i);
        return card;
    }


    //回合结算，并将剩余的卡送入牌堆
    public void RoundSettlement(int ranking,CardsType type)
    {
        curRoundCard = type;
        if(ranking==1)
        {
            groundCard.Add(curCard[0]);
            EventManager.Instance.FireEvent(eventType.AddDrawWeight, curCard[0]);
            AddGroundCard(curCard[0].getCardColor(), curCard[0].getCardRank());
            groundCard.Add(curCard[1]);
            EventManager.Instance.FireEvent(eventType.AddDrawWeight, curCard[1]);
            AddGroundCard(curCard[1].getCardColor(), curCard[1].getCardRank());
            curCard[2] = null;
            curCard.Clear();
        }
        else if(ranking==2)
        {
            groundCard.Add(curCard[0]);
            EventManager.Instance.FireEvent(eventType.AddDrawWeight, curCard[0]);
            AddGroundCard(curCard[0].getCardColor(), curCard[0].getCardRank());
            curCard[1] = null;
            curCard[2] = null;
            curCard.Clear();
        }
        else
        {
            curCard[0] = null;
            curCard[1] = null;
            curCard[2] = null;
            curCard.Clear();
        }
        EventManager.Instance.FireEvent(eventType.refreshRoundResult,groundState);
    }
    //开始新一局时调用
    public void StartNewRound()
    {
        handCards.Clear();
        groundCard.Clear();
        curCard.Clear();
        groundState.id = id;
    }

    private void AddGroundCard(CardColor color,int point)
    {
        if(color==CardColor.moutain)
        {
            groundState.moutainNum++;
            groundState.moutainSum += point;
        }
        else if(color == CardColor.plant)
        {
            groundState.plantNum++;
            groundState.plantSum += point;
        }
        else
        {
            groundState.yardNum++;
            groundState.yardSum += point;
        }
    }
}


public class CardPool
{
    string CardPath = "Cards/card_";
    string settingpath = "Cards/GlobalSetting";
    List<Card> cardPool;
    private int PoolSum;
    private int PoolCur;
    public CardPool()
    {
        GlobalSetting setting = Resources.Load<GlobalSetting>(settingpath);
        int suitNum = System.Enum.GetNames(typeof(CardColor)).Length;
        cardPool = new List<Card>();
        int cardNum = setting.maxPoint;
        PoolSum = suitNum * cardNum* setting.cardNum;
        PoolCur = PoolSum;
        for (int i = 0;i<suitNum;i++)
        {
            CardColor type = (CardColor)0 + i;
            string path = CardPath+ type.ToString();
            CardSetting set = Resources.Load<CardSetting>(CardPath);
            for(int j = 0;j< cardNum; j++)
            {
                for (int k = 0; k < setting.cardNum;k++)
                {
                    cardPool.Add(new Card((CardColor)i, j + 1, set.tex));
                }
            }
        }
        cardPool = CardRandom(cardPool);
    }

    public Card Draw()
    {
        if(PoolCur>0)
        {
            Card card = cardPool[0];
            cardPool.RemoveAt(0);
            PoolCur--;
            return card;
        }
        else if(cardPool.Count>0)
        {
            cardPool = CardRandom(cardPool);
            PoolCur = cardPool.Count;
            return Draw();
        }
        return null;
    }
    //可用于弃卡堆
    public void AddInPool(Card card)
    {
        cardPool.Add(card);
    }
    public void AddInPool(List<Card> cards)
    {
        foreach(var card in cards)
        {
            cardPool.Add(card);
        }
    }
    //洗牌
    private List<Card> CardRandom(List<Card> myCards)
    {
        //排序之后的List b
        List<Card> b = new List<Card>();
        //为了降低运算的数量级，当执行完一个元素时，就需要把此元素从原List中移除
        int countNum = myCards.Count;
        //使用while循环，保证将a中的全部元素转移到b中而不产生遗漏
        while (b.Count < countNum)
        {
            //随机将a中序号为index的元素作为b中的第一个元素放入b中
            int index = Random.Range(0, myCards.Count - 1);
            //检测是否重复，保险起见
            if (!b.Contains(myCards[index]))
            {
                //若b中还没有此元素，添加到b中
                b.Add(myCards[index]);
                //成功添加后，将此元素从a中移除，避免重复取值
                myCards.Remove(myCards[index]);
            }
        }
        return b;
    }
}
