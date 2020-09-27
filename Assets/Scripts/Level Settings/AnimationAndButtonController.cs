using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AnimationAndButtonController : MonoBehaviour
{
    public Animator fadeAnim;
    public Animator targetInfoAnim;
    public Animator PauseAnim;
    public Animator HelpShow;
    public Text HelpText;
    public Animator rewardPanel;
    public GameObject[] rewards;
    public Text WatchText;
    public Text rewardText;

    private GameData gameData;
    private Board board;
    private ScoreManager scoreManager;
    private MusicController musicController;
    private EndGameManager endGame;
    private void Start()
    {
        gameData = FindObjectOfType<GameData>();
        endGame = endGame = FindObjectOfType<EndGameManager>();
        musicController = FindObjectOfType<MusicController>();
        board = FindObjectOfType<Board>();
        scoreManager = FindObjectOfType<ScoreManager>();
        
    }
    public void TargetStarterCo()
    {
        if (fadeAnim != null && targetInfoAnim != null)
        {
            fadeAnim.SetBool("fadeOut", true);
            targetInfoAnim.SetBool("panelOut", true);
            StartCoroutine(StartGame());
        }
    }
    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1.4f);
        //showing Help in level 3
        if (board.level == 2)
        {
            if (HelpShow != null && HelpText != null)
            {
                if (PlayerPrefs.HasKey("Language"))
                {
                    var tempL = PlayerPrefs.GetString("Language");
                    if (tempL == "Turkey")
                    {
                        HelpText.text = "Bir çekiç almak için kavanoz doldurun!";
                    }
                    else if (tempL == "Germany")
                    {
                        HelpText.text = "Fülle das Glas, um einen Hammer zu bekommen!";
                    }
                    else if (tempL == "Spain")
                    {
                        HelpText.text = "¡Llena la jarra para conseguir un martillo!";
                    }
                    else
                    {
                        HelpText.text = "fill the jar to get a hammer !";
                    }
                }
                else
                {
                    HelpText.text = "fill the jar to get a hammer !";
                }
                HelpShow.SetBool("helpShow", true);
                yield return new WaitForSeconds(2.3f);
                HelpShow.SetBool("helpShow", false);
            }
        }
    board.CurrentState = GameState.move;
    }
    public void PauseButton()
    { 
        if (fadeAnim != null && targetInfoAnim != null)
        {
            fadeAnim.SetBool("fadeOut", false);
            PauseAnim.SetBool("PauseDown",false);
        }
    }
    public void PauseButtonDown()
    {
        musicController.PlayClickSound();
        if (fadeAnim != null && targetInfoAnim != null)
        {
            board.CurrentState = GameState.move;
            fadeAnim.SetBool("fadeOut", true);
            PauseAnim.SetBool("PauseDown",true);
        }
    }
    public void Exitgame()
    {
        musicController.PlayClickSound();
        gameData.OnApplicationQuit();
        Application.Quit();
    }
    public void Play()
    {
        musicController.PlayClickSound();
        SceneManager.LoadScene("LevelMap");
    }
    public void Rate()
    {
        musicController.PlayClickSound();
        //googlelay Rate Page
    }
    public void Home()
    {
        musicController.PlayClickSound();
        SceneManager.LoadScene("StartScene");
    }
    public void BackToLevelWinPanel()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            if (AdmobTest.instance.hasNet)
            {
                AdmobTest.instance.DisplayInterstitial();
            }
        }
        musicController.PlayClickSound();
        
        if (gameData != null)
        {
            if (board != null)
            {
                gameData.dataSaver.isActive[board.level + 1] = true;
                if (gameData.dataSaver.EarnedStars[board.level] < scoreManager.numberOfActiveStars)
                {
                    gameData.dataSaver.EarnedStars[board.level] = scoreManager.numberOfActiveStars;
                }
                gameData.Save();
            }
        }
        SceneManager.LoadScene("LevelMap");
    }
    public void BackToLevelLosePanel()
    {
        musicController.PlayClickSound();
       
        SceneManager.LoadScene("LevelMap");
    }
    public void NextLevel()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            if (AdmobTest.instance.hasNet)
            {
                AdmobTest.instance.DisplayInterstitial();
            }
        }
        musicController.PlayClickSound();
        musicController.PlayClickSound();
       
        if (gameData != null)
        {
            if (board != null)
            {
                gameData.dataSaver.isActive[board.level + 1] = true;
                if (gameData.dataSaver.EarnedStars[board.level] < scoreManager.numberOfActiveStars)
                {
                    gameData.dataSaver.EarnedStars[board.level] = scoreManager.numberOfActiveStars;
                }
                gameData.Save();
            }
        }
        if (gameData.dataSaver.isActive.Length >= board.level)
        {
            PlayerPrefs.SetInt("Current Level", board.level + 1);
            SceneManager.LoadScene("Main");
        }
    }
    public void ReStartLevel()
    {
        
        musicController.PlayClickSound();
        SceneManager.LoadScene(2);
    }
  
}
