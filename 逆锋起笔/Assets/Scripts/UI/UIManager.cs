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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChooseCard()
    {
        //播放动效

        Card card = GameManager.Instance.playerManager.localPlayer.ChooseOneCard(curChooseCard);
        print("打出了" + card.getCardColor().ToString() + card.getCardRank());
        playerArea.ChooseCard(curChooseCard);
        curChooseCard = 0;
        //发送选牌
        CardInfo cardInfo = new CardInfo();
        cardInfo.cardType =(Network.CardColor)card.getCardColor();
        cardInfo.num = card.getCardRank();
        MsgChooseCard chooseCard = new MsgChooseCard();
        chooseCard.playerID = GameManager.Instance.playerManager.localPlayer.id;
        chooseCard.card = cardInfo;
        NetManager.Send(chooseCard);
    }

    public void ContiuneGame(bool choice)
    {
        MsgNextBattle msgNextBattle = new MsgNextBattle();
        msgNextBattle.playerID = GameManager.Instance.playerManager.localPlayer.id;
        msgNextBattle.choice = choice;
        NetManager.Send(msgNextBattle);
    }

    public void RefreshHandCard()
    {
        playerArea.RefreshHandCard();
    }

    private void SelectCard(int i)
    {
        curChooseCard = i;
        print("目前选择了第" + curChooseCard + "张卡");
    }

    public void InitRoom()
    {
        playerArea.InitRoom();
    }
}
