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
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// A group member enlister.
/// </summary>
public class GroupMemberEnlister : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// The button label.
    /// </summary>
    public Text ButtonLabel;

    /// <summary>
    /// My toggle.
    /// </summary>
    public Toggle MyToggle;

    /// <summary>
    /// This user.
    /// </summary>
    public UserObject ThisUser;

    /// <summary>
    /// Manager for persistent login data.
    /// </summary>
    public UserDataManager _persistentLoginDataManager;

    #endregion Fields

    #region Methods

    /// <summary>
    /// Enlist member.
    /// </summary>
    ///
    /// <param name="Enlist"> True to enlist. </param>
    public void EnlistMember(bool Enlist)
    {
        if (Enlist)
        {
            StartCoroutine(EnlistUser());
        }
        else
        {
            StartCoroutine(DelistUSer());
        }
    }

    /// <summary>
    /// Delist u ser.
    /// </summary>
    ///
    /// <returns>
    /// An IEnumerator.
    /// </returns>
    IEnumerator DelistUSer()
    {
        LoadingOverlay.AddLoader();
        WWWForm form = new WWWForm();

        form.AddField("username", _persistentLoginDataManager.Username);
        form.AddField("password", _persistentLoginDataManager.Password);
        form.AddField("userid", ThisUser.UserID);
        form.AddField("courseid", _persistentLoginDataManager.GroupData.CourseID);
        form.AddField("groupid", _persistentLoginDataManager.GroupData.GroupID);

        string output = "";
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(UriMaker.InsertScriptInUri(PersistentData.WebAdress, "PP_DelistUser.php"), form))
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

        //WWW www = new WWW(PersistentData.WebAdress + "PP_DelistUser.php", form);
        //yield return www;

        Debug.Log($"Delisted {ThisUser.Nickname}");
        LoadingOverlay.RemoveLoader();
        yield return new WaitForEndOfFrame();
    }

    /// <summary>
    /// Enlist user.
    /// </summary>
    ///
    /// <returns>
    /// An IEnumerator.
    /// </returns>
    IEnumerator EnlistUser()
    {
        LoadingOverlay.AddLoader();
        WWWForm form = new WWWForm();

        form.AddField("username", _persistentLoginDataManager.Username);
        form.AddField("password", _persistentLoginDataManager.Password);
        form.AddField("userid", ThisUser.UserID);
        form.AddField("groupid", _persistentLoginDataManager.GroupData.GroupID);
        form.AddField("courseid", _persistentLoginDataManager.GroupData.CourseID);

        string output = "";

        using (UnityWebRequest WWW_ = UnityWebRequest.Post(UriMaker.InsertScriptInUri(PersistentData.WebAdress, "PP_EnlistUser.php"), form))
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

        //WWW www = new WWW(PersistentData.WebAdress + "PP_EnlistUser.php", form);
        //yield return www;

        Debug.Log($"Enlisted {ThisUser.Nickname} [{output}]");
        LoadingOverlay.RemoveLoader();
        yield return new WaitForEndOfFrame();
    }

    #endregion Methods
}