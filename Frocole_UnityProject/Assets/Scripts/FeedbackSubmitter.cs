#region Header

/*
    Feedback and Reflection in Online Collaborative Learning.

    Copyright (C) 2021  Open University of the Netherlands

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

#endregion Header

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// A feedback submitter.
/// </summary>
public class FeedbackSubmitter : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// The active parameter.
    /// </summary>
    public int ActiveParameter;

    /// <summary>
    /// The active parameter name.
    /// </summary>
    public string ActiveParameterName;

    /// <summary>
    /// The feedback data holder.
    /// </summary>
    public FillProfile FeedbackDataHolder;

    /// <summary>
    /// The highlight color.
    /// </summary>
    public Color HighlightColor = new Color(0.9176471f, 0.07058824f, 0.5411765f);

    /// <summary>
    /// The load gpf scene.
    /// </summary>
    public LoadScene loadGPFScene;

    /// <summary>
    /// The load ipf scene.
    /// </summary>
    public LoadScene loadIPFScene;

    /// <summary>
    /// Name of the parameter.
    /// </summary>
    public Text ParameterName;

    /// <summary>
    /// The profile user review.
    /// </summary>
    public SpiderGraph_Profile Profile_UserReview;

    /// <summary>
    /// The slider.
    /// </summary>
    public Slider slider;

    /// <summary>
    /// The highlighted axis.
    /// </summary>
    private Image HighlightedAxis = null;

    /// <summary>
    /// Manager for persistent login data.
    /// </summary>
    private UserDataManager _persistentLoginDataManager;

    #endregion Fields

    #region Methods

    /// <summary>
    /// Highlight axis.
    /// </summary>
    ///
    /// <param name="newHighlightedImage"> The new highlighted image. </param>
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

    /// <summary>
    /// Opens feed back submitter.
    /// </summary>
    ///
    /// <param name="Parameter"> The parameter. </param>
    public void OpenFeedBackSubmitter(string Parameter)
    {
        ActiveParameterName = Parameter;
        ParameterName.text = Parameter;
        slider.gameObject.SetActive(true);
    }

    /// <summary>
    /// Sets a parameter.
    /// </summary>
    ///
    /// <param name="score"> The score. </param>
    public void SetParameter(Single score)
    {
        ParameterName.text = ActiveParameterName + " : " + score.ToString() + "%";
        FeedbackDataHolder.setCompiledFeedbackScore(_persistentLoginDataManager.UserData.UserID, ActiveParameter, (int)score);
    }

    /// <summary>
    /// Submitscores this object.
    /// </summary>
    public void Submitscore()
    {
        StartCoroutine(SubmitScoreToDB());
    }

    /// <summary>
    /// Submitscore and leave.
    /// </summary>
    public void SubmitscoreAndLeave()
    {
        StartCoroutine(SubmitScoreToDBAndLeave());
    }

    /// <summary>
    /// Start is called just before any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        _persistentLoginDataManager = PersistentData.Instance.LoginDataManager;
    }

    /// <summary>
    /// Submit score to database.
    /// </summary>
    ///
    /// <returns>
    /// An IEnumerator.
    /// </returns>
    private IEnumerator SubmitScoreToDB()
    {
        if (!string.IsNullOrEmpty(ActiveParameterName))
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
            using (UnityWebRequest WWW_ = UnityWebRequest.Post(UriMaker.InsertScriptInUri(PersistentData.WebAdress, "PP_SubmitFeedBack.php"), form))
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
        }
        //WWW www = new WWW(PersistentData.WebAdress + "PP_SubmitFeedBack.php", form);
        //yield return www;
    }

    /// <summary>
    /// Submit score to database and leave.
    /// </summary>
    ///
    /// <returns>
    /// An IEnumerator.
    /// </returns>
    private IEnumerator SubmitScoreToDBAndLeave()
    {
        if (!string.IsNullOrEmpty(ActiveParameterName))
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
            using (UnityWebRequest WWW_ = UnityWebRequest.Post(UriMaker.InsertScriptInUri(PersistentData.WebAdress, "PP_SubmitFeedBack.php"), form))
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
        }
        if (_persistentLoginDataManager.SubjectData.FeedbackType == SpiderGraph.FeedbackType.IPF_RD) loadIPFScene.Load();
        else loadGPFScene.Load();
    }

    #endregion Methods
}