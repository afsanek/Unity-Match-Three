using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectLevel : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject UnlockMark;
    public Image[] star;
    public Text LevelNumber;
    private Image buttonImage;
    private Button buttonComponent;
    public Text highScoreText;

    public bool isActive;
    public int Level;
    private int ActiveStars;
    public int HighScore;
    public GameObject ConfirmPanel;

    private GameData gameData;
    private MusicController musicController;
    private void Start()
    {
        gameData = FindObjectOfType<GameData>();
        musicController = FindObjectOfType<MusicController>();
        buttonComponent = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        if (gameData != null)
        {
            LoadData();
            ActivateStars();
            LevelInfo();
            SpriteChanger();
        }
    }
    void ActivateStars()
    {
        for (int i = 0; i < ActiveStars; i++)
        {
            star[i].enabled = true;
        }
    }
    private void SpriteChanger()
    {
        if (isActive)
        {
            UnlockMark.SetActive(false);
            buttonComponent.enabled = true;
            LevelNumber.enabled = true;
        }
        else
        {
            UnlockMark.SetActive(true);
            buttonComponent.enabled = false;
            LevelNumber.enabled = false;
        }
    }
    void LoadData()
    {
        if (gameData != null)
        {
            if (gameData.dataSaver.isActive.Length >= Level)
            {
                if (gameData.dataSaver.isActive[Level - 1])
                {
                    isActive = true;
                }
                else
                {
                    isActive = false;
                }
                ActiveStars = gameData.dataSaver.EarnedStars[Level - 1];
                HighScore = gameData.dataSaver.HighScore[Level - 1];
            }
        }
    }
    private void LevelInfo()
    {
        LevelNumber.text = "" + Level;
    }
    public void Play()
    {
        
        musicController.PlayClickSound();
        if (gameData.dataSaver.isActive.Length >= Level)
        {
            PlayerPrefs.SetInt("Current Level", Level - 1);
            SceneManager.LoadScene("Main");
        }
    }
   
}
