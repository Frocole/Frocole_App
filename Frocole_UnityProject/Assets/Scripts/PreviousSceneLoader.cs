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

/// <summary>
/// The previous scene loader.
/// </summary>
public class PreviousSceneLoader : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// The back.
    /// </summary>
    public static int Back = 0;

    /// <summary>
    /// Identifier for the current scene.
    /// </summary>
    public int currentSceneID = 0;

    /// <summary>
    /// Identifier for the previous scene.
    /// </summary>
    public int PreviousSceneID = 0;

    #endregion Fields

    #region Methods

    /// <summary>
    /// Executes the 'level was loaded' action.
    /// </summary>
    ///
    /// <param name="level"> The level. </param>
    private void OnLevelWasLoaded(int level)
    {
        PreviousSceneID = currentSceneID;
        currentSceneID = level;
        Back = PreviousSceneID;
    }

    #endregion Methods
}