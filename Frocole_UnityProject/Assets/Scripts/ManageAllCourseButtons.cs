using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ManageAllCourseButtons : MonoBehaviour
{
    private UserDataManager _persistentLoginDataManager;
    public RootCourseObject RootCourseObject;
    public GameObject Contentholder;
    public GameObject CourseSelectorPrefab;

    private int YOffset = 122;

    private void Start()
    {
        _persistentLoginDataManager = PersistentData.Instance.LoginDataManager;
        StartCoroutine(RequestAllCourses());
    }

    IEnumerator RequestAllCourses()
    {
        LoadingOverlay.AddLoader();
        // attempt to login
        WWWForm form = new WWWForm();

        form.AddField("username", _persistentLoginDataManager.Username);
        form.AddField("password", _persistentLoginDataManager.Password);

        string output = "";
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(PersistentData.WebAdress + "PP_GetAllMySubScribedCourses.php", form))
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

        //WWW www = new WWW(PersistentData.WebAdress + "PP_GetAllMySubScribedCourses.php", form);
        //yield return www;
        Debug.Log("ALL SUBSCRIBED COURSES {\"courses\": " + output + "}");
        RootCourseObject = JsonUtility.FromJson<RootCourseObject>("{\"courses\": " + output + "}");

        yield return new WaitForEndOfFrame();



        CourseSelector GO;
        int i = 0;
        Contentholder.GetComponent<RectTransform>().sizeDelta = new Vector2(0, YOffset * RootCourseObject.courses.Length);

        _persistentLoginDataManager.Subscriptions.Clear();

        foreach (CourseObject course in RootCourseObject.courses)
        {
            _persistentLoginDataManager.Subscriptions.Add(course.CourseID);


            //Debug.Log(course.CourseName);
            GO = GameObject.Instantiate(CourseSelectorPrefab, Contentholder.transform).GetComponent<CourseSelector>();
            GO.GetComponent<RectTransform>().localPosition = new Vector3(0, -YOffset, 0) * i + new Vector3(0, -0.5f * YOffset, 0);
            GO.ThisCourse = course;
            GO.ButtonLabel.text = course.CourseName;

            i++;
        }
        LoadingOverlay.RemoveLoader();
    }
}
