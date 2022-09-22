using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GuidelineInterFace : MonoBehaviour
{
    public Dropdown DdType, DdIPFParameter, DdGPFParameter;
    public InputField IfDelta, IfAdvice;
    public Text label;
    public Button Create, Update, delete;

    private Dropdown activeparameterselection;
    private string SelectedAdvice;

    public LoadScene OnEditComplete;

    // Start is called before the first frame update
    void Start()
    {
        DdIPFParameter.ClearOptions();
        foreach (string IPFparameter in PersistentData.Instance.LoginDataManager.CourseData.IPF_RD_parameters.Split('/'))
        {
            DdIPFParameter.options.Add(new Dropdown.OptionData(IPFparameter));
        }

        DdGPFParameter.ClearOptions();
        foreach (string GPFparameter in PersistentData.Instance.LoginDataManager.CourseData.GPF_RD_parameters.Split('/'))
        {
            DdGPFParameter.options.Add(new Dropdown.OptionData(GPFparameter));
        }

        DdType.onValueChanged.AddListener((int a) => SelectType(a));
        activeparameterselection = DdIPFParameter;
        DdIPFParameter.value = 1;
        DdGPFParameter.value = 1;

        if (PersistentData.Instance.LoginDataManager.GuidelineData != null)
        {
            SelectedAdvice = PersistentData.Instance.LoginDataManager.GuidelineData.PAGuidelineID;
            DdType.value = (PersistentData.Instance.LoginDataManager.GuidelineData.SubjectType == "ipf-rd") ? 0 : 1;
            activeparameterselection.value = activeparameterselection.options.FindIndex(X => X.text == PersistentData.Instance.LoginDataManager.GuidelineData.Parameter);
            IfDelta.text = PersistentData.Instance.LoginDataManager.GuidelineData.Delta;
            IfAdvice.text = PersistentData.Instance.LoginDataManager.GuidelineData.Advice;


            Update.gameObject.SetActive(true);
            delete.gameObject.SetActive(true);
        }
        else
        {
            Create.gameObject.SetActive(true);
        }

        triggerDescription();
    }

    public void SelectType(int i)
    {
        DdIPFParameter.transform.parent.gameObject.SetActive(i == 0);
        DdGPFParameter.transform.parent.gameObject.SetActive(i == 1);

        switch (i)
        {
            case 0:
                activeparameterselection = DdIPFParameter;
                break;

            case 1:
                activeparameterselection = DdGPFParameter;
                break;

            default:
                break;
        }
    }

    public void triggerDescription()
    {
        bool Valid = true;

        var table = LocalizationSettings.StringDatabase.GetTable("UI Text");

        var a = ((DdType.value == 0) ?
            table.GetEntry("L09_zichzelf").GetLocalizedString() :
            table.GetEntry("L09_de groep").GetLocalizedString());
        int b = 0;
        Valid &= int.TryParse(IfDelta.text, out b);
        var c = ((b >= 0) ?
            table.GetEntry("L09_hoger").GetLocalizedString() :
            table.GetEntry("L09_lager").GetLocalizedString());
        var d = activeparameterselection.options[activeparameterselection.value].text;

        //! Obsolete (not used in translation)
        var e = ((Mathf.Abs(b) == 1) ? "punt" : "punten");

        //! Original
        //string description = $"\"De student heeft {d} van {a} {Mathf.Abs(b)} {e} {c} ingeschat dan de anderen in zijn of haar groep.\"";
        // 
        //! Recoded to String.Format 
        //string.Format($"\"De student heeft {0} van {1} {2} {3} {4} ingeschat dan de anderen in zijn of haar groep.\"", d, a, Mathf.Abs(b), e, c);
        // 
        //! Linked to Localization. 
        var entry = table.GetEntry("L09_description");
        string fmt = entry.GetLocalizedString(d, a, Mathf.Abs(b), e, c);

        string description = string.Format(fmt, d, a, Mathf.Abs(b), e, c);

        if (Valid)
        {
            label.text = description;
            CheckValidity();
        }
        else
        {
            label.text = "Ongeldig";
        }
    }

    public void CheckValidity()
    {
        bool Valid = true;

        int b = 0;
        Valid &= int.TryParse(IfDelta.text, out b);

        Valid &= !string.IsNullOrEmpty(IfAdvice.text);

        Update.interactable = Valid;
        Create.interactable = Valid;
    }

    public void CreateGuideline()
    {
        StartCoroutine(CreateGuidelineOndDB());
    }

    public void UpdateGuideline()
    {
        StartCoroutine(UpdateGuidelineOndDB());
    }

    public void DeleteGuideline()
    {
        StartCoroutine(DeleteGuidelineOndDB());
    }

    private IEnumerator CreateGuidelineOndDB()
    {
        LoadingOverlay.AddLoader();

        int delta = 0;
        if (int.TryParse(IfDelta.text, out delta))
        {

            WWWForm form = new WWWForm();

            form.AddField("username", PersistentData.Instance.LoginDataManager.Username);
            form.AddField("password", PersistentData.Instance.LoginDataManager.Password);
            form.AddField("courseid", PersistentData.Instance.LoginDataManager.CourseData.CourseID);
            form.AddField("subjecttype", ((DdType.value == 0) ? "ipf-rd" : "gpf-rd"));
            form.AddField("parameter", activeparameterselection.options[activeparameterselection.value].text);
            form.AddField("delta", delta);
            form.AddField("advice", IfAdvice.text);



            using (UnityWebRequest WWW_ = UnityWebRequest.Post(UriMaker.InsertScriptInUri(PersistentData.WebAdress, "PP_CreatePAGuideline.php"), form))
            {

                yield return WWW_.SendWebRequest();

                if (WWW_.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(WWW_.error);
                }
                else
                {
                }

            }
        }

        LoadingOverlay.RemoveLoader();

        OnEditComplete.Load();
        yield return null;
    }

    private IEnumerator UpdateGuidelineOndDB()
    {
        LoadingOverlay.AddLoader();


        int delta;
        if (int.TryParse(IfDelta.text, out delta))
        {
            WWWForm form = new WWWForm();

            form.AddField("username", PersistentData.Instance.LoginDataManager.Username);
            form.AddField("password", PersistentData.Instance.LoginDataManager.Password);
            form.AddField("courseid", PersistentData.Instance.LoginDataManager.CourseData.CourseID);
            form.AddField("paguidelineid", SelectedAdvice);
            form.AddField("subjecttype", ((DdType.value == 0) ? "ipf-rd" : "gpf-rd"));
            form.AddField("parameter", activeparameterselection.options[activeparameterselection.value].text);
            form.AddField("delta", delta);
            form.AddField("advice", IfAdvice.text);

            Debug.Log($"UPDATE paguidelines SET ( `SubjectType` = '{((DdType.value == 0) ? "ipf-rd" : "gpf - rd")}', `Parameter` = '{activeparameterselection.options[activeparameterselection.value].text}', `Delta` = '{delta}', `Advice` = '{IfAdvice.text}' ) " +
                $" Where CourseID = ( SELECT CourseID  FROM courses  WHERE CourseID = '{PersistentData.Instance.LoginDataManager.CourseData.CourseID}'  AND LeraarUserID = ( SELECT UserID  FROM users  WHERE Username = '{PersistentData.Instance.LoginDataManager.Username}'  AND Password = '{PersistentData.Instance.LoginDataManager.Password}' ) ) AND PAGuidelineID = '{SelectedAdvice}'");




            using (UnityWebRequest WWW_ = UnityWebRequest.Post(UriMaker.InsertScriptInUri(PersistentData.WebAdress, "PP_UpdatePAGuideline.php"), form))
            {
                yield return WWW_.SendWebRequest();
                if (WWW_.result != UnityWebRequest.Result.Success) Debug.Log(WWW_.error);
            }
        }

        yield return null;

        LoadingOverlay.RemoveLoader();

        OnEditComplete.Load();


    }

    private IEnumerator DeleteGuidelineOndDB()
    {
        LoadingOverlay.AddLoader();

        WWWForm form = new WWWForm();

        form.AddField("username", PersistentData.Instance.LoginDataManager.Username);
        form.AddField("password", PersistentData.Instance.LoginDataManager.Password);
        form.AddField("courseid", PersistentData.Instance.LoginDataManager.CourseData.CourseID);
        form.AddField("paguidelineid", SelectedAdvice);

        using (UnityWebRequest WWW_ = UnityWebRequest.Post(UriMaker.InsertScriptInUri(PersistentData.WebAdress, "PP_DeletePAGuideline.php"), form))
        {
            yield return WWW_.SendWebRequest();
            if (WWW_.result != UnityWebRequest.Result.Success) Debug.Log(WWW_.error);
        }

        yield return null;

        LoadingOverlay.RemoveLoader();

        OnEditComplete.Load();

    }

}
