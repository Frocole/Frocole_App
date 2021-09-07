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
/// Manager for cursist groups.
/// </summary>
public class CursistGroupManager : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// The group nickname.
    /// </summary>
    public Text GroupNickname;

    /// <summary>
    /// The no group found overlay.
    /// </summary>
    public GameObject NoGroupFoundOverlay;

    /// <summary>
    /// Manager for persistent login data.
    /// </summary>
    private UserDataManager _persistentLoginDataManager;

    #endregion Fields

    #region Methods

    /// <summary>
    /// Gets my group in this course.
    /// </summary>
    ///
    /// <returns>
    /// my group in this course.
    /// </returns>
    IEnumerator GetMyGroupInThisCourse()
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
                if (output == "")
                {
                    //The User is not in any group
                    NoGroupFoundOverlay.SetActive(true);
                }
                else
                {
                    Debug.Log("{\"groups\": " + output + "}");
                    var data = JsonUtility.FromJson<RootGroupObject>("{\"groups\": " + output + "}");
                    // Debug.Log("{\"users\": " + www.text + "}");

                    if (data.groups.Length != 1)
                    {
                        Debug.Log("Login Failed");
                    }
                    else
                    {
                        _persistentLoginDataManager.GroupData = (JsonUtility.FromJson<RootGroupObject>("{\"groups\": " + output + "}")).groups[0];
                        GroupNickname.text = _persistentLoginDataManager.GroupData.GroupNickname;
                    }
                }
            }
        }
        LoadingOverlay.RemoveLoader();
        //WWW www = new WWW(PersistentData.WebAdress + "PP_GetGroup.php", form);
        //yield return www;
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start()
    {
        //
        _persistentLoginDataManager = PersistentData.Instance.LoginDataManager;

        // fetch the group
        StartCoroutine(GetMyGroupInThisCourse());
    }

    #endregion Methods
}