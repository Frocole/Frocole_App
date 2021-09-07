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
using UnityEngine.Events;

/// <summary>
/// A spider graph.
/// </summary>
public class SpiderGraph : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// List of names of the axis.
    /// </summary>
    public string[] AxisNames;

    /// <summary>
    /// Number of axis.
    /// </summary>
    public int Axis_count = 3;

    /// <summary>
    /// The axis scale.
    /// </summary>
    public int Axis_scale = 100;

    /// <summary>
    /// If viewer only.
    /// </summary>
    public UnityEvent IfViewerOnly;

    /// <summary>
    /// My course object.
    /// </summary>
    public CourseObject MyCourseObject;

    /// <summary>
    /// Type of my feedback.
    /// </summary>
    public FeedbackType MyFeedbackType = FeedbackType.GPF_RD;

    /// <summary>
    /// The on ready.
    /// </summary>
    public UnityEvent OnReady;

    /// <summary>
    /// The spider graph axes.
    /// </summary>
    public SpiderGraphAxis[] spiderGraphAxes;

    /// <summary>
    /// True to teacher.
    /// </summary>
    public bool Teacher;

    /// <summary>
    /// True to test refresh.
    /// </summary>
    public bool TestRefresh = false;

    /// <summary>
    /// Manager for persistent login data.
    /// </summary>
    private UserDataManager _persistentLoginDataManager;

    #endregion Fields

    #region Enumerations

    /// <summary>
    /// Values that represent feedback types.
    /// </summary>
    public enum FeedbackType
    {
        /// <summary>
        /// An enum constant representing the gpf rd option.
        /// </summary>
        GPF_RD,
        /// <summary>
        /// An enum constant representing the ipf rd option.
        /// </summary>
        IPF_RD
    }

    #endregion Enumerations

    #region Methods

    /// <summary>
    /// Instantiate axes.
    /// </summary>
    public void InstantiateAxes()
    {
        switch (MyFeedbackType)
        {
            case FeedbackType.GPF_RD:
                AxisNames = MyCourseObject.GPF_RD_parameters.Split('/');

                break;
            case FeedbackType.IPF_RD:

                AxisNames = MyCourseObject.IPF_RD_parameters.Split('/');
                break;
        }

        Axis_count = Mathf.Clamp(AxisNames.Length, 3, 10);

        foreach (SpiderGraphAxis item in spiderGraphAxes)
        {
            item.gameObject.SetActive(false);
        }

        float angle = 360f / (float)Axis_count;

        for (int i = 0; i < Axis_count; i++)
        {
            spiderGraphAxes[i].AxisID = i;
            spiderGraphAxes[i].SpidergraphAxisName.text = AxisNames[i];
            spiderGraphAxes[i].gameObject.SetActive(true);
            spiderGraphAxes[i].GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, -angle * i);
            spiderGraphAxes[i].MyAngle = -angle * i;

            spiderGraphAxes[i].AllignText();
        }

        if (Teacher && (MyFeedbackType == FeedbackType.IPF_RD || MyFeedbackType == FeedbackType.GPF_RD))
        {
            foreach (var item in spiderGraphAxes)
            {
                item.InteractionButton.interactable = false;
            }
            IfViewerOnly.Invoke();

        }

        OnReady.Invoke();
    }

    /// <summary>
    /// Start is called just before any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        _persistentLoginDataManager = PersistentData.Instance.LoginDataManager;
        MyFeedbackType = _persistentLoginDataManager.SubjectData.FeedbackType;
        MyCourseObject = _persistentLoginDataManager.CourseData;
        InstantiateAxes();
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        if (TestRefresh)
        {
            TestRefresh = false;
            InstantiateAxes();
        }
    }

    #endregion Methods
}