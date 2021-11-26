using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WinProgress : MonoBehaviour
{
    public PlayerGroundCard groundInfo;
    public Text plant;
    public Text moutain;
    public Text yard;
    public Text baodi;
    public Text xiaohu;

    public int playerID;
    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.AddEventListener(eventType.initRoom, InitRoom);
        EventManager.Instance.AddEventListener(eventType.refreshRoundResult, OnFreshRoundResult);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnFreshRoundResult(object info)
    {
        groundInfo =(PlayerGroundCard)info;
        if(groundInfo.id!=playerID)
        {
            return;
        }
        int plantNum = groundInfo.plantNum;
        int yardNum = groundInfo.yardNum;
        int moutainNum = groundInfo.moutainNum;
        FreshText(plant, plantNum, 6 + moutainNum);
        FreshText(yard, yardNum, 6 + plantNum);
        FreshText(moutain, moutainNum, 6 + yardNum);
        FreshText(baodi, plantNum > 4 ? 4 : plantNum + yardNum > 4 ? 4 : yardNum + moutainNum > 4 ? 4 : moutainNum, 12);
        if(groundInfo.plantSum+ groundInfo.yardSum+ groundInfo.moutainSum<25)
        {
            FreshText(xiaohu, plantNum + yardNum + moutainNum, 25);
        }
        else
        {
            FreshText(xiaohu, plantNum + yardNum + moutainNum, 25,false);
        }
    }
    void FreshText(Text text,int num,int target,bool active = true)
    {
        if (active)
        {
            //tmpText = num.ToString() + "//" + target.ToString();
            text.text = num.ToString() + "//" + target.ToString();
        }
        else
            text.text = "无役";
    }
    private void InitRoom(object arg)
    {
        FreshText(plant, 0, 5);
        FreshText(moutain, 0, 5);
        FreshText(yard, 0, 5);
        FreshText(baodi, 0, 5);
        FreshText(xiaohu, 0, 5);
    }
}
