using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{

    public InputField WebAdressInputfield;
    public GameObject NoServerFoundNotification;
    public InputField UsernameInputField;
    public InputField PasswordInputField;

    public Toggle SavePassword;

    public Button ConfirmloginButton;
    public bool DoAttemptAutoLogin = true;

    private UserDataManager _persistentLoginDataManager;
    public GameObject LoginFailedNotification;
    public GameObject NewAccountNotification;
    public GameObject AutomaticLoginCover;
    public UserObject ThisUser;



    private void Start()
    {
        _persistentLoginDataManager = PersistentData.Instance.LoginDataManager;
        WebAdressInputfield.SetTextWithoutNotify(PersistentData.WebAdress);
        if (PlayerPrefs.HasKey(PersistentData.FROCOLE_USERNAME)) UsernameInputField.SetTextWithoutNotify(PlayerPrefs.GetString(PersistentData.FROCOLE_USERNAME));

        AttemptAutoLogin();
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
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(newURL + "CheckIfServerExists.php", form))
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

        Debug.Log(output);
        if (output == "This Frocole Server Exists.")
        {
            PersistentData.Instance.SetWebAdress(newURL);
            NoServerFoundNotification.SetActive(false);
        }
        else
        {
            output = "";
            using (UnityWebRequest WWW_ = UnityWebRequest.Post(newURL + "/CheckIfServerExists.php", form))
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

            Debug.Log(output);

        }

        //WWW www = new WWW(newURL + "CheckIfServerExists.php", form);
        //yield return www;
        //if (www.text == "This Frocole Server Exists.")
        //{
        //    PersistentData.Instance.SetWebAdress(newURL);
        //    NoServerFoundNotification.SetActive(false);
        //}
        //else
        //{
        //    www = new WWW(newURL + "/CheckIfServerExists.php", form);
        //    yield return www;

        //    if (www.text == "This Frocole Server Exists.")
        //    {
        //        PersistentData.Instance.SetWebAdress(newURL + "/");
        //        NoServerFoundNotification.SetActive(false);
        //    }
        //    else
        //    {
        //        // WrongAdressMessage
        //        Debug.Log(" WrongAdress" + www.text);
        //        NoServerFoundNotification.SetActive(true);
        //    }
        //}
        LoadingOverlay.RemoveLoader();
    }

    public void ForgetPassord(bool remember)
    {
        if (!remember) PlayerPrefs.DeleteKey(PersistentData.FROCOLE_PASSWORD);
    }


    public void ValidateInputFields()
    {
        ConfirmloginButton.interactable = (UsernameInputField.text != "" && PasswordInputField.text != "");
    }

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

    IEnumerator AttemptAutoLogin(string _username, string _encryptedpassword)
    {
        LoadingOverlay.AddLoader();
        _username = _username.ToLower();

        // attempt to login
        WWWForm form = new WWWForm();

        form.AddField("username", _username);
        form.AddField("password", _encryptedpassword);


        string output = "";
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(PersistentData.WebAdress + "PP_GetUser.php", form))
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
                // If succesfull:
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

    public void AttemptLogin()
    {
        if (PersistentData.Instance.LoginDataManager.NewAccount && PasswordInputField.text == "DummyPassword")
        {
            StartCoroutine(AttemptAutoLogin(PlayerPrefs.GetString(PersistentData.FROCOLE_USERNAME), PlayerPrefs.GetString(PersistentData.FROCOLE_PASSWORD)));
        }
        StartCoroutine(AttemptManualLogin(UsernameInputField.text.ToLower(), PasswordInputField.text));
        PersistentData.Instance.LoginDataManager.NewAccount = false;
    }

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
        using (UnityWebRequest WWW_ = UnityWebRequest.Post(PersistentData.WebAdress + "PP_GetUser.php", form))
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


}
