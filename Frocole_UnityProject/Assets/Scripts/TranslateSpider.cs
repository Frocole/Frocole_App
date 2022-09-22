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
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.UI;

using static SpiderGraph;

/// <summary>
/// This script allows to dynamically translate an array of Text objects. It uses the key from
/// the Localization Tables with a '{FeedbackType}_' as prefix (so either a 'GPF_RD_' or
/// 'IPF_RD_' prefix).
/// 
/// In the IDE the Text objects are linked to the prefix-less keys in the _SpiderTranslationItems
/// array.
/// 
/// This solution is meant for screens C_07 and C_03.
/// </summary>
public class TranslateSpider : MonoBehaviour
{
    [SerializeField]
    public SpiderTranslationItem[] _SpiderTranslationItems;

    private StringTable table;

    /// <summary>
    /// Helper to get a translation.
    /// </summary>
    ///
    /// <param name="code"> The code. </param>
    ///
    /// <returns>
    /// The string.
    /// </returns>
    private string getString(string code)
    {
        return table.GetEntry(code)?.GetLocalizedString() ?? $"[[{code}]]";
    }

    /// <summary>
    /// Start is called just before any of the Update methods is called the first time.
    /// </summary>
    ///
    /// <returns>
    /// An IEnumerator.
    /// </returns>
    private IEnumerator Start()
    {
        yield return LocalizationSettings.InitializationOperation;

        var tableOp = LocalizationSettings.StringDatabase.GetTableAsync("UI Text");
        yield return tableOp;

        table = tableOp.Result;

        UserDataManager _persistentLoginDataManager = PersistentData.Instance.LoginDataManager;
        FeedbackType MyFeedbackType = _persistentLoginDataManager.SubjectData.FeedbackType;

        foreach (SpiderTranslationItem item in _SpiderTranslationItems)
        {
            // Look-up the FeedbackType dependent key and apply it to the Text object.
            // 
            string text = getString($"{MyFeedbackType}_" + item.localeKey);
            if (text.StartsWith("[[") && text.EndsWith("]]"))
            {
                text = getString(item.localeKey);
            }
            item.target.text = text;
        }
    }
}

/// <summary>
/// A spider translation item class to associate keys with Text objects to be dynamically stranated.
/// </summary>
[System.Serializable]
public class SpiderTranslationItem
{
    /// <summary>
    /// The locale (prefix-less) key.
    /// </summary>
    public string localeKey;

    /// <summary>
    /// The Text target.
    /// </summary>
    public Text target;
}

