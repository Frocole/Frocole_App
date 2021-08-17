using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GroupEditor : MonoBehaviour
{
    public InputField InputField;
    public Button CreateGroupButton;
    public GameObject NameAlreadyExistsNotification;
    private UserDataManager _persistentLoginDataManager;
    public LoadScene loadScene;

    public void Start()
    {

        _persistentLoginDataManager = PersistentData.Instance.LoginDataManager;
        InputField.SetTextWithoutNotify(_persistentLoginDataManager.GroupData.GroupNickname);
    }

    public void CheckIfGroupNameExistsInCourse()
    {
        StartCoroutine(CheckIfGroupNameExistsInCourseOnDB());
    }

    IEnumerator CheckIfGroupNameExistsInCourseOnDB()
    {
        LoadingOverlay.AddLoader();
        WWWForm form = new WWWForm();

        form.AddField("username", _persistentLoginDataManager.Username);
        form.AddField("password", _persistentLoginDataManager.Password);
        form.AddField("courseid", _persistentLoginDataManager.CourseData.CourseID);
        form.AddField("groupnickname", InputField.text);

        string output = "";
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(PersistentData.WebAdress + "PP_CheckIfGroupWithNameExistsInCourse.php", form))
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
                Debug.Log("Group with that name found: {\"groups\": " + output + "}");

                var RootGroupObject = JsonUtility.FromJson<RootGroupObject>("{\"groups\": " + output + "}");

                if ((output == "" || RootGroupObject.groups == null || RootGroupObject.groups.Length == 0))
                {
                    CreateGroupButton.interactable = true;
                    NameAlreadyExistsNotification.SetActive(false);
                }
                else
                {
                    if (RootGroupObject.groups[0].GroupID != _persistentLoginDataManager.GroupData.GroupID) NameAlreadyExistsNotification.SetActive(true);
                    CreateGroupButton.interactable = false;
                }
            }
        }

        //WWW www = new WWW(PersistentData.WebAdress + "PP_CheckIfGroupWithNameExistsInCourse.php", form);
        //yield return www;
        LoadingOverlay.RemoveLoader();
    }

    public void RenameGroup()
    {
        StartCoroutine(RenameGroupOnDB());
    }

    IEnumerator RenameGroupOnDB()
    {
        LoadingOverlay.AddLoader();
        WWWForm form = new WWWForm();

        form.AddField("username", _persistentLoginDataManager.Username);
        form.AddField("password", _persistentLoginDataManager.Password);
        form.AddField("courseid", _persistentLoginDataManager.CourseData.CourseID);
        form.AddField("groupid", _persistentLoginDataManager.GroupData.GroupID);
        form.AddField("groupnickname", InputField.text);

        string output = "";
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(PersistentData.WebAdress + "PP_RenameGroup.php", form))
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

        //WWW www = new WWW(PersistentData.WebAdress + "PP_RenameGroup.php", form);
        //yield return www;
        LoadingOverlay.RemoveLoader();
        loadScene.Load();

    }
}
