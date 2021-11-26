using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;
//用于管理服务器上的玩家行为、流程控制
public class PlayerManager
{
    public List<Player> remotePlayers = new List<Player>();
    public Player localPlayer;
    private List<CardSetting> settings = new List<CardSetting>();
    public List<Player> players = new List<Player>();        //所有玩家

    private int receivedCards = 0;
    public PlayerManager()
    {
        NetManager.AddMsgListener("MsgRoomInfo", OnReceiveRoomInfo);
        NetManager.AddMsgListener("MsgInitCards", OnDrawCard);
        NetManager.AddMsgListener("MsgRoundResult", OnReceiveRoundResult);
        NetManager.AddMsgListener("MsgBattleResult", OnReceiveBattleResult);
        NetManager.AddMsgListener("MsgChooseCard", OnReceiveChooseCard);
        string CardPath = "Cards/card";
        for(int i = 0;i<3;i++)
        {
            string path = CardPath + ((CardColor)i).ToString();
            CardSetting card = Resources.Load(path) as CardSetting;
            settings.Add(card);
            Debug.Log("Load Card: " + card.curType);
        }
    }
    //接收房间信息
    private void OnReceiveRoomInfo(MsgBase msgBase)
    {
        MsgRoomInfo roomInfo = (MsgRoomInfo)msgBase;
        
        if (roomInfo.remotePlayers.Count == 2)
        {
            localPlayer = new Player(roomInfo.localPlayer.playerId, roomInfo.localPlayer.playerName);
            for (int i = 0; i < roomInfo.remotePlayers.Count; i++)
            {
                remotePlayers.Add(new Player(roomInfo.remotePlayers[i].playerId, roomInfo.remotePlayers[i].playerName));
            }
            GetAllPlayers();
            //to do调用游戏管理器 初始化房间
            EventManager.Instance.FireEvent(eventType.initRoom);
        }
    }

    private void OnReceiveRoundResult(MsgBase msgBase)
    {
        MsgRoundResult roundResult = (MsgRoundResult)msgBase;
        foreach(var player in players)
        {
            if (roundResult.result.ContainsKey(player.id))
            {
                player.RoundSettlement(roundResult.result[player.id].rank, (CardsType)roundResult.result[player.id].cardsType);
            }
        }
        //GameManager.Instance.RefreshRoundResult();

    }
    //刷新选牌
    private void OnReceiveChooseCard(MsgBase msgBase)
    {
        MsgChooseCard choose = (MsgChooseCard)msgBase;
        if (choose.playerID == localPlayer.id)
        {
            receivedCards++;
        }
        else
        {
            for (int i = 0; i < remotePlayers.Count; i++)
            {
                if (remotePlayers[i].id == choose.playerID)
                {
                    CardInfo curCard = choose.card;
                    remotePlayers[i].curCard.Add(new Card((CardColor)curCard.cardColor, curCard.num, settings[(int)curCard.cardColor].tex));
                }
            }
            receivedCards++;
        }
        if (receivedCards == 3)
        {
            EventManager.Instance.FireEvent(eventType.receiveChooseCard);
            receivedCards = 0;
        }



        //GameManager.Instance.RefreshCurCard();
    }
    /// <summary>
    /// 有人胡牌，更新胡牌信息
    /// </summary>
    /// <param name="msgBase"></param>
    private void OnReceiveBattleResult(MsgBase msgBase)
    {
        MsgBattleResult battleResult = (MsgBattleResult)msgBase;
        List<Player> winPlayer = new List<Player>();
        foreach(var player in players)
        {
            if(battleResult.result.ContainsKey(player.id))
            {
                winPlayer.Add(player);
                player.wintype =(WinType)battleResult.result[player.id];
            }
        }
        EventManager.Instance.FireEvent(eventType.battleEnd, winPlayer);
    }
    //抽牌
    private void OnDrawCard(MsgBase msgBase)
    {
        MsgInitCards cards = (MsgInitCards)msgBase;

        if (cards.Cards.ContainsKey(localPlayer.id))
        {
            List<CardInfo> cardInfos = cards.Cards[localPlayer.id];
            for (int i = 0; i < cardInfos.Count; i++)
            {
                CardInfo curCard = cardInfos[i];
                localPlayer.DrawHandCard(new Card((CardColor)curCard.cardColor, curCard.num, settings[(int)curCard.cardColor].tex));
            }
        }
        EventManager.Instance.FireEvent(eventType.refreshHandCard);
    }

    private void GetAllPlayers()
    {
        players.Add(localPlayer);
        for(int i = 0;i<remotePlayers.Count;i++)
        {
            players.Add(remotePlayers[i]);
        }
    }


}
