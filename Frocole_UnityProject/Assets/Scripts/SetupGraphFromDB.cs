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

/// <summary>
/// A setup graph from database.
/// </summary>
public class SetupGraphFromDB : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// The spider graph.
    /// </summary>
    public SpiderGraph SpiderGraph;

    /// <summary>
    /// Manager for persistent login data.
    /// </summary>
    private UserDataManager _persistentLoginDataManager;

    #endregion Fields

    #region Methods

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start()
    {
        _persistentLoginDataManager = PersistentData.Instance.LoginDataManager;
        SpiderGraph.MyCourseObject = _persistentLoginDataManager.CourseData;
        SpiderGraph.InstantiateAxes();
    }

    #endregion Methods
}