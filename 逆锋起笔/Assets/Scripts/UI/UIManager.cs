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
    private int curChooseCard = 0;
    public Button chooseCardBtn;
    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.AddEventListener(eventType.initRoom, InitRoom);
        EventManager.Instance.AddEventListener(eventType.refreshRoundResult, OnReceiveRoundEnd);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitRoom(object obj)
    {
        int id = GameManager.Instance.playerManager.localPlayer.id;
        int r_id1 = (id + 1) % 4 == 0 ? 1 : (id + 1) % 4;
        int r_id2 = (id + 2) % 4 == 0 ? 1 : (id + 2) % 4;
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
        EndPanel.SetActive(false);
    }

    private void OnReceiveBattleEnd(object obj)
    {
        List<Player> winPlayer = (List<Player>)obj;
        EndPanel.SetActive(true);
        EndPanel.GetComponent<EndPanel>().GameOver(winPlayer);
    }

    private void OnReceiveRoundEnd(object obj)
    {
        chooseCardBtn.gameObject.SetActive(true);
    }

    private void SelectCard(int i)
    {
        curChooseCard = i;
        print("目前选择了第" + curChooseCard + "张卡");
    }

}
