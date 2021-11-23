﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;
//用于管理服务器上的玩家行为、流程控制
public class PlayerManager
{
    public List<Player> remotePlayers;
    public Player localPlayer;
    private List<CardSetting> settings;
    public List<Player> players;        //所有玩家
    public PlayerManager()
    {
        NetManager.AddMsgListener("MsgRoomInfo", OnReceiveRoomInfo);
        NetManager.AddMsgListener("MsgInitCards", OnDrawCard);
        NetManager.AddMsgListener("MsgRoundResult", OnReceiveRoundResult);
        NetManager.AddMsgListener("MsgBattleResult", OnReceiveBattleResult);

        string CardPath = "Cards/card_";
        CardColor type = CardColor.plant;
        for(int i = 0;i<3;i++)
        {
            string path = CardPath + (type+i).ToString();
            settings.Add(Resources.Load<CardSetting>(CardPath));
        }
    }
    //接收房间信息
    private void OnReceiveRoomInfo(MsgBase msgBase)
    {
        MsgRoomInfo roomInfo = (MsgRoomInfo)msgBase;
        localPlayer = new Player(roomInfo.localPlayer.playerId, roomInfo.localPlayer.playerName);
        for(int i = 0;i<roomInfo.remotePlayers.Count;i++)
        {
            remotePlayers.Add(new Player(roomInfo.remotePlayers[i].playerId, roomInfo.remotePlayers[i].playerName));
        }
        GetAllPlayers();
        //to do调用游戏管理器 初始化房间
        GameManager.Instance.InitRoom();
    }

    private void OnReceiveRoundResult(MsgBase msgBase)
    {
        MsgRoundResult roundResult = (MsgRoundResult)msgBase;
        if(roundResult.result.ContainsKey(localPlayer.id))
        {
            localPlayer.RoundSettlement(roundResult.result[localPlayer.id].rank, (CardsType)roundResult.result[localPlayer.id].cardsType);
        }
        foreach(var player in remotePlayers)
        {
            if (roundResult.result.ContainsKey(player.id))
            {
                player.RoundSettlement(roundResult.result[player.id].rank, (CardsType)roundResult.result[player.id].cardsType);
            }
        }

    }
    //刷新选牌
    private void OnReceiveChooseCard(MsgBase msgBase)
    {
        MsgChooseCard choose = (MsgChooseCard)msgBase;
        if(choose.playerID==localPlayer.id)
        {
            return;
        }
        else
        {
            for(int i = 0;i<remotePlayers.Count;i++)
            {
                if(remotePlayers[i].id==choose.playerID)
                {
                    CardInfo curCard = choose.card;
                    remotePlayers[i].curCard.Add(new Card((CardColor)curCard.cardType, curCard.num, settings[(int)curCard.cardType].tex));
                }
            }
        }
        GameManager.Instance.RefreshCurCard();
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
    }
    //抽牌
    private void OnDrawCard(MsgBase msgBase)
    {
        MsgInitCards cards = (MsgInitCards)msgBase;

        if(cards.Cards.ContainsKey(localPlayer.id))
        {
            List<CardInfo> cardInfos = cards.Cards[localPlayer.id];
            for (int i = 0; i <cardInfos.Count;i++ )
            {
                CardInfo curCard = cardInfos[i];
                localPlayer.DrawHandCard(new Card((CardColor)curCard.cardType, curCard.num, settings[(int)curCard.cardType].tex));
            }
        }
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
