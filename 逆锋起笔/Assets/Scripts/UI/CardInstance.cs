using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CardInstance : MonoBehaviour
{
    public Card card { set { /*GetComponent<Image>().sprite = value.tex;*/CardInfo.text = value.getCardColor().ToString() + value.getCardRank(); } get { return card; } }
    public int number = 0;
    [SerializeField]
    private Text CardInfo;
    // Start is called before the first frame update
    void Start()
    {
        CardInfo = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void select()
    {
        //EventManager.Instance.FireEvent(eventType.chooseCard, number);
        SendMessageUpwards("SelectCard", number);
    }
}
