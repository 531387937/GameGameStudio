using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CardSetting : ScriptableObject
{
    public CardType curType = CardType.red;
    public string tex;
    public string model;
}
