using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData : MonoBehaviour
{
    public static string FROCOLE_USERNAME = "FROCOLE_USERNAME";
    public static string FROCOLE_PASSWORD = "FROCOLE_PASSWORD";
    public static string FROCOLE_URL = "FROCOLE_URL";

    public static PersistentData Instance;
    public static string WebAdress;

    public void SetWebAdress(string newURL)
    {
        WebAdress = newURL;
    }

    public UserDataManager LoginDataManager;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        if (PlayerPrefs.HasKey(FROCOLE_URL)) WebAdress = PlayerPrefs.GetString(FROCOLE_URL);
        

        Instance = this;
        DontDestroyOnLoad(this);
    }
}
