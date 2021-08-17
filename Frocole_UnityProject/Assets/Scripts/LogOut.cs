using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogOut : MonoBehaviour
{
    public LoadScene Sceneloader;
    public void LogOutNow()
    {
        PlayerPrefs.DeleteKey(PersistentData.FROCOLE_PASSWORD);
        Sceneloader.Load();
    }
}
