using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Network;
public class RemotePlayerArea : MonoBehaviour
{
    private List<RectTransform> cardAreas = new List<RectTransform>();
    [SerializeField]
    private List<RectTransform> roundAreas = new List<RectTransform>();
    public GameObject cardObject;
    public GameObject cardBackObject;
    private List<GameObject> handCard = new List<GameObject>();
    private List<GameObject> roundCard = new List<GameObject>();
    private List<GameObject> groundCard = new List<GameObject>();
    public GameObject roundCardArea;
    public Text playerName;
    public WinProgress progress;
    public int remotePlayer;

    private int curChooseCard = 0;
    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.AddEventListener(eventType.refreshHandCard, RefreshHandCard);
        EventManager.Instance.AddEventListener(eventType.refreshRoundResult, RefreshHandCard);
        EventManager.Instance.AddEventListener(eventType.refreshRoundResult, OnRefreshRoundResult);
        NetManager.AddMsgListener("MsgChooseCard", OnReceiveChooseCard);
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


    private void OnReceiveChooseCard(MsgBase msg)
    {
        MsgChooseCard cardMsg = (MsgChooseCard)msg;
        if (cardMsg.playerID != GameManager.Instance.playerManager.remotePlayers[remotePlayer].id)
            return;
        Card card = new Card((CardColor)cardMsg.card.cardColor, cardMsg.card.num, "");
        GameObject newCard = Instantiate(cardObject, transform);
        newCard.GetComponent<CardInstance>().card = card;
        newCard.GetComponent<RectTransform>().position = roundAreas[roundCard.Count].position;
        roundCard.Add(newCard);
        GameManager.Instance.playerManager.remotePlayers[remotePlayer].curCard.Add(card);
    }


    private void RefreshHandCard(object arg)
    {
        Player localPlayer = GameManager.Instance.playerManager.localPlayer;
        for (; handCard.Count < localPlayer.handCards.Count;)
        {
            GameObject newCard = Instantiate(cardBackObject, transform);
            newCard.GetComponent<RectTransform>().position = cardAreas[handCard.Count].position;
            handCard.Add(newCard);
        }
        for (int i = 0; i < handCard.Count; i++)
        {
            handCard[i].GetComponent<CardInstance>().number = i;
            handCard[i].GetComponent<RectTransform>().position = cardAreas[i].position;
        }
    }
    private void OnRefreshRoundResult(object arg)
    {
        foreach (var obj in roundCard)
        {
            Destroy(obj);
        }
        roundCard.Clear();
    }

    public void InitRoom(object arg)
    {
        foreach (var a in handCard)
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
        playerName.text = GameManager.Instance.playerManager.remotePlayers[remotePlayer].playerName;
        progress.playerID = GameManager.Instance.playerManager.remotePlayers[remotePlayer].id;
    }
}
