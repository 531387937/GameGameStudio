using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    _default,card1
}

public class Card
{
    private CardType card_Type;

    private int point;
    Card(CardType type,int point)
    {
        card_Type = type;
        this.point = point;
    }

    public CardType getCardType()
    {
        return card_Type;
    }
    public int getPoint()
    {
        return point;
    }
}
[System.Serializable]
public class CardSetting:ScriptableObject
{
    public CardType curType = CardType._default;
    public int cardTypeNum;
    public int maxPoint;
}
