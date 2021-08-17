using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GroupMemberListManager : MonoBehaviour
{
    private UserDataManager _persistentLoginDataManager;
    public RootUserObject RootUserObject;
    public RootUserObject RootUserObject_AlreadyInGroup;
    public GameObject Contentholder;
    public GameObject UserSelectorPrefab;

    private int YOffset = 100;

    private void Start()
    {
        _persistentLoginDataManager = PersistentData.Instance.LoginDataManager;
        StartCoroutine(RequestAllUsers());
    }

    IEnumerator RequestAllUsers()
    {
        LoadingOverlay.AddLoader();
        // attempt to login
        WWWForm form = new WWWForm();

        form.AddField("username", _persistentLoginDataManager.Username);
        form.AddField("password", _persistentLoginDataManager.Password);
        form.AddField("groupid", _persistentLoginDataManager.GroupData.GroupID);
        form.AddField("courseid", _persistentLoginDataManager.CourseData.CourseID);

        string output = "";
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(PersistentData.WebAdress + "PP_GetAvailableGroupMembers.php", form))
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

        //WWW www = new WWW(PersistentData.WebAdress + "PP_GetAvailableGroupMembers.php", form);
        //yield return www;

        RootUserObject = JsonUtility.FromJson<RootUserObject>("{\"users\": " + output + "}");

        yield return new WaitForEndOfFrame();

        Debug.Log("FREE SUBJECT WITHIN COURSE {\"users\": " + output + "}");

        output = "";
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(PersistentData.WebAdress + "PP_GetAllSubjectsInMyGroup.php", form))
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

        //www = new WWW(PersistentData.WebAdress + "PP_GetAllSubjectsInMyGroup.php", form);

        //yield return www;

        RootUserObject_AlreadyInGroup = JsonUtility.FromJson<RootUserObject>("{\"users\": " + output + "}");

        yield return new WaitForEndOfFrame();

        Debug.Log("SUBJECT ALREADY WITHIN GROUP {\"users\": " + output + "}");
              

        GroupMemberEnlister GO;
        int i = 0;
        Contentholder.GetComponent<RectTransform>().sizeDelta = new Vector2(0, YOffset * (RootUserObject.users.Length + RootUserObject_AlreadyInGroup.users.Length));

        

        foreach (UserObject user in RootUserObject_AlreadyInGroup.users)
        {
            if (user.UserID == _persistentLoginDataManager.UserData.UserID) continue;

            GO = GameObject.Instantiate(UserSelectorPrefab, Contentholder.transform).GetComponent<GroupMemberEnlister>();
            GO.GetComponent<RectTransform>().localPosition = new Vector3(0, -YOffset, 0) * i + new Vector3(0, -0.5f * YOffset, 0);

            GO._persistentLoginDataManager = _persistentLoginDataManager;

            GO.ButtonLabel.text = user.Nickname;
            GO.MyToggle.SetIsOnWithoutNotify(true);

            GO.ThisUser = user;

            i++;
        }
        
        foreach (UserObject user in RootUserObject.users)
        {
            if (user.UserID == _persistentLoginDataManager.UserData.UserID) continue;

            GO = GameObject.Instantiate(UserSelectorPrefab, Contentholder.transform).GetComponent<GroupMemberEnlister>();
            GO.GetComponent<RectTransform>().localPosition = new Vector3(0, -YOffset, 0) * i + new Vector3(0, -0.5f * YOffset, 0);

            GO._persistentLoginDataManager = _persistentLoginDataManager;

            GO.ButtonLabel.text = user.Nickname;
            GO.MyToggle.SetIsOnWithoutNotify(false);

            GO.ThisUser = user;

            i++;
        }
        LoadingOverlay.RemoveLoader();
    }
}
