using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameType
{
    Moves,
    Time
}
[System.Serializable]
public class EndGameRequirements
{
    public GameType gameType;
    public int counterValue;
}
public class EndGameManager : MonoBehaviour
{
    public GameObject MovesLable;
   // public GameObject TimeLable;
    public GameObject WinPanel;
    public GameObject LosePanel;
    public Text counter;
    public Text WinScoreTxt;
    public Text WinHighScoreTxt;
    public Image[] winStars;
    public EndGameRequirements requirements;

    private int currentCounterValue;
    private float timer;
    private Board board;
    private ScoreManager scoreManager;
    private MusicController musicController;

    private void Start()
    {
        board = GameObject.FindWithTag("Board").GetComponent<Board>();
        musicController = FindObjectOfType<MusicController>();
        scoreManager = GameObject.FindWithTag("ScoreManager").GetComponent<ScoreManager>();
        SettingLevelTarget();
        SetUpGame();
    }
    public void SettingLevelTarget()
    {
        if (board != null)
        {
            if (board.world != null)
            {
                if (board.world.levels[board.level] != null)
                {
                    requirements = board.world.levels[board.level].EndGameReq;
                }
            }
        }
    }
    public void DecreaseCounterValue()
    {
        if (board.CurrentState != GameState.pause)
        {
            currentCounterValue--;
            if (currentCounterValue <= 0 )
            {
                StartCoroutine(LoserPanel());
            }
            counter.text = "" + currentCounterValue;
        }
    }
    IEnumerator LoserPanel()
    {
        yield return new WaitForSeconds(.5f);
        if(board.CurrentState != GameState.win)
        {
            LoseGame();
        }
    }
    void SetUpGame()
    {
        counter.text = "0";
        currentCounterValue = requirements.counterValue;
        if (requirements.gameType == GameType.Moves)
        {
            MovesLable.SetActive(true);
           // TimeLable.SetActive(false);
        }
        else
        {
            timer = 1;
            MovesLable.SetActive(false);
           // TimeLable.SetActive(true);
        }
        counter.text = "" + currentCounterValue;
    }
    public void IncreaseMoves(int reward)
    {
        currentCounterValue += reward;
        counter.text = "" + currentCounterValue;
    }
    public void WinGame()
    {
        board.CurrentState = GameState.win;
        WinScoreTxt.text = "" + scoreManager.Score;
        int highscore = scoreManager.HighScoreReturn();
        print("highscore is " + highscore);
        if (highscore < scoreManager.Score)
        {
            WinHighScoreTxt.text = "" + scoreManager.Score;
        }
        else
        {
            WinHighScoreTxt.text = "" + highscore;
        }
        switch (scoreManager.numberOfActiveStars)
        {
            case 1: winStars[0].enabled = true; break;
            case 2: winStars[1].enabled = true; break;
            case 3: winStars[2].enabled = true; break;
        }
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
                musicController.Sounds[1].Play();
        }
        WinPanel.SetActive(true);
        print("board state win");
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;
        // StartCoroutine(WinGameCo());
    }
    IEnumerator WinGameCo()
    {
        if (currentCounterValue > 0)
        {
            yield return new WaitForSeconds((float)(currentCounterValue / 2)); // wait for half of currentCounterValue means for every 2 mov wait one sec. 
            // select random position as column or row bomb
        }
        yield return new WaitForSeconds(0.2f);
        WinScoreTxt.text = "" + scoreManager.Score;
        int highscore = scoreManager.HighScoreReturn();
        print("highscore is " + highscore);
        if (highscore < scoreManager.Score)
        {
            WinHighScoreTxt.text = "" + scoreManager.Score;
        }
        else
        {
            WinHighScoreTxt.text = "" + highscore;
        }
        switch (scoreManager.numberOfActiveStars)
        {
            case 1: winStars[0].enabled = true; break;
            case 2: winStars[1].enabled = true; break;
            case 3: winStars[2].enabled = true; break;
        }
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
                musicController.Sounds[1].Play();
        }
        WinPanel.SetActive(true);
        print("board state win");
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;

    }
    public void LoseGame()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
                musicController.Sounds[2].Play();
        }
        LosePanel.SetActive(true);
        board.CurrentState = GameState.lose;
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;
    }
    private void Update()
    {
        if (requirements.gameType == GameType.Time && currentCounterValue > 0 && board.CurrentState != GameState.pause)
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                DecreaseCounterValue();
                timer = 1;
            }
        }
    }
}
