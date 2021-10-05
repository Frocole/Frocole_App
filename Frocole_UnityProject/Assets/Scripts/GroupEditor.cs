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
/// Editor for group.
/// </summary>
public class GroupEditor : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// The create group control.
    /// </summary>
    public Button CreateGroupButton;

    /// <summary>
    /// The input field.
    /// </summary>
    public InputField InputField;

    /// <summary>
    /// The load scene.
    /// </summary>
    public LoadScene loadScene;

    /// <summary>
    /// The name already exists notification.
    /// </summary>
    public GameObject NameAlreadyExistsNotification;

    /// <summary>
    /// Manager for persistent login data.
    /// </summary>
    private UserDataManager _persistentLoginDataManager;

    #endregion Fields

    #region Methods

    /// <summary>
    /// Check if group name exists in course.
    /// </summary>
    public void CheckIfGroupNameExistsInCourse()
    {
        StartCoroutine(CheckIfGroupNameExistsInCourseOnDB());
    }

    /// <summary>
    /// Rename group.
    /// </summary>
    public void RenameGroup()
    {
        StartCoroutine(RenameGroupOnDB());
    }

    /// <summary>
    /// Start is called just before any of the Update methods is called the first time.
    /// </summary>
    public void Start()
    {
        _persistentLoginDataManager = PersistentData.Instance.LoginDataManager;
        InputField.SetTextWithoutNotify(_persistentLoginDataManager.GroupData.GroupNickname);
    }

    /// <summary>
    /// Check if group name exists in course on database.
    /// </summary>
    ///
    /// <returns>
    /// An IEnumerator.
    /// </returns>
    IEnumerator CheckIfGroupNameExistsInCourseOnDB()
    {
        LoadingOverlay.AddLoader();
        WWWForm form = new WWWForm();

        form.AddField("username", _persistentLoginDataManager.Username);
        form.AddField("password", _persistentLoginDataManager.Password);
        form.AddField("courseid", _persistentLoginDataManager.CourseData.CourseID);
        form.AddField("groupnickname", InputField.text);

        string output = "";
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(UriMaker.InsertScriptInUri(PersistentData.WebAdress, "PP_CheckIfGroupWithNameExistsInCourse.php"), form))
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
                Debug.Log("Group with that name found: {\"groups\": " + output + "}");

                var RootGroupObject = JsonUtility.FromJson<RootGroupObject>("{\"groups\": " + output + "}");

                if ((output == "" || RootGroupObject.groups == null || RootGroupObject.groups.Length == 0))
                {
                    CreateGroupButton.interactable = true;
                    NameAlreadyExistsNotification.SetActive(false);
                }
                else
                {
                    if (RootGroupObject.groups[0].GroupID != _persistentLoginDataManager.GroupData.GroupID) NameAlreadyExistsNotification.SetActive(true);
                    CreateGroupButton.interactable = false;
                }
            }
        }

        //WWW www = new WWW(PersistentData.WebAdress + "PP_CheckIfGroupWithNameExistsInCourse.php", form);
        //yield return www;
        LoadingOverlay.RemoveLoader();
    }

    /// <summary>
    /// Rename group on database.
    /// </summary>
    ///
    /// <returns>
    /// An IEnumerator.
    /// </returns>
    IEnumerator RenameGroupOnDB()
    {
        LoadingOverlay.AddLoader();
        WWWForm form = new WWWForm();

        form.AddField("username", _persistentLoginDataManager.Username);
        form.AddField("password", _persistentLoginDataManager.Password);
        form.AddField("courseid", _persistentLoginDataManager.CourseData.CourseID);
        form.AddField("groupid", _persistentLoginDataManager.GroupData.GroupID);
        form.AddField("groupnickname", InputField.text);

        string output = "";
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(UriMaker.InsertScriptInUri(PersistentData.WebAdress, "PP_RenameGroup.php"), form))
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
            }
        }

        //WWW www = new WWW(PersistentData.WebAdress + "PP_RenameGroup.php", form);
        //yield return www;
        LoadingOverlay.RemoveLoader();
        loadScene.Load();
    }

    #endregion Methods
}