using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EndPanel : MonoBehaviour
{
    public Text winInfo;
    public Text EndTimer;
    float timer = 0;
    bool gameOver = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        EndTimer.text = string.Format( "{0:F1}s后自动开始下一局",timer);
        if(timer>0&&gameOver)
        {
            timer -= Time.deltaTime;
        }
        if(gameOver&&timer<=0)
        {
            UIManager.Instance.ContiuneGame(true);
        }
    }

    public void GameOver(List<Player> players)
    {
        gameOver = true;
        for(int i = 0;i<players.Count;i++)
        {
            winInfo.text = players[i].playerName + "和牌,牌型为" + players[i].wintype.ToString()+"\n";
        }
        timer = 10;
    }

    private void OnDisable()
    {
        gameOver = false;
    }
}
