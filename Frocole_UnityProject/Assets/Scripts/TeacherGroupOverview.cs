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

using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// A teacher group overview.
/// </summary>
public class TeacherGroupOverview : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// The gpfrd selector.
    /// </summary>
    public SubjectSelector GPFRDSelector;

    /// <summary>
    /// The root user object.
    /// </summary>
    public RootUserObject RootUserObject;

    /// <summary>
    /// Manager for persistent login data.
    /// </summary>
    private UserDataManager _persistentLoginDataManager;

    #endregion Fields

    #region Methods

    /// <summary>
    /// Sets up the group subject selector.
    /// </summary>
    ///
    /// <returns>
    /// An IEnumerator.
    /// </returns>
    IEnumerator SetUpGroupSubjectSelector()
    {
        LoadingOverlay.AddLoader();
        // attempt to login
        WWWForm form = new WWWForm();

        form.AddField("username", _persistentLoginDataManager.Username);
        form.AddField("password", _persistentLoginDataManager.Password);
        form.AddField("groupid", _persistentLoginDataManager.GroupData.GroupID);
        form.AddField("courseid", _persistentLoginDataManager.CourseData.CourseID);

        Debug.Log($"{_persistentLoginDataManager.Username } {_persistentLoginDataManager.Password}{_persistentLoginDataManager.GroupData.GroupID}{_persistentLoginDataManager.CourseData.CourseID}");

        string output = "";
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(PersistentData.WebAdress + "PP_GetAllSubjectsInMyGroup.php", form))
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
                // If succesfull:
            }
        }

        RootUserObject = JsonUtility.FromJson<RootUserObject>("{\"users\": " + output + "}");

        yield return new WaitForEndOfFrame();

        Debug.Log("AVAILABLE SUBJECT WITHIN GROUP {\"users\": " + output + "}");

        List<string> contributors = new List<string>();
        foreach (UserObject user in RootUserObject.users)
        {
            contributors.Add(user.UserID);
        }
        contributors.Add(_persistentLoginDataManager.CourseData.LeraarUserID);

        GPFRDSelector.ButtonLabel.text = _persistentLoginDataManager.GroupData.GroupNickname;
        GPFRDSelector.ThisSubject.Contributors = contributors.ToArray();
        GPFRDSelector.ThisSubject.FeedbackType = SpiderGraph.FeedbackType.GPF_RD;
        GPFRDSelector.ThisSubject.SubjectID = "GPF" + _persistentLoginDataManager.GroupData.GroupID;
        GPFRDSelector.ThisSubject.SubjectName = _persistentLoginDataManager.GroupData.GroupNickname;

        LoadingOverlay.RemoveLoader();
    }

    //private int YOffset = 500;

    /// <summary>
    /// Start is called just before any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {
        _persistentLoginDataManager = PersistentData.Instance.LoginDataManager;
        StartCoroutine(SetUpGroupSubjectSelector());
    }

    #endregion Methods
}