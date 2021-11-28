using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;
using UnityEngine.UI;
public class UIManager : Singleton<UIManager>
{
    public PlayerArea playerArea;
    public RemotePlayerArea remoteArea1;
    public RemotePlayerArea remoteArea2;
    public GameObject EndPanel;
    public GameObject gamePanel;
    public GameObject waitingPanel;
    private int curChooseCard = 0;
    public Button chooseCardBtn;
    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.AddEventListener(eventType.initRoom, InitRoom);
        EventManager.Instance.AddEventListener(eventType.refreshRoundResult, OnReceiveRoundEnd);
        EventManager.Instance.AddEventListener(eventType.receiveChooseCard, OnReceiveRoundEnd);
        EventManager.Instance.AddEventListener(eventType.battleEnd, OnReceiveBattleEnd);
        NetManager.AddMsgListener("MsgNextBattle", OnCountinueGame);
        Button[] buttons = transform.GetComponentsInChildren<Button>(true);
        foreach(var button in buttons)
        {
            button.onClick.AddListener(()=> { AudioManager.GetInstance().Post2D("Click_UI"); });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitRoom(object obj)
    {
        int id = GameManager.Instance.playerManager.localPlayer.id;
        int r_id1 = (id + 1) % 3 == 0 ? 3 : (id + 1) % 3;
        int r_id2 = (id + 2) % 3 == 0 ? 3 : (id + 2) % 3;

        for(int i = 0;i<2;i++)
        {
            if(GameManager.Instance.playerManager.remotePlayers[i].id==r_id1)
            {
                remoteArea1.remotePlayer = i;
            }
            else if(GameManager.Instance.playerManager.remotePlayers[i].id ==r_id2)
            {
                remoteArea2.remotePlayer = i;
            }
        }
        remoteArea1.InitRoom(null);
        remoteArea2.InitRoom(null);
    }

    //由button调用
    public void ChooseCard()
    {
        EventManager.Instance.FireEvent(eventType.chooseCard, curChooseCard);
        curChooseCard = 0;
        chooseCardBtn.gameObject.SetActive(false);
    }

    public void ContiuneGame(bool choice)
    {
        MsgNextBattle msgNextBattle = new MsgNextBattle();
        msgNextBattle.playerID = GameManager.Instance.playerManager.localPlayer.id;
        msgNextBattle.choice = choice;
        NetManager.Send(msgNextBattle);
        
        EventManager.Instance.FireEvent(eventType.initRoom);
    }

    private void OnReceiveBattleEnd(object obj)
    {
        print("游戏结束");
        List<Player> winPlayer = (List<Player>)obj;
        EndPanel.SetActive(true);
        EndPanel.GetComponent<EndPanel>().GameOver(winPlayer);
    }

    private void OnReceiveRoundEnd(object obj)
    {
        chooseCardBtn.gameObject.SetActive(true);
    }

    private void OnCountinueGame(MsgBase msg)
    {
        MsgNextBattle msgNextBattle = (MsgNextBattle)msg;
        if(msgNextBattle.choice)
        {
            EndPanel.SetActive(false);
        }
        else
        {
            gamePanel.SetActive(false);
            waitingPanel.SetActive(true);
        }
    }

    private void SelectCard(int i)
    {
        curChooseCard = i;
    }

}
