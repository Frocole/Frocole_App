using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FeedbackSubmitter : MonoBehaviour
{
    public string ActiveParameterName;
    public Text ParameterName;
    public int ActiveParameter;
    public SpiderGraph_Profile Profile_UserReview;
    public FillProfile FeedbackDataHolder;
    public Slider slider;
    private UserDataManager _persistentLoginDataManager;
    private Image HighlightedAxis = null;
    public Color HighlightColor = new Color(0.9176471f, 0.07058824f, 0.5411765f);
    public LoadScene loadGPFScene;
    public LoadScene loadIPFScene;


    void Start()
    {
        _persistentLoginDataManager = PersistentData.Instance.LoginDataManager;
    }

    public void OpenFeedBackSubmitter(string Parameter)
    {
        ActiveParameterName = Parameter;
        ParameterName.text = Parameter;
        slider.gameObject.SetActive(true);
    }

    public void SetParameter(Single score)
    {
        ParameterName.text = ActiveParameterName + " : " + score.ToString() + "%";
        FeedbackDataHolder.setCompiledFeedbackScore(_persistentLoginDataManager.UserData.UserID, ActiveParameter, (int)score);
    }

    public void HighlightAxis(Image newHighlightedImage)
    {
        if (HighlightedAxis != null)
        {
            HighlightedAxis.color = Color.black;
        }
        slider.gameObject.SetActive(true);
        HighlightedAxis = newHighlightedImage;
        HighlightedAxis.color = HighlightColor;
    }

    public void Submitscore()
    {
        StartCoroutine(SubmitScoreToDB());
    }

    public void SubmitscoreAndLeave()
    {
        StartCoroutine(SubmitScoreToDBAndLeave());
    }

    private IEnumerator SubmitScoreToDB()
    {
        LoadingOverlay.AddLoader();
        WWWForm form = new WWWForm();

        form.AddField("username", _persistentLoginDataManager.Username);
        form.AddField("password", _persistentLoginDataManager.Password);
        form.AddField("subject", _persistentLoginDataManager.SubjectData.SubjectID);
        form.AddField("parameter", ActiveParameterName);
        form.AddField("score", Mathf.FloorToInt(slider.value).ToString());
        form.AddField("groupid", _persistentLoginDataManager.GroupData.GroupID);

        string output = "";
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(PersistentData.WebAdress + "PP_SubmitFeedBack.php", form))
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
        LoadingOverlay.RemoveLoader();
        //WWW www = new WWW(PersistentData.WebAdress + "PP_SubmitFeedBack.php", form);
        //yield return www;
    }

    private IEnumerator SubmitScoreToDBAndLeave()
    {
        LoadingOverlay.AddLoader();
        WWWForm form = new WWWForm();

        form.AddField("username", _persistentLoginDataManager.Username);
        form.AddField("password", _persistentLoginDataManager.Password);
        form.AddField("subject", _persistentLoginDataManager.SubjectData.SubjectID);
        form.AddField("parameter", ActiveParameterName);
        form.AddField("score", Mathf.FloorToInt(slider.value).ToString());
        form.AddField("groupid", _persistentLoginDataManager.GroupData.GroupID);

        string output = "";
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(PersistentData.WebAdress + "PP_SubmitFeedBack.php", form))
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

        //WWW www = new WWW(PersistentData.WebAdress + "PP_SubmitFeedBack.php", form);
        //yield return www;
        LoadingOverlay.RemoveLoader();
        if (_persistentLoginDataManager.SubjectData.FeedbackType == SpiderGraph.FeedbackType.IPF_RD) loadIPFScene.Load();
        else loadGPFScene.Load();
    }
}
