using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LevelSelectManager : MonoBehaviour
{
    [Header("Page Holder Setting")]
    public GameObject[] panels;
    public GameObject currentPage;
    public int page;
    private int CurrentChildLevel;
    public int CurrentLevel = 0;

    [Header("Page Sprites")]
    public Sprite SelectedPage;
    public Sprite NotSelectedPage;
    public Image[] PageIcon;

    private GameData gameData;
    private MusicController musicController;

    private void Start()
    {
        gameData = FindObjectOfType<GameData>();
        musicController = FindObjectOfType<MusicController>();
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }
        if(gameData != null)
        {
            for (int i = 0; i < gameData.dataSaver.isActive.Length; i++)
            {
                if (gameData.dataSaver.isActive[i])
                {
                    CurrentLevel = i ;
                }
            }
        }
        page = (int)Mathf.Floor(CurrentLevel / 9);
        CurrentChildLevel = (int)(CurrentLevel % 9);
        currentPage = panels[page];
        GameObject CurrentLevelChild = currentPage.transform.GetChild(CurrentChildLevel).gameObject;
        CurrentLevelChild.GetComponent<Animator>().enabled = true;
        panels[page].SetActive(true);
        PageSelectionController();
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            SceneManager.LoadScene("StartScene");
        }
    }
    public void RightClick()
    {
        musicController.PlayClickSound();
        if (page < panels.Length - 1)
        {
            panels[page].SetActive(false);
            page++;
            currentPage = panels[page];
            currentPage.SetActive(true);
            PageSelectionController();
        }
    }
    public void LefttClick()
    {
        musicController.PlayClickSound();
        if (page > 0 )
        {
            panels[page].SetActive(false);
            page--;
            currentPage = panels[page];
            currentPage.SetActive(true);
            PageSelectionController();
        }
    }
    private void PageSelectionController()
    {
        for (int i = 0; i < PageIcon.Length; i++)
        {
            PageIcon[i].sprite = NotSelectedPage;
            if (i == page)
            {
                PageIcon[i].sprite = SelectedPage;
            }
        }
    }
}
