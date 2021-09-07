﻿#region Header

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
/// A course subscriber.
/// </summary>
public class CourseSubscriber : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// The back control.
    /// </summary>
    public Button BackButton;

    /// <summary>
    /// The button label.
    /// </summary>
    public Text ButtonLabel;

    /// <summary>
    /// My toggle.
    /// </summary>
    public Toggle MyToggle;

    /// <summary>
    /// This course.
    /// </summary>
    public CourseObject ThisCourse;

    /// <summary>
    /// Manager for persistent login data.
    /// </summary>
    public UserDataManager _persistentLoginDataManager;

    #endregion Fields

    #region Methods

    /// <summary>
    /// Sub course.
    /// </summary>
    ///
    /// <param name="Sub"> True to sub. </param>
    public void SubCourse(bool Sub)
    {
        if (Sub)
        {
            StartCoroutine(SubmitSubToCourse());
        }
        else
        {
            StartCoroutine(SubmitUnsubToCourse());
        }
    }

    /// <summary>
    /// Submit sub to course.
    /// </summary>
    ///
    /// <returns>
    /// An IEnumerator.
    /// </returns>
    IEnumerator SubmitSubToCourse()
    {
        LoadingOverlay.AddLoader();
        WWWForm form = new WWWForm();

        form.AddField("username", _persistentLoginDataManager.Username);
        form.AddField("password", _persistentLoginDataManager.Password);
        form.AddField("courseid", ThisCourse.CourseID);

        string output;
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(PersistentData.WebAdress + "PP_SubToCourse.php", form))
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

        Debug.Log($"Subbed to {ThisCourse.CourseID}");
        LoadingOverlay.RemoveLoader();
        yield return new WaitForEndOfFrame();
    }

    /// <summary>
    /// Submit unsub to course.
    /// </summary>
    ///
    /// <returns>
    /// An IEnumerator.
    /// </returns>
    IEnumerator SubmitUnsubToCourse()
    {
        LoadingOverlay.AddLoader();
        WWWForm form = new WWWForm();

        form.AddField("username", _persistentLoginDataManager.Username);
        form.AddField("password", _persistentLoginDataManager.Password);
        form.AddField("courseid", ThisCourse.CourseID);

        string output;
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(PersistentData.WebAdress + "PP_UnsubCourse.php", form))
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

        Debug.Log($"Unsubbed {ThisCourse.CourseID}");
        LoadingOverlay.RemoveLoader();
        yield return new WaitForEndOfFrame();
    }

    #endregion Methods
}