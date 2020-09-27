using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class VersionChecker : MonoBehaviour
{
    public string[] info;
    public GameObject ForceUpdate;
    public Text upTxt;
    private void Start()
    {
        Debug.Log("Application Version : " + Application.version);
         if (Application.internetReachability != NetworkReachability.NotReachable)
         {
             StartCoroutine(GetRequest("http://54.36.152.137/FrutyGarden/appData.php"));
         }
       /* WWW Data = new WWW("https://mrtv.me/game/appData.php");
        yield return Data;
        char[] tst = { ':', '{', '"', ',', '}' };
        string data = Data.text;
        info = data.Split(tst, 5, System.StringSplitOptions.RemoveEmptyEntries);
        Checkvers();
        */
    }
    
    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();
            AdmobTest.instance.hasNet = true;
            string data = webRequest.downloadHandler.text;
            char[] tst= { ':', '{', '"', ',','}'};
            info = data.Split(tst,5,System.StringSplitOptions.RemoveEmptyEntries);

            if (webRequest.isNetworkError)
            {
                Debug.Log(uri+ ": \nError: " + webRequest.error);
            }
            else
            {
                Debug.Log(uri+ ":\nReceived ");
                Checkvers();
            }
        }
    }
    void Checkvers()
    {
        if (info.Length > 3)
        {
            if (info[1] != null && info[3] != null)
            {
                if (!Application.version.Equals(info[1]) && (info[3] == "True" || info[3] == "true"))
                {
                    if (upTxt != null)
                    {
                        if (PlayerPrefs.HasKey("Language"))
                        {
                            var temp = PlayerPrefs.GetString("Language");
                            if (temp == "UK")
                            {
                                upTxt.text = "We've released a new version of our game. Download the update in the Google Play to continue playing.";
                            }
                            else if (temp == "Spain")
                            {
                                upTxt.text = "Hemos lanzado una nueva versión de nuestro juego.\nDescarga la actualización en Google Play para seguir jugando.";
                            }
                            else if (temp == "German")
                            {
                                upTxt.text = "Wir haben eine neue Version unseres Spiels veröffentlicht.\nLaden Sie das Update in Google Play herunter, um die Wiedergabe fortzusetzen.";
                            }
                            else if (temp == "Turkey")
                            {
                                upTxt.text = "Oyunumuzun yeni bir versiyonunu yayınladık.\nOynatmaya devam etmek için güncellemeyi Google Play'de indirin.";
                            }
                        }
                    }
                    ForceUpdate.SetActive(true);
                }
            }
        }
    }
    public void OpenGooglePlay()
    {
        //google play link
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.fesanebyfesane.fruty.garden");
    }
    public void OpenPrivacyPolicy()
    {
        //google play link
        Application.OpenURL("https://fruity-garden.flycricket.io/privacy.html");
    }
}
