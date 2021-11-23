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
        public GameObject matchPanel;//匹配面板
        public Text textCount;//用于显示玩家人数
        public GameObject gamePanel;//游戏面板
        public Text textMatch;//显示是否能够点击匹配
        public Text[] textPlayers;//显示各个玩家的名称

        private bool isConnected = false;//是否已连接服务器
        private int playerCount = 0;//房间人数

        private List<PlayerInfo> players = new List<PlayerInfo>();//房间内的玩家信息
        private int localPlayerId = 0;//本地玩家的id
        

        private void Start()
        {
            NetManager.AddEventListener(NetEvent.ConnectSucc, OnConnectSucc);
            NetManager.AddEventListener(NetEvent.ConnectFail, OnConnectFail);
            NetManager.AddEventListener(NetEvent.Close, OnConnectClose);

            NetManager.AddMsgListener("MsgRoomInfo", OnMsgRoomInfo);//添加消息监听函数

            matchPanel.SetActive(false);
            gamePanel.SetActive(false);
            textMatch.text = "开始匹配";
        }

        //Update
        public void Update()
        {
            NetManager.Update();//驱动网络通信的tick

            if (isConnected)
            {
                //如果已连接服务器，显示匹配面板
                matchPanel.SetActive(true);
            }

            if (matchPanel.activeInHierarchy==true&& playerCount > 0)
            {
                //显示当前房间人数
                textCount.text = string.Format("匹配中：[{0}/3]", playerCount);
                //此时不能再次匹配
                textMatch.text = "取消匹配";
            }
            else
            {
                textCount.text = "未匹配";
            }

            if (playerCount == 3)
            {
                //如果满3人，显示游戏面板
                gamePanel.SetActive(true);
                foreach (var p in players)
                {
                    textPlayers[p.playerId - 1].text = string.Format("P{0}: {1}", p.playerId, p.playerName);
                    if(p.playerId == localPlayerId)
                    {
                        textPlayers[p.playerId - 1].text += "(本地玩家)";
                    }
                }
            }
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

        public void OnMatchClick()
        {
            if(textMatch.text == "开始匹配")
            {
                MsgStartMatch msg = new MsgStartMatch();
                NetManager.Send(msg);
            }
            else
            {
                //取消匹配
                MsgCancelMatch msg = new MsgCancelMatch();
                NetManager.Send(msg);

                playerCount = 0;
                textMatch.text = "开始匹配";
            }
        }

        //监听连接成功的事件
        void OnConnectSucc(string err)
        {
            Debug.Log("OnConnectSuccc");
            isConnected = true;
        }

        //监听连接失败的事件
        void OnConnectFail(string err)
        {
            Debug.Log("OnConnectFail" + err);

        }

        //监听关闭连接的事件
        void OnConnectClose(string err)
        {
            Debug.Log("OnConnectClose");
        }

        /// <summary>
        /// 监听MsgRoomInfo的函数
        /// </summary>
        /// <param name="msgBase"></param>
        void OnMsgRoomInfo(MsgBase msgBase)
        {
            MsgRoomInfo msg = (MsgRoomInfo)msgBase;//转为子类

            //读取消息中的内容
            playerCount = msg.remotePlayers.Count + 1;

            players.Clear();
            players.Add(msg.localPlayer);
            foreach(var p in msg.remotePlayers)
            {
                players.Add(p);
            }
            localPlayerId = msg.localPlayer.playerId;
        }

        /// <summary>
        /// 监听MsgCancel的函数
        /// 表示有玩家强行退出
        /// </summary>
        /// <param name="msgBase"></param>
        void OnMsgCancelMatch(MsgBase msgBase)
        {
            //TODO
        }
    }
}
