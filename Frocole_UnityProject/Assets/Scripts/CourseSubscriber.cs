using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CourseSubscriber : MonoBehaviour
{
    public Text ButtonLabel;
    public Button BackButton;
    public CourseObject ThisCourse;
    public UserDataManager _persistentLoginDataManager;
    public Toggle MyToggle;
    
   
    public void SubCourse(bool Sub)
    {       
        if (Sub)
        {
            StartCoroutine(SubmitSubToCourse());
        }
        else
        {
            StartCoroutine(SubmitUnsubToCourse());
        }    
    }     

    IEnumerator SubmitSubToCourse()
    {
        LoadingOverlay.AddLoader();
        WWWForm form = new WWWForm();

        form.AddField("username", _persistentLoginDataManager.Username);
        form.AddField("password", _persistentLoginDataManager.Password);
        form.AddField("courseid", ThisCourse.CourseID);

        string output;
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(PersistentData.WebAdress + "PP_SubToCourse.php", form))
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

        Debug.Log($"Subbed to {ThisCourse.CourseID}");
        LoadingOverlay.RemoveLoader();
        yield return new WaitForEndOfFrame();
    }

    IEnumerator SubmitUnsubToCourse()
    {
        LoadingOverlay.AddLoader();
        WWWForm form = new WWWForm();

        form.AddField("username", _persistentLoginDataManager.Username);
        form.AddField("password", _persistentLoginDataManager.Password);
        form.AddField("courseid", ThisCourse.CourseID);

        string output;
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(PersistentData.WebAdress + "PP_UnsubCourse.php", form))
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

        Debug.Log($"Unsubbed {ThisCourse.CourseID}");
        LoadingOverlay.RemoveLoader();
        yield return new WaitForEndOfFrame();
    }

}
