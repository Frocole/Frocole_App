using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Globalization;
using UnityEngine.Networking;

public class FillProfile : MonoBehaviour
{
    public RootFeedBackObject MyFeedback;
    public UserDataManager UserDataManagerReference;
    public FeedbackSource DisplayFeedbackFrom;
    //public SpiderGraph.FeedbackType FeedbackType;

    public SpiderGraph_Profile[] MyProfiles;
    public CompiledFeedbackOnSubject compiledFeedback;
    public bool LeraarOverride = false;

    public enum FeedbackSource
    {
        GroupAverage,
        MyFeedback,
        TeacherFeedback,
        SelfReflection
    }

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

    public void setCompiledFeedbackScore(string Source, int parameterCount, int score)
    {
        compiledFeedback.Sources.Find(x => x.Source == Source).parameterValues[parameterCount] = score;
        FillProfiles();
    }

    public void Start()
    {
        UserDataManagerReference = PersistentData.Instance.LoginDataManager;
        StartCoroutine(DownloadFeedbackItems());
        // CompileFeedback();
    }



    public void CompileFeedback() // IPF
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
}


[System.Serializable]
public struct CompiledFeedbackOnSubject
{
    public string Subject;
    public List<CompiledFeedbackOnSubjectFromSource> Sources;
    public string[] parameters;
}

[System.Serializable]
public struct CompiledFeedbackOnSubjectFromSource
{
    public string Source;
    public int[] parameterValues;
}

