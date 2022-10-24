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

using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;



/// <summary>
/// A fill profile.
/// </summary>
public class FillProfileDate : MonoBehaviour
{
    public string TestDatePicker = "2022-10-20";


    #region Fields

    public SpiderGraph spiderGraph;

    public SpiderGraph.FeedbackType type = SpiderGraph.FeedbackType.IPF_RD;

    /// <summary>
    /// The compiled feedback.
    /// </summary>
    public FillProfile.CompiledFeedbackOnSubject compiledFeedback;

    /// <summary>
    /// The display feedback from.
    /// </summary>
    public FeedbackSource DisplayFeedbackFrom;

    /// <summary>
    /// True to leraar override.
    /// </summary>
    public bool LeraarOverride = false;

    /// <summary>
    /// My feedback.
    /// </summary>
    public RootFeedBackObject MyFeedback;

    //public SpiderGraph.FeedbackType FeedbackType;

    /// <summary>
    /// My profiles.
    /// </summary>
    public SpiderGraph_Profile[] MyProfiles;

    /// <summary>
    /// The user data manager reference.
    /// </summary>
    public UserDataManager UserDataManagerReference;

    #endregion Fields

    #region Enumerations

    /// <summary>
    /// Values that represent feedback sources.
    /// </summary>
    public enum FeedbackSource
    {
        /// <summary>
        /// An enum constant representing the group average option.
        /// </summary>
        GroupAverage,
        /// <summary>
        /// An enum constant representing my feedback option.
        /// </summary>
        MyFeedback,
        /// <summary>
        /// An enum constant representing the teacher feedback option.
        /// </summary>
        TeacherFeedback,
        /// <summary>
        /// An enum constant representing the self reflection option.
        /// </summary>
        SelfReflection
    }

    #endregion Enumerations

    #region Methods

    /// <summary>
    /// Average scores.
    /// </summary>
    ///
    /// <returns>
    /// An int[].
    /// </returns>
    public int[] AverageScores()
    {
        int[] Averages = new int[compiledFeedback.parameters.Length];
        int[] weight = new int[compiledFeedback.parameters.Length];

        foreach (FillProfile.CompiledFeedbackOnSubjectFromSource source in compiledFeedback.Sources)
        {
            if (!(source.Source == UserDataManagerReference.UserData.UserID && type == SpiderGraph.FeedbackType.GPF_RD) && source.Source != UserDataManagerReference.CourseData.LeraarUserID)
            {
                for (int i = 0; i < compiledFeedback.parameters.Length; i++)
                {
                    if (source.parameterValues[i] >= 0 && source.Source != UserDataManagerReference.SubjectData.SubjectID)
                    {
                        Averages[i] = (Averages[i] * weight[i] + source.parameterValues[i]) / (weight[i] + 1);
                        weight[i]++;
                    }
                }
            }
        }

        return Averages;
    }

    /// <summary>
    /// IPF.
    /// </summary>
    public void CompileFeedback()
    {
        compiledFeedback = new FillProfile.CompiledFeedbackOnSubject();
        compiledFeedback.Subject = UserDataManagerReference.SubjectData.SubjectID;

        switch (UserDataManagerReference.SubjectData.FeedbackType)
        {
            case SpiderGraph.FeedbackType.IPF_RD:
                compiledFeedback.parameters = UserDataManagerReference.CourseData.IPF_RD_parameters.Split('/');
                break;

            case SpiderGraph.FeedbackType.GPF_RD:
                compiledFeedback.parameters = UserDataManagerReference.CourseData.GPF_RD_parameters.Split('/');
                break;

            case SpiderGraph.FeedbackType.PF_RD:
                compiledFeedback.parameters = UserDataManagerReference.CourseData.PF_RD_parameters.Split('/');
                break;
        }

        compiledFeedback.Sources = new List<FillProfile.CompiledFeedbackOnSubjectFromSource>();
        foreach (var contributor in UserDataManagerReference.SubjectData.Contributors)
        {
            FillProfile.CompiledFeedbackOnSubjectFromSource NewSource = new FillProfile.CompiledFeedbackOnSubjectFromSource();


            NewSource.Source = contributor;

            
            NewSource.parameterValues = new int[compiledFeedback.parameters.Length];

            for (int i = 0; i < compiledFeedback.parameters.Length; i++)
            {
                NewSource.parameterValues[i] = -1; // the unranked value
            }

            compiledFeedback.Sources.Add(NewSource);
        }

        //at this point we need to have gotten the Feedback from the server
        List<FeedBackObject> FeedBackForProcessing = new List<FeedBackObject>(MyFeedback.Feedback);
        int Param_i = 0; // 3 to 10 itterations
        List<FeedBackObject> ProcessedFeedBack = new List<FeedBackObject>();
        string debugstring;

        foreach (string Parameter in compiledFeedback.parameters)
        {
            debugstring = $"On {Parameter} the following pieces of feedback were found: ";

            foreach (FeedBackObject FeedBackitem in FeedBackForProcessing)
            {
                if (FeedBackitem.Parameter == Parameter && compiledFeedback.Sources.Exists(x => x.Source == FeedBackitem.FeedbackSuplierID))
                {
                    debugstring += $"[{FeedBackitem.FeedbackSuplierID} gives : {FeedBackitem.Score}]";

                    compiledFeedback.Sources.Find(x => x.Source == FeedBackitem.FeedbackSuplierID).parameterValues[Param_i] = int.Parse(FeedBackitem.Score, CultureInfo.InvariantCulture.NumberFormat);
                    ProcessedFeedBack.Add(FeedBackitem);
                }
            }

            debugstring += " thats all.";

            FeedBackForProcessing = FeedBackForProcessing.Except(ProcessedFeedBack).ToList();
            ProcessedFeedBack.Clear();
            Param_i++;
        }

        FillProfiles();
    }

    /// <summary>
    /// Fill profiles.
    /// </summary>
    public void FillProfiles()
    {
        foreach (var profile in MyProfiles)
        {
            switch (profile.feedbackSource)
            {
                case FillProfile.FeedbackSource.GroupAverage:
                    profile.Scores = AverageScores();
                    break;

                case FillProfile.FeedbackSource.MyFeedback:
                    profile.Scores = compiledFeedback.Sources.Find(x => x.Source == UserDataManagerReference.UserData.UserID).parameterValues;
                    break;

                case FillProfile.FeedbackSource.TeacherFeedback:
                    profile.Scores = compiledFeedback.Sources.Find(x => x.Source == UserDataManagerReference.CourseData.LeraarUserID).parameterValues;
                    break;
                case FillProfile.FeedbackSource.SelfReflection:
                    if (LeraarOverride || (UserDataManagerReference.SubjectData.Public && UserDataManagerReference.SubjectData.FeedbackType == SpiderGraph.FeedbackType.IPF_RD && UserDataManagerReference.GroupData.Public == "1"))
                        profile.Scores = compiledFeedback.Sources.Find(x => x.Source == UserDataManagerReference.SubjectData.SubjectID).parameterValues;
                    break;
            }
            profile.gameObject.SetActive(false);
            profile.gameObject.SetActive(true);
        }

        if (type == SpiderGraph.FeedbackType.GPF_RD || (type == SpiderGraph.FeedbackType.IPF_RD && UserDataManagerReference.SubjectData.SubjectID == UserDataManagerReference.UserData.UserID))
        {
            StartCoroutine(CompareGuidelines());
        }
    }

    /// <summary>
    /// Sets compiled feedback score.
    /// </summary>
    ///
    /// <param name="Source">         Source for the. </param>
    /// <param name="parameterCount"> Number of parameters. </param>
    /// <param name="score">          The score. </param>
    public void setCompiledFeedbackScore(string Source, int parameterCount, int score)
    {
        compiledFeedback.Sources.Find(x => x.Source == Source).parameterValues[parameterCount] = score;
        FillProfiles();
    }

    /// <summary>
    /// Start is called just before any of the Update methods is called the first time.
    /// </summary>
    public void OnEnable()
    {
        UserDataManagerReference = PersistentData.Instance.LoginDataManager;
        StartCoroutine(DownloadFeedbackItems());
        // CompileFeedback();
    }

    /// <summary>
    /// Downloads the feedback items.
    /// </summary>
    ///
    /// <returns>
    /// An IEnumerator.
    /// </returns>
    IEnumerator DownloadFeedbackItems()
    {
        LoadingOverlay.AddLoader();
        WWWForm form = new WWWForm();

        form.AddField("username", UserDataManagerReference.Username);
        form.AddField("password", UserDataManagerReference.Password);
        form.AddField("courseid", UserDataManagerReference.CourseData.CourseID);
        form.AddField("subject", UserDataManagerReference.SubjectData.SubjectID);
        form.AddField("date", TestDatePicker);


        string output = "";
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(UriMaker.InsertScriptInUri(PersistentData.WebAdress, "PP_GetFeedbackOnSubjectBeforeDate.php"), form))
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
                if (output != "") MyFeedback = JsonUtility.FromJson<RootFeedBackObject>("{\"Feedback\": " + output + "}");
                CompileFeedback();
            }
        }

        Debug.Log($"select t1.* from FeedBackItems t1 inner join ( select max(FeedBackItemID) FeedBackItemID, Parameter from FeedBackItems group by Parameter, FeedbackSuplierID) t2 on t1.Parameter = t2.Parameter and t1.FeedBackItemID = t2.FeedBackItemID WHERE `GroupID` = (SELECT GroupID FROM UserAndCourseRelations WHERE UserID = (SELECT Users.UserID FROM Users WHERE Users.Username = '{UserDataManagerReference.Username}' AND Users.Password = '{UserDataManagerReference.Password}') AND CourseID = '{UserDataManagerReference.CourseData.CourseID}') AND `Subject` = '{UserDataManagerReference.SubjectData.SubjectID}'");

        yield return new WaitForEndOfFrame();
        LoadingOverlay.RemoveLoader();
    }

    private IEnumerator CompareGuidelines()
    {
        WWWForm form = new WWWForm();

        form.AddField("username", UserDataManagerReference.Username);
        form.AddField("password", UserDataManagerReference.Password);
        form.AddField("courseid", UserDataManagerReference.CourseData.CourseID);


        // Get all Guidelines for this course.
        RootPAGuidelineObject Guidelineset = null;
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(UriMaker.InsertScriptInUri(PersistentData.WebAdress, "PP_GetAllMyPAGuidelines.php"), form))
        {
            yield return WWW_.SendWebRequest();

            if (WWW_.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(WWW_.error);
            }
            else
            {
                Debug.Log(WWW_.downloadHandler.text);
                Guidelineset = JsonUtility.FromJson<RootPAGuidelineObject>("{\"paguidelines\": " + WWW_.downloadHandler.text + "}");
            }
        }

        var a = AverageScores();
        var b = compiledFeedback.Sources.Find(X => X.Source == UserDataManagerReference.UserData.UserID).parameterValues;

        if (Guidelineset.paguidelines != null)
        {
            if (type == SpiderGraph.FeedbackType.IPF_RD) for (int i = 0; i < compiledFeedback.parameters.Length; i++)
            {
                foreach (var GuidelineOfParameter in Guidelineset.paguidelines.ToList().FindAll(X => X.Parameter == compiledFeedback.parameters[i] && X.SubjectType == "ipf-rd"))
                {
                    float PADelta = 0;
                    float.TryParse(GuidelineOfParameter.Delta, out PADelta);

                    if ((PADelta < 0 && PADelta > b[i] - a[i]) || (PADelta > 0 && PADelta < b[i] - a[i])) MarkParameter(i);
                }
            }

            if (type == SpiderGraph.FeedbackType.GPF_RD) for (int i = 0; i < compiledFeedback.parameters.Length; i++)
                {
                    foreach (var GuidelineOfParameter in Guidelineset.paguidelines.ToList().FindAll(X => X.Parameter == compiledFeedback.parameters[i] && X.SubjectType == "gpf-rd"))
                    {
                        float PADelta = 0;
                        float.TryParse(GuidelineOfParameter.Delta, out PADelta);

                        if ((PADelta < 0 && PADelta > b[i] - a[i]) || (PADelta > 0 && PADelta < b[i] - a[i])) MarkParameter(i);
                    }
                }

        }

        yield return null;
    }

    private void MarkParameter(int parameteri)
    {
        Debug.Log($"Marked parameter {parameteri}: {compiledFeedback.parameters[parameteri]}");

        spiderGraph.spiderGraphAxes[parameteri].MarkedByPA.gameObject.SetActive(true);
    }


    #endregion Methods
}