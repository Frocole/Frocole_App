using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GroupMemberEnlister : MonoBehaviour
{
    public Text ButtonLabel;
    public UserObject ThisUser;
    public UserDataManager _persistentLoginDataManager;
    public Toggle MyToggle;
    
   
    public void EnlistMember(bool Enlist)
    {
        if (Enlist)
        {
            StartCoroutine(EnlistUser());
        }
        else
        {
            StartCoroutine(DelistUSer());
        }    
    }
       

    IEnumerator EnlistUser()
    {
        LoadingOverlay.AddLoader();
        WWWForm form = new WWWForm();

        form.AddField("username", _persistentLoginDataManager.Username);
        form.AddField("password", _persistentLoginDataManager.Password);
        form.AddField("userid", ThisUser.UserID);
        form.AddField("groupid", _persistentLoginDataManager.GroupData.GroupID);
        form.AddField("courseid", _persistentLoginDataManager.GroupData.CourseID);

        string output = "";
      
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(PersistentData.WebAdress + "PP_EnlistUser.php", form))
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
            }
        }

        //WWW www = new WWW(PersistentData.WebAdress + "PP_EnlistUser.php", form);
        //yield return www;


        
        Debug.Log($"Enlisted {ThisUser.Nickname} [{output}]");
        LoadingOverlay.RemoveLoader();
        yield return new WaitForEndOfFrame();
    }

    IEnumerator DelistUSer()
    {
        LoadingOverlay.AddLoader();
        WWWForm form = new WWWForm();

        form.AddField("username", _persistentLoginDataManager.Username);
        form.AddField("password", _persistentLoginDataManager.Password);
        form.AddField("userid", ThisUser.UserID);
        form.AddField("courseid", _persistentLoginDataManager.GroupData.CourseID);
        form.AddField("groupid", _persistentLoginDataManager.GroupData.GroupID);

        string output = "";
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(PersistentData.WebAdress + "PP_DelistUser.php", form))
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
            }
        }

        //WWW www = new WWW(PersistentData.WebAdress + "PP_DelistUser.php", form);
        //yield return www;

        Debug.Log($"Delisted {ThisUser.Nickname}");
        LoadingOverlay.RemoveLoader();
        yield return new WaitForEndOfFrame();
    }

}
