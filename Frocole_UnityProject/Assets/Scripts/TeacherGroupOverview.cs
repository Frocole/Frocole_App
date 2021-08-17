using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TeacherGroupOverview : MonoBehaviour
{
    private UserDataManager _persistentLoginDataManager;
    public RootUserObject RootUserObject;
    public SubjectSelector GPFRDSelector;

    //private int YOffset = 500;

    private void Start()
    {
        _persistentLoginDataManager = PersistentData.Instance.LoginDataManager;
        StartCoroutine(SetUpGroupSubjectSelector());
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

        Debug.Log($"{_persistentLoginDataManager.Username } {_persistentLoginDataManager.Password}{_persistentLoginDataManager.GroupData.GroupID}{_persistentLoginDataManager.CourseData.CourseID}");

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
            }
        }

        RootUserObject = JsonUtility.FromJson<RootUserObject>("{\"users\": " + output + "}");

        yield return new WaitForEndOfFrame();

        Debug.Log("AVAILABLE SUBJECT WITHIN GROUP {\"users\": " + output + "}");

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

        LoadingOverlay.RemoveLoader();
    }
}
