using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetTextToSubjectName : MonoBehaviour
{
    public string Prefix;
    public Text text;
    public string Affix;

    // Start is called before the first frame update
    void Start()
    {
        text.text = Prefix + PersistentData.Instance.LoginDataManager.SubjectData.SubjectName + Affix;
    }
}
