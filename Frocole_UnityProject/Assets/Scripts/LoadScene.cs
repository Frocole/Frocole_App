using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public bool LoadOnLoad = false;
    public string LevelName = "";
    public bool Back = false;

    private void Start()
    {
        if (LoadOnLoad) Load();
    }

    public void Load()
    {
        if (Back) SceneManager.LoadScene(PreviousSceneLoader.Back);

        else SceneManager.LoadScene(LevelName);
    }
}
