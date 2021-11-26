using GameServer.proto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GameServer.net
{
    public class Room
    {
        public List<string> names = new List<string>()
        {
            "神蛊温皇",
            "俏如来",
            "雪山银燕",
            "神田京一",
            "宫本总司",
            "秋水浮萍任缥缈",
            "柳生鬼哭",
            "剑无极",
            "黑白郎君",
            "酆都月",
            "藏镜人"
        };
        //{
        //    "Naruto",
        //    "Sasuke",
        //    "Sakura",
        //    "Kakashi",
        //    "Shikamaru",
        //    "Hinata",
        //    "Jiraiya",
        //    "Orochimaru",
        //    "Tsunade"
        //};
        public Dictionary<int, Player> players = new Dictionary<int, Player>();
        public const int MAX_PLAYER = 3;
        public bool isMatchSucc = false;

        private const int COLOR_COUNT = 3;//花色数
        private const int NUM_COUNT = 7;// 点数数量
        private const int SAME_COUNT = 4;//相同牌的数量
        private const int TOTAL_COUNT = COLOR_COUNT * NUM_COUNT * SAME_COUNT;//总数
        private const int INIT_COUNT = 6;//初始牌数
        private const int DIS_COUNT = 3;//每轮发牌数

        private List<CardInfo> remainCards = new List<CardInfo>();//牌堆
        private Dictionary<int, List<CardInfo>> curCards = new Dictionary<int, List<CardInfo>>();//持有的牌
        private Dictionary<int, List<CardInfo>> preCards = new Dictionary<int, List<CardInfo>>();//留牌区
        private Dictionary<int, List<CardInfo>> chooseCards = new Dictionary<int, List<CardInfo>>();//选牌
        private Dictionary<int, MsgNextBattle> battleChoice = new Dictionary<int, MsgNextBattle>();//下一局的选择

        /// <summary>
        /// 获取玩家个数
        /// </summary>
        /// <returns></returns>
        public int GetPlayerCount()
        {
            return players.Count;
        }

        /// <summary>
        /// 获取所有玩家
        /// </summary>
        /// <returns></returns>
        public List<Player> GetPlayers()
        {
            return new List<Player>(players.Values);
        }

        /// <summary>
        /// 随机获取不重复的名字
        /// </summary>
        /// <returns></returns>
        public string GetRandomName()
        {
            Random rd = new Random();
            int idx = -1;
            for(int i = 0; i < 10000&&idx==-1;i++)
            {
                idx = rd.Next(0,names.Count);
                foreach(var player in players.Values)
                {
                    if (names[idx] == player.data.playerName)
                    {
                        idx = -1;
                        break;
                    }
                }
            }

            if (idx == -1)
                return (DateTime.Now.ToString());
            else
                return names[idx];
        }

        /// <summary>
        /// 添加或者修改玩家编号，从小排起
        /// </summary>
        /// <param name="player"></param>
        public void SetPlayer(Player player)
        {
            for(int i = 1; i <= MAX_PLAYER; i++)
            {
                if (!players.ContainsKey(i))
                {
                    players.Add(i, player);
                    //给玩家设置id
                    player.data.id = i;
                    player.room = this;
                    break;
                }
            }
        }

        /// <summary>
        /// 删除玩家
        /// </summary>
        /// <param name="id"></param>
        public void RemovePlayer(int id)
        {
            if (players.ContainsKey(id))
            {
                players.Remove(id);

                //把剩余的往前移
                for (int i = 1; i <= MAX_PLAYER; i++)
                {
                    if (players.ContainsKey(i))
                    {
                        Player player = players[i];
                        players.Remove(i);
                        SetPlayer(player);
                    }
                }
            }
        }

        /// <summary>
        /// 获取房间信息
        /// </summary>
        /// <param name="localPlayerId"></param>
        /// <returns></returns>
        public MsgRoomInfo GetRoomInfo(int localPlayerId)
        {
            MsgRoomInfo roomInfo = new MsgRoomInfo();
            roomInfo.localPlayer = new PlayerInfo(localPlayerId, players[localPlayerId].data.playerName);
            foreach(var i in players.Keys)
            {
                if (i != localPlayerId)
                {
                    roomInfo.remotePlayers.Add(new PlayerInfo(i, players[i].data.playerName));
                }
            }

            return roomInfo;
        }

        /// <summary>
        /// 给房间内的玩家广播房间信息
        /// </summary>
        public void SendRoomInfo()
        {
            List<Player> players = GetPlayers();
            foreach (var p in players)
            {
                MsgRoomInfo msg = GetRoomInfo(p.data.id);
                NetManager.Send(p.state, msg);
            }
        }

        /// <summary>
        /// 进入游戏
        /// </summary>
        public void StartGame()
        {
            isMatchSucc = true;
            //初始化变量
            remainCards = new List<CardInfo>();
            curCards = new Dictionary<int, List<CardInfo>>();
            preCards = new Dictionary<int, List<CardInfo>>();
            chooseCards = new Dictionary<int, List<CardInfo>>();
            battleChoice = new Dictionary<int, MsgNextBattle>();

            for (int i = 1; i <= MAX_PLAYER; i++)
            {
                curCards.Add(i, new List<CardInfo>());
                preCards.Add(i, new List<CardInfo>());
                chooseCards.Add(i, new List<CardInfo>());
                //battleChoice.Add(i, new MsgNextBattle());
            }

            //初始化牌组
            for (int i = 0; i < COLOR_COUNT; i++)
            {
                //花色
                for(int j = 0; j < NUM_COUNT; j++)
                {
                    //点数
                    for(int k = 0; k < SAME_COUNT; k++)
                    {
                        //相同
                        CardInfo card = new CardInfo();
                        card.cardColor = (CardColor)i;
                        card.num = j + 1;
                        remainCards.Add(card);
                    }
                }
            }

            //获取初始牌
            List<int> idx = GenerateRandom(remainCards.Count, INIT_COUNT*3);
            MsgInitCards msg = new MsgInitCards();
            msg.Cards = new Dictionary<int, List<CardInfo>>();
            for (int i = 0; i < MAX_PLAYER; i++)
            {
                msg.Cards.Add(i+1, new List<CardInfo>());
                for (int j = 0; j < INIT_COUNT; j++)
                {
                    CardInfo card = remainCards[idx[i * INIT_COUNT + j]];
                    msg.Cards[i + 1].Add(card);
                }
            }

            foreach(var cards in msg.Cards.Values)
            {
                foreach (var card in cards)
                {
                    remainCards.Remove(card);
                }
            }

            //广播
            Thread.Sleep(3000);
            BroadCast(msg);
        }

        public void ChooseCard(MsgChooseCard msg)
        {
            chooseCards[msg.playerID].Add(msg.card);
            //广播
            BroadCast(msg);
            
            //是否全部已选满
            int count = 0;
            foreach(var cards in chooseCards.Values)
            {
                count += cards.Count;
            }
            if(count == DIS_COUNT * MAX_PLAYER)
            {

                //对比
                MsgRoundResult msgRoundresult = GetRoundResult();
                //广播
                BroadCast(msgRoundresult);

                //判断是否胡牌
                MsgBattleResult msgBattleResult = GetBattleResult();
                if (msgBattleResult != null)
                {
                    //广播
                    BroadCast(msgBattleResult);
                }
                else
                {
                    //下一回合发牌
                    MsgInitCards msgRoundCards = GetRoundCards();
                    //广播
                    BroadCast(msgRoundCards);
                }
            }
        }

        /// <summary>
        /// 回合对比
        /// </summary>
        /// <returns></returns>
        private MsgRoundResult GetRoundResult()
        {
            MsgRoundResult msg = new MsgRoundResult();
            
            int[] scores = new int[MAX_PLAYER + 1];//用于比较大小，等于(10-CardsType)*1000+总点数
            for(int i = 1; i <= MAX_PLAYER; i++)
            {
                RoundResult res = new RoundResult();

                List<CardInfo> cards = chooseCards[i];
                
                //炸弹
                if(cards[0].cardColor==cards[1].cardColor
                    &&cards[0].cardColor==cards[2].cardColor
                    &&cards[0].num==cards[1].num
                    && cards[0].num == cards[2].num)
                {
                    res.cardsType = CardsType.ZhaDan;
                }
                //同花顺
                else if(cards[0].cardColor == cards[1].cardColor
                    && cards[0].cardColor == cards[2].cardColor
                    &&cards[1].num-cards[0].num==1
                    &&cards[2].num-cards[1].num==1
                    )
                {
                    res.cardsType = CardsType.TongHuaShun;
                }
                //同数字
                else if(cards[0].num == cards[1].num
                    && cards[0].num == cards[2].num
                    )
                {
                    res.cardsType = CardsType.TongShuZi;
                }
                //顺子
                else if(cards[1].num - cards[0].num == 1
                    && cards[2].num - cards[1].num == 1
                    )
                {
                    res.cardsType = CardsType.ShunZi;
                }
                //同花
                else if(cards[0].cardColor == cards[1].cardColor
                    && cards[0].cardColor == cards[2].cardColor
                    )
                {
                    res.cardsType = CardsType.TongHua;
                }
                //同色一对
                else if((cards[0].cardColor==cards[1].cardColor&&cards[0].num==cards[1].num)
                    || (cards[0].cardColor == cards[2].cardColor && cards[0].num == cards[2].num)
                    || (cards[2].cardColor == cards[1].cardColor && cards[2].num == cards[1].num)
                    )
                {
                    res.cardsType = CardsType.TongSeYiDui;
                }
                //异色一对
                else if(cards[0].num == cards[1].num
                    || cards[0].num == cards[2].num
                    || cards[2].num == cards[1].num
                    )
                {
                    res.cardsType = CardsType.YiSeYiDui;
                }
                //单张
                else
                {
                    res.cardsType = CardsType.DanZhang;
                }

                scores[i] = (10 - (int)res.cardsType) * 1000 + cards[0].num + cards[1].num + cards[2].num;
                msg.result.Add(i, res);
            }

            int[] ranks = new int[MAX_PLAYER + 1];//rank[a]=b表示第a名是b
            ranks[1] = 1;
            //排序
            for(int i = 2; i <= MAX_PLAYER; i++)
            {
                int j = i;
                for(; j >1&&scores[i]>scores[j-1]; j--)
                {
                    ranks[j] = ranks[j - 1];
                }
                ranks[j] = i;
            }

            for(int i = 1; i <= MAX_PLAYER; i++)
            {
                for(int j = 1; j <= MAX_PLAYER; j++)
                {
                    if (ranks[j] == i) {
                        msg.result[i].rank = j;//给排名赋值
                    }
                }
            }

            //留牌
            if (scores[ranks[1]] == scores[ranks[2]])
            {
                preCards[ranks[1]].Add(chooseCards[ranks[1]][0]);//留左边一张
                remainCards.Add(chooseCards[ranks[1]][1]);//弃两张
                remainCards.Add(chooseCards[ranks[1]][2]);

                preCards[ranks[2]].Add(chooseCards[ranks[2]][0]);
                remainCards.Add(chooseCards[ranks[2]][1]);
                remainCards.Add(chooseCards[ranks[2]][2]);

                remainCards.Add(chooseCards[ranks[3]][0]);
                remainCards.Add(chooseCards[ranks[3]][1]);
                remainCards.Add(chooseCards[ranks[3]][2]);
            }
            else if(scores[ranks[1]]==scores[ranks[2]])
            {
                preCards[ranks[1]].Add(chooseCards[ranks[1]][0]);//留2弃1
                preCards[ranks[1]].Add(chooseCards[ranks[1]][1]);
                remainCards.Add(chooseCards[ranks[1]][2]);

                remainCards.Add(chooseCards[ranks[2]][0]);
                remainCards.Add(chooseCards[ranks[2]][1]);
                remainCards.Add(chooseCards[ranks[2]][2]);

                remainCards.Add(chooseCards[ranks[3]][0]);
                remainCards.Add(chooseCards[ranks[3]][1]);
                remainCards.Add(chooseCards[ranks[3]][2]);
            }
            else
            {
                preCards[ranks[1]].Add(chooseCards[ranks[1]][0]);//留2弃1
                preCards[ranks[1]].Add(chooseCards[ranks[1]][1]);
                remainCards.Add(chooseCards[ranks[1]][2]);

                preCards[ranks[2]].Add(chooseCards[ranks[2]][0]);
                remainCards.Add(chooseCards[ranks[2]][1]);
                remainCards.Add(chooseCards[ranks[2]][2]);

                remainCards.Add(chooseCards[ranks[3]][0]);
                remainCards.Add(chooseCards[ranks[3]][1]);
                remainCards.Add(chooseCards[ranks[3]][2]);
            }

            for(int i = 1; i <= MAX_PLAYER; i++)
            {
                chooseCards[i].Clear();//清空选牌
            }

            return msg;
        }

        /// <summary>
        /// 胡牌判断
        /// </summary>
        /// <returns></returns>
        private MsgBattleResult GetBattleResult()
        {
            MsgBattleResult msg = new MsgBattleResult();
            int winCount = 0;//胡牌的人数

            for(int i = 1; i <= MAX_PLAYER; i++)
            {
                WinType tmpWinType = WinType.None;
                int numCount = 0;

                List<CardInfo> cards = preCards[i];
                Dictionary<CardColor, int> colorCount = new Dictionary<CardColor, int>();
                colorCount.Add(CardColor.JianZhu, 0);
                colorCount.Add(CardColor.ShanShui, 0);
                colorCount.Add(CardColor.ZhiWu, 0);
                foreach(var card in cards)
                {
                    colorCount[card.cardColor] += 1;
                    numCount += card.num;
                }

                //山水6
                if(colorCount[CardColor.ShanShui]- colorCount[CardColor.JianZhu] >= 6)
                {
                    tmpWinType = WinType.ShanShui;
                }
                //建筑6
                else if (colorCount[CardColor.JianZhu] - colorCount[CardColor.ZhiWu] >= 6)
                {
                    tmpWinType = WinType.JianZhu;
                }
                //植物6
                else if (colorCount[CardColor.ZhiWu] - colorCount[CardColor.ShanShui] >= 6)
                {
                    tmpWinType = WinType.ZhiWu;
                }
                //4张
                else if (colorCount[CardColor.ShanShui] ==4&& colorCount[CardColor.JianZhu] ==4&& colorCount[CardColor.ZhiWu]==4)
                {
                    tmpWinType = WinType.BaoDi;
                }
                //9张，<25
                else if (cards.Count == 9 && numCount == 25)
                {
                    tmpWinType = WinType.XiaoHe;
                }

                if (tmpWinType != WinType.None)
                {
                    msg.result.Add(i, tmpWinType);
                    winCount += 1;
                }
            }

            if (winCount == 0)
                return null;
            else
                return msg;
        }

        /// <summary>
        /// 获取下回合的发牌
        /// </summary>
        /// <returns></returns>
        private MsgInitCards GetRoundCards()
        {
            MsgInitCards msg = new MsgInitCards();
            List<int> idx = GenerateRandom(remainCards.Count, DIS_COUNT * MAX_PLAYER);
            for(int i = 1; i <= MAX_PLAYER; i++)
            {
                msg.Cards.Add(i, new List<CardInfo>());
                for(int j = 0; j < DIS_COUNT; j++)
                {
                    CardInfo card = remainCards[idx[(i - 1) * DIS_COUNT + j]];
                    msg.Cards[i].Add(card);
                }
            }

            foreach (var cards in msg.Cards.Values)
            {
                foreach (var card in cards)
                {
                    remainCards.Remove(card);
                }
            }

            return msg;
        }

        /// <summary>
        /// 获取一组不重复的随机数
        /// 不包括maxNum
        /// </summary>
        /// <param name="maxNum"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private List<int> GenerateRandom(int maxNum, int count)
        {
            List<int> nums = new List<int>();
            Random rd = new Random();
            for(int i = 0; i < 10000 && nums.Count < count; i++)
            {
                int num = rd.Next(maxNum);
                if (!nums.Contains(num))
                {
                    nums.Add(num);
                }
            }

            if(nums.Count < count)
            {
                for(int i = 0; i < maxNum; i++)
                {
                    if (!nums.Contains(i))
                    {
                        nums.Add(i);
                    }
                }
            }

            return nums;
        }

        /// <summary>
        /// 关闭房间
        /// </summary>
        public void CloseRoom()
        {
            List<Player> players = GetPlayers();
            foreach (var p in players)
            {
                MsgCancelMatch msg = new MsgCancelMatch();
                NetManager.Send(p.state, msg);
                p.room = null;
                p.state.player = null;
            }
            RoomManager.rooms.Remove(this);
        }

        /// <summary>
        /// 向房间内的玩家广播消息
        /// </summary>
        /// <param name="msg"></param>
        public void BroadCast(MsgBase msg)
        {
            foreach(var p in players.Values)
            {
                NetManager.Send(p.state, msg);
            }
        }

        /// <summary>
        /// 处理下一局的选择
        /// </summary>
        /// <param name="msg"></param>
        public void ChooseNextBattle(MsgNextBattle msg)
        {
            battleChoice.Add(msg.playerID, msg);
            if (battleChoice.Count == MAX_PLAYER)
            {
                NextBattle();
            }
        }

        /// <summary>
        /// 开启下一局
        /// </summary>
        public void NextBattle()
        {
            MsgNextBattle msgNextBattle = new MsgNextBattle();
            foreach(var msg in battleChoice.Values)
            {
                if (msg.choice == false)
                {
                    msgNextBattle.choice = false;
                    break;
                }  
            }

            BroadCast(msgNextBattle);
            if (msgNextBattle.choice == false)
            {
                //不继续，关闭房间
                List<Player> players = GetPlayers();
                foreach (var p in players)
                {
                    p.room = null;
                    p.state.player = null;
                }
                RoomManager.rooms.Remove(this);
            }
            else
            {
                //继续，开启下一局
                StartGame();
            }
        }
    }
}
