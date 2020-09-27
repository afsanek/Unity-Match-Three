using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class LevelTarget
{
   public int NumberOfNeeded;
   public int NumberOfCollected;
   public Sprite TargetSprite;
   public string TargetTag;
}
[System.Serializable]
public enum TargetType
{
    fruity,
    score
}
public class GoalManager : MonoBehaviour
{
    public LevelTarget[] targets;
    public TargetType targetType;
    public int ScoreTarget;
    public GameObject GoalPrefab;
    public GameObject GoalContainerInPanel;
    public GameObject GoalContainerInLevel;
    public GameObject ScoreGoalContainerInPanel;
    public GameObject ScoreGoalContainerInLevel;
    public Text ScoreTextInPanel;
    public Text ScoreTextInLevel;
    public List<GoalPanel> targetsPanel = new List<GoalPanel>();
    public Image randomTarget;
    public Text randomTargetText;
    public GameObject[] Bees;
    public Image JarFiller;
    public GameObject randTarget;
    public GameObject Hammer;
    public GameObject[] tmpHammer;
    public bool HammerOn;
    public int NumberofRandHammer = 0;
    public Text HammerText;
    public Animator HammerPanelIn;

    public LevelTarget randomGoal;     //jar Goal
    private EndGameManager endGame;
    private ScoreManager scoreManager;
    private Board board;

    private void Start()
    {
        endGame = GameObject.FindWithTag("EndGameManager").GetComponent<EndGameManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
        board = GameObject.FindWithTag("Board").GetComponent<Board>();

        SettingLevelTarget();
        SetUpInLevelGoals();
    }
    public void SettingLevelTarget()
    {
        if (board != null)
        {
            if (board.world != null)
            {
                if (board.world.levels[board.level] != null)
                {
                    targets = board.world.levels[board.level].levelTargets;
                    targetType = board.world.levels[board.level].targetType;
                    ScoreTarget = board.world.levels[board.level].ScoreTarget;

                    if (board.level > 1)
                    {
                        randTarget.SetActive(true);
                        // Setting Random Goal : 
                        randomGoal = new LevelTarget
                        {
                            NumberOfNeeded = Random.Range(5, 30),
                            NumberOfCollected = 0
                        };
                        int temp = Random.Range(0, board.world.levels[board.level].Dots.Length - 1);
                        randomGoal.TargetSprite = board.world.levels[board.level].Dots[temp].GetComponent<SpriteRenderer>().sprite;
                        randomGoal.TargetTag = board.world.levels[board.level].Dots[temp].tag;
                    }
                    else
                    {
                        randTarget.SetActive(false);
                        randomGoal = new LevelTarget
                        {
                            NumberOfNeeded = 200,
                            NumberOfCollected = -200,
                            TargetTag = "alaki",
                            TargetSprite = null
                        };
                    }
                }
                else
                {
                    if (board.level > 1)
                    {
                        randTarget.SetActive(true);
                        // Setting Random Goal : 
                        randomGoal = new LevelTarget
                        {
                            NumberOfNeeded = Random.Range(9, 30),
                            NumberOfCollected = 0
                        };
                        int temp = Random.Range(0, board.Dots.Length - 1);
                        randomGoal.TargetSprite = board.Dots[temp].GetComponent<SpriteRenderer>().sprite;
                        randomGoal.TargetTag = board.Dots[temp].tag;
                    }
                    else
                    {
                        randTarget.SetActive(false);
                        randomGoal = new LevelTarget
                        {
                            NumberOfNeeded = 200,
                            NumberOfCollected = -200,
                            TargetTag = "alaki",
                            TargetSprite = null
                        };
                    }
                }
            }
        }
    }
    private void SetUpInLevelGoals()
    {
        randomTarget.sprite = randomGoal.TargetSprite;
        randomTargetText.text = "" + randomGoal.NumberOfNeeded;
        if (targetType == TargetType.fruity)
        {
            GoalContainerInLevel.SetActive(true);
            GoalContainerInPanel.SetActive(true);
            ScoreGoalContainerInLevel.SetActive(false);
            ScoreGoalContainerInPanel.SetActive(false);
            for (int i = 0; i < targets.Length; i++)
            {
                ///in game start panel
                var inGame = Instantiate(GoalPrefab, GoalContainerInPanel.transform.position, Quaternion.identity);
                inGame.transform.SetParent(GoalContainerInPanel.transform);
                inGame.GetComponent<RectTransform>().localScale = Vector3.one;
                var inGameComponent = inGame.GetComponent<GoalPanel>();
                inGameComponent.childSprite.sprite = targets[i].TargetSprite;
                inGameComponent.targetText.text = "" + targets[i].NumberOfNeeded;

                /// in level panel
                var inLevel = Instantiate(GoalPrefab, GoalContainerInLevel.transform.position, Quaternion.identity);
                inLevel.transform.SetParent(GoalContainerInLevel.transform);
                var inLevelComponent = inLevel.GetComponent<GoalPanel>();
                targetsPanel.Add(inLevelComponent);
                inLevelComponent.childSprite.sprite = targets[i].TargetSprite;
                inLevelComponent.targetText.text = "" + targets[i].NumberOfNeeded;
                inLevel.GetComponent<RectTransform>().localScale = Vector3.one;
            }
        }
        if (targetType == TargetType.score)
        {
            GoalContainerInLevel.SetActive(false);
            GoalContainerInPanel.SetActive(false);
            ScoreGoalContainerInLevel.SetActive(true);
            ScoreGoalContainerInPanel.SetActive(true);
            ScoreTextInLevel.text = "" + ScoreTarget;
            ScoreTextInPanel.text = "" + ScoreTarget;
        }

    }
    public void UpdateTargets()
    {
        while(randomGoal.NumberOfCollected >= randomGoal.NumberOfNeeded)
        {
            NumberofRandHammer = randomGoal.NumberOfCollected / randomGoal.NumberOfNeeded;
            randomGoal.NumberOfCollected = randomGoal.NumberOfCollected % randomGoal.NumberOfNeeded;
            print("hammers : " + NumberofRandHammer + "collected : " + randomGoal.NumberOfCollected + "Needed : " + randomGoal.NumberOfNeeded);
            UpdateHammerText();
            HammerPanelIn.SetBool("hammerIn", true);
            IncreaseJarScore();
            if (NumberofRandHammer > 0)
            {
                MakeHamer();
            }
        }
        randomTargetText.text = "" + (randomGoal.NumberOfNeeded - randomGoal.NumberOfCollected);
        if (targetType == TargetType.fruity)
        {
            int goalsCompleted = 0;
            for (int i = 0; i < targets.Length; i++)
            {
                targetsPanel[i].targetText.text = "" + (targets[i].NumberOfNeeded - targets[i].NumberOfCollected);
                if (targets[i].NumberOfCollected >= targets[i].NumberOfNeeded)
                {
                    goalsCompleted++;
                    targetsPanel[i].targetText.text = "";
                    targetsPanel[i].checkMark.SetActive(true);
                }
            }
            if (goalsCompleted >= targets.Length)
            {
                endGame.WinGame();
            }
        }
        if (targetType == TargetType.score)
        {
            if (scoreManager.Score >= ScoreTarget)
            {
                ScoreTextInLevel.text = "0";
                endGame.WinGame();
            }
            else
            {
                ScoreTextInLevel.text = "" + (ScoreTarget - scoreManager.Score);
            }
        }
    }
    public void CompareGoal(string tag)
    {
        if(tag == randomGoal.TargetTag)
        {
            randomGoal.NumberOfCollected++;
            IncreaseJarScore();
        }
        if (targetType == TargetType.fruity)
        {
            for (int i = 0; i < targets.Length; i++)
            {
                if (tag == targets[i].TargetTag)
                {
                    targets[i].NumberOfCollected++;
                }
            }
        }
    }
    public void IncreaseJarScore()
    {
        if (JarFiller != null)
        {
            JarFiller.fillAmount = (randomGoal.NumberOfCollected / (float) randomGoal.NumberOfNeeded);
        }
    }
    public void MakeHamer()
    {
        var tmp = NumberofRandHammer;
        tmpHammer = new GameObject[tmp];
        while (tmp > 0)
        {
            tmpHammer[tmp-1] = Instantiate(Hammer, new Vector3(100,100, 0), Quaternion.identity);
            tmp--;
        }
        HammerOn = true;
    }
    public void UpdateHammerText()
    {
        HammerText.text = "" + NumberofRandHammer;
    }
}
