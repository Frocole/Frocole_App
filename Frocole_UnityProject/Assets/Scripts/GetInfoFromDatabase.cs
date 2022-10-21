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
using UnityEngine.UI;

/// <summary>
/// A get information from database.
/// </summary>
public class GetInfoFromDatabase : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// The contentholder.
    /// </summary>
    public RectTransform contentholder;

    /// <summary>
    /// The infotext.
    /// </summary>
    public Text infotext;

    #endregion Fields

    #region Methods

    string connectionPrefix = "Momenteel is de app verbonden met {0}. Hieronder vindt u extra informatie aangeleverd door dit domein.";
    string noConnectionPrefix = "De extra informatie kan niet worden opgehaald. Er is door u geen valide Frocole server adres opgegeven";
    public void SetConnectionPrefix(string Text) => connectionPrefix = Text;
    public void SetNoConnectionPrefix(string Text) => noConnectionPrefix = Text;

    /// <summary>
    /// Downloads the information text.
    /// </summary>
    ///
    /// <returns>
    /// An IEnumerator.
    /// </returns>
    public IEnumerator DownloadInfoText()
    {
        LoadingOverlay.AddLoader();
     
        UnityWebRequest WWW = UnityWebRequest.Get(UriMaker.InsertScriptInUri(PersistentData.WebAdress, "GetFrocoleInfo.php"));
        yield return WWW.SendWebRequest();

        if (WWW.downloadHandler.text != "")
        {
            infotext.text += string.Format(connectionPrefix, PersistentData.WebAdress) + "\n \n";

            infotext.text += WWW.downloadHandler.text;
        }
        else 
        {
            infotext.text += noConnectionPrefix;
        }

        WWW.Dispose();

        contentholder.sizeDelta = new Vector2(contentholder.sizeDelta.x, contentholder.sizeDelta.y + infotext.preferredHeight + 50);

        LoadingOverlay.RemoveLoader();
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start()
    {
        if (!string.IsNullOrEmpty(PersistentData.WebAdress))
            StartCoroutine(DownloadInfoText());
    }

    #endregion Methods
}