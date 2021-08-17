using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SetSelfReflectionPublic : MonoBehaviour
{
    public UserDataManager _persistentLoginDataManager;

    private void Start()
    {
        _persistentLoginDataManager = PersistentData.Instance.LoginDataManager;
    }

    public void PublishSelfreflection(bool publish)
    {
        if (publish)
        {
            StartCoroutine(setValuepublic());
        }
        else
        {
            StartCoroutine(setValueprivate());
        }

    }

    IEnumerator setValuepublic()
    {
        LoadingOverlay.AddLoader();
        WWWForm form = new WWWForm();

        form.AddField("username", _persistentLoginDataManager.Username);
        form.AddField("password", _persistentLoginDataManager.Password);
        form.AddField("groupid", _persistentLoginDataManager.GroupData.GroupID);
        form.AddField("public", "1");

        string output = "";
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(PersistentData.WebAdress + "PP_SetSelfReflectionPublic.php", form))
        {
            yield return WWW_.SendWebRequest();

            if (WWW_.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(WWW_.error);
                // If failed:
            }
            else
            {
                output = WWW_.downloadHandler.text;
                // If succesfull:
            }
        }

        //Debug.Log($"published Selfreflection {_persistentLoginDataManager.Username} {_persistentLoginDataManager.Password} {_persistentLoginDataManager.GroupData.GroupID} , 1");
        Debug.Log(output);
        LoadingOverlay.RemoveLoader();
        yield return new WaitForEndOfFrame();
    }

    IEnumerator setValueprivate()
    {
        LoadingOverlay.AddLoader();
        WWWForm form = new WWWForm();

        form.AddField("username", _persistentLoginDataManager.Username);
        form.AddField("password", _persistentLoginDataManager.Password);
        form.AddField("groupid", _persistentLoginDataManager.GroupData.GroupID);
        form.AddField("public", "0");


        string output = "";
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(PersistentData.WebAdress + "PP_SetSelfReflectionPublic.php", form))
        {
            yield return WWW_.SendWebRequest();

            if (WWW_.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(WWW_.error);
                // If failed:
            }
            else
            {
                output = WWW_.downloadHandler.text;
                // If succesfull:
            }
        }
      
        Debug.Log("un-published Selfreflection");
        LoadingOverlay.RemoveLoader();
        yield return new WaitForEndOfFrame();
    }
}
