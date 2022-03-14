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
using UnityEngine.UI;

/// <summary>
/// A locale dropdown.
/// </summary>
public class LocaleChanger : MonoBehaviour
{
    /// <summary>
    /// Gets the start.
    /// </summary>
    ///
    /// <returns>
    /// An IEnumerator.
    /// </returns>
    IEnumerator Start()
    {
        // Wait for the localization system to initialize, loading Locales, preloading etc.
        yield return LocalizationSettings.InitializationOperation;

        //! This should work on Android/iPhone to match the UI Language to the 
        //! System Language (if Localization is available) or else use the 
        //! ProjectLocale as fallback.
        // 
        //! Note: Uses a Language Locale without (Formatter).
        //! Note: This means this code cannot distinguish between Dutch (nl) and Dutch (be) or English (us) and English (uk).
        
        //! Make sure we first switch to the default Locale (English) frist 
        //! before attempting to match to the Application.systemLanguage 
        //! if that particular Localization is available.
        LocalizationSettings.SelectedLocale = LocalizationSettings.ProjectLocale;

        //! So English  -> English(en)  ; localization
        //!    French   -> English(en)  ; use fallback
        //!    Finnish  -> English(en)  ; use fallback
        //!    Dutch    -> Dutch (nl)   ; localization

        //! TEST CODE, Creates English/English/en)
        Locale targetLocale = Locale.CreateLocale(Application.systemLanguage);

        //! Debug Code
        //targetLocale = Locale.CreateLocale(SystemLanguage.Finnish);

        //! ! Debug Code
        //Debug.Log($"ProjectLocale:  {LocalizationSettings.ProjectLocale}");
        //Debug.Log($"SystemLanguage: {Application.systemLanguage}");
        //Debug.Log($"TargetLocale:   {targetLocale}");

        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
        {
            //! Match with identifiers so we include some form of a (Formatter).
            // 
            Locale location = Locale.CreateLocale(LocalizationSettings.AvailableLocales.Locales[i].Identifier);

            if (targetLocale.Identifier == location.Identifier)
            {
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[i];
                break;
            }
        }

        //! ! Debug Code
        Debug.Log($"TargetLocale:   {targetLocale}");
        Debug.Log($"SelectedLocale: {LocalizationSettings.SelectedLocale}");
    }
}
