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
    public GameObject cardEffect;
    public int remotePlayer;
    float timer = 1.5f;
    private int curChooseCard = 0;

    Dictionary<CardsType, string> winStringDic = new Dictionary<CardsType, string>() {
        { CardsType.DanZhang, "铁线描" },{ CardsType.ShunZi, "撅头丁" },
        {CardsType.TongHua,"蚯蚓描" },{CardsType.TongHuaShun,"行云流水"},
        {CardsType.TongSeYiDui,"折芦描" },{CardsType.YiSeYiDui,"枯柴描"},
        {CardsType.ZhaDan,"战笔水纹" },{CardsType.TongShuZi,"金错刀"}
    };
    // Start is called before the first frame update
    private void Awake()
    {
        EventManager.Instance.AddEventListener(eventType.refreshHandCard, RefreshHandCard);
        EventManager.Instance.AddEventListener(eventType.refreshRoundResult, RefreshHandCard);
        EventManager.Instance.AddEventListener(eventType.refreshRoundResult, OnRefreshRoundResult);
        //EventManager.Instance.AddEventListener(eventType.initRoom, InitRoom);
    }

    void Start()
    {
        
        //NetManager.AddMsgListener("MsgChooseCard", OnReceiveChooseCard);
        EventManager.Instance.AddEventListener(eventType.receiveChooseCard, OnReceiveChooseCard);
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
        if (cardEffect.activeInHierarchy)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                cardEffect.SetActive(false);
                timer = 1.5f;
            }
            foreach (var obj in roundCard)
            {
                Destroy(obj);
            }
            roundCard.Clear();
        }
    }


    private void OnReceiveChooseCard(object msg)
    {
        Card card = GameManager.Instance.playerManager.remotePlayers[remotePlayer].curCard[roundCard.Count];
        GameObject newCard = Instantiate(cardObject, transform);
        newCard.GetComponent<CardInstance>().card = card;
        newCard.GetComponent<RectTransform>().position = roundAreas[roundCard.Count].position;
        roundCard.Add(newCard);
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
        cardEffect.SetActive(true);
        cardEffect.GetComponentInChildren<Text>().text = winStringDic[GameManager.Instance.playerManager.remotePlayers[remotePlayer].curRoundCard];
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
