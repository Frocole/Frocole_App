using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GuidelineList : MonoBehaviour
{
    public GameObject ListHolder;
    public GuidelinePrefab guidelinePrefab;

    void Start()
    {
        StartCoroutine(GetAllGuideLinesFromCourse());
    }

    RootPAGuidelineObject Guidelineset;

    private IEnumerator GetAllGuideLinesFromCourse()
    {

        PersistentData.Instance.LoginDataManager.GuidelineData = null;

        WWWForm form = new WWWForm();

        form.AddField("username", PersistentData.Instance.LoginDataManager.Username);
        form.AddField("password", PersistentData.Instance.LoginDataManager.Password);
        form.AddField("courseid", PersistentData.Instance.LoginDataManager.CourseData.CourseID);

        using (UnityWebRequest WWW_ = UnityWebRequest.Post(UriMaker.InsertScriptInUri(PersistentData.WebAdress, "PP_GetAllPAGuidelinesInCourse.php"), form))
        {

            yield return WWW_.SendWebRequest();

            if (WWW_.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(WWW_.error);
            }
            else
            {
                Guidelineset = JsonUtility.FromJson<RootPAGuidelineObject>("{\"paguidelines\": " + WWW_.downloadHandler.text + "}");
            }

        }


        GuidelinePrefab go;
        foreach (var item in Guidelineset.paguidelines)
        {
            go = GameObject.Instantiate(guidelinePrefab, ListHolder.transform);
            go.PAGuidelineObject = item;
            go.Label.text = $"{item.SubjectType} {item.Parameter} {item.Delta}";
        }

        yield return null;


    }
}
