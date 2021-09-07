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
/// A log out.
/// </summary>
public class LogOut : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// The sceneloader.
    /// </summary>
    public LoadScene Sceneloader;

    #endregion Fields

    #region Methods

    /// <summary>
    /// Logs out now.
    /// </summary>
    public void LogOutNow()
    {
        PlayerPrefs.DeleteKey(PersistentData.FROCOLE_PASSWORD);
        Sceneloader.Load();
    }

    #endregion Methods
}