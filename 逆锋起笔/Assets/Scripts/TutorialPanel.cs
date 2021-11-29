using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TutorialPanel : MonoBehaviour
{
    public List<string> explains;
    public List<Sprite> sprites;
    public Text explainText;
    public Text Title1;
    public Text Title2;
    public Image explainImage;
    public Sprite step1;
    public Sprite step2;
    [SerializeField]
    private List<Button> btns;
    // Start is called before the first frame update
    void Start()
    {
        btns =new List<Button>(transform.GetComponentsInChildren<Button>());
        for (int i = 0; i < btns.Count; i++)
        {
            Button btn = btns[i];
            if (btn.gameObject.name == "BackButton")
            {
                btns.Remove(btn);
                i--;
                continue;
            }
        }
            for (int i = 0;i<btns.Count;i++)
        {
            Button btn = btns[i];
            btn.onClick.AddListener(() =>
            {
                Title1.text = btn.gameObject.name[0].ToString();
                Title2.text = btn.gameObject.name[1].ToString();
                btn.GetComponent<Image>().sprite = step1;
                int temp = 0;
                for (int j = 0;j<btns.Count;j++)
                {
                    if (btns[j] != btn)
                    {
                        btns[j].GetComponent<Image>().sprite = step2;
                    }
                    else
                        temp = j;
                }
                print(temp);
                explainText.text = explains[temp];
                explainImage.sprite = sprites[temp];
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
