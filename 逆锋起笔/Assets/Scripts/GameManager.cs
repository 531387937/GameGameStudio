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
        curState = GameState.begin;
    }

    // Update is called once per frame
    void Update()
    {
        NetManager.Update();
        AudioManager.GetInstance().Init();
        AudioManager.GetInstance().LoadBank("Common");

        
    }


    /// <summary>
    /// 初始化房间
    /// </summary>
    private void InitRoom()
    {
        curState = GameState.playing;
    }



    void OnRefreshRoundResult()
    {

    }

    public void CheckBattleResult()
    {
        //有人胡牌，本轮结束
        curState = GameState.end;
    }

    
}
