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

using System;

public static class UriMaker
{
    public static Uri InsertScriptInUri(String baseUrl, String script)
    {
        return InsertScriptInUri(new Uri(baseUrl), script);
    }

    public static Uri InsertScriptInUri(Uri baseUrl, String script)
    {
        String s = baseUrl.GetComponents(UriComponents.SchemeAndServer, UriFormat.UriEscaped);    // "https://frocole.ou.nl:81"
        String p = baseUrl.GetComponents(UriComponents.Path, UriFormat.UriEscaped).TrimEnd('/');  // "frocole"
        String q = baseUrl.GetComponents(UriComponents.Query, UriFormat.UriEscaped).Trim();       // "i=zuyd"

        if (!String.IsNullOrEmpty(q))
        {
            Uri uri = new Uri(new Uri(s), p + "/" + script + "?" + q);

            // "https://frocole.ou.nl:81/frocole/CheckIfServerExists.php?i=zuyd"
            return uri;
        }
        else
        {
            Uri uri = new Uri(new Uri(s), p + "/" + script);

            // "https://frocole.ou.nl:81/frocole/CheckIfServerExists.php"
            return uri;
        }
    }
}
