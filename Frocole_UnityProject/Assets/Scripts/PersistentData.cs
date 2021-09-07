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
/// A persistent data.
/// </summary>
public class PersistentData : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// The frocole password.
    /// </summary>
    public static string FROCOLE_PASSWORD = "FROCOLE_PASSWORD";

    /// <summary>
    /// URL of the frocole.
    /// </summary>
    public static string FROCOLE_URL = "FROCOLE_URL";

    /// <summary>
    /// The frocole username.
    /// </summary>
    public static string FROCOLE_USERNAME = "FROCOLE_USERNAME";

    /// <summary>
    /// The instance.
    /// </summary>
    public static PersistentData Instance;

    /// <summary>
    /// The web adress.
    /// </summary>
    public static string WebAdress;

    /// <summary>
    /// Manager for login data.
    /// </summary>
    public UserDataManager LoginDataManager;

    #endregion Fields

    #region Methods

    /// <summary>
    /// Sets web adress.
    /// </summary>
    ///
    /// <param name="newURL"> URL of the new. </param>
    public void SetWebAdress(string newURL)
    {
        WebAdress = newURL;
    }

    /// <summary>
    /// Awakes this object.
    /// </summary>
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        if (PlayerPrefs.HasKey(FROCOLE_URL)) WebAdress = PlayerPrefs.GetString(FROCOLE_URL);

        Instance = this;
        DontDestroyOnLoad(this);
    }

    #endregion Methods
}