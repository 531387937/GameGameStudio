using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eventType
{
    refreshHandCard,
    initRoom,
    chooseCard,
    refreshRoundResult,
    receiveChooseCard,
    battleEnd,
    roundVision,
    roundDraw,
    waitTween,
    AddDrawWeight
}

public class EventManager : Singleton<EventManager>
{
    public delegate void EventListener(object arg);

    private Dictionary<eventType, EventListener> eventListeners = new Dictionary<eventType, EventListener>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddEventListener(eventType e, EventListener listener)
    {
        //添加事件
        if (eventListeners.ContainsKey(e))
        {
            eventListeners[e] += listener;
        }
        //新增事件
        else
        {
            eventListeners[e] = listener;
        }
    }

    //删除监听事件
    public void RemoveEventListener(eventType e, EventListener listener)
    {
        if (eventListeners.ContainsKey(e))
        {
            eventListeners[e] -= listener;
            //删除
            if (eventListeners[e] == null)
            {
                eventListeners.Remove(e);
            }
        }
    }

    //分发事件
    public void FireEvent(eventType e, object arg)
    {
        if (eventListeners.ContainsKey(e))
        {
            eventListeners[e](arg);
        }
    }
    public void FireEvent(eventType e)
    {
        if (eventListeners.ContainsKey(e))
        {
            eventListeners[e](null);
        }
    }
}
