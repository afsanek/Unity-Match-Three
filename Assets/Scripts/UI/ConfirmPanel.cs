using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConfirmPanel : MonoBehaviour
{
    private int level;
    public void SetLevel(int Level)
    {
        level = Level;
    }
    public void OkConfirmPanel()
    {
        PlayerPrefs.SetInt("Current Level", level - 1);
        SceneManager.LoadScene("Main");
    }
    public void CancelConfirmPanel()
    {
        gameObject.SetActive(false);
    }
}
