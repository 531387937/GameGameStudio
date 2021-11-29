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
        AudioManager.GetInstance().Init();
        AudioManager.GetInstance().LoadBank("Common");
        AudioManager.GetInstance().Post2D("Play_Ambience");
        EventManager.Instance.AddEventListener(eventType.initRoom, InitRoom);
        EventManager.Instance.AddEventListener(eventType.battleEnd, BattleResult);
        //playerManager.localPlayer = new Player(0, "111");
        curState = GameState.begin;
    }

    // Update is called once per frame
    void Update()
    {
        NetManager.Update();
        if (curState == GameState.playing&&playerManager.players.Count==3)
        {
            if (!playerManager.players[0].almostHu && !playerManager.players[0].almostHu && !playerManager.players[0].almostHu)
            {
                AudioManager.GetInstance().Post2D("Set_State_Normal");
            }
            else
            {
                AudioManager.GetInstance().Post2D("Set_State_Bridge");
            }
        }
        else if(curState == GameState.end)
        {
            AudioManager.GetInstance().Post2D("Set_State_Chorus");
        }
    }


    /// <summary>
    /// 初始化房间
    /// </summary>
    private void InitRoom(object obj)
    {
        curState = GameState.playing;
    }



    void OnRefreshRoundResult()
    {

    }

    private void BattleResult(object obj)
    {
        //有人胡牌，本轮结束
        curState = GameState.end;
    }

    
}
