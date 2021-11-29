using Newtonsoft.Json;
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
        public GameObject waitingPanel;
        private bool isConnected = false;//是否已连接服务器
        private int playerCount = 0;//房间人数

        private List<PlayerInfo> players = new List<PlayerInfo>();//房间内的玩家信息
        private int localPlayerId = 0;//本地玩家的id

        private bool canMatch = true;//是否可以匹配
        private bool isMatching = false;//是否正在匹配


        private void Start()
        {
            

            NetManager.AddEventListener(NetEvent.ConnectSucc, OnConnectSucc);
            NetManager.AddEventListener(NetEvent.ConnectFail, OnConnectFail);
            NetManager.AddEventListener(NetEvent.Close, OnConnectClose);

            NetManager.AddMsgListener("MsgRoomInfo", OnMsgRoomInfo);//添加消息监听函数
            NetManager.AddMsgListener("MsgCancelMatch", OnMsgCancelMatch);

            matchPanel.SetActive(false);
            gamePanel.SetActive(false);
            textMatch.text = "开始匹配";
        }

        //Update
        public void Update()
        {
            //NetManager.Update();//驱动网络通信的tick

            if (isConnected)
            {
                //如果已连接服务器，显示匹配面板
                matchPanel.SetActive(true);
                waitingPanel.SetActive(false);
            }

            if (matchPanel.activeInHierarchy == true)
            {
                if (isMatching)
                {
                    //显示当前房间人数
                    textCount.text = string.Format("匹配中：[{0}/3]", playerCount);
                }
                else
                {
                    textCount.text = "未匹配";
                }

                if (canMatch)
                {
                    textMatch.text = "开始匹配";
                }
                else
                {
                    textMatch.text = "取消匹配";
                }
            }

            if (playerCount == 3)
            {
                //如果满3人，显示游戏面板
                gamePanel.SetActive(true);
                matchPanel.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                OnConnectClick();
            }
        }

        public void OnSerializeClick()
        {
            MsgInitCards msg = new MsgInitCards();
            msg.Cards = new Dictionary<int, List<CardInfo>>();
            byte[] bytes = Encode(msg);
            MsgInitCards newMsg = (MsgInitCards)(Decode("MsgInitCards", bytes, 0, bytes.Length));

            bytes = MsgBase.Encode(msg);
            newMsg = (MsgInitCards)MsgBase.Decode("MsgInitCards", bytes, 0, bytes.Length);

            Debug.Log("sss");
        }

        public byte[] Encode(MsgBase msgBase)
        {
            string s = JsonConvert.SerializeObject(msgBase);
            return System.Text.Encoding.Default.GetBytes(s);
        }

        public MsgBase Decode(
            string protoName,
            byte[] bytes,
            int offset,
            int count)
        {
            string s = System.Text.Encoding.Default.GetString(bytes, offset, count);
            //MsgBase msgBase = (MsgBase)JsonUtility.FromJson(s, Type.GetType(protoName));
            Console.WriteLine(s);
            MsgBase msgBase = (MsgBase)JsonConvert.DeserializeObject(s, Type.GetType("Network." + protoName));
            return msgBase;
        }

        //玩家点击连接按钮
        public void OnConnectClick()
        {
            NetManager.Connect("127.0.0.1", 8888);
        }

        //玩家点击连接按钮
        public void OnConnectClick2()
        {
            NetManager.Connect("42.193.169.30", 8888);
        }

        //玩家点击关闭按钮
        public void OnCloseClick()
        {
            NetManager.Close();
        }

        public void OnMatchClick()
        {
            if (canMatch)
            {
                MsgStartMatch msg = new MsgStartMatch();
                NetManager.Send(msg);

                canMatch = false;
            }
            else
            {
                //取消匹配
                MsgCancelMatch msg = new MsgCancelMatch();
                NetManager.Send(msg);
                isMatching = false;
                playerCount = 0;

                canMatch = true;
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
            foreach (var p in msg.remotePlayers)
            {
                players.Add(p);
            }
            localPlayerId = msg.localPlayer.playerId;
            isMatching = true;
        }

        /// <summary>
        /// 监听MsgCancel的函数
        /// 表示有玩家强行退出
        /// </summary>
        /// <param name="msgBase"></param>
        void OnMsgCancelMatch(MsgBase msgBase)
        {
            isMatching = false;
            canMatch = true;
            playerCount = 0;
        }

        /// <summary>
        /// 退出游戏时关闭网络连接
        /// </summary>
        private void OnApplicationQuit()
        {
            NetManager.Close();
        }
    }
}
