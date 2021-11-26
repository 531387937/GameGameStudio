using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private List<Drawable> plantDrawable;
    private List<Drawable> mountainDrawable;
    private List<Drawable> yardDrawable;

    int point = 1;

    private struct GroundState
    {
        public int plantNum;
        public int moutainNum;
        public int yardNum;
    }
    private GroundState groundState;
    // Start is called before the first frame update
    void Start()
    {
        playerManager = new PlayerManager();

        //playerManager.localPlayer = new Player(0, "111");
        Drawable[] objects = FindObjectsOfType<Drawable>();
        foreach(var draw in objects)
        {
            if(draw.color==CardColor.plant)
            {
                int i;
                for (i = 0;i<plantDrawable.Count;i++)
                {
                    if(draw.weight<plantDrawable[i].weight)
                    {
                        break;
                    }
                }
                plantDrawable.Insert(i, draw);
            }
            else if(draw.color==CardColor.moutain)
            {
                int i;
                for (i = 0; i < mountainDrawable.Count; i++)
                {
                    if (draw.weight < mountainDrawable[i].weight)
                    {
                        break;
                    }
                }
                mountainDrawable.Insert(i, draw);
            }
            else
            {
                int i;
                for (i = 0; i < yardDrawable.Count; i++)
                {
                    if (draw.weight < yardDrawable[i].weight)
                    {
                        break;
                    }
                }
                yardDrawable.Insert(i, draw);
            }
        }
        curState = GameState.begin;
    }

    // Update is called once per frame
    void Update()
    {
        NetManager.Update();
        //if(Input.GetKeyDown(KeyCode.Space))
        //{
        //    List<Player> winPlayer = new List<Player>();
        //    winPlayer.Add(playerManager.localPlayer);
        //    playerManager.localPlayer.wintype = WinType.ShanShui;
        //    EventManager.Instance.FireEvent(eventType.battleEnd, winPlayer);
        //}
    }


    /// <summary>
    /// 初始化房间
    /// </summary>
    private void InitRoom()
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
    public void RefreshRoundResult(object arg)
    {
        int plantSum=0, moutainSum=0, yardSum = 0;
        foreach(var player in players)
        {
            plantSum += player.groundState.plantSum;
            moutainSum += player.groundState.moutainSum;
            yardSum += player.groundState.yardSum;
        }
        
        //展示结果,展示结束后检查是否有人胡牌
    }

    public void CheckBattleResult()
    {
        //有人胡牌，本轮结束
        curState = GameState.end;
    }

    
}
