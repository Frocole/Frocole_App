using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadBasedOnSubjectType : MonoBehaviour
{
    public LoadScene loadIPFScene;
    public LoadScene loadGPFScene;

    public void Load()
    {
        if (PersistentData.Instance.LoginDataManager.SubjectData.FeedbackType == SpiderGraph.FeedbackType.IPF_RD) loadIPFScene.Load();
        else loadGPFScene.Load();
    }
}
