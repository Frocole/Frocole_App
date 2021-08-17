using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SubjectSelector : MonoBehaviour
{
    public string SceneName = "C_03_ProvideFeedback";

    public Button Button;
    public Text ButtonLabel;
    public SubjectObject ThisSubject;

    public void Start()
    {
        Button.onClick.AddListener(OpenCourse);
    }

    public void OpenCourse()
    {
        PersistentData.Instance.LoginDataManager.SubjectData = ThisSubject;
        SceneManager.LoadScene(SceneName);
    }
}
