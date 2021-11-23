using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network.proto;
using Network;
public class UIManager : Singleton<UIManager>
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChooseCard(int i)
    {
        //播放动效

        Card card = GameManager.Instance.playerManager.localPlayer.ChooseOneCard(i);
        //发送选牌
        CardInfo cardInfo = new CardInfo();
        cardInfo.cardType = card.getCardColor();
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
}
