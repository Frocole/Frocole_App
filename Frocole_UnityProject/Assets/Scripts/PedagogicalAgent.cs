using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class PedagogicalAgent : MonoBehaviour
{
    public static PedagogicalAgent Instance;
    public PAPopup PaDeadlinePopup;
    public PAPopup PaGuidelinePopup;
    public GameObject FadeEffect;
    public List<PAPopup> ActivePopups = new List<PAPopup>();

    private const string LASTDEADLINEREVIEWSKEY = "frocole_LastDeadlineReviewDates";
    private const string LASTGUIDELINEREVIEWSKEY = "frocole_LastGuidelineReviewDates";

    private static Dictionary<string, List<PAGuidelineObject>> GuidelinesPerCourse = new Dictionary<string, List<PAGuidelineObject>>();

    private void Awake()
    {
        if (Instance != null) Debug.LogError("Duplicate PA!");
        else Instance = this;
    }

    [ContextMenu("reset playerprefs")]
    private void resetplayerprefs()
    {
        PlayerPrefs.DeleteKey(LASTDEADLINEREVIEWSKEY);
        PlayerPrefs.DeleteKey(LASTGUIDELINEREVIEWSKEY);
    }

    public void StartPAReview()
    {
        ActivePopups.Clear();

        StartCoroutine(GetDeadlines());
        StartCoroutine(PAReviewGuidelines());
    }

    private IEnumerator GetDeadlines()
    {
        LoadingOverlay.AddLoader();
        // attempt to login
        WWWForm form = new WWWForm();

        form.AddField("username", PersistentData.Instance.LoginDataManager.Username);
        form.AddField("password", PersistentData.Instance.LoginDataManager.Password);

        string output = "";
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(UriMaker.InsertScriptInUri(PersistentData.WebAdress, "PP_GetAllMyDeadlines.php"), form))
        {

            yield return WWW_.SendWebRequest();

            if (WWW_.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(WWW_.error);
            }
            else
            {

                output = WWW_.downloadHandler.text;

                foreach (var item in SortDeadlines(output))
                {
                    
                    var go = GameObject.Instantiate<PAPopup>(PaDeadlinePopup, this.transform);
                    go._subjectText.text = item.CourseName;
                    go._contentText.text = $"Er is een nieuwe Deadline op {item.Deadline}. Geef je team je feedback!";
                    go.PA = this;
                    ActivePopups.Add(go);
                }
            }            
        }

        LoadingOverlay.RemoveLoader();

        yield return null;
    }

    private List<CourseObject> SortDeadlines(string output)
    {
        RootCourseObject deadlines = JsonUtility.FromJson<RootCourseObject>("{\"courses\": " + output + "}");
        List<CourseObject> newDeadlines = new List<CourseObject>();


        if (deadlines.courses.Length != 0)
        {
            var CachedDeadlineDates = new List<string>();
            if (PlayerPrefs.HasKey(LASTDEADLINEREVIEWSKEY))
            {
                CachedDeadlineDates = PlayerPrefs.GetString(LASTDEADLINEREVIEWSKEY).Split(',').ToList();
            }
            string UpdatedDeadlineDates = "";

            foreach (var CourseWithDeadline in deadlines.courses)
            {
                UpdatedDeadlineDates += $"{CourseWithDeadline.CourseID}_{CourseWithDeadline.Deadline},";

                if (!CachedDeadlineDates.Contains($"{CourseWithDeadline.CourseID}_{CourseWithDeadline.Deadline}"))
                {
                    newDeadlines.Add(CourseWithDeadline);
                }
            }

            PlayerPrefs.SetString(LASTDEADLINEREVIEWSKEY, UpdatedDeadlineDates);
        }

        return newDeadlines;
    }

    List<CourseObject> AllPassedDeadlines = new List<CourseObject>();
    List<CourseObject> NewPassedDeadlines = new List<CourseObject>();

    KeyValuePair<string, PAGuidelineObject[]> RelevantGuidelinesHolder = new KeyValuePair<string, PAGuidelineObject[]>();
    Dictionary<string, PAGuidelineObject[]> RelevantPAGuidelinesPerCourse = new Dictionary<string, PAGuidelineObject[]>();

    private IEnumerator PAReviewGuidelines()
    {
        // First get all courses with a passed deadline 
        yield return StartCoroutine(RetrieveAllPassedDeadlines());

        if (AllPassedDeadlines.Count > 0)
        {
            //Debug.Log("___________________________________");
            //Debug.Log("<b>RetrieveAllPassedDeadlines: </b>");
            for (int i = 0; i < AllPassedDeadlines.Count; i++)
            {
                //Debug.Log($"[{i}]: <i> CourseID [{AllPassedDeadlines[i].CourseID}], CourseName [{AllPassedDeadlines[i].CourseName}], Deadline [{AllPassedDeadlines[i].Deadline}]</i>");
            }
        }

        // Then compare each deadline with the deadline stored in the playerprefs, and ignore the ones that are already there in the following steps
        NewPassedDeadlines = FilterCachedDeadlines(AllPassedDeadlines);


        if (NewPassedDeadlines.Count > 0)
        {
            Debug.Log("___________________________________");
            Debug.Log($"<b>NewPassedDeadlines[{NewPassedDeadlines.Count}]: </b>");
            for (int i = 0; i < NewPassedDeadlines.Count; i++)
            {
                Debug.Log($"[{i}]: <i> CourseID [{NewPassedDeadlines[i].CourseID}], CourseName [{NewPassedDeadlines[i].CourseName}], Deadline [{NewPassedDeadlines[i].Deadline}]</i>");
            }
        }


        // reset the RelevantPAGuidelinesPerCourse dictionaryzeventien
        RelevantPAGuidelinesPerCourse.Clear();

        // For each deadline that remains do:
        foreach (CourseObject course in NewPassedDeadlines)
        {
            //Debug.Log($"Calculating {course.CourseName} parameter feedback Delta's:");

            // get all the PAGuidelines and Get al the Feedback.
            // The app then calculates which guidelines apply, and creates a PAGuideline popup for each.

            yield return StartCoroutine(ApplicableGuidelinesFromCourse(course));
            //Debug.Log($"item: RelevantGuidelinesHolder<{RelevantGuidelinesHolder.Key},{RelevantGuidelinesHolder.Value.Length}>");
            RelevantPAGuidelinesPerCourse.Add(RelevantGuidelinesHolder.Key, RelevantGuidelinesHolder.Value);
            //         RelevantPAGuidelinesPerCourse.Add(RelevantGuidelinesHolder.Key, RelevantGuidelinesHolder.Value);

        }

        foreach (var GuidelineSet in RelevantPAGuidelinesPerCourse)
        {
            foreach (var item in GuidelineSet.Value)
            {
                var go = GameObject.Instantiate<PAPopup>(PaGuidelinePopup, this.transform);
                go._subjectText.text = PersistentData.Instance.LoginDataManager.SubscriptionNameFromID(GuidelineSet.Key);
                go._contentText.text = item.Advice;
                go.PA = this;
                ActivePopups.Add(go);
            }
        }

        // After all courses have been processed, covert the list of "all courses with a passed deadline" to a JSONstring and store it in the playerprefs
        CacheRetrievedGuidelines();

        yield return new WaitForEndOfFrame();
    }

    // First get all courses with a passed deadline 
    private IEnumerator RetrieveAllPassedDeadlines()
    {
        WWWForm form = new WWWForm();

        form.AddField("username", PersistentData.Instance.LoginDataManager.Username);
        form.AddField("password", PersistentData.Instance.LoginDataManager.Password);

        string output = "";

        using (UnityWebRequest WWW_ = UnityWebRequest.Post(UriMaker.InsertScriptInUri(PersistentData.WebAdress, "PP_GetAllMyPastDeadlines.php"), form))
        {

            yield return WWW_.SendWebRequest();

            if (WWW_.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(WWW_.error);
            }
            else
            {
                output = WWW_.downloadHandler.text;
                RootCourseObject deadlines = JsonUtility.FromJson<RootCourseObject>("{\"courses\": " + output + "}");
                AllPassedDeadlines = deadlines.courses.ToList();
            }

        }

    }

    // Then compare each deadline with the deadline stored in the playerprefs, and ignore the ones that are already there in the following steps
    private List<CourseObject> FilterCachedDeadlines(List<CourseObject> courseObjects)
    {
        //Debug.Log("PlayerPrefs contained:");

        if (!PlayerPrefs.HasKey(LASTGUIDELINEREVIEWSKEY) || courseObjects.Count == 0)
        {
            //Debug.Log($"no key at LASTGUIDELINEREVIEWSKEY: {LASTGUIDELINEREVIEWSKEY}");

            return courseObjects;
        }

       // Debug.Log($"[{PlayerPrefs.GetString(LASTGUIDELINEREVIEWSKEY)}] at LASTGUIDELINEREVIEWSKEY: {LASTGUIDELINEREVIEWSKEY}");

        var CachedDeadlineDates = PlayerPrefs.GetString(LASTGUIDELINEREVIEWSKEY).Split(',').ToList();

        var UncachedDeadlines = new List<CourseObject>();

        foreach (var course in courseObjects) if (!CachedDeadlineDates.Contains($"{course.CourseID}_{course.Deadline}")) UncachedDeadlines.Add(course);

        return UncachedDeadlines;
    }

    // For each deadline that remains do:
    // get all the PAGuidelines and Get al the Feedback.
    // The app then calculates which guidelines apply, and creates a PAGuideline popup for each.
    private IEnumerator ApplicableGuidelinesFromCourse(CourseObject course)
    {

        WWWForm form = new WWWForm();

        form.AddField("username", PersistentData.Instance.LoginDataManager.Username);
        form.AddField("password", PersistentData.Instance.LoginDataManager.Password);
        form.AddField("courseid", course.CourseID);


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
                Debug.Log(Guidelineset.paguidelines.Length);
            }
        }
        if (Guidelineset != null)
        {
            RootFeedBackObject feedBackObjects = null;
            using (UnityWebRequest WWW_ = UnityWebRequest.Post(UriMaker.InsertScriptInUri(PersistentData.WebAdress, "PP_GetPADataCourse.php"), form))
            {
                yield return WWW_.SendWebRequest();

                if (WWW_.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(WWW_.error);
                }
                else
                {
                    Debug.Log(WWW_.downloadHandler.text);
                    feedBackObjects = JsonUtility.FromJson<RootFeedBackObject>("{\"Feedback\": " + WWW_.downloadHandler.text + "}");
                    Debug.Log(feedBackObjects.Feedback.Length);
                }
            }

            List<PAGuidelineObject> ReviewedGuidelineset = new List<PAGuidelineObject>();

            int i;
            string user = PersistentData.Instance.LoginDataManager.UserData.UserID; ;
            float peerscore;
            float peeravg;
            float selfscore;
            float PADelta;
            float delta;

            if (feedBackObjects.Feedback != null)
            {
                foreach (var paguidelinesitem in Guidelineset.paguidelines)
                {
                    selfscore = 0;
                    peerscore = 0;
                    peeravg = 0;
                    delta = 0;
                    PADelta = 0;
                    i = 0;

                    if (float.TryParse(paguidelinesitem.Delta, out PADelta))
                    {
                        if (paguidelinesitem.SubjectType == "ipf-rd")
                        {
                            // review each feedbackitem:
                            foreach (var feedBackitem in feedBackObjects.Feedback)
                            {
                                if (paguidelinesitem.Parameter == feedBackitem.Parameter && user == feedBackitem.Subject)
                                {
                                    if (feedBackitem.FeedbackSuplierID == user)
                                    {
                                        float.TryParse(feedBackitem.Score, out selfscore);
                                    }
                                    else
                                    {
                                        if (float.TryParse(feedBackitem.Score, out peerscore))
                                        {
                                            peeravg += peerscore;
                                            i++;
                                        }
                                    }
                                }
                            }
                            if (i != 0)
                            {
                                peeravg = peeravg / i;
                                delta = selfscore - peeravg;

                                if (PADelta < 0)
                                {
                                    if (delta < PADelta) ReviewedGuidelineset.Add(paguidelinesitem);
                                }
                                else
                                {
                                    if (delta > PADelta) ReviewedGuidelineset.Add(paguidelinesitem);
                                }
                            }



                        }
                        else
                        {
                            // review each feedbackitem:
                            foreach (var feedBackitem in feedBackObjects.Feedback)
                            {
                                if (paguidelinesitem.Parameter == feedBackitem.Parameter && user != feedBackitem.Subject)
                                {
                                    if (feedBackitem.FeedbackSuplierID == user)
                                    {
                                        float.TryParse(feedBackitem.Score, out selfscore);
                                    }
                                    else
                                    {
                                        if (float.TryParse(feedBackitem.Score, out peerscore))
                                        {
                                            peeravg += peerscore;
                                            i++;
                                        }
                                    }
                                }
                            }
                            if (i != 0)
                            {
                                peeravg = peeravg / i;
                                delta = selfscore - peeravg;

                                if (PADelta < 0)
                                {
                                    if (delta < PADelta) ReviewedGuidelineset.Add(paguidelinesitem);
                                }
                                else
                                {
                                    if (delta > PADelta) ReviewedGuidelineset.Add(paguidelinesitem);
                                }
                            }
                        }
                    }
                }
            }


            RelevantGuidelinesHolder = new KeyValuePair<string, PAGuidelineObject[]>(course.CourseID, ReviewedGuidelineset.ToArray());

        }
        else
        {

            Debug.Log("no guidelineset found!");

            PAGuidelineObject[] ReviewedGuidelineset = new PAGuidelineObject[0];
            RelevantGuidelinesHolder = new KeyValuePair<string, PAGuidelineObject[]>(course.CourseID, ReviewedGuidelineset);

        }
    }

    // After all courses have been processed, covert the list of "all courses with a passed deadline" to a JSONstring and store it in the playerprefs
    private void CacheRetrievedGuidelines()
    {
        string WriteToPlayerPref = "";
        foreach (var course in AllPassedDeadlines)
        {
            WriteToPlayerPref += $"{course.CourseID}_{course.Deadline},";
        }

        //Debug.Log(WriteToPlayerPref);
        PlayerPrefs.SetString(LASTGUIDELINEREVIEWSKEY, WriteToPlayerPref);
    }

    
}

