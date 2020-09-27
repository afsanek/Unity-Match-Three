using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GoalPanel : MonoBehaviour
{
    public Image childSprite;
    public Text targetText;
    public GameObject checkMark;

    private void Start()
    {
        checkMark.SetActive(false);
    }
}
