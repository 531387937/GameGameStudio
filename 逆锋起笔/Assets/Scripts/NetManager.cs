using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetManager : Singleton<NetManager>
{
    private int playerNum = -1;         //用于唯一定义一局游戏中每个玩家编号
    bool clint = false;
    Player self;
    //用于暂时记录其他玩家的信息
    Player[] otherPlayer;
    //卡池
    CardPool cardPool;
    bool allReady = false;
    bool gameStart = false;
    PlayerManager playerManager;
    enum MessageType
    {
        //主机发送给用户的信息枚举
        begin = 0,
        refresh = 1,
        //用户发送给主机的信息枚举
        play = 101,
    }
    // Start is called before the first frame update
    void Start()
    {
        CreateGame();
        playerManager.SetPlayers(3);
    }

    // Update is called once per frame
    void Update()
    {
        if(allReady)
        {

        }
        ReceiveMessage();
        if(Input.GetKeyDown(KeyCode.Space))
        {
            playerManager.DrawCard(cardPool);
        }
    }

    public bool CreateGame()
    {
        //to do用于作为主机创建游戏
        cardPool = new CardPool();
        playerManager = new PlayerManager();
        return true;
    }

    public bool JoinGame()
    {
        //to do作为用户加入游戏
        clint = true;
        return true;
    }

    void SendMessage(MessageType type,object message)
    {

    }

    void ReceiveMessage()
    {

    }
    void GameStart()
    {

    }
}
