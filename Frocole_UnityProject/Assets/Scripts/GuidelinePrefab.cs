using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuidelinePrefab : MonoBehaviour
{
    public Text Label;
    public PAGuidelineObject PAGuidelineObject;

    public LoadScene EditGuideline;

    public void OnEditGuideline()
    {
        PersistentData.Instance.LoginDataManager.GuidelineData = PAGuidelineObject;
        EditGuideline.Load();
    }

}
