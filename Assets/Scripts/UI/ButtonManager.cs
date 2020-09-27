using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    private GameData gameData;
    private Board board;
    private void Start()
    {
        gameData = FindObjectOfType<GameData>();
        board = FindObjectOfType<Board>();
    }
    public void Play()
    {
        SceneManager.LoadScene("LevelMap");
    }
    public void Rate()
    {
        //googlelay Rate Page
    }

    public void Setting()
    {
        //choose language
        //Sound Controls
    }
    public void Home()
    {
        SceneManager.LoadScene("StartScene");
    }
    public void BackToLevelWinPanel()
    {
        if(gameData != null)
        {
            if(board != null)
            {
                gameData.dataSaver.isActive[board.level + 1] = true;
                gameData.Save();
            }
        }
        SceneManager.LoadScene("LevelMap");
    }
    public void BackToLevelLosePanel()
    {
        SceneManager.LoadScene("LevelMap");
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
                Application.Quit();
        }
    }

    public void SoundsController()
    {

    }

    public void MusicController()
    {

    }
}
