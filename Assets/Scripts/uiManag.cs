using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class uiManag : MonoBehaviour
{
    public static uiManag instance;

    public GameObject rewarIn;
    public GameObject rewarOut;
    public Text rewardText;
    public Text WatchText;

    private EndGameManager endGame;
    private AnimationAndButtonController uico;
    private Animator rewardInAnim;
    private Animator rewardOutAnim;
    private int reward;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    private void Start()
    {
        endGame = FindObjectOfType<EndGameManager>();
        uico = FindObjectOfType<AnimationAndButtonController>();
        rewardInAnim = rewarIn.GetComponent<Animator>();
        rewardOutAnim = rewarOut.GetComponent<Animator>();
    }
    public void RewardPanelIn()
    {
        if (rewarIn != null)
        {
            if (PlayerPrefs.HasKey("Language"))
            {
                var temp = PlayerPrefs.GetString("Language");
                if (temp == "UK")
                {
                    WatchText.text = "Get free moves by watching a short video !";
                }
                else if (temp == "Germany")
                {
                    WatchText.text = "Holen Sie sich Freizüge, indem Sie sich ein kurzes Video ansehen!";
                }
                else if (temp == "Spain")
                {
                    WatchText.text = "Consigue movimientos gratis viendo un video corto!";
                }
                else if (temp == "Turkey")
                {
                    WatchText.text = "Kısa bir video izleyerek ücretsiz hamle yapın!";
                }
            }
            rewardInAnim.SetBool("rewardIn", true);
        }
    }
    public void RewardPanelOut()
    {
        if (rewarOut != null)
        {
            if (endGame != null)
            {
                endGame.IncreaseMoves(reward);
            }
        }
        rewardOutAnim.SetBool("rewardOut", true);
        StartCoroutine(StartetTarg());
    }
    IEnumerator StartetTarg()
    {
        yield return new WaitForSeconds(1f);
        uico.TargetStarterCo();
    }
    public void RewardPanelClose()
    {
        if (rewarIn != null)
        {
            rewardInAnim.SetBool("rewardIn", false);
        }
        StartCoroutine(StartetTarg());
    }
    public void GetRewards()
    {
        AdmobTest.instance.DisplayRewardVideo();
        reward = UnityEngine.Random.Range(1, 5);
        rewardText.text = "" + reward;
        // print("rew " + reward);
    }
}
