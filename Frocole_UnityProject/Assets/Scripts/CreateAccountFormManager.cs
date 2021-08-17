using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreateAccountFormManager : MonoBehaviour
{
    public UserObject UserObject;

    public InputField WebAdressInputfield;
    public GameObject NoServerFoundNotification;

    public InputField UserName;
    public GameObject UserNameAlreadyExistNotification;

    public Toggle SavePassword;

    public InputField NickName;

    public InputField Password1;
    public InputField Password2;
    public GameObject PasswordsDoNotMatchNotification;

    public Button SubmitButton;

    #region ChecklistItems

    private bool _validUserName = false;
    private bool _validNickName = false;
    private bool _validPassword = false;

    private string _encryptedPassword;

    #endregion

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

    public void SetWebAdress(string newURL)
    {
        //PersistentData.Instance.SetWebAdress(newURL);
        StartCoroutine(VerifyServer(newURL));
    }

    IEnumerator VerifyServer(string newURL)
    {
        LoadingOverlay.AddLoader();
        WWWForm form = new WWWForm();

        string output = "";
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(PersistentData.WebAdress + "CheckIfServerExists.php", form))
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

        //WWW www = new WWW(newURL + "CheckIfServerExists.php", form);
        //yield return www;
        if (output == "This Frocole Server Exists.")
        {
            PersistentData.Instance.SetWebAdress(newURL);
            NoServerFoundNotification.SetActive(false);
        }
        else
        {
            output = "";
            using (UnityWebRequest WWW_ = UnityWebRequest.Post(PersistentData.WebAdress + "/CheckIfServerExists.php", form))
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

            //www = new WWW(newURL + "/CheckIfServerExists.php", form);
            //yield return www;

            if (output == "This Frocole Server Exists.")
            {
                PersistentData.Instance.SetWebAdress(newURL + "/");
                NoServerFoundNotification.SetActive(false);
            }
            else
            {
                // WrongAdressMessage
                Debug.Log(" WrongAdress" + output);
                NoServerFoundNotification.SetActive(true);
            }
        }
        LoadingOverlay.RemoveLoader();
    }

    public void ValidateUsername(string Username)
    {
        StartCoroutine(WebRequestValidateUsername());
        return;
    }
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

    public void ValidateNickname(string input)
    {
        _validNickName = (NickName.text != "");
        UpdateButton();
    }

    public void UpdateButton()
    {
        SubmitButton.interactable = (_validUserName && _validNickName && _validPassword);
    }

    public void CreateUser()
    {
        StartCoroutine(WebRequestCreateUser());
    }

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
}
