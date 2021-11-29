using DG.Tweening;
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
    public RectTransform born;
    public RectTransform end;
    public GameObject Stamp;
    public GameObject endUI;
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
            gameOver = false;
        }
    }

    public void GameOver(List<Player> players)
    {
        gameOver = true;
        Stamp.SetActive(true);
        endUI.SetActive(false);
        Stamp.GetComponent<RectTransform>().position = born.position;
        Stamp.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        Text[] name = Stamp.transform.GetComponentsInChildren<Text>();
        for(int i = 0;i<players[0].playerName.Length;i++)
        {
            name[i].text = players[0].playerName[i].ToString();
        }
        Sequence sequence = DOTween.Sequence();
        sequence.Append(Stamp.GetComponent<RectTransform>().DOMove(end.position, 1.5f));
        sequence.Append(Stamp.GetComponent<RectTransform>().DOScale(new Vector3(1.3f,1.3f,1.3f), 1));
        sequence.Append(Stamp.GetComponent<RectTransform>().DOScale(new Vector3(0.5f,0.5f, 0.5f), 0.5f));
        sequence.OnComplete(() => { endUI.SetActive(true); timer = 10; EventManager.Instance.FireEvent(eventType.waitTween, true); });
    }

    private void OnDisable()
    {
        gameOver = false;
        endUI.SetActive(false);
    }
}
