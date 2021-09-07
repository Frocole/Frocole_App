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
/// A set self reflection public.
/// </summary>
public class SetSelfReflectionPublic : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Manager for persistent login data.
    /// </summary>
    public UserDataManager _persistentLoginDataManager;

    #endregion Fields

    #region Methods

    /// <summary>
    /// Publish selfreflection.
    /// </summary>
    ///
    /// <param name="publish"> True to publish. </param>
    public void PublishSelfreflection(bool publish)
    {
        if (publish)
        {
            StartCoroutine(setValuepublic());
        }
        else
        {
            StartCoroutine(setValueprivate());
        }
    }

    /// <summary>
    /// Sets the valueprivate.
    /// </summary>
    ///
    /// <returns>
    /// An IEnumerator.
    /// </returns>
    IEnumerator setValueprivate()
    {
        LoadingOverlay.AddLoader();
        WWWForm form = new WWWForm();

        form.AddField("username", _persistentLoginDataManager.Username);
        form.AddField("password", _persistentLoginDataManager.Password);
        form.AddField("groupid", _persistentLoginDataManager.GroupData.GroupID);
        form.AddField("public", "0");

        string output = "";
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(PersistentData.WebAdress + "PP_SetSelfReflectionPublic.php", form))
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

        Debug.Log("un-published Selfreflection");
        LoadingOverlay.RemoveLoader();
        yield return new WaitForEndOfFrame();
    }

    /// <summary>
    /// Sets the valuepublic.
    /// </summary>
    ///
    /// <returns>
    /// An IEnumerator.
    /// </returns>
    IEnumerator setValuepublic()
    {
        LoadingOverlay.AddLoader();
        WWWForm form = new WWWForm();

        form.AddField("username", _persistentLoginDataManager.Username);
        form.AddField("password", _persistentLoginDataManager.Password);
        form.AddField("groupid", _persistentLoginDataManager.GroupData.GroupID);
        form.AddField("public", "1");

        string output = "";
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(PersistentData.WebAdress + "PP_SetSelfReflectionPublic.php", form))
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

        //Debug.Log($"published Selfreflection {_persistentLoginDataManager.Username} {_persistentLoginDataManager.Password} {_persistentLoginDataManager.GroupData.GroupID} , 1");
        Debug.Log(output);
        LoadingOverlay.RemoveLoader();
        yield return new WaitForEndOfFrame();
    }

    /// <summary>
    /// Start is called just before any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {
        _persistentLoginDataManager = PersistentData.Instance.LoginDataManager;
    }

    #endregion Methods
}