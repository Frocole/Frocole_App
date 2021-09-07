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
using System.Linq;

using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// A manage feedback subject buttons.
/// </summary>
public class ManageFeedbackSubjectButtons : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// The contentholder.
    /// </summary>
    public GameObject Contentholder;

    /// <summary>
    /// True if is teacher, false if not.
    /// </summary>
    public bool IsTeacher = false;

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
    /// The user selector prefab.
    /// </summary>
    public GameObject UserSelectorPrefab;

    /// <summary>
    /// The offset.
    /// </summary>
    private int YOffset = 122;

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
        WWWForm form = new WWWForm();

        form.AddField("username", _persistentLoginDataManager.Username);
        form.AddField("password", _persistentLoginDataManager.Password);
        form.AddField("courseid", _persistentLoginDataManager.CourseData.CourseID);

        string output = "";
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(PersistentData.WebAdress + "PP_GetGroup.php", form))
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
                RootGroupObject = JsonUtility.FromJson<RootGroupObject>("{\"groups\": " + output + "}");

                if (output == "" || RootGroupObject.groups == null || RootGroupObject.groups.Length == 0)
                {
                    NoGroupOverlay.SetActive(true);
                }
                else
                {
                    _persistentLoginDataManager.GroupData = RootGroupObject.groups[0];
                    StartCoroutine(RequestAllUsers());
                }
            }
        }

        LoadingOverlay.RemoveLoader();
    }

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

        SubjectSelector GO;
        int i = 0;
        Contentholder.GetComponent<RectTransform>().sizeDelta = new Vector2(0, YOffset * RootUserObject.users.Length);

        List<string> contributors = new List<string>();
        foreach (UserObject user in RootUserObject.users)
        {
            contributors.Add(user.UserID);
        }

        List<UserObject> usersInGroup = RootUserObject.users.ToList();

        foreach (UserObject user in RootUserObject.users)
        {
            GO = GameObject.Instantiate(UserSelectorPrefab, Contentholder.transform).GetComponent<SubjectSelector>();
            GO.GetComponent<RectTransform>().localPosition = new Vector3(0, -YOffset, 0) * i + new Vector3(0, -0.5f * YOffset, 0);

            //manage Nicknames (avoid Doubles and Mark myself as "(ikzelf)")

            string nickname = user.Nickname;
            if (usersInGroup.FindAll(x => x.Nickname == user.Nickname).Count > 1) // more than one user with the same Nickname
            {
                int nickNameNR = 1;
                foreach (UserObject otherUser in usersInGroup.FindAll(x => x.Nickname == user.Nickname))
                {
                    if (int.Parse(otherUser.UserID) < int.Parse(user.UserID)) nickNameNR++;
                }
                nickname = user.Nickname + " " + nickNameNR.ToString();
            }
            if (user.UserID == _persistentLoginDataManager.UserData.UserID) nickname += " (ikzelf)";

            GO.ButtonLabel.text = nickname;

            GO.ThisSubject.SubjectName = nickname;
            GO.ThisSubject.SubjectID = user.UserID;
            GO.ThisSubject.Contributors = contributors.ToArray();
            GO.ThisSubject.FeedbackType = SpiderGraph.FeedbackType.IPF_RD;

            GO.ThisSubject.Public = (user.Public == "1");
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
        if (IsTeacher) StartCoroutine(RequestAllUsers());
        else StartCoroutine(FindGroup());
    }

    #endregion Methods
}