using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class CardCreator : EditorWindow
{
   [MenuItem("Assets/Card Setting")]
   static void AddWindow()
    {
        Rect wr = new Rect(0, 0, 800, 800);
        CardCreator creator = (CardCreator)EditorWindow.GetWindowWithRect(typeof(CardCreator), wr, true, "Card Setting");
        creator.Show();
    }
    [MenuItem("Assets/Global Setting")]
    static void CreateSetting()
    {
        string path = "Assets/Resources/Cards/GlobalSetting.asset";
        GlobalSetting setting = CreateInstance<GlobalSetting>();
        setting.cardNum = 3;
        setting.maxPoint = 16;
        AssetDatabase.CreateAsset(setting, path);
    }
    private CardSetting card = null;
    private bool isCreate = false;
    private string texName;
    private string modelName;
    private CardType type;
    private void OnGUI()
    {
        if (!isCreate)
        {    

            if (GUILayout.Button("创建新卡片", GUILayout.Width(200)))
            {
                isCreate = true;
            }
        }
        else
        {
            card = CreateInstance<CardSetting>();
            EditorGUILayout.TextArea("该类卡牌对应贴图名，填充内容为\"Resources下的文件名\\图片名_\"（对应贴图命名为 名_+点数)");
            EditorGUILayout.Space();
            texName = EditorGUILayout.TextField(texName);
            EditorGUILayout.Space();
            EditorGUILayout.TextArea("该类卡牌对应预制体,填充内容为\"Resources下的文件名\\模型名_\"（对应预制体命名为 名_+点数)");
            modelName = EditorGUILayout.TextField(modelName);
            EditorGUILayout.Space();
            type =(CardType)EditorGUILayout.EnumPopup("花色", type);

            string path = "Assets/Resources/Cards/card" + type.ToString() + ".asset";
            if (GUILayout.Button("保存卡片", GUILayout.Width(200)))
            {
                card.tex = texName;
                card.model = modelName;
                card.curType = type;
                AssetDatabase.CreateAsset(card, path);
            }
        }
    }
}
