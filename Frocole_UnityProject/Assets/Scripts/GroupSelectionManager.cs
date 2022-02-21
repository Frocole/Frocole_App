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
/// Manager for group selections.
/// </summary>
public class GroupSelectionManager : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Groups all belongs to.
    /// </summary>
    public RootGroupObject AllGroups;

    /// <summary>
    /// The contentholder.
    /// </summary>
    public GameObject Contentholder;

    /// <summary>
    /// The group manager prefab.
    /// </summary>
    public GameObject GroupManagerPrefab;

    /// <summary>
    /// The offset.
    /// </summary>
    private int YOffset = 100;

    /// <summary>
    /// Manager for persistent login data.
    /// </summary>
    private UserDataManager _persistentLoginDataManager;

    #endregion Fields

    #region Methods

    /// <summary>
    /// Searches for the first groups.
    /// </summary>
    ///
    /// <returns>
    /// The found groups.
    /// </returns>
    IEnumerator FindGroups()
    {
        LoadingOverlay.AddLoader();
        // attempt to login
        WWWForm form = new WWWForm();

        form.AddField("username", _persistentLoginDataManager.Username);
        form.AddField("password", _persistentLoginDataManager.Password);
        form.AddField("courseid", _persistentLoginDataManager.CourseData.CourseID);

        string output = "";
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(UriMaker.InsertScriptInUri(PersistentData.WebAdress, "PP_GetAllGroupsInCourse.php"), form))
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

        //WWW www = new WWW(PersistentData.WebAdress + "PP_GetAllGroupsInCourse.php", form);
        //yield return www;

        //Debug.Log("GROUPS WITHIN COURSE {\"groups\": " + output + "}");

        AllGroups = JsonUtility.FromJson<RootGroupObject>("{\"groups\": " + output + "}");

        if (output == "" || AllGroups.groups == null || AllGroups.groups.Length == 0)
        {
            Debug.Log("NO GROUPS FOUND");
        }
        else
        {

            GroupSelector GO;
            int i = 0;
            Contentholder.GetComponent<RectTransform>().sizeDelta = new Vector2(0, YOffset * AllGroups.groups.Length);

            foreach (var item in AllGroups.groups)
            {
                GO = GameObject.Instantiate(GroupManagerPrefab, Contentholder.transform).GetComponent<GroupSelector>();
                GO.GetComponent<RectTransform>().localPosition = new Vector3(0, -YOffset, 0) * i;// + new Vector3(0, -0.5f * YOffset, 0);

                GO.ThisGroup = item;
                GO.buttonLabel.text = item.GroupNickname;

                i++;
            }

        }

        LoadingOverlay.RemoveLoader();
    }

    /// <summary>
    /// Start is called just before any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {
        _persistentLoginDataManager = PersistentData.Instance.LoginDataManager;
        StartCoroutine(FindGroups());
    }

    #endregion Methods
}