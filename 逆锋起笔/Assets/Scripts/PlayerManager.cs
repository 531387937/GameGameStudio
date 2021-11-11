using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//用于管理服务器上的玩家行为、流程控制
public class PlayerManager
{
    private List<Player> players;

    public void SetPlayers(int n)
    {
        players = new List<Player>();
        for(int i = 0;i<n;i++)
        {
            Player p = new Player(i);
            players.Add(p);
        }
    }

    public void DrawCard(CardPool pool)
    {
        for(int i = 0;i<players.Count;i++)
        {
            for(int j = 0;j<6;j++)
            {
                players[i].DrawHandCard(pool.Draw());
            }
        }
    }
    //比拼点数
    void CamparePower()
    {

    }
}
