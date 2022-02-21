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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using UnityEngine;

/// <summary>
/// Manager for user data.
/// </summary>
public class UserDataManager : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Information describing the course.
    /// </summary>
    public CourseObject CourseData = new CourseObject();

    /// <summary>
    /// Information describing the group.
    /// </summary>
    public GroupObject GroupData = new GroupObject();

    /// <summary>
    /// True to new account.
    /// </summary>
    public bool NewAccount = false;

    /// <summary>
    /// Information describing the subject.
    /// </summary>
    public SubjectObject SubjectData = new SubjectObject();

    /// <summary>
    /// The subscriptions.
    /// </summary>
    public List<string> Subscriptions = new List<string>();
    public Dictionary<string,string> SubscriptionNames = new Dictionary<string, string>();
    public string SubscriptionNameFromID(string id) => SubscriptionNames[id];

    /// <summary>
    /// Information describing the user.
    /// </summary>
    public UserObject UserData = new UserObject();

    public PAGuidelineObject GuidelineData;


    #endregion Fields

    #region Properties

    /// <summary>
    /// Gets the password.
    /// </summary>
    ///
    /// <value>
    /// The password.
    /// </value>
    public string Password => UserData.Password;

    /// <summary>
    /// Gets the user name.
    /// </summary>
    ///
    /// <value>
    /// The user name.
    /// </value>
    public string Username => UserData.Username;

    #endregion Properties

    #region Methods

    /// <summary>
    /// to encrypt a password, I've made a function that's supposed to take a string, and return an
    /// (realistically irreversible) encrypted string. I also feed it a "salt" string which is just
    /// the user's username, a hardcoded "pepper" string, and a hardcoded "cost" which i'll tweak
    /// based on performance.
    /// </summary>
    ///
    /// <param name="toEncrypt"> to encrypt. </param>
    /// <param name="salt">      The salt. </param>
    ///
    /// <returns>
    /// A string.
    /// </returns>
    public string Encrypt(string toEncrypt, string salt)
    {
        // toEncrypt is the enterred password.
        // salt is the enterred username.

        byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt + salt.ToLower() + EncryptionKey.Seed);

        toEncryptArray = new MD5CryptoServiceProvider().ComputeHash(toEncryptArray);

        SHA256CryptoServiceProvider SHA256 = new SHA256CryptoServiceProvider();
        SHA256.ComputeHash(toEncryptArray);

        // this only happens when a cost of more then 0 is set up.
        for (int i = 0; i < EncryptionKey.Cost; i++)
        {
            toEncryptArray = UTF8Encoding.UTF8.GetBytes(Convert.ToBase64String(SHA256.Hash, 0, SHA256.Hash.Length));
            SHA256.ComputeHash(toEncryptArray);
        }

        return Convert.ToBase64String(SHA256.Hash, 0, SHA256.Hash.Length);
    }

    /// <summary>
    /// Stores login data.
    /// </summary>
    ///
    /// <param name="_username">    The username. </param>
    /// <param name="_password">    The password. </param>
    /// <param name="Savepassword"> True to savepassword. </param>
    public void StoreLoginData(string _username, string _password, bool Savepassword)
    {
        UserData.Username = _username;
        UserData.Password = _password;

        PlayerPrefs.SetString(PersistentData.FROCOLE_USERNAME, _username);
        if (Savepassword) PlayerPrefs.SetString(PersistentData.FROCOLE_PASSWORD, _password);
        PlayerPrefs.SetString(PersistentData.FROCOLE_URL, PersistentData.WebAdress);
    }

    #endregion Methods
}