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
/// Manager for group member lists.
/// </summary>
public class GroupMemberListManager : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// The contentholder.
    /// </summary>
    public GameObject Contentholder;

    /// <summary>
    /// The root user object.
    /// </summary>
    public RootUserObject RootUserObject;

    /// <summary>
    /// Group the root user object already in belongs to.
    /// </summary>
    public RootUserObject RootUserObject_AlreadyInGroup;

    /// <summary>
    /// The user selector prefab.
    /// </summary>
    public GameObject UserSelectorPrefab;

    /// <summary>
    /// The offset.
    /// </summary>
    private int YOffset = 100;

    /// <summary>
    /// Manager for persistent login data.
    /// </summary>
    private UserDataManager _persistentLoginDataManager;

    #endregion Fields

    #region Methods

    /// <summary>
    /// Request all users.
    /// </summary>
    ///
    /// <returns>
    /// An IEnumerator.
    /// </returns>
    IEnumerator RequestAllUsers()
    {
        LoadingOverlay.AddLoader();
        // attempt to login
        WWWForm form = new WWWForm();

        form.AddField("username", _persistentLoginDataManager.Username);
        form.AddField("password", _persistentLoginDataManager.Password);
        form.AddField("groupid", _persistentLoginDataManager.GroupData.GroupID);
        form.AddField("courseid", _persistentLoginDataManager.CourseData.CourseID);

        string output = "";
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(UriMaker.InsertScriptInUri(PersistentData.WebAdress, "PP_GetAvailableGroupMembers.php"), form))
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

        //WWW www = new WWW(PersistentData.WebAdress + "PP_GetAvailableGroupMembers.php", form);
        //yield return www;

        RootUserObject = JsonUtility.FromJson<RootUserObject>("{\"users\": " + output + "}");

        yield return new WaitForEndOfFrame();

        Debug.Log("FREE SUBJECT WITHIN COURSE {\"users\": " + output + "}");

        output = "";
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(UriMaker.InsertScriptInUri(PersistentData.WebAdress, "PP_GetAllSubjectsInMyGroup.php"), form))
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

        //www = new WWW(PersistentData.WebAdress + "PP_GetAllSubjectsInMyGroup.php", form);

        //yield return www;

        RootUserObject_AlreadyInGroup = JsonUtility.FromJson<RootUserObject>("{\"users\": " + output + "}");

        yield return new WaitForEndOfFrame();

        Debug.Log("SUBJECT ALREADY WITHIN GROUP {\"users\": " + output + "}");

        GroupMemberEnlister GO;
        int i = 0;
        Contentholder.GetComponent<RectTransform>().sizeDelta = new Vector2(0, YOffset * (RootUserObject.users.Length + RootUserObject_AlreadyInGroup.users.Length));

        foreach (UserObject user in RootUserObject_AlreadyInGroup.users)
        {
            if (user.UserID == _persistentLoginDataManager.UserData.UserID) continue;

            GO = GameObject.Instantiate(UserSelectorPrefab, Contentholder.transform).GetComponent<GroupMemberEnlister>();
            GO.GetComponent<RectTransform>().localPosition = new Vector3(0, -YOffset, 0) * i + new Vector3(0, -0.5f * YOffset, 0);

            GO._persistentLoginDataManager = _persistentLoginDataManager;

            GO.ButtonLabel.text = user.Nickname;
            GO.MyToggle.SetIsOnWithoutNotify(true);

            GO.ThisUser = user;

            i++;
        }

        foreach (UserObject user in RootUserObject.users)
        {
            if (user.UserID == _persistentLoginDataManager.UserData.UserID) continue;

            GO = GameObject.Instantiate(UserSelectorPrefab, Contentholder.transform).GetComponent<GroupMemberEnlister>();
            GO.GetComponent<RectTransform>().localPosition = new Vector3(0, -YOffset, 0) * i + new Vector3(0, -0.5f * YOffset, 0);

            GO._persistentLoginDataManager = _persistentLoginDataManager;

            GO.ButtonLabel.text = user.Nickname;
            GO.MyToggle.SetIsOnWithoutNotify(false);

            GO.ThisUser = user;

            i++;
        }
        LoadingOverlay.RemoveLoader();
    }

    /// <summary>
    /// Start is called just before any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {
        _persistentLoginDataManager = PersistentData.Instance.LoginDataManager;
        StartCoroutine(RequestAllUsers());
    }

    #endregion Methods
}