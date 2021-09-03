using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class UserDataManager : MonoBehaviour
{
    public bool NewAccount = false;

    public UserObject UserData = new UserObject();
    public List<string> Subscriptions = new List<string>();
    public CourseObject CourseData = new CourseObject();
    public GroupObject GroupData = new GroupObject();
    public SubjectObject SubjectData = new SubjectObject();

    public string Username => UserData.Username;
    public string Password => UserData.Password;

    public void StoreLoginData(string _username, string _password, bool Savepassword)
    {
        UserData.Username = _username;
        UserData.Password = _password;

        PlayerPrefs.SetString(PersistentData.FROCOLE_USERNAME, _username);
        if (Savepassword) PlayerPrefs.SetString(PersistentData.FROCOLE_PASSWORD, _password);
        PlayerPrefs.SetString(PersistentData.FROCOLE_URL, PersistentData.WebAdress);
    }

    /// <summary>
    /// to encrypt a password, I've made a function that's supposed to take a string, and return an (realistically irreversible) encrypted string.
    /// I also feed it a "salt" string which is just the user's username, a hardcoded string, and a hardcoded "cost" which i'll tweak based on performance.
    /// </summary>
    public string Encrypt(string toEncrypt, string salt)
    {

        // toEncrypt is the enterred password.
        // salt is the enterred username.

        // hashing itterations
        int cost = 10;

        byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt + salt.ToLower() + EncryptionKey.Seed);

        toEncryptArray = new MD5CryptoServiceProvider().ComputeHash(toEncryptArray);

        SHA256CryptoServiceProvider SHA256 = new SHA256CryptoServiceProvider();
        SHA256.ComputeHash(toEncryptArray);

        // this only happens when a cost of more then 0 is set up.
        for (int i = 0; i < cost; i++)
        {
            toEncryptArray = UTF8Encoding.UTF8.GetBytes(Convert.ToBase64String(SHA256.Hash, 0, SHA256.Hash.Length));
            SHA256.ComputeHash(toEncryptArray);
        }

        return Convert.ToBase64String(SHA256.Hash, 0, SHA256.Hash.Length);
    }



}
