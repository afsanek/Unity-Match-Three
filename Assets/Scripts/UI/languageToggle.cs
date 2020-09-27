using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class languageToggle : MonoBehaviour
{
    ToggleGroup LangGroupInstance;
    private void Start()
    {
        LangGroupInstance = GetComponent<ToggleGroup>();
        if (PlayerPrefs.HasKey("Language"))
        {
            string name = PlayerPrefs.GetString("Language");
            if (name == "UK")
            {
                SetCurrentLang(0);
            }else if(name == "Germany")
            {
                SetCurrentLang(1);
            }else if(name == "Spain")
            {
                SetCurrentLang(2);
            }else if (name == "Turkey")
            {
                SetCurrentLang(3);
            }
        }
        else
        {
            SetCurrentLang(0);
        }
        SaveCurrentLanguage();
    }
    public Toggle CurrentSelection
    {
        get { return LangGroupInstance.ActiveToggles().FirstOrDefault(); }
    }
    public void SetCurrentLang(int curr)
    {
        var toggles = LangGroupInstance.GetComponentsInChildren<Toggle>();
        toggles[curr].isOn = true;
    }
    public void SaveCurrentLanguage()
    {
        PlayerPrefs.SetString("Language", CurrentSelection.name);
       // print(PlayerPrefs.GetString("Language"));
    }
}
