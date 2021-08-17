using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CourseSelector : MonoBehaviour
{
    public Button Button;
    public Text ButtonLabel;
    public CourseObject ThisCourse;

    public void Start()
    {
        Button.onClick.AddListener(OpenCourse);    
    }

    public void OpenCourse()
    {
        PersistentData.Instance.LoginDataManager.CourseData = ThisCourse;


        if (ThisCourse.LeraarUserID == PersistentData.Instance.LoginDataManager.UserData.UserID)
        {
            SceneManager.LoadScene("L_01_Home");
        }
        else
        {
            SceneManager.LoadScene("C_01_Home");
        }
    
    }

    

}
