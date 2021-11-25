using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;
public class UIManager : Singleton<UIManager>
{
    public PlayerArea playerArea;
    public RemotePlayerArea remoteArea1;
    public RemotePlayerArea remoteArea2;

    private int curChooseCard = 0;
    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.AddEventListener(eventType.initRoom, InitRoom);
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
    }

    public void ContiuneGame(bool choice)
    {
        MsgNextBattle msgNextBattle = new MsgNextBattle();
        msgNextBattle.playerID = GameManager.Instance.playerManager.localPlayer.id;
        msgNextBattle.choice = choice;
        NetManager.Send(msgNextBattle);
    }



    private void SelectCard(int i)
    {
        curChooseCard = i;
        print("目前选择了第" + curChooseCard + "张卡");
    }

}
