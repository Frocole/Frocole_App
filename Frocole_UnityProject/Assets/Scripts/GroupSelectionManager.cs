using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GroupSelectionManager : MonoBehaviour
{
    private UserDataManager _persistentLoginDataManager;

    public RootGroupObject AllGroups;
    public GameObject Contentholder;
    public GameObject GroupManagerPrefab;

    private int YOffset = 100;

    private void Start()
    {
        _persistentLoginDataManager = PersistentData.Instance.LoginDataManager;
        StartCoroutine(FindGroups());
    }
    IEnumerator FindGroups()
    {
        LoadingOverlay.AddLoader();
        // attempt to login
        WWWForm form = new WWWForm();

        form.AddField("username", _persistentLoginDataManager.Username);
        form.AddField("password", _persistentLoginDataManager.Password);
        form.AddField("courseid", _persistentLoginDataManager.CourseData.CourseID);

        string output = "";
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(PersistentData.WebAdress + "PP_GetAllGroupsInCourse.php", form))
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

        //WWW www = new WWW(PersistentData.WebAdress + "PP_GetAllGroupsInCourse.php", form);
        //yield return www;

        Debug.Log("GROUPS WITHIN COURSE {\"groups\": " + output + "}");

        AllGroups = JsonUtility.FromJson<RootGroupObject>("{\"groups\": " + output + "}");

        if (output == "" || AllGroups.groups == null || AllGroups.groups.Length == 0)
        {
            Debug.Log("NO GROUPS FOUND");
        }
        else 
        {

            GroupSelector GO;
            int i = 0;
            Contentholder.GetComponent<RectTransform>().sizeDelta = new Vector2(0, YOffset * AllGroups.groups.Length);

            foreach (var item in AllGroups.groups)
            {
                GO = GameObject.Instantiate(GroupManagerPrefab, Contentholder.transform).GetComponent<GroupSelector>();
                GO.GetComponent<RectTransform>().localPosition = new Vector3(0, -YOffset, 0) * i;// + new Vector3(0, -0.5f * YOffset, 0);

                GO.ThisGroup = item;
                GO.buttonLabel.text = item.GroupNickname;

                i++;
            }
        
        }

        LoadingOverlay.RemoveLoader();

    }

}
