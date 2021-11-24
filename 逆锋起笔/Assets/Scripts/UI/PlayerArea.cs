using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    public Text name;

    private int curChooseCard = 0;
    // Start is called before the first frame update
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

    public void RefreshHandCard()
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
    public void ChooseCard(int index)
    {
        roundCard.Add(handCard[index]);
        handCard[index].transform.SetParent(roundCardArea.transform);
        handCard.RemoveAt(index);
        RefreshHandCard();
        for (int i = 0; i < roundCard.Count; i++)
        {
            roundCard[i].GetComponent<RectTransform>().position = roundAreas[i].position;
        }
    }

    public void InitRoom()
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
        name.text = GameManager.Instance.playerManager.localPlayer.playerName;
    }
}
