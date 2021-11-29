using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScrollPlay : MonoBehaviour
{
    private Image image;
    private Animator anim;
    private SpriteRenderer sp;
    public GameObject battleGround;
    private bool play = false;
    private void Awake()
    {
        image = GetComponent<Image>();
        anim = GetComponent<Animator>();
        sp = GetComponent<SpriteRenderer>();
        EventManager.Instance.AddEventListener(eventType.initRoom, InitRoom);
    }

    private void InitRoom(object obj)
    {
        play = true;
        anim.SetBool("played", play);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (play)
        {
            anim.SetTrigger("play");
            battleGround.SetActive(false);
            EventManager.Instance.FireEvent(eventType.waitTween, false);
            play = false;
        }
        image.sprite = sp.sprite;
        
    }
    public void EndPlay()
    {
        anim.SetBool("played", play);
        battleGround.SetActive(true);
        EventManager.Instance.FireEvent(eventType.waitTween, true);
    }
}
