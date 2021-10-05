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
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manager for logins.
/// </summary>
public class LoginManager : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// The automatic login cover.
    /// </summary>
    public GameObject AutomaticLoginCover;

    /// <summary>
    /// The confirmlogin control.
    /// </summary>
    public Button ConfirmloginButton;

    /// <summary>
    /// True to do attempt automatic login.
    /// </summary>
    public bool DoAttemptAutoLogin = true;

    /// <summary>
    /// The login failed notification.
    /// </summary>
    public GameObject LoginFailedNotification;

    /// <summary>
    /// The new account notification.
    /// </summary>
    public GameObject NewAccountNotification;

    /// <summary>
    /// The no server found notification.
    /// </summary>
    public GameObject NoServerFoundNotification;

    /// <summary>
    /// The password input field.
    /// </summary>
    public InputField PasswordInputField;

    /// <summary>
    /// The save password.
    /// </summary>
    public Toggle SavePassword;

    /// <summary>
    /// This user.
    /// </summary>
    public UserObject ThisUser;

    /// <summary>
    /// The username input field.
    /// </summary>
    public InputField UsernameInputField;

    /// <summary>
    /// The web adress inputfield.
    /// </summary>
    public InputField WebAdressInputfield;

    /// <summary>
    /// Manager for persistent login data.
    /// </summary>
    private UserDataManager _persistentLoginDataManager;

    #endregion Fields

    #region Methods

    /// <summary>
    /// Attempt login.
    /// </summary>
    public void AttemptLogin()
    {
        if (PersistentData.Instance.LoginDataManager.NewAccount && PasswordInputField.text == "DummyPassword")
        {
            StartCoroutine(AttemptAutoLogin(PlayerPrefs.GetString(PersistentData.FROCOLE_USERNAME), PlayerPrefs.GetString(PersistentData.FROCOLE_PASSWORD)));
        }
        StartCoroutine(AttemptManualLogin(UsernameInputField.text.ToLower(), PasswordInputField.text));
        PersistentData.Instance.LoginDataManager.NewAccount = false;
    }

    /// <summary>
    /// Forget passord.
    /// </summary>
    ///
    /// <param name="remember"> True to remember. </param>
    public void ForgetPassord(bool remember)
    {
        if (!remember) PlayerPrefs.DeleteKey(PersistentData.FROCOLE_PASSWORD);
    }

    /// <summary>
    /// Sets web adress.
    /// </summary>
    ///
    /// <param name="newURL"> URL of the new. </param>
    public void SetWebAdress(string newURL)
    {
        //PersistentData.Instance.SetWebAdress(newURL);
        StartCoroutine(VerifyServer(newURL));
    }

    /// <summary>
    /// Validates the input fields.
    /// </summary>
    public void ValidateInputFields()
    {
        ConfirmloginButton.interactable = (UsernameInputField.text != "" && PasswordInputField.text != "");
    }

    /// <summary>
    /// Attempt automatic login.
    /// </summary>
    private void AttemptAutoLogin()
    {
        if (!DoAttemptAutoLogin)
        {
            AutomaticLoginCover.SetActive(false);
            return;
        }

        if (PersistentData.Instance.LoginDataManager.NewAccount)
        {
            AutomaticLoginCover.SetActive(false);
            NewAccountNotification.gameObject.SetActive(true);
            if (PlayerPrefs.HasKey(PersistentData.FROCOLE_USERNAME) && PlayerPrefs.HasKey(PersistentData.FROCOLE_PASSWORD))
            {
                PasswordInputField.SetTextWithoutNotify("DummyPassword");
            }
            else
            {
                PersistentData.Instance.LoginDataManager.NewAccount = false;
            }
            return;
        }

        if (PlayerPrefs.HasKey(PersistentData.FROCOLE_USERNAME) && PlayerPrefs.HasKey(PersistentData.FROCOLE_PASSWORD))
        {
            StartCoroutine(AttemptAutoLogin(PlayerPrefs.GetString(PersistentData.FROCOLE_USERNAME), PlayerPrefs.GetString(PersistentData.FROCOLE_PASSWORD)));
            //Debug.Log(PlayerPrefs.GetString(_persistentLoginDataManager.FROCOLE_USERNAME) + "  " + PlayerPrefs.GetString(_persistentLoginDataManager.FROCOLE_PASSWORD));
        }
        else AutomaticLoginCover.SetActive(false);
    }

    /// <summary>
    /// Attempt automatic login.
    /// </summary>
    ///
    /// <param name="_username">          The username. </param>
    /// <param name="_encryptedpassword"> The encryptedpassword. </param>
    ///
    /// <returns>
    /// An IEnumerator.
    /// </returns>
    IEnumerator AttemptAutoLogin(string _username, string _encryptedpassword)
    {
        LoadingOverlay.AddLoader();
        _username = _username.ToLower();

        // attempt to login
        WWWForm form = new WWWForm();

        form.AddField("username", _username);
        form.AddField("password", _encryptedpassword);

        string output = "";
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(UriMaker.InsertScriptInUri(PersistentData.WebAdress, "PP_GetUser.php"), form))
        {
            yield return WWW_.SendWebRequest();

            if (WWW_.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(WWW_.error);
                // If failed:
                AutomaticLoginCover.SetActive(false);
                Debug.Log("Automatic Login Failed");
                PasswordInputField.SetTextWithoutNotify("");

            }
            else
            {
                output = WWW_.downloadHandler.text;
                // If successful:
                Debug.Log("LOGIN DATA {\"users\": " + output + "}");
                var data = JsonUtility.FromJson<RootUserObject>("{\"users\": " + output + "}");

                if (data.users.Length != 1)
                {
                    AutomaticLoginCover.SetActive(false);
                    Debug.Log("Automatic Login Failed");
                    PasswordInputField.SetTextWithoutNotify("");

                }
                else
                {
                    _persistentLoginDataManager.UserData = (JsonUtility.FromJson<RootUserObject>("{\"users\": " + output + "}")).users[0];

                    // on a succesful login, save the password and username.
                    yield return new WaitForEndOfFrame();

                    LoadingOverlay.RemoveLoader();
                    // which scene is loaded will depend on which type of user
                    SceneManager.LoadScene("04_SelectCourse");

                    yield break;
                }
            }
        }
        LoadingOverlay.RemoveLoader();
    }

    /// <summary>
    /// Attempt manual login.
    /// </summary>
    ///
    /// <param name="_username"> The username. </param>
    /// <param name="_password"> The password. </param>
    ///
    /// <returns>
    /// An IEnumerator.
    /// </returns>
    IEnumerator AttemptManualLogin(string _username, string _password)
    {
        LoadingOverlay.AddLoader();
        // FIRST encrypt the password.
        _password = _persistentLoginDataManager.Encrypt(_password, _username);
        // THEN attempt to login
        WWWForm form = new WWWForm();

        form.AddField("username", _username);
        form.AddField("password", _password);

        string output = "";
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(UriMaker.InsertScriptInUri(PersistentData.WebAdress, "PP_GetUser.php"), form))
        {
            yield return WWW_.SendWebRequest();

            if (WWW_.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(WWW_.error);
                // If failed:
                Debug.Log("Login Failed");
                LoginFailedNotification.SetActive(true);
            }
            else
            {
                output = WWW_.downloadHandler.text;
                // If succesfull:
                var data = JsonUtility.FromJson<RootUserObject>("{\"users\": " + output + "}");
                // Debug.Log("{\"users\": " + www.text + "}");

                if (data.users.Length != 1)
                {
                    //Debug.Log(_password);
                    Debug.Log("Login Failed");
                    LoginFailedNotification.SetActive(true);
                }
                else
                {
                    _persistentLoginDataManager.UserData = (JsonUtility.FromJson<RootUserObject>("{\"users\": " + output + "}")).users[0];
                    _persistentLoginDataManager.StoreLoginData(_username, _password, SavePassword.isOn);

                    // on a succesful login, save the password and username.
                    yield return new WaitForEndOfFrame();

                    // which scene is loaded will depend on which type of user
                    SceneManager.LoadScene("04_SelectCourse");
                }
            }
        }

        LoadingOverlay.RemoveLoader();
        //WWW www = new WWW(PersistentData.WebAdress + "PP_GetUser.php", form);
        //yield return www;
    }

    /// <summary>
    /// Start is called just before any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {
        _persistentLoginDataManager = PersistentData.Instance.LoginDataManager;
        WebAdressInputfield.SetTextWithoutNotify(PersistentData.WebAdress);
        if (PlayerPrefs.HasKey(PersistentData.FROCOLE_USERNAME)) UsernameInputField.SetTextWithoutNotify(PlayerPrefs.GetString(PersistentData.FROCOLE_USERNAME));

        AttemptAutoLogin();
    }

    /// <summary>
    /// Verify server.
    /// </summary>
    ///
    /// <param name="newURL"> URL of the new. </param>
    ///
    /// <returns>
    /// An IEnumerator.
    /// </returns>
    IEnumerator VerifyServer(string newURL)
    {
        LoadingOverlay.AddLoader();
        WWWForm form = new WWWForm();

        string output = "";
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(UriMaker.InsertScriptInUri(newURL, "CheckIfServerExists.php"), form))
        {
            yield return WWW_.SendWebRequest();

            if (WWW_.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(WWW_.error); // If failed:
            }
            else
            {
                output = WWW_.downloadHandler.text;
            }
        }

        if (output == "This Frocole Server Exists.")
        {
            PersistentData.Instance.SetWebAdress(newURL);
            NoServerFoundNotification.SetActive(false);
        }
        else
        {
            // WrongAdressMessage
            Debug.Log(" WrongAdress" + output);
            NoServerFoundNotification.SetActive(true);
        }

        LoadingOverlay.RemoveLoader();
    }

    #endregion Methods
}