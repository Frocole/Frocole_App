using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CursistGroupManager : MonoBehaviour
{
    private UserDataManager _persistentLoginDataManager;
    public Text GroupNickname;
    public GameObject NoGroupFoundOverlay;



    // Start is called before the first frame update
    void Start()
    {
        //
        _persistentLoginDataManager = PersistentData.Instance.LoginDataManager;

        // fetch the group 
        StartCoroutine(GetMyGroupInThisCourse());

    }


    IEnumerator GetMyGroupInThisCourse()
    {
        LoadingOverlay.AddLoader();
        WWWForm form = new WWWForm();

        form.AddField("username", _persistentLoginDataManager.Username);
        form.AddField("password", _persistentLoginDataManager.Password);
        form.AddField("courseid", _persistentLoginDataManager.CourseData.CourseID);

        string output = "";
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(PersistentData.WebAdress + "PP_GetGroup.php", form))
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
                if (output == "")
                {
                    //The User is not in any group 
                    NoGroupFoundOverlay.SetActive(true);
                }
                else
                {
                    Debug.Log("{\"groups\": " + output + "}");
                    var data = JsonUtility.FromJson<RootGroupObject>("{\"groups\": " + output + "}");
                    // Debug.Log("{\"users\": " + www.text + "}");

                    if (data.groups.Length != 1)
                    {
                        Debug.Log("Login Failed");
                    }
                    else
                    {
                        _persistentLoginDataManager.GroupData = (JsonUtility.FromJson<RootGroupObject>("{\"groups\": " + output + "}")).groups[0];
                        GroupNickname.text = _persistentLoginDataManager.GroupData.GroupNickname;
                    }
                }
            }
        }
        LoadingOverlay.RemoveLoader();
        //WWW www = new WWW(PersistentData.WebAdress + "PP_GetGroup.php", form);
        //yield return www;
    }
}
