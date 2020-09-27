using System.Collections;
using UnityEngine;

public class DotController : MonoBehaviour
{
    [Header("Board Variables :")]
    public int row;
    public int column;
    public int targetX;
    public int targetY;
    public bool isMatched;

    [Header("Powerup Stuff")]
    public bool isColumnBomb;
    public bool isRowBomb;
    public bool isAdjacentBomb;
    public bool isHammer;
   // public bool isBee;
    public GameObject colorBomb;
    public GameObject AdjacentBomb;
    public GameObject rowBomb;
    public GameObject columnBomb;

    private Vector2 tempPos;
    private Board board;
    private FindMatches findMatches;
    private EndGameManager endGame;
    private Animator anim;

    private void Start()
    {
        endGame = FindObjectOfType<EndGameManager>();
        anim = GetComponent<Animator>();
        findMatches = FindObjectOfType<FindMatches>();
        board = GameObject.FindWithTag("Board").GetComponent<Board>();
        isColumnBomb = false;
        isRowBomb = false;
        isAdjacentBomb = false;
    }
    private void Update()
    {
        targetX = column;
        targetY = row;
        if (Mathf.Abs(targetX - transform.position.x) > .1f)
        {
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPos, .4f);
            if (board.allDots[column, row] != gameObject && !board.EmptySpaces[column,row])
            {
                board.allDots[column, row] = gameObject;
            }
        }
        else
        {
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = tempPos;
        }
        if (Mathf.Abs(targetY - transform.position.y) > .1f)
        {
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPos, .4f);
            if (board.allDots[column, row] != gameObject && !board.EmptySpaces[column, row])
            {
                board.allDots[column, row] = gameObject;
            }
        }
        else
        {
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = tempPos;
        }
        if(this.gameObject.tag == "key")
        {
            if (row == 0)
            {
                board.KeyFound(column);
                Destroy(this.gameObject);
            }
        }
    }
    public void TouchEnter()
    {
        if (board.CurrentState == GameState.move)
        {
            if (!board.selectedItems.Contains(gameObject))
            {
                int listCount = board.selectedItems.Count;
                if (listCount == 0)
                {
                    if (PlayerPrefs.HasKey("Sound"))
                    {
                        if (PlayerPrefs.GetInt("Sound") == 0)
                            board.musicController.Sounds[0].Play();
                    }
                    board.selectedItems.Add(gameObject);
                    anim.SetBool("isSelected", true);
                }
                else
                {
                    if (Mathf.Abs(board.selectedItems[listCount - 1].transform.position.x - transform.position.x) < 1.1f &&
                        Mathf.Abs(board.selectedItems[listCount - 1].transform.position.y - transform.position.y) < 1.1f)
                    {
                        if (board.selectedItems[listCount - 1].tag == gameObject.tag)
                        {
                            if (PlayerPrefs.HasKey("Sound"))
                            {
                                if (PlayerPrefs.GetInt("Sound") == 0)
                                    board.musicController.Sounds[0].Play();
                            }
                            board.selectedItems.Add(gameObject);
                            /*
                            if(isAdjacentBomb || isRowBomb || isColumnBomb)
                            {

                            }
                            if (board.selectedItems.Count == 7)
                            {
                                Instantiate(board.GlowingDot, transform.position, Quaternion.identity).transform.parent = transform;
                            }
                            if (board.selectedItems.Count >= 9)
                            {
                                var temp = Instantiate(board.AdjacentDot, transform.position, Quaternion.identity);
                                temp.transform.parent = transform;
                                isAdjacentEffect = true;
                                temp.GetComponent<Animator>().SetInteger("adjacentNum", (board.selectedItems.Count - 8));
                                for (int i = 8; i < board.selectedItems.Count - 2; i++)
                                {
                                    board.selectedItems[i].GetComponent<DotController>().isAdjacentEffect = false;
                                }
                            }
                            */
                            anim.SetBool("isSelected", true);
                            board.DrawLineBetweenFruits();
                        }
                    }
                }
            }
            else
            {
                if (board.selectedItems.Count > 1)
                {
                    int index = 0;
                    for (int i = 0; i < board.selectedItems.Count; i++)
                    {
                        if (board.selectedItems[i] == gameObject)
                        {
                            index = i;
                            break;
                        }
                    }
                    index++;
                    for(int i = index; i < board.selectedItems.Count; i++)
                    {
                        board.selectedItems[i].GetComponent<Animator>().SetBool("isSelected", false);
                    }
                    board.selectedItems.RemoveRange(index, (board.selectedItems.Count - index));
                    board.DrawLineBetweenFruits();
                }
            }
        }
    }
    public void TouchExit()
    {
        if (board.selectedItems.Count > 2)
        {
            for (int i = 0; i < board.selectedItems.Count; i++)
            {
                board.selectedItems[i].GetComponent<DotController>().isMatched = true;
                board.selectedItems[i].GetComponent<Animator>().SetBool("isSelected", false);
            }
            findMatches.SpeedUp();
            if (endGame != null)
            {
                if (endGame.requirements.gameType == GameType.Moves)
                {
                    endGame.DecreaseCounterValue();
                }
            }
            board.DestroyMatches();
        }
        board.selectedItems.Clear();
        board.DrawLineBetweenFruits();
    }
    public void MakeRowBomb()
    {
        isRowBomb = true;
        Instantiate(rowBomb, transform.position, Quaternion.identity).transform.parent = transform;
    }
    public void MakeColumnBomb()
    {
        isColumnBomb = true;
        Instantiate(columnBomb, transform.position, Quaternion.identity).transform.parent = transform;
    }
    public void MakeAdjacentBomb()
    {
        isAdjacentBomb = true;
        Instantiate(AdjacentBomb, transform.position, Quaternion.identity).transform.parent = transform;
    }
    public void IsHammer()
    {
        if (board.CurrentState == GameState.move)
        {
            isMatched = true;
            findMatches.GetAdjacentDots(column, row);
            board.DestroyMatches();
        }
    }
}
