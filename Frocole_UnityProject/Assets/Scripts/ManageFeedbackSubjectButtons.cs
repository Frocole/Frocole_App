using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class ManageFeedbackSubjectButtons : MonoBehaviour
{
    private UserDataManager _persistentLoginDataManager;
    public GameObject NoGroupOverlay;

    public RootUserObject RootUserObject; public RootGroupObject RootGroupObject;
    public GameObject Contentholder;
    public GameObject UserSelectorPrefab;
    public bool IsTeacher = false;

    private int YOffset = 122;

    private void Start()
    {
        _persistentLoginDataManager = PersistentData.Instance.LoginDataManager;
        if (IsTeacher) StartCoroutine(RequestAllUsers());
        else StartCoroutine(FindGroup());
    }

    IEnumerator FindGroup()
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
                // If succesfull:
                RootGroupObject = JsonUtility.FromJson<RootGroupObject>("{\"groups\": " + output + "}");

                if (output == "" || RootGroupObject.groups == null || RootGroupObject.groups.Length == 0)
                {
                    NoGroupOverlay.SetActive(true);
                }
                else
                {
                    _persistentLoginDataManager.GroupData = RootGroupObject.groups[0];
                    StartCoroutine(RequestAllUsers());
                }
            }
        }
        
        LoadingOverlay.RemoveLoader();
    }

    IEnumerator RequestAllUsers()
    {
        LoadingOverlay.AddLoader();
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

        SubjectSelector GO;
        int i = 0;
        Contentholder.GetComponent<RectTransform>().sizeDelta = new Vector2(0, YOffset * RootUserObject.users.Length);

        List<string> contributors = new List<string>();
        foreach (UserObject user in RootUserObject.users)
        {
            contributors.Add(user.UserID);
        }

        List<UserObject> usersInGroup = RootUserObject.users.ToList();

        foreach (UserObject user in RootUserObject.users)
        {
            GO = GameObject.Instantiate(UserSelectorPrefab, Contentholder.transform).GetComponent<SubjectSelector>();
            GO.GetComponent<RectTransform>().localPosition = new Vector3(0, -YOffset, 0) * i + new Vector3(0, -0.5f * YOffset, 0);

            //manage Nicknames (avoid Doubles and Mark myself as "(ikzelf)")

            string nickname = user.Nickname;
            if (usersInGroup.FindAll(x => x.Nickname == user.Nickname).Count > 1) // more than one user with the same Nickname
            {
                int nickNameNR = 1;
                foreach (UserObject otherUser in usersInGroup.FindAll(x => x.Nickname == user.Nickname))
                {
                    if (int.Parse(otherUser.UserID) < int.Parse(user.UserID)) nickNameNR++;
                }
                nickname = user.Nickname + " " + nickNameNR.ToString();
            }
            if (user.UserID == _persistentLoginDataManager.UserData.UserID) nickname += " (ikzelf)";


            GO.ButtonLabel.text = nickname;

            GO.ThisSubject.SubjectName = nickname;
            GO.ThisSubject.SubjectID = user.UserID;
            GO.ThisSubject.Contributors = contributors.ToArray();
            GO.ThisSubject.FeedbackType = SpiderGraph.FeedbackType.IPF_RD;


            GO.ThisSubject.Public = (user.Public == "1");
            i++;

        }            
        LoadingOverlay.RemoveLoader();

    }
}
