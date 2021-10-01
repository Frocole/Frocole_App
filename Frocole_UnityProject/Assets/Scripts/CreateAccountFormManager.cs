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

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manager for create account forms.
/// </summary>
public class CreateAccountFormManager : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Name of the nick.
    /// </summary>
    public InputField NickName;

    /// <summary>
    /// The no server found notification.
    /// </summary>
    public GameObject NoServerFoundNotification;

    /// <summary>
    /// The first password.
    /// </summary>
    public InputField Password1;

    /// <summary>
    /// The second password.
    /// </summary>
    public InputField Password2;

    /// <summary>
    /// The passwords do not match notification.
    /// </summary>
    public GameObject PasswordsDoNotMatchNotification;

    /// <summary>
    /// The save password.
    /// </summary>
    public Toggle SavePassword;

    /// <summary>
    /// The submit control.
    /// </summary>
    public Button SubmitButton;

    /// <summary>
    /// Name of the user.
    /// </summary>
    public InputField UserName;

    /// <summary>
    /// The user name already exist notification.
    /// </summary>
    public GameObject UserNameAlreadyExistNotification;

    /// <summary>
    /// The user object.
    /// </summary>
    public UserObject UserObject;

    /// <summary>
    /// The web adress inputfield.
    /// </summary>
    public InputField WebAdressInputfield;

    /// <summary>
    /// The encrypted password.
    /// </summary>
    private string _encryptedPassword;

    /// <summary>
    /// True to valid nick name.
    /// </summary>
    private bool _validNickName = false;

    /// <summary>
    /// True to valid password.
    /// </summary>
    private bool _validPassword = false;

    /// <summary>
    /// True to valid user name.
    /// </summary>
    private bool _validUserName = false;

    #endregion Fields

    #region Methods

    /// <summary>
    /// Creates the user.
    /// </summary>
    public void CreateUser()
    {
        StartCoroutine(WebRequestCreateUser());
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
    /// Updates the button.
    /// </summary>
    public void UpdateButton()
    {
        SubmitButton.interactable = (_validUserName && _validNickName && _validPassword);
    }

    /// <summary>
    /// Validates the nickname described by input.
    /// </summary>
    ///
    /// <param name="input"> The input. </param>
    public void ValidateNickname(string input)
    {
        _validNickName = (NickName.text != "");
        UpdateButton();
    }

    /// <summary>
    /// Validates the password described by input.
    /// </summary>
    ///
    /// <param name="input"> The input. </param>
    public void ValidatePassword(string input)
    {
        if (Password1.text == "" || Password2.text == "")
        {
            PasswordsDoNotMatchNotification.SetActive(false);
            _validPassword = false;
            UpdateButton();
            return;
        }
        if (Password1.text == Password2.text && Password1.text != "") // overkill
        {
            _validPassword = true;
            PasswordsDoNotMatchNotification.SetActive(false);
            _encryptedPassword = PersistentData.Instance.LoginDataManager.Encrypt(Password1.text, UserName.text.ToLower());
            UpdateButton();
            return;
        }
        else
        {
            _validPassword = false;
            PasswordsDoNotMatchNotification.SetActive(true);
            UpdateButton();
            return;
        }
    }

    /// <summary>
    /// Validates the username described by Username.
    /// </summary>
    ///
    /// <param name="Username"> The username. </param>
    public void ValidateUsername(string Username)
    {
        StartCoroutine(WebRequestValidateUsername());
        return;
    }

    /// <summary>
    /// Start is called just before any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {
        UserName.onValueChanged.AddListener(ValidateUsername);
        NickName.onValueChanged.AddListener(ValidateNickname);
        Password1.onValueChanged.AddListener(ValidatePassword);
        Password2.onValueChanged.AddListener(ValidatePassword);

        WebAdressInputfield.SetTextWithoutNotify(PersistentData.WebAdress);

        SubmitButton.onClick.AddListener(CreateUser);

        UpdateButton();
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

#warning The POST url code has to be replaced by a method inserting a php script (and keep the query) and correcting the / as well.

        // Example of such method:
        // 
        // public static Uri InsertScript(Uri baseUrl, String script) {
        // 
        //  String s = baseUrl.GetComponents(UriComponents.SchemeAndServer, UriFormat.UriEscaped);    // "https://frocole.ou.nl:81"
        //  String p = baseUrl.GetComponents(UriComponents.Path, UriFormat.UriEscaped).TrimEnd('/');  // "frocole"
        //  String q = baseUrl.GetComponents(UriComponents.Query, UriFormat.UriEscaped);              // "i=zuyd"
        // 
        //  Uri u = new Uri(new Uri(s), p + "/" + script + "?" + q);                                  // "https://frocole.ou.nl:81/frocole/CheckIfServerExists.php?i=zuyd"
        // 
        //  return u;
        // }
        //
        // Usage:
        // 
        // InsertScript("https://frocole.ou.nl:81/frocole?i=zuyd","CheckIfServerExists.php");       // Parameter is either PersistentData.WebAddress or newURL.
        
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(newURL.TrimEnd('/') + "/CheckIfServerExists.php", form))
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
            PersistentData.Instance.SetWebAdress(newURL.TrimEnd('/') + "/");
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

    /// <summary>
    /// Web request create user.
    /// </summary>
    ///
    /// <returns>
    /// An IEnumerator.
    /// </returns>
    IEnumerator WebRequestCreateUser()
    {
        LoadingOverlay.AddLoader();
        yield return new WaitForEndOfFrame();

        WWWForm form = new WWWForm();

        form.AddField("username", UserName.text.ToLower());
        form.AddField("nickname", NickName.text);
        form.AddField("password", _encryptedPassword);

        yield return new WaitForEndOfFrame();

        string output = "";
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(PersistentData.WebAdress + "CreateUser.php", form))
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
                Debug.Log(output);
            }
        }

        //WWW www = new WWW(PersistentData.WebAdress + "CreateUser.php", form);
        //yield return www;

        PersistentData.Instance.LoginDataManager.StoreLoginData(UserName.text, _encryptedPassword, SavePassword.isOn);
        PersistentData.Instance.LoginDataManager.NewAccount = true;
        LoadingOverlay.RemoveLoader();
        SceneManager.LoadScene("02_Login");
    }

    /// <summary>
    /// Web request validate username.
    /// </summary>
    ///
    /// <returns>
    /// An IEnumerator.
    /// </returns>
    IEnumerator WebRequestValidateUsername()
    {
        LoadingOverlay.AddLoader();
        WWWForm form = new WWWForm();

        form.AddField("username", UserName.text.ToLower());

        string output = "";
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(PersistentData.WebAdress + "CheckIfUserNameExists.php", form))
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
            }
        }

        //WWW www = new WWW(PersistentData.WebAdress + "CheckIfUserNameExists.php", form);
        //yield return www;

        if (output == "The name already exists in the database.")
        {
            UserNameAlreadyExistNotification.SetActive(true);
            _validUserName = false;
        }
        else
        {
            UserNameAlreadyExistNotification.SetActive(false);
            _validUserName = true;
        }
        LoadingOverlay.RemoveLoader();
        UpdateButton();
    }

    #endregion Methods
}