using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class StudentGroupOverview : MonoBehaviour
{
    private UserDataManager _persistentLoginDataManager;
    public GameObject NoGroupOverlay;
    public GameObject LoadingOverlayGO;

    public RootUserObject RootUserObject; public RootGroupObject RootGroupObject;
    public SubjectSelector GPFRDSelector;
    public Toggle ToggleSelfreflectionpublic;

    //private int YOffset = 500;

    private void Start()
    {
        _persistentLoginDataManager = PersistentData.Instance.LoginDataManager;
        StartCoroutine(FindGroup());
    }

    IEnumerator FindGroup()
    {
        LoadingOverlay.AddLoader();
        // attempt to login
        WWWForm form = new WWWForm();

        form.AddField("username", _persistentLoginDataManager.Username);
        form.AddField("password", _persistentLoginDataManager.Password);
        form.AddField("courseid", _persistentLoginDataManager.CourseData.CourseID);

        Debug.Log("FindGroup 1");

        string output = "";
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(PersistentData.WebAdress + "PP_GetGroup.php", form))
        {
            Debug.Log("FindGroup 2");
            yield return WWW_.SendWebRequest();

            Debug.Log("FindGroup 3");


            if (WWW_.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("FindGroup 4a");
                Debug.Log(WWW_.error);
                NoGroupOverlay.SetActive(true);
                // If failed:
            }
            else
            {
                Debug.Log("FindGroup 4b");
                output = WWW_.downloadHandler.text;
                // If succesfull: 
                RootGroupObject = JsonUtility.FromJson<RootGroupObject>("{\"groups\": " + output + "}");

                if (output == "" || RootGroupObject.groups == null || RootGroupObject.groups.Length == 0)
                {
                    NoGroupOverlay.SetActive(true);
                }
                else
                {
                    LoadingOverlayGO?.SetActive(false);

                    _persistentLoginDataManager.GroupData = RootGroupObject.groups[0];
                    StartCoroutine(SetUpGroupSubjectSelector());
                    if (ToggleSelfreflectionpublic != null) ToggleSelfreflectionpublic.SetIsOnWithoutNotify(_persistentLoginDataManager.GroupData.Public == "1");
                }
            }
        }

        //Debug.Log("ACTIVE GROUP WITHIN COURSE {\"groups\": " + www.text + "}");

        LoadingOverlay.RemoveLoader();
    }

    IEnumerator SetUpGroupSubjectSelector()
    {
        LoadingOverlay.AddLoader();
        // attempt to login
        WWWForm form = new WWWForm();

        form.AddField("username", _persistentLoginDataManager.Username);
        form.AddField("password", _persistentLoginDataManager.Password);
        form.AddField("groupid", _persistentLoginDataManager.GroupData.GroupID);
        form.AddField("courseid", _persistentLoginDataManager.CourseData.CourseID);

        string output = "";
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
                RootUserObject = JsonUtility.FromJson<RootUserObject>("{\"users\": " + output + "}");

                yield return new WaitForEndOfFrame();

                //Debug.Log("AVAILABLE SUBJECT WITHIN GROUP {\"users\": " + www.text + "}");

                List<string> contributors = new List<string>();
                foreach (UserObject user in RootUserObject.users)
                {
                    contributors.Add(user.UserID);
                }
                contributors.Add(_persistentLoginDataManager.CourseData.LeraarUserID);

                GPFRDSelector.ButtonLabel.text = _persistentLoginDataManager.GroupData.GroupNickname;
                GPFRDSelector.ThisSubject.Contributors = contributors.ToArray();
                GPFRDSelector.ThisSubject.FeedbackType = SpiderGraph.FeedbackType.GPF_RD;
                GPFRDSelector.ThisSubject.SubjectID = "GPF" + _persistentLoginDataManager.GroupData.GroupID;
                GPFRDSelector.ThisSubject.SubjectName = _persistentLoginDataManager.GroupData.GroupNickname;
            }
        }



        LoadingOverlay.RemoveLoader();
    }
}
