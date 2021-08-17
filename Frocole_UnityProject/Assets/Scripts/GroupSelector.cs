using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupSelector : MonoBehaviour
{
    public GroupObject ThisGroup;
    public Text buttonLabel;

    public void SetThisGroupActive()
    {
        PersistentData.Instance.LoginDataManager.GroupData = ThisGroup;
    }

}
