using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleVision : MonoBehaviour
{
    private List<List<int>> compute1 = new List<List<int>>() {
        new List<int>{ 1 },
        new List<int>{ 1 },
    };
    private List<List<int>> compute2 = new List<List<int>>() {
        new List<int>{ 1,3 },
        new List<int>{1,1},
    };
    private List<List<int>> compute3 = new List<List<int>>() {
        new List<int>{ 1, 5 },
        new List<int>{3,3},
    };
    private List<List<int>> compute4 = new List<List<int>>() {
        new List<int>{ 3, 5 },
        new List<int>{1,3,3},
    };
    private List<List<int>> compute5 = new List<List<int>>() {
        new List<int> { 1, 3, 5 },
        new List<int>{3,3,5},
    };
    private List<List<List<int>>> computes;
    int plantSum = 0;
    int moutainSum = 0;
    int yardSum = 0;

    private Dictionary<int, List<Drawable>> plantDrawables = new Dictionary<int, List<Drawable>>();
    private Dictionary<int, List<Drawable>> moutainDrawables = new Dictionary<int, List<Drawable>>();
    private Dictionary<int, List<Drawable>> yardDrawables = new Dictionary<int, List<Drawable>>();
    private void Awake()
    {
        EventManager.Instance.AddEventListener(eventType.roundVision, OnRoundVision);
        EventManager.Instance.AddEventListener(eventType.initRoom, InitRoom);
        EventManager.Instance.AddEventListener(eventType.roundDraw, DrawPic);
        EventManager.Instance.AddEventListener(eventType.AddDrawWeight, AddDrawWeight);
    }
    // Start is called before the first frame update
    void Start()
    {
        computes = new List<List<List<int>>>() { compute1, compute2, compute3, compute4, compute5 };
    }

    // Update is called once per frame
    void Update()
    {

    }
    void AddDrawWeight(object obj)
    {
        Card card = (Card)obj;
        switch(card.getCardColor())
        {
            case CardColor.moutain:
                moutainSum += card.getCardRank();
                break;
            case CardColor.plant:
                plantSum += card.getCardRank();
                break;
            case CardColor.yard:
                yardSum += card.getCardRank();
                break;
        }
    }
    void DrawPic(object obj)
    {
        if(plantSum>0)
        {
            int comp = (plantSum - 2) / 5;
            SelectToDraw(computes[comp], plantDrawables);
            plantSum = 0;
        }
        if(moutainSum>0)
        {
            int comp = (moutainSum - 2) / 5;
            SelectToDraw(computes[comp], moutainDrawables);
            moutainSum = 0;
        }
        if (yardSum > 0)
        {
            int comp = (yardSum - 2) / 5;
            SelectToDraw(computes[comp], yardDrawables);
            yardSum = 0;
        }
    }

    void SelectToDraw(List<List<int>> compute, Dictionary<int, List<Drawable>> drawDic)
    {
        bool isEmpty = true;
        foreach(var k in drawDic)
        {
            if(k.Value.Count>0)
            {
                isEmpty = false;
                break;
            }
        }
        if(isEmpty)
        {
            print("已经没有可绘制建筑物");
            return;
        }
        int l = compute.Count;
        int index = Random.Range(0, l - 1);
        List<int> combine = compute[index];
        foreach(var k in combine)
        {
            if (drawDic.ContainsKey(k)&&drawDic[k].Count>0)
            {
                drawDic[k][0].gameObject.SetActive(true);
                drawDic[k].RemoveAt(0);
                while(drawDic[k].Count==0)
                {
                    drawDic[k] = drawDic[(k + 2) % 6];
                }
            }
            else
            {
                print("已经没有可绘制建筑物");
            }
        }
    }

    void OnRoundVision(object obj)
    {
        Card card = (Card)obj;
        switch(card.getCardColor())
        {
            case CardColor.moutain:
                moutainSum += card.getCardRank();
                break;
            case CardColor.plant:
                plantSum += card.getCardRank();
                break;
            case CardColor.yard:
                yardSum += card.getCardRank();
                break;
        }
    }
    void InitRoom(object obj)
    {
        plantDrawables.Clear();
        moutainDrawables.Clear();
        yardDrawables.Clear();
        //打乱所有可绘制物品，形成随机绘画效果
        List<Drawable> objects = DrawRandom(new List<Drawable>(FindObjectsOfType<Drawable>()));
        foreach (var draw in objects)
        {
            if (draw.color == CardColor.plant)
            {
                if (plantDrawables.ContainsKey(draw.weight))
                {
                    plantDrawables[draw.weight].Add(draw);
                }
                else
                {
                    plantDrawables.Add(draw.weight, new List<Drawable>() { draw });
                }
            }
            else if (draw.color == CardColor.moutain)
            {
                if (moutainDrawables.ContainsKey(draw.weight))
                {
                    moutainDrawables[draw.weight].Add(draw);
                }
                else
                {
                    moutainDrawables.Add(draw.weight, new List<Drawable>() { draw });
                }
            }
            else
            {
                if (yardDrawables.ContainsKey(draw.weight))
                {
                    yardDrawables[draw.weight].Add(draw);
                }
                else
                {
                    yardDrawables.Add(draw.weight, new List<Drawable>() { draw });
                }
            }
            draw.Erase();
        }
    }
    private List<Drawable> DrawRandom(List<Drawable> drawable)
    {
        //排序之后的List b
        List<Drawable> b = new List<Drawable>();
        //为了降低运算的数量级，当执行完一个元素时，就需要把此元素从原List中移除
        int countNum = drawable.Count;
        //使用while循环，保证将a中的全部元素转移到b中而不产生遗漏
        while (b.Count < countNum)
        {
            //随机将a中序号为index的元素作为b中的第一个元素放入b中
            int index = Random.Range(0, drawable.Count - 1);
            //检测是否重复，保险起见
            if (!b.Contains(drawable[index]))
            {
                //若b中还没有此元素，添加到b中
                b.Add(drawable[index]);
                //成功添加后，将此元素从a中移除，避免重复取值
                drawable.Remove(drawable[index]);
            }
        }
        return b;
    }
}
