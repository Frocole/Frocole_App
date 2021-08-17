using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CourseSubscribtionManager : MonoBehaviour
{
    private UserDataManager _persistentLoginDataManager;
    public RootCourseObject RootCourseObject;
    public GameObject Contentholder;
    public GameObject CourseSelectorPrefab;

    private int YOffset = 100;

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

        string output;
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(PersistentData.WebAdress + "GetAllSubscribableCourses.php", form))
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
                RootCourseObject = JsonUtility.FromJson<RootCourseObject>("{\"courses\": " + output + "}");

                yield return new WaitForEndOfFrame();

                CourseSubscriber GO;
                int i = 0;
                Contentholder.GetComponent<RectTransform>().sizeDelta = new Vector2(0, YOffset * RootCourseObject.courses.Length);

                foreach (CourseObject course in RootCourseObject.courses)
                {
                    Debug.Log(course.CourseName);
                    GO = GameObject.Instantiate(CourseSelectorPrefab, Contentholder.transform).GetComponent<CourseSubscriber>();
                    GO.GetComponent<RectTransform>().localPosition = new Vector3(0, -YOffset, 0) * i + new Vector3(0, -0.5f * YOffset, 0);
                    GO.ThisCourse = course;
                    GO.ButtonLabel.text = course.CourseName;
                    GO._persistentLoginDataManager = _persistentLoginDataManager;

                    if (_persistentLoginDataManager.Subscriptions.Contains(course.CourseID)) GO.MyToggle.SetIsOnWithoutNotify(true);

                    i++;
                }
            }
        }
        LoadingOverlay.RemoveLoader();
        //WWW www = new WWW(PersistentData.WebAdress + "GetAllSubscribableCourses.php", form);
        //yield return www;


    }
}
