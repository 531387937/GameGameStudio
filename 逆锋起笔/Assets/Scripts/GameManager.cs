using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network.proto;
using Network;

public enum GameState
{
    begin,playing,end
}

//用于管理本地游戏
public class GameManager : Singleton<GameManager>
{
    public PlayerManager playerManager;
    public GameState curState;
    public List<Player> players;
    // Start is called before the first frame update
    void Start()
    {
        playerManager = new PlayerManager();
        curState = GameState.begin;
    }

    // Update is called once per frame
    void Update()
    {
        Network.NetManager.Update();
    }


    /// <summary>
    /// 初始化房间
    /// </summary>
    public void InitRoom()
    {
        curState = GameState.playing;

    }
    /// <summary>
    /// 同步玩家出牌
    /// </summary>
    public void RefreshCurCard()
    {
        //当收到所有玩家出牌信息时刷新敌方玩家的出牌
        foreach(var player in playerManager.remotePlayers)
        {
            if(player.curCard.Count!=playerManager.localPlayer.curCard.Count)
            {
                return;
            }
        }

        //刷新
    }

    /// <summary>
    /// 同步本轮结果
    /// </summary>
    public void RefreshRoundResult()
    {
        //展示结果,展示结束后检查是否有人胡牌
    }

    public void CheckBattleResult()
    {
        //有人胡牌，本轮结束
        curState = GameState.end;
    }

    
}
