﻿using System.Collections;
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

    public Text pointSum;
    public Text moutainSum;
    public Text plantSum;
    public Text yardSum;

    public int playerID;

    private void Awake()
    {
        EventManager.Instance.AddEventListener(eventType.initRoom, InitRoom);
        EventManager.Instance.AddEventListener(eventType.refreshRoundResult, OnFreshRoundResult);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!groundInfo.used)
        {

            int plantNum = groundInfo.plantNum;
            int yardNum = groundInfo.yardNum;
            int moutainNum = groundInfo.moutainNum;
            FreshText(plant, plantNum-moutainNum, 6);
            FreshText(yard, yardNum-plantNum, 6);
            FreshText(moutain, moutainNum-yardNum, 6);
            FreshText(baodi, plantNum > 4 ? 4 : plantNum + yardNum > 4 ? 4 : yardNum + moutainNum > 4 ? 4 : moutainNum, 12);
            if (groundInfo.pointSum < 25)
            {
                FreshText(xiaohu, groundInfo.pointSum, 25);
            }
            else
            {
                FreshText(xiaohu, groundInfo.pointSum, 25, false);
            }
            FreshText(plantSum, groundInfo.plantSum);
            FreshText(moutainSum, groundInfo.moutainSum);
            FreshText(yardSum, groundInfo.yardSum);
            FreshText(pointSum, groundInfo.pointSum);
            groundInfo.used = true;
        }
    }
    private void OnFreshRoundResult(object info)
    {
        PlayerGroundCard card = (PlayerGroundCard)info;
        if (card.id != playerID)
        {
            return;
        }
        else
        {
            groundInfo = card;
            groundInfo.used = false;
        }
        
    }
    void FreshText(Text text, int num, int target, bool active = true)
    {
        if(text==null)
        {
            return;
        }
        if (active)
        {
            //tmpText = num.ToString() + "//" + target.ToString();
            text.text = num.ToString() + "/" + target.ToString();
        }
        else
            text.text = "无役";
    }
    void FreshText(Text text,int num)
    {
        if(text==null)
        {
            return;
        }
        text.text = num.ToString();
    }
    private void InitRoom(object arg)
    {
        groundInfo = new PlayerGroundCard();
        groundInfo.used = false;
    }
}
