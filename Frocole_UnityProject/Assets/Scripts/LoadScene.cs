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
/// A load scene.
/// </summary>
public class LoadScene : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// True to back.
    /// </summary>
    public bool Back = false;

    /// <summary>
    /// Name of the level.
    /// </summary>
    public string LevelName = "";

    /// <summary>
    /// True to load on load.
    /// </summary>
    public bool LoadOnLoad = false;

    #endregion Fields

    #region Methods

    /// <summary>
    /// Loads this object.
    /// </summary>
    public void Load()
    {
        if (Back) SceneManager.LoadScene(PreviousSceneLoader.Back);

        else SceneManager.LoadScene(LevelName);
    }

    /// <summary>
    /// Start is called just before any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {
        if (LoadOnLoad) Load();
    }

    #endregion Methods
}