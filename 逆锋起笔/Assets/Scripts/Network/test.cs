using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Network
{
    public class test : MonoBehaviour
    {
        private void Start()
        {
            NetManager.AddEventListener(NetEvent.ConnectSucc, OnConnectSucc);
            NetManager.AddEventListener(NetEvent.ConnectFail, OnConnectFail);
            NetManager.AddEventListener(NetEvent.Close, OnConnectClose);
        }

        //Update
        public void Update()
        {
            NetManager.Update();
        }

        //玩家点击连接按钮
        public void OnConnectClick()
        {
            NetManager.Connect("127.0.0.1", 8888);
        }

        //玩家点击关闭按钮
        public void OnCloseClick()
        {
            NetManager.Close();
        }


        //连接成功回调
        void OnConnectSucc(string err)
        {
            Debug.Log("OnConnectSuccc");

        }

        //连接失败回调
        void OnConnectFail(string err)
        {
            Debug.Log("OnConnectFail" + err);

        }

        //关闭连接
        void OnConnectClose(string err)
        {
            Debug.Log("OnConnectClose");
        }

    }
}
