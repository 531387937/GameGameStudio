using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CardInstance : MonoBehaviour
{
    public Card card { set { /*GetComponent<Image>().sprite = value.tex;*/CardInfo.text = value.getCardRank().ToString();ColorIcon.sprite = sprites[(int)value.getCardColor()]; } get { return card; } }
    public int number = 0;
    [SerializeField]
    private Text CardInfo;
    [SerializeField]
    private Image ColorIcon;
    public Sprite[] sprites;

    /*
    植物 90, 104, 85, 255
    建筑 89, 81, 62, 255
    山水 81, 88, 98, 255
    */
    // Start is called before the first frame update
    void Start()
    {
        CardInfo = GetComponentInChildren<Text>();
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
