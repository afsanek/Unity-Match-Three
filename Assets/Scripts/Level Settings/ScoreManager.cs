using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ScoreMotTex
{
    public Sprite Good;
    public Sprite Amazing;
    public Sprite Wonderful;
}
public class ScoreManager : MonoBehaviour
{
    public ScoreMotTex[] LanMot; // 0.uk 1. spain 2.german 3.turkey
    public Image ScoreMot;
    public GameObject Moti;
    public Text ScoreText;
    public Text LevelText;
    public int Score;
    public Image ScoreBar;
    public Image[] starsInScoreBar;
    public short numberOfActiveStars;

    private GameData gameData;
    private Board board;

    void Start()
    {
        Moti.SetActive(false);
        board = FindObjectOfType<Board>();
        gameData = FindObjectOfType<GameData>();
        LevelText.text = "Level " + (board.level + 1);
    }
    void Update()
    {
        ScoreText.text = "Score " + Score;
    }
    public int HighScoreReturn()
    {
        return gameData.dataSaver.HighScore[board.level];
    }
    public void IncreaseScore(int amountOfIncrement)
    {
        Score += amountOfIncrement;
        if (gameData != null)
        {
            var tempScore = gameData.dataSaver.HighScore[board.level];
            if (Score > tempScore)
            {
                gameData.dataSaver.HighScore[board.level] = Score;
                gameData.Save();
            }
        }
        if (board != null && ScoreBar != null)
        {
            int length = board.ScoreTargets.Length;
            if (length > 0)
            {
                ScoreBar.fillAmount = (float)Score / board.ScoreTargets[length - 1];
                if (Score >= board.ScoreTargets[0] && !starsInScoreBar[0].enabled)
                {
                    starsInScoreBar[0].enabled = true;
                    numberOfActiveStars = 1;
                    if (PlayerPrefs.HasKey("Language"))
                    {
                        var tmpL = PlayerPrefs.GetString("Language");
                        if (tmpL == "German")
                        {
                            ScoreMot.sprite = LanMot[2].Good;
                            Moti.SetActive(true);
                        }
                        else if (tmpL == "Spain")
                        {
                            ScoreMot.sprite = LanMot[1].Good;
                            Moti.SetActive(true);
                        }
                        else if (tmpL == "Turkey")
                        {
                            ScoreMot.sprite = LanMot[3].Good;
                            Moti.SetActive(true);
                        }
                        else
                        {
                            ScoreMot.sprite = LanMot[0].Good;
                            Moti.SetActive(true);
                        }
                        StartCoroutine(DeactiveMot());
                    }
                }
                if (Score >= board.ScoreTargets[1] && !starsInScoreBar[1].enabled)
                {
                    starsInScoreBar[1].enabled = true;
                    numberOfActiveStars = 2;
                    if (PlayerPrefs.HasKey("Language"))
                    {
                        var tmpL = PlayerPrefs.GetString("Language");
                        if (tmpL == "German")
                        {
                            ScoreMot.sprite = LanMot[2].Amazing;
                            Moti.SetActive(true);
                        }
                        else if (tmpL == "Spain")
                        {
                            ScoreMot.sprite = LanMot[1].Amazing;
                            Moti.SetActive(true);
                        }
                        else if (tmpL == "Turkey")
                        {
                            ScoreMot.sprite = LanMot[3].Amazing;
                            Moti.SetActive(true);
                        }
                        else
                        {
                            ScoreMot.sprite = LanMot[0].Amazing;
                            Moti.SetActive(true);
                        }
                        StartCoroutine(DeactiveMot());
                    }
                }
                if (Score >= board.ScoreTargets[2] && !starsInScoreBar[2].enabled)
                {
                    starsInScoreBar[2].enabled = true;
                    numberOfActiveStars = 3;
                    if (PlayerPrefs.HasKey("Language"))
                    {
                        var tmpL = PlayerPrefs.GetString("Language");
                        if (tmpL == "German")
                        {
                            ScoreMot.sprite = LanMot[2].Wonderful;
                            Moti.SetActive(true);
                        }
                        else if (tmpL == "Spain")
                        {
                            ScoreMot.sprite = LanMot[1].Wonderful;
                            Moti.SetActive(true);
                        }
                        else if (tmpL == "Turkey")
                        {
                            ScoreMot.sprite = LanMot[3].Wonderful;
                            Moti.SetActive(true);
                        }
                        else
                        {
                            ScoreMot.sprite = LanMot[0].Wonderful;
                            Moti.SetActive(true);
                        }
                        StartCoroutine(DeactiveMot());
                    }
                }
            }
        }
    }
    IEnumerator DeactiveMot()
    {
        yield return new WaitForSeconds(1.1f);
        Moti.SetActive(false);
    }
}
