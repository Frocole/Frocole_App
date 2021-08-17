using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GetInfoFromDatabase : MonoBehaviour
{
    public Text infotext;
    public RectTransform contentholder;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DownloadInfoText());
    }

    public IEnumerator DownloadInfoText()
    {
        LoadingOverlay.AddLoader();
        UnityWebRequest WWW = UnityWebRequest.Get(PersistentData.WebAdress + "GetFrocoleInfo.php");
        yield return WWW.SendWebRequest();

        if (WWW.downloadHandler.text != "")
        {
            infotext.text += "Momenteel is de app verbonden met " + PersistentData.WebAdress + ". Hieronder vind u extra informatie aangeleverd door dit domein. \n \n";

            infotext.text += WWW.downloadHandler.text;
        }


        //Debug.Log("text : " + WWW.downloadHandler.text);

        WWW.Dispose();

        contentholder.sizeDelta = new Vector2(contentholder.sizeDelta.x, infotext.preferredHeight + 50);
        LoadingOverlay.RemoveLoader();
    }
}
