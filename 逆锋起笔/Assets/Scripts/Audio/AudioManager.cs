using UnityEngine;

public class AudioManager
{
    private static AudioManager _instance = null;
    private GameObject m_2dGO = null;

    public static AudioManager GetInstance()
    {
        if(_instance == null)
        {
            _instance = new AudioManager();
        }
        return _instance;
    }

    public void Init()
    {
        m_2dGO = GameObject.Find("WwiseGlobal");
    }


    public void Post2D(string eventName)
    {
        Post3D(eventName, m_2dGO);
    }


    public void Post3D(string eventName, GameObject go)
    {
        AkSoundEngine.PostEvent(eventName, go);
    }


    public void LoadBank(string bankName)
    {
        AkBankManager.LoadBank(bankName, false, false);
    }


    public void UnloadBank(string bankName)
    {
        AkBankManager.UnloadBank(bankName);
    }
}
