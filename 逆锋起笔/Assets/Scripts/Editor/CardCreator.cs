using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class CardCreator : EditorWindow
{
   [MenuItem("Assets/Card Setting")]
   static void AddWindow()
    {
        Rect wr = new Rect(0, 0, 500, 500);
        CardCreator creator = (CardCreator)EditorWindow.GetWindowWithRect(typeof(CardCreator), wr, true, "Card Setting");
        creator.Show();
    }
    private CardSetting card = null;
    private bool isCreate = false;
    private int maxPoint;
    private int cardNum;
    private CardType type;
    private void OnGUI()
    {
        if (!isCreate&&!card)
        {    
            card = EditorGUILayout.ObjectField("设置卡片", card, typeof(CardSetting), true) as CardSetting;


            //string path = "Assets/Resource/Cards" + card.GetType().ToString() + ".asset";

            if (GUILayout.Button("创建新卡片", GUILayout.Width(200)))
            {
                isCreate = true;
                Repaint();
            }
        }
        else if(isCreate)
        {
            card = new CardSetting();
            card = EditorGUILayout.ObjectField("设置卡片", card, typeof(CardSetting), true) as CardSetting;

            isCreate = false;
        }
        else if(card&&!isCreate)
        {
            maxPoint = EditorGUILayout.IntField("花牌最大点数", maxPoint);
            EditorGUILayout.Space();
            cardNum = EditorGUILayout.IntField("每张牌有几张", cardNum);
            EditorGUILayout.Space();
            type =(CardType)EditorGUILayout.EnumPopup("花色", type);

            string path = "Assets/Resources/Cards/" + type.ToString() + ".asset";
            if (GUILayout.Button("保存卡片", GUILayout.Width(200)))
            {
                card.maxPoint = maxPoint;
                card.cardTypeNum = cardNum;
                AssetDatabase.CreateAsset(card, path);
            }
        }
    }
}
