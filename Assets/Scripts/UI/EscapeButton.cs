using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscapeButton : MonoBehaviour
{
    public Animator SettingAnim;
    public Text SelectLangText;
    public GameObject noSoundBtn;
    public GameObject noMusicBtn;

    private bool SoundOn ;
    private bool MusicOn ;
    private languageToggle language;
    private MusicController musicCo;

    private void Start()
    {
        language = FindObjectOfType<languageToggle>();
        musicCo = FindObjectOfType<MusicController>();

        if (PlayerPrefs.HasKey("Sound"))
        {
            int temp = PlayerPrefs.GetInt("Sound");
            if (temp == 0)
            {
                noSoundBtn.SetActive(false);
                SoundOn = true;
            }
            else
            {
                noSoundBtn.SetActive(true);
                SoundOn = false;
            }
        }
        else
        {
            SoundOn = true;
            PlayerPrefs.SetInt("Sound", 0);
        }
        if (PlayerPrefs.HasKey("Music"))
        {
            int temp = PlayerPrefs.GetInt("Music");
            if (temp == 0)
            {
                noMusicBtn.SetActive(false);
                MusicOn = true;
            }
            else
            {
                noMusicBtn.SetActive(true);
                MusicOn = false;
            }
        }
        else
        {
            MusicOn = true;
            PlayerPrefs.SetInt("Music", 0);
        }
        musicCo.MusicControll();
        musicCo.SoundControll();
    }
    public void Setting()
    {
        musicCo.PlayClickSound();
        if (SelectLangText != null)
        {
            if (PlayerPrefs.HasKey("Language"))
            {
                string name = PlayerPrefs.GetString("Language");
                if (name == "UK")
                {
                    SelectLangText.text = "Please select the language";
                }
                else if (name == "Germany")
                {
                    SelectLangText.text = "Bitte wählen Sie die Sprache";
                }
                else if (name == "Spain")
                {
                    SelectLangText.text = "Por favor seleccione el idioma";
                }
                else if (name == "Turkey")
                {
                    SelectLangText.text = "Lütfen dili seçiniz";
                }
            }
            else
            {
                SelectLangText.text = "Please select the language";
            }
        }

        if (SettingAnim != null)
        {
            SettingAnim.SetBool("settingIn", true);
        }
    }
    public void SettingClose()
    {
        musicCo.PlayClickSound();
        if (SettingAnim != null)
        {
            if (language != null)
            {
                language.SaveCurrentLanguage();
            }
            SettingAnim.SetBool("settingIn", false);
        }
    }
    public void SoundsController()
    {
        if (noSoundBtn != null)
        {
            SoundOn = !SoundOn;
            if (SoundOn)
            {
                noSoundBtn.SetActive(false);
                PlayerPrefs.SetInt("Sound", 0);
            }
            else
            {
                noSoundBtn.SetActive(true);
                PlayerPrefs.SetInt("Sound", 1);
            }
        }
        musicCo.SoundControll();
    }
    public void MusicController()
    {
        if (noMusicBtn != null)
        {
            MusicOn = !MusicOn;
            if (MusicOn)
            {
                noMusicBtn.SetActive(false);
                PlayerPrefs.SetInt("Music", 0);
            }
            else
            {
                noMusicBtn.SetActive(true);
                PlayerPrefs.SetInt("Music", 1);
            }
            musicCo.MusicControll();
        }
    }
}
