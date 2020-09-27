using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardThingsController : MonoBehaviour
{
    public int hitPoint;
    private GoalManager goalManager;
    private SpriteRenderer spriteRenderer;
    public GameObject[] hits;
    public int col;
    public int row;
    private Board board;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        goalManager = GameObject.FindWithTag("GoalManager").GetComponent<GoalManager>();
        board = GameObject.FindWithTag("Board").GetComponent<Board>();
    }
    void Update()
    {
        if (hitPoint <= 0)
        {
            if (goalManager != null)
            {
                goalManager.CompareGoal(gameObject.tag);
                goalManager.UpdateTargets();
            }
            board.HardTiles[col, row] = null;
            board.HardTileFiller(col, row);
            Destroy(this.gameObject);
        }
    }
    public void SetHitPoint(int hitpoint,int i,int j)
    {
        if (hitpoint == 0)
        {
            hitpoint = 1;
        }
        hitPoint = hitpoint;
        col = i;
        row = j;
        SetSprite();
    }
    public void TakeDamage(int damage)
    {
        hitPoint -= damage;
        //if (hitPoint < 0)
       // {
            if (goalManager != null)
            {
                goalManager.CompareGoal(gameObject.tag);
                goalManager.UpdateTargets();
            }
       // }
        SetSprite();
    }
    private void SetSprite()
    {
        for (int i = 0; i < hits.Length; i++)
        {
            if (i == hitPoint)
            {
                hits[i].SetActive(true);
            }
            else
            {
                hits[i].SetActive(false);
            }
        }
    }
}
