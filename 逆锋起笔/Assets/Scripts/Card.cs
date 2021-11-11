using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//花色
public enum CardType
{
    red,blue,green
}

public enum WinType
{
    noPair = 0,
    diffPair = 1,
    samePair = 2,
    straight = 3,
    flush = 4,
    fullNum = 5,
    straightFlush = 6,
    boom = 7,
}

public struct PlayerInformation
{
    Card[] cards;
}

public struct CardPower
{
    WinType winType;
    int weight;
}

public class Card
{
    private CardType suit;

    private int rank;
    private string texture;
    public Card(CardType type,int point)
    {
        suit = type;
        this.rank = point;
        //texture = tex;
        //to do 给每张牌加载对应的贴图
    }

    public CardType getCardSuit()
    {
        return suit;
    }
    public int getCardRank()
    {
        return rank;
    }
}

public class Player
{
    //玩家唯一标识符
    public readonly int id;

    public Player(int id)
    {
        this.id = id;
    }
    private int handNum = 0;
    //现在的手牌
    private List<Card> handCards = new List<Card>();
    //现在留在场上的牌
    public List<Card> groundCard = new List<Card>();
    //本回合的出牌
    public List<Card> curCard = new List<Card>();
    //to do从牌库中取出六张牌
    public void DrawHandCard(Card card)
    {
        if(handNum<6)
        {
            handCards.Add(card);
            handNum++;
        }
        Debug.Log("玩家" + id + "抽牌成功，抽到了" + card.getCardSuit() + card.getCardRank()+"\n");
    }

    //调用前由游戏管理判断是否还可以选牌
    public Card ChooseOneCard(int i)
    {
        curCard.Add(handCards[i]);
        return handCards[i];
    }
    public void CancelOneCard(Card card)
    {
        if(curCard.Contains(card))
        {
            curCard.Remove(card);
        }
    }

    private CardPower power;
    //调用前由游戏管理判断是否准备完毕（已选出三张牌）
    public void EnsurePlay()
    {
        foreach(var card in curCard)
        {
            handCards.Remove(card);
        }
        //to do判断牌型，填补power
    }

    public CardPower getPower()
    {
        return power;
    }

    //回合结算，并将剩余的卡送入牌堆
    public void RoundSettlement(int ranking,CardPool pool)
    {
        if(ranking==1)
        {
            groundCard.Add(curCard[0]);
            groundCard.Add(curCard[1]);
            pool.AddInPool(curCard[2]);
            curCard.Clear();
        }
        else if(ranking==2)
        {
            groundCard.Add(curCard[0]);
            pool.AddInPool(curCard[1]);
            pool.AddInPool(curCard[2]);
            curCard.Clear();
        }
        else
        {
            pool.AddInPool(curCard);
            curCard.Clear();
        }
        pool.AddInPool(handCards);
        handCards.Clear();
        handNum = 0;
    }
}

[System.Serializable]
public class CardSetting:ScriptableObject
{
    public CardType curType = CardType.red;
    public int cardTypeNum;
    public int maxPoint;
}

public class CardPool
{
    List<Card> cardPool;
    private int PoolSum;
    private int PoolCur;
    public CardPool()
    {
        int suitNum = System.Enum.GetNames(typeof(CardType)).Length;
        cardPool = new List<Card>();
        int cardNum = 16;
        PoolSum = suitNum * cardNum*3;
        PoolCur = PoolSum;
        for (int i = 0;i<suitNum;i++)
        {
            for(int j = 0;j< cardNum; j++)
            {
                cardPool.Add(new Card((CardType)i, j+1));
                cardPool.Add(new Card((CardType)i, j + 1));
                cardPool.Add(new Card((CardType)i, j + 1));
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
