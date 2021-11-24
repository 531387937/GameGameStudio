using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CardInstance : MonoBehaviour
{
    public Card card { set => GetComponent<Image>().sprite = value.tex; get { return card; } }
    public int number = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void select()
    {
        SendMessageUpwards("SelectCard", number);
    }
}
