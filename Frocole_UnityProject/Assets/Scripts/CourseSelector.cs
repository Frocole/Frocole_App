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
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// A course selector.
/// </summary>
public class CourseSelector : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// The button control.
    /// </summary>
    public Button Button;

    /// <summary>
    /// The button label.
    /// </summary>
    public Text ButtonLabel;

    /// <summary>
    /// This course.
    /// </summary>
    public CourseObject ThisCourse;

    #endregion Fields

    #region Methods

    /// <summary>
    /// Opens the course.
    /// </summary>
    public void OpenCourse()
    {
        PersistentData.Instance.LoginDataManager.CourseData = ThisCourse;

        if (ThisCourse.LeraarUserID == PersistentData.Instance.LoginDataManager.UserData.UserID)
        {
            SceneManager.LoadScene("L_01_Home");
        }
        else
        {
            SceneManager.LoadScene("C_01_Home");
        }
    }

    /// <summary>
    /// Start is called just before any of the Update methods is called the first time.
    /// </summary>
    public void Start()
    {
        Button.onClick.AddListener(OpenCourse);
    }

    #endregion Methods
}