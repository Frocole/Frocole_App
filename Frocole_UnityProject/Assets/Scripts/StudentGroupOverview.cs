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
using UnityEngine.UI;

/// <summary>
/// A student group overview.
/// </summary>
public class StudentGroupOverview : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// The gpfrd selector.
    /// </summary>
    public SubjectSelector GPFRDSelector;

    /// <summary>
    /// The loading overlay go.
    /// </summary>
    public GameObject LoadingOverlayGO;

    /// <summary>
    /// The no group overlay.
    /// </summary>
    public GameObject NoGroupOverlay;

    /// <summary>
    /// The root group object.
    /// </summary>
    public RootGroupObject RootGroupObject;

    /// <summary>
    /// The root user object.
    /// </summary>
    public RootUserObject RootUserObject;

    /// <summary>
    /// The toggle selfreflectionpublic.
    /// </summary>
    public Toggle ToggleSelfreflectionpublic;

    /// <summary>
    /// Manager for persistent login data.
    /// </summary>
    private UserDataManager _persistentLoginDataManager;

    #endregion Fields

    #region Methods

    /// <summary>
    /// Searches for the first group.
    /// </summary>
    ///
    /// <returns>
    /// The found group.
    /// </returns>
    IEnumerator FindGroup()
    {
        LoadingOverlay.AddLoader();
        // attempt to login
        WWWForm form = new WWWForm();

        form.AddField("username", _persistentLoginDataManager.Username);
        form.AddField("password", _persistentLoginDataManager.Password);
        form.AddField("courseid", _persistentLoginDataManager.CourseData.CourseID);

        Debug.Log("FindGroup 1");

        string output = "";
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(PersistentData.WebAdress + "PP_GetGroup.php", form))
        {
            Debug.Log("FindGroup 2");
            yield return WWW_.SendWebRequest();

            Debug.Log("FindGroup 3");

            if (WWW_.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("FindGroup 4a");
                Debug.Log(WWW_.error);
                NoGroupOverlay.SetActive(true);
                // If failed:
            }
            else
            {
                Debug.Log("FindGroup 4b");
                output = WWW_.downloadHandler.text;
                // If succesfull:
                RootGroupObject = JsonUtility.FromJson<RootGroupObject>("{\"groups\": " + output + "}");

                if (output == "" || RootGroupObject.groups == null || RootGroupObject.groups.Length == 0)
                {
                    NoGroupOverlay.SetActive(true);
                }
                else
                {
                    LoadingOverlayGO?.SetActive(false);

                    _persistentLoginDataManager.GroupData = RootGroupObject.groups[0];
                    StartCoroutine(SetUpGroupSubjectSelector());
                    if (ToggleSelfreflectionpublic != null) ToggleSelfreflectionpublic.SetIsOnWithoutNotify(_persistentLoginDataManager.GroupData.Public == "1");
                }
            }
        }

        //Debug.Log("ACTIVE GROUP WITHIN COURSE {\"groups\": " + www.text + "}");

        LoadingOverlay.RemoveLoader();
    }

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
                RootUserObject = JsonUtility.FromJson<RootUserObject>("{\"users\": " + output + "}");

                yield return new WaitForEndOfFrame();

                //Debug.Log("AVAILABLE SUBJECT WITHIN GROUP {\"users\": " + www.text + "}");

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
            }
        }

        LoadingOverlay.RemoveLoader();
    }

    //private int YOffset = 500;

    /// <summary>
    /// Start is called just before any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {
        _persistentLoginDataManager = PersistentData.Instance.LoginDataManager;
        StartCoroutine(FindGroup());
    }

    #endregion Methods
}