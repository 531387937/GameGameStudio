using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Network;
public class PlayerArea : MonoBehaviour
{
    private List<RectTransform> cardAreas = new List<RectTransform>();
    [SerializeField]
    private List<RectTransform> roundAreas = new List<RectTransform>();
    public GameObject cardObject;
    private List<GameObject> handCard = new List<GameObject>();
    private List<GameObject> roundCard = new List<GameObject>();
    private List<GameObject> groundCard = new List<GameObject>();
    public GameObject roundCardArea;
    public GameObject cardEffect;

    public Text playerName;
    public WinProgress progress;

    private int curChooseCard = 0;
    // Start is called before the first frame update

    private void Awake()
    {
        EventManager.Instance.AddEventListener(eventType.refreshHandCard, RefreshHandCard);
        EventManager.Instance.AddEventListener(eventType.chooseCard, ChooseCard);
        EventManager.Instance.AddEventListener(eventType.refreshRoundResult, OnFreshRoundResult);
        EventManager.Instance.AddEventListener(eventType.initRoom, InitRoom);
    }
    void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.tag == "CardArea")
            {
                cardAreas.Add(child.GetComponent<RectTransform>());
            }

        }
        foreach (Transform child in roundCardArea.transform)
        {
            if (child.tag == "RoundArea")
            {
                roundAreas.Add(child.GetComponent<RectTransform>());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnFreshRoundResult(object info)
    {
        //cardEffect.SetActive(true);
        foreach(var obj in roundCard)
        {
            Destroy(obj);
        }
        roundCard.Clear();
    }
    private void RefreshHandCard(object arg)
    {
        Player localPlayer = GameManager.Instance.playerManager.localPlayer;
        for (; handCard.Count < localPlayer.handCards.Count;)
        {
            GameObject newCard = Instantiate(cardObject, transform);
            newCard.GetComponent<CardInstance>().card = localPlayer.handCards[handCard.Count];
            newCard.GetComponent<RectTransform>().position = cardAreas[handCard.Count].position;
            newCard.GetComponent<CardInstance>().number = handCard.Count;
            handCard.Add(newCard);
        }
        for (int i = 0; i < handCard.Count; i++)
        {
            handCard[i].GetComponent<CardInstance>().number = i;
            handCard[i].GetComponent<RectTransform>().position = cardAreas[i].position;
        }
    }
    private void ChooseCard(object index)
    {
        int _index = (int)index;

        Card card = GameManager.Instance.playerManager.localPlayer.ChooseOneCard(_index);
        print("打出了" + card.getCardColor().ToString() + card.getCardRank());

        roundCard.Add(handCard[_index]);
        handCard[_index].transform.SetParent(roundCardArea.transform);
        handCard.RemoveAt(_index);
        RefreshHandCard(null);
        for (int i = 0; i < roundCard.Count; i++)
        {
            roundCard[i].GetComponent<RectTransform>().position = roundAreas[i].position;
        }

        

        //发送选牌
        CardInfo cardInfo = new CardInfo();
        cardInfo.cardColor = (Network.CardColor)card.getCardColor();
        cardInfo.num = card.getCardRank();
        MsgChooseCard chooseCard = new MsgChooseCard();
        chooseCard.playerID = GameManager.Instance.playerManager.localPlayer.id;
        chooseCard.card = cardInfo;
        NetManager.Send(chooseCard);
    }

    public void InitRoom(object arg)
    {
        foreach(var a in handCard)
        {
            Destroy(a);
        }
        handCard.Clear();
        foreach (var a in groundCard)
        {
            Destroy(a);
        }
        groundCard.Clear();
        foreach (var a in roundCard)
        {
            Destroy(a);
        }
        roundCard.Clear();
        playerName.text = GameManager.Instance.playerManager.localPlayer.playerName;
        progress.playerID = GameManager.Instance.playerManager.localPlayer.id;
    }
}
