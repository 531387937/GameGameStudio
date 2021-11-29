using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Network;
using DG.Tweening;
public class PlayerArea : MonoBehaviour
{
    private List<RectTransform> cardAreas = new List<RectTransform>();
    [SerializeField]
    private List<RectTransform> roundAreas = new List<RectTransform>();
    public GameObject cardObject;
    public List<GameObject> handCard = new List<GameObject>();
    private List<GameObject> roundCard = new List<GameObject>();
    private List<GameObject> groundCard = new List<GameObject>();
    public GameObject roundCardArea;
    public GameObject cardEffect;

    public Text playerName;
    public WinProgress progress;

    float timer = 1.5f;

    bool tweenEnd = true;

    Dictionary<CardsType, string> winStringDic = new Dictionary<CardsType, string>() {
        { CardsType.DanZhang, "铁线描" },{ CardsType.ShunZi, "撅头丁" },
        {CardsType.TongHua,"蚯蚓描" },{CardsType.TongHuaShun,"行云流水"},
        {CardsType.TongSeYiDui,"折芦描" },{CardsType.YiSeYiDui,"枯柴描"},
        {CardsType.ZhaDan,"战笔水纹" },{CardsType.TongShuZi,"金错刀"}
    };

    private int curChooseCard = 0;
    // Start is called before the first frame update

    private void Awake()
    {
        EventManager.Instance.AddEventListener(eventType.refreshHandCard, RefreshHandCard);
        EventManager.Instance.AddEventListener(eventType.chooseCard, ChooseCard);
        EventManager.Instance.AddEventListener(eventType.refreshRoundResult, ReFreshRoundResult);
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
        //暂留的比拼动效，需要调到下面
        if(cardEffect.activeInHierarchy)
        {
            timer -= Time.deltaTime;
            if(timer<0)
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
    private void ReFreshRoundResult(object info)
    {
UIManager.Instance.StartCoroutine(UIManager.Instance.GetNext(OnReFreshRoundResult, info));
    }
    private void OnReFreshRoundResult(object info)
    {
        
        //每轮的比拼结果动效应该在这里
        cardEffect.SetActive(true);
        cardEffect.GetComponentInChildren<Text>().text = winStringDic[GameManager.Instance.playerManager.localPlayer.curRoundCard];
        //cardEffect.SetActive(true);
    }
    private void RefreshHandCard(object arg)
    {
        UIManager.Instance.StartCoroutine(UIManager.Instance.GetNext(OnRefreshHandCard, arg));
    }
    private void OnRefreshHandCard(object arg)
    {
        //播放抽牌动画应在这里
        Player localPlayer = GameManager.Instance.playerManager.localPlayer;
        for (; handCard.Count < localPlayer.handCards.Count;)
        {
            GameObject newCard = Instantiate(cardObject, transform);
            newCard.GetComponent<CardInstance>().card = localPlayer.handCards[handCard.Count];
            newCard.GetComponent<RectTransform>().position = cardAreas[handCard.Count].position;    //先在一个序列tween中记录，for循环结束后调用
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
        UIManager.Instance.StartCoroutine(UIManager.Instance.GetNext(OnChooseCard, index));
    }
    private void OnChooseCard(object index)
    {
        //播放出牌动效应该在这里
        int _index = (int)index;

        Card card = GameManager.Instance.playerManager.localPlayer.ChooseOneCard(_index);
        print("打出了" + card.getCardColor().ToString() + card.getCardRank());

        roundCard.Add(handCard[_index]);
        handCard[_index].transform.SetParent(roundCardArea.transform);
        handCard.RemoveAt(_index);
        RefreshHandCard(null);
        EventManager.Instance.FireEvent(eventType.waitTween, false);
        int i = roundCard.Count - 1;
        roundCard[i].GetComponent<RectTransform>().DOMove(roundAreas[i].position,0.5f).OnComplete(()=> { EventManager.Instance.FireEvent(eventType.waitTween, true); });

        

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
