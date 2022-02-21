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
/// A loading overlay.
/// </summary>
public class LoadingOverlay : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// The loaders.
    /// </summary>
    public static int Loaders = 0;

    //private Dictionary<string, float> activeProcesses = new Dictionary<string, float>();

    /// <summary>
    /// The timer.
    /// </summary>
    public static float Timer = 0;

    /// <summary>
    /// True to allow, false to suppress the time out.
    /// </summary>
    public bool AllowTimeOut;

    /// <summary>
    /// The overlay.
    /// </summary>
    public GameObject overlay;

    /// <summary>
    /// The time out.
    /// </summary>
    public int TimeOut = 15;

    /// <summary>
    /// The time out overlay.
    /// </summary>
    public GameObject TimeOutOverlay;

    #endregion Fields

    #region Methods

    // LoadingOverlay.Addloader();

    /// <summary>
    /// Adds loader.
    /// </summary>
    public static void AddLoader()
    {
        Loaders += 1;
        Timer = 0;
        //Debug.Log("Added Loader " + Loaders);
    }

    // LoadingOverlay.RemoveLoader();

    /// <summary>
    /// Removes the loader.
    /// </summary>
    public static void RemoveLoader()
    {
        Loaders = Mathf.Max(0, Loaders - 1);
        Timer = 0;
       //Debug.Log("Removed Loader " + Loaders);
    }

    /// <summary>
    /// Resets this object.
    /// </summary>
    public static void reset()
    {
        Loaders = 0;
        Timer = 0;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        Timer += Time.deltaTime;
        overlay.gameObject.SetActive(Loaders > 0);
        TimeOutOverlay.gameObject.SetActive(AllowTimeOut && Loaders > 0 && Timer > TimeOut);
    }

    #endregion Methods
}