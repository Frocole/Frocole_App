using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreviousSceneLoader : MonoBehaviour
{
    public static int Back = 0;

    public int PreviousSceneID = 0;
    public int currentSceneID = 0;

    

    private void OnLevelWasLoaded(int level)
    {
        PreviousSceneID = currentSceneID;
        currentSceneID = level;
        Back = PreviousSceneID;
    }
}
