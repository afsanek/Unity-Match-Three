using GoogleMobileAds.Api;
using UnityEngine;
using System;

public class AdmobTest : MonoBehaviour
{
    public static AdmobTest instance;
    public bool isRewarded;
    public bool reloadVid;
    public int isVeloadVid;
    public bool isBannFail;
    public int isBanFail;
    public bool isInterFail;
    public int isInterrFail;
    public bool hasNet;
    public int thresh = 20;


    private string APP_ID = "ca-app-pub-1918554260302883~9981687912";

    private BannerView bannerAD;
    private InterstitialAd interstitialAD;
    private RewardBasedVideoAd rewardVideoAD;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    private void Start()
    {

        MobileAds.Initialize(APP_ID);

        RequestBanner();
        RequestInterstitial();
        RequestVideoAD();

        DisplayBanner();
    }
    private void Update()
    {
        if (isRewarded)
        {
            uiManag.instance.rewarIn.SetActive(false);
            uiManag.instance.rewarOut.SetActive(true);
            isRewarded = false;
        }
        if (reloadVid && isVeloadVid > thresh)
        {
            RequestVideoAD();
            reloadVid = false;
            isVeloadVid = 0;
        }
        if (isBannFail && isBanFail > thresh)
        {
            RequestBanner();
            isBannFail = false;
            isBanFail = 0;
        }
        if (isInterFail && isInterrFail > thresh)
        {
            RequestInterstitial();
            isInterFail = false;
            isInterrFail = 0;
        }
    }
    void RequestBanner()
    {
        string banner_ID = "ca-app-pub-1918554260302883/9960885466";
        bannerAD = new BannerView(banner_ID, AdSize.SmartBanner , AdPosition.Bottom);


        bannerAD.OnAdLoaded += HandleOnAdLoaded;
        bannerAD.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        bannerAD.OnAdClosed += HandleOnAdClosed;


        //real
        AdRequest adRequest = new AdRequest.Builder().Build();

        //test
        /*AdRequest adRequest = new AdRequest.Builder()
            .AddTestDevice("2077ef9a63d2b398840261c8221a0c9b")
            .Build();*/
            
        bannerAD.LoadAd(adRequest);
    }
    void RequestInterstitial()
    {
        string interstitial_ID = "ca-app-pub-1918554260302883/1699252068";

        interstitialAD = new InterstitialAd(interstitial_ID);


        this.interstitialAD.OnAdLoaded += HandleOnAdLoaded;
        this.interstitialAD.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        this.interstitialAD.OnAdClosed += HandleOnAdClosed;

        //real
        AdRequest adRequest = new AdRequest.Builder().Build();

        //test
        /*AdRequest adRequest = new AdRequest.Builder()
            .AddTestDevice("2077ef9a63d2b398840261c8221a0c9b")
            .Build();
            */
        interstitialAD.LoadAd(adRequest);
    }
    void RequestVideoAD()
    {
        string reward_ID = "ca-app-pub-1918554260302883/4614966116";

        rewardVideoAD = RewardBasedVideoAd.Instance;

        HandleRewardAdEvents(true);

        //for real
        AdRequest adRequest = new AdRequest.Builder().Build();

        //for testing
        /*AdRequest adRequest = new AdRequest.Builder()
            .AddTestDevice("2077ef9a63d2b398840261c8221a0c9b")
            .Build();
            */
        rewardVideoAD.LoadAd(adRequest, reward_ID);
    }
    public void DisplayBanner()
    {
        bannerAD.Show();
    }
    public void DisplayInterstitial()
    {
        if (interstitialAD.IsLoaded())
        {
            interstitialAD.Show();
        }
    }
    public void DisplayRewardVideo()
    {
        if (rewardVideoAD.IsLoaded())
        {
            rewardVideoAD.Show();
        }
    }

    //HANDLE EVENTS
    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
    }
    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        if (sender.ToString() == "GoogleMobileAds.Api.BannerView")
        {
            isInterFail = true;
            isInterrFail++;
        }
        if (sender.ToString() == "GoogleMobileAds.Api.BannerView")
        {
            isBannFail = true;
            isBanFail++;
        }
    }
    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        if (sender.ToString() == "GoogleMobileAds.Api.InterstitialAd")
        {
            isInterFail = true;
            isInterrFail = thresh + 1;
        }
        if (sender.ToString() == "GoogleMobileAds.Api.BannerView")
        {
            isBannFail = true;
            isBanFail = thresh + 1;
        }
    }
    public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
    {
        //DisplayRewardVideo();
    }
    public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        reloadVid = true;
        isVeloadVid++;
    }
    public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
    {
        print("reward Closed !");
        // RequestVideoAD();
        reloadVid = true;
        isVeloadVid = thresh + 1;
    }
    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        print("reward earned !");
        // reward player !!
        isRewarded = true;
    }
    void HandleBannerAdEvents(bool subs)
    {
        if (subs)
        {
            this.bannerAD.OnAdLoaded += HandleOnAdLoaded;
            this.bannerAD.OnAdFailedToLoad += HandleOnAdFailedToLoad;
            this.bannerAD.OnAdClosed += HandleOnAdClosed;
            this.interstitialAD.OnAdLoaded += HandleOnAdLoaded;
            this.interstitialAD.OnAdFailedToLoad += HandleOnAdFailedToLoad;
            this.interstitialAD.OnAdClosed += HandleOnAdClosed;
        }
        else
        {
            this.bannerAD.OnAdLoaded -= HandleOnAdLoaded;
            this.bannerAD.OnAdFailedToLoad -= HandleOnAdFailedToLoad;
            this.bannerAD.OnAdClosed -= HandleOnAdClosed;
            this.interstitialAD.OnAdLoaded -= HandleOnAdLoaded;
            this.interstitialAD.OnAdFailedToLoad -= HandleOnAdFailedToLoad;
            this.interstitialAD.OnAdClosed -= HandleOnAdClosed;
        }
    }
    void HandleRewardAdEvents(bool subs)
    {
        if (subs)
        {
            this.rewardVideoAD.OnAdLoaded += HandleRewardBasedVideoLoaded;
            this.rewardVideoAD.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
            this.rewardVideoAD.OnAdRewarded += HandleRewardBasedVideoRewarded;
            this.rewardVideoAD.OnAdClosed += HandleRewardBasedVideoClosed;
        }
        else
        {
            this.rewardVideoAD.OnAdLoaded -= HandleRewardBasedVideoLoaded;
            this.rewardVideoAD.OnAdFailedToLoad -= HandleRewardBasedVideoFailedToLoad;
            this.rewardVideoAD.OnAdRewarded -= HandleRewardBasedVideoRewarded;
            this.rewardVideoAD.OnAdClosed -= HandleRewardBasedVideoClosed;
        }

    }
}
