using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingOverlay : MonoBehaviour
{
    //private Dictionary<string, float> activeProcesses = new Dictionary<string, float>();
    public static float Timer = 0;
    public static int Loaders = 0;
    public GameObject overlay;

    public bool AllowTimeOut;
    public int TimeOut = 15;
    public GameObject TimeOutOverlay;

    private void Update()
    {
        Timer += Time.deltaTime;
        overlay.gameObject.SetActive(Loaders > 0);
        TimeOutOverlay.gameObject.SetActive(AllowTimeOut && Loaders > 0 && Timer > TimeOut);
    }

    public static void AddLoader() // LoadingOverlay.Addloader();
    {        
        Loaders += 1;
        Timer = 0;
        Debug.Log("Added Loader " + Loaders);
    }
    public static void RemoveLoader() // LoadingOverlay.RemoveLoader();
    {        
        Loaders = Mathf.Max(0, Loaders - 1);
        Timer = 0;
        Debug.Log("Removed Loader " + Loaders);
    }

    public static void reset()
    {
        Loaders = 0;
        Timer = 0;

    }

}
