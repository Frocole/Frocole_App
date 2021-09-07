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
/// A compiled feedback on subject.
/// </summary>
[System.Serializable]
public struct CompiledFeedbackOnSubject
{
    #region Fields

    /// <summary>
    /// Options for controlling the operation.
    /// </summary>
    public string[] parameters;

    /// <summary>
    /// The sources.
    /// </summary>
    public List<CompiledFeedbackOnSubjectFromSource> Sources;

    /// <summary>
    /// The subject.
    /// </summary>
    public string Subject;

    #endregion Fields
}

/// <summary>
/// A compiled feedback on subject from source.
/// </summary>
[System.Serializable]
public struct CompiledFeedbackOnSubjectFromSource
{
    #region Fields

    /// <summary>
    /// The parameter values.
    /// </summary>
    public int[] parameterValues;

    /// <summary>
    /// Source for the.
    /// </summary>
    public string Source;

    #endregion Fields
}

/// <summary>
/// A fill profile.
/// </summary>
public class FillProfile : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// The compiled feedback.
    /// </summary>
    public CompiledFeedbackOnSubject compiledFeedback;

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

        foreach (CompiledFeedbackOnSubjectFromSource source in compiledFeedback.Sources)
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

        return Averages;
    }

    /// <summary>
    /// IPF.
    /// </summary>
    public void CompileFeedback()
    {
        compiledFeedback = new CompiledFeedbackOnSubject();
        compiledFeedback.Subject = UserDataManagerReference.SubjectData.SubjectID;

        switch (UserDataManagerReference.SubjectData.FeedbackType)
        {
            case SpiderGraph.FeedbackType.IPF_RD:
                compiledFeedback.parameters = UserDataManagerReference.CourseData.IPF_RD_parameters.Split('/');
                break;

            case SpiderGraph.FeedbackType.GPF_RD:
                compiledFeedback.parameters = UserDataManagerReference.CourseData.GPF_RD_parameters.Split('/');
                break;
        }

        compiledFeedback.Sources = new List<CompiledFeedbackOnSubjectFromSource>();
        foreach (var contributor in UserDataManagerReference.SubjectData.Contributors)
        {
            CompiledFeedbackOnSubjectFromSource NewSource = new CompiledFeedbackOnSubjectFromSource();
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
                case FeedbackSource.GroupAverage:
                    profile.Scores = AverageScores();
                    break;

                case FeedbackSource.MyFeedback:
                    profile.Scores = compiledFeedback.Sources.Find(x => x.Source == UserDataManagerReference.UserData.UserID).parameterValues;
                    break;

                case FeedbackSource.TeacherFeedback:
                    profile.Scores = compiledFeedback.Sources.Find(x => x.Source == UserDataManagerReference.CourseData.LeraarUserID).parameterValues;
                    break;
                case FeedbackSource.SelfReflection:
                    if (LeraarOverride || (UserDataManagerReference.SubjectData.Public && UserDataManagerReference.SubjectData.FeedbackType == SpiderGraph.FeedbackType.IPF_RD && UserDataManagerReference.GroupData.Public == "1"))
                        profile.Scores = compiledFeedback.Sources.Find(x => x.Source == UserDataManagerReference.SubjectData.SubjectID).parameterValues;
                    break;
            }
            profile.gameObject.SetActive(false);
            profile.gameObject.SetActive(true);
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
    public void Start()
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

        string output = "";
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(PersistentData.WebAdress + "PP_GetFeedbackOnSubject.php", form))
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

    #endregion Methods
}