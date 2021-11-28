using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawable : MonoBehaviour
{
    public CardColor color; //GameObject对应花色
    public int weight;      //GameObject的权重大小
    // Start is called before the first frame update
    void Start()
    {
        //gameObject.SetActive(false);
    }

    public void Draw()
    {
        gameObject.SetActive(true);
    }
    public void Erase()
    {
        gameObject.SetActive(false);
    }
}
