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
/// A manage all course buttons.
/// </summary>
public class ManageAllCourseButtons : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// The contentholder.
    /// </summary>
    public GameObject Contentholder;

    /// <summary>
    /// The course selector prefab.
    /// </summary>
    public GameObject CourseSelectorPrefab;

    /// <summary>
    /// The root course object.
    /// </summary>
    public RootCourseObject RootCourseObject;

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
    /// Request all courses.
    /// </summary>
    ///
    /// <returns>
    /// An IEnumerator.
    /// </returns>
    IEnumerator RequestAllCourses()
    {
        LoadingOverlay.AddLoader();
        // attempt to login
        WWWForm form = new WWWForm();

        form.AddField("username", _persistentLoginDataManager.Username);
        form.AddField("password", _persistentLoginDataManager.Password);

        string output = "";
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(UriMaker.InsertScriptInUri(PersistentData.WebAdress, "PP_GetAllMySubScribedCourses.php"), form))
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

        //WWW www = new WWW(PersistentData.WebAdress + "PP_GetAllMySubScribedCourses.php", form);
        //yield return www;
        Debug.Log("ALL SUBSCRIBED COURSES {\"courses\": " + output + "}");
        RootCourseObject = JsonUtility.FromJson<RootCourseObject>("{\"courses\": " + output + "}");

        yield return new WaitForEndOfFrame();

        CourseSelector GO;
        int i = 0;
        Contentholder.GetComponent<RectTransform>().sizeDelta = new Vector2(0, YOffset * RootCourseObject.courses.Length);

        _persistentLoginDataManager.Subscriptions.Clear();

        foreach (CourseObject course in RootCourseObject.courses)
        {
            _persistentLoginDataManager.Subscriptions.Add(course.CourseID);

            //Debug.Log(course.CourseName);
            GO = GameObject.Instantiate(CourseSelectorPrefab, Contentholder.transform).GetComponent<CourseSelector>();
            GO.GetComponent<RectTransform>().localPosition = new Vector3(0, -YOffset, 0) * i + new Vector3(0, -0.5f * YOffset, 0);
            GO.ThisCourse = course;
            GO.ButtonLabel.text = course.CourseName;

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
        StartCoroutine(RequestAllCourses());
    }

    #endregion Methods
}