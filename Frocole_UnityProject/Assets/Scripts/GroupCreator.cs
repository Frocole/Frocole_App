using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GroupCreator : MonoBehaviour
{
    public InputField InputField;
    public Button CreateGroupButton;
    public GameObject NameAlreadyExistsNotification;
    private UserDataManager _persistentLoginDataManager;
    public LoadScene loadScene;

    public void Start()
    {
        _persistentLoginDataManager = PersistentData.Instance.LoginDataManager;
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

                if (output == "" || RootGroupObject.groups == null || RootGroupObject.groups.Length == 0)
                {
                    CreateGroupButton.interactable = true;
                    NameAlreadyExistsNotification.SetActive(false);
                }
                else
                {
                    CreateGroupButton.interactable = false;
                    NameAlreadyExistsNotification.SetActive(true);
                }
            }
        }
        LoadingOverlay.RemoveLoader();
        //WWW www = new WWW(PersistentData.WebAdress + "PP_CheckIfGroupWithNameExistsInCourse.php", form);
        //yield return www;

        //Debug.Log("Group with that name found: {\"groups\": " + www.text + "}");



    }

    public void CreateGroup()
    {
        StartCoroutine(CreateGroupOnDB());
    }

    IEnumerator CreateGroupOnDB()
    {
        LoadingOverlay.AddLoader();
        WWWForm form = new WWWForm();

        form.AddField("username", _persistentLoginDataManager.Username);
        form.AddField("password", _persistentLoginDataManager.Password);
        form.AddField("courseid", _persistentLoginDataManager.CourseData.CourseID);
        form.AddField("groupnickname", InputField.text);

        string output = "";
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(PersistentData.WebAdress + "PP_CreateGroup.php", form))
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
        LoadingOverlay.RemoveLoader();
        //WWW www = new WWW(PersistentData.WebAdress + "PP_CreateGroup.php", form);
        //yield return www;

        loadScene.Load();
    }
}
