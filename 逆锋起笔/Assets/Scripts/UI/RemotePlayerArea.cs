using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Network;
using DG.Tweening;
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
        EventManager.Instance.AddEventListener(eventType.refreshRoundResult, RefreshRoundResult);
        EventManager.Instance.AddEventListener(eventType.receiveChooseCard, ReceiveChooseCard);
        //EventManager.Instance.AddEventListener(eventType.initRoom, InitRoom);
    }

    void Start()
    {
        
        //NetManager.AddMsgListener("MsgChooseCard", OnReceiveChooseCard);
        
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
        //暂时的比拼显示
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
    private void ReceiveChooseCard(object msg)
    {
        UIManager.Instance.StartCoroutine(UIManager.Instance.GetNext(OnReceiveChooseCard, msg));
    }

    private void OnReceiveChooseCard(object msg)
    {
        //这里添加出牌动效
        Card card = GameManager.Instance.playerManager.remotePlayers[remotePlayer].curCard[roundCard.Count];
        GameObject newCard = Instantiate(cardObject, transform);
        newCard.GetComponent<CardInstance>().card = card;
        newCard.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 75);
        newCard.GetComponent<RectTransform>().position = cardAreas[cardAreas.Count - 1].position;
        EventManager.Instance.FireEvent(eventType.waitTween, false);
        newCard.GetComponent<RectTransform>().DOSizeDelta(new Vector2(120,160), 0.5f);
        newCard.GetComponent<RectTransform>().DOMove(roundAreas[roundCard.Count].position,0.5f).OnComplete(()=> { EventManager.Instance.FireEvent(eventType.waitTween, true); });
        roundCard.Add(newCard);
    }

    private void RefreshHandCard(object arg)
    {
        UIManager.Instance.StartCoroutine(UIManager.Instance.GetNext(OnRefreshHandCard, arg));
    }
    private void OnRefreshHandCard(object arg)
    {
        //这里添加抽牌特效
        Player localPlayer = GameManager.Instance.playerManager.localPlayer;
        for (; handCard.Count < localPlayer.handCards.Count;)
        {
            GameObject newCard = Instantiate(cardBackObject, transform);
            newCard.GetComponent<RectTransform>().position = cardAreas[handCard.Count].position;
            handCard.Add(newCard);
        }
        for (int i = 0; i < handCard.Count; i++)
        {
            //handCard[i].GetComponent<CardInstance>().number = i;
            handCard[i].GetComponent<RectTransform>().position = cardAreas[i].position;
        }
    }
    private void RefreshRoundResult(object arg)
    {
        UIManager.Instance.StartCoroutine(UIManager.Instance.GetNext(OnRefreshRoundResult, arg));
    }
    private void OnRefreshRoundResult(object arg)
    {
        //这里添加比拼动效
        cardEffect.SetActive(true);
        cardEffect.GetComponentInChildren<Text>().text = winStringDic[GameManager.Instance.playerManager.remotePlayers[remotePlayer].curRoundCard];
        switch(GameManager.Instance.playerManager.remotePlayers[remotePlayer].curRoundCard)
        {
            case CardsType.ZhaDan:
                AudioManager.GetInstance().Post2D("paixing_ZBSW");
            break;
            case CardsType.TongHuaShun:
                AudioManager.GetInstance().Post2D("paixing_XYLS");
            break;
            case CardsType.TongShuZi:
                AudioManager.GetInstance().Post2D("paixing_JCD");
            break;
            default:
                AudioManager.GetInstance().Post2D("paixing_COMMON");
            break;
        }
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
        cardEffect.SetActive(false);
        playerName.text = GameManager.Instance.playerManager.remotePlayers[remotePlayer].playerName;
        progress.playerID = GameManager.Instance.playerManager.remotePlayers[remotePlayer].id;
    }
}
