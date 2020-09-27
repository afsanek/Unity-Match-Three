using UnityEngine;
using System.Collections.Generic;

public class FindMatches : MonoBehaviour
{
    [Range(0,.5f)]
    public float FindMatchDelay = .3f;
    public GameData SelectedTile;

    private Board board;
    private List<GameObject> TempFruits;
    void Start()
    {
        board = GameObject.FindWithTag("Board").GetComponent<Board>();
    }
    public void SpeedUp()
    {
        for (int i = 0; i < board.selectedItems.Count; i++)
        {
            DotController Fruit = board.selectedItems[i].GetComponent<DotController>();
            #region test
            /*
            DotController lastDot = board.selectedItems[board.selectedItems.Count-1].GetComponent<DotController>();
            if (dot.isAdjacentBomb)
            {
                if (!lastDot.isAdjacentBomb)
                {
                    lastDot.isAdjacentBomb = true;
                    dot.isAdjacentBomb = false;
                    GetAdjacentDots(lastDot.column, lastDot.row);
                }
                else
                {
                    GetAdjacentDots(dot.column, dot.row);
                }
            }
            if (dot.isColumnBomb)
            {
                
                if (!lastDot.isColumnBomb)
                {
                    lastDot.isColumnBomb = true;
                    GetColumnDots(lastDot.column);
                }
                else
                {
                    GetColumnDots(dot.column);
                }
            }
            else if (dot.isColumnBomb)
            {
                if (!lastDot.isRowBomb)
                {
                    lastDot.isRowBomb = true;
                    GetRowDots(lastDot.row);
                }
                else
                {
                    GetRowDots(dot.row);
                }
            }
            else if (dot.isColumnBomb && lastDot.isColumnBomb && lastDot.isRowBomb )
            {
                GetColumnDots(dot.row);
            }
            if(dot.isRowBomb && !lastDot.isRowBomb )
            {
                lastDot.isRowBomb = true;
                GetRowDots(lastDot.row);
            }else if(dot.isRowBomb && !lastDot.isColumnBomb)
            {
                lastDot.isColumnBomb = true;
                GetColumnDots(lastDot.column);
            }
            else if (dot.isRowBomb && lastDot.isColumnBomb && lastDot.isRowBomb)
            {
                GetRowDots(dot.row);
            }
            */
            #endregion
            if (Fruit.isAdjacentBomb)
            {
                GetAdjacentDots(Fruit.column, Fruit.row);
            }
            if (Fruit.isColumnBomb)
            {
                GetColumnDots(Fruit.column);
            }
            if (Fruit.isRowBomb)
            {
                GetRowDots(Fruit.row);
            }
        }

    }
    public void GetAdjacentDots(int column,int row)
    {
        for (int i = column - 1; i <= column + 1; i++) 
        {
            for (int j = row - 1; j <= row + 1; j++) 
            {
                if (i >= 0 && i < board.Width && j >= 0 && j < board.Height)
                {
                    if (!board.blankSpaces[i, j]
                        && !board.EmptySpaces[i, j] 
                        && board.allDots[i, j] != null)
                    {
                        if (board.allDots[i, j].tag != "key")
                        {
                            DotController temp = board.allDots[i, j].GetComponent<DotController>();
                            if (temp != null)
                            {
                                board.allDots[i, j].GetComponent<DotController>().isMatched = true;
                            }
                        }
                    }
                    if (board.HardTiles[i, j] != null)
                    {
                        board.HardTiles[i, j].TakeDamage(1);
                    }
                }
            }
        }
    }
    public void GetFourAdjacentDots(int column, int row)
    {
        if (row + 1 <= board.Height - 1)
        {
            if (!board.blankSpaces[column, row + 1] &&
                !board.EmptySpaces[column, row + 1])
            {
                if (board.HardTiles[column, row + 1] != null)
                {
                    board.HardTiles[column, row + 1].GetComponent<HardThingsController>().TakeDamage(1);
                }
            }
        }
        if (row - 1 >= 0)
        {
            if (!board.blankSpaces[column, row - 1] &&
                !board.EmptySpaces[column, row - 1])
            {
                if (board.HardTiles[column, row - 1] != null)
                {
                    board.HardTiles[column, row - 1].GetComponent<HardThingsController>().TakeDamage(1);
                }
            }
        }
        if (column + 1 <= board.Width - 1)
        {
            if (!board.blankSpaces[column + 1, row] &&
                !board.EmptySpaces[column + 1, row])
            {
                if (board.HardTiles[column + 1, row] != null)
                {
                    board.HardTiles[column + 1, row].GetComponent<HardThingsController>().TakeDamage(1);
                }
            }
        }
        if (column - 1 >= 0)
        {
            if (!board.blankSpaces[column - 1, row] &&
                !board.EmptySpaces[column - 1, row])
            {
                if (board.HardTiles[column - 1, row] != null)
                {
                    board.HardTiles[column - 1, row].GetComponent<HardThingsController>().TakeDamage(1);
                }
            }
        }
    }
    private void GetColumnDots(int column)
    {
        for (int i = 0; i < board.Height; i++)
        {
            if (board.allDots[column, i] != null && board.allDots[column,i].tag != "key")
            {
                board.allDots[column, i].GetComponent<DotController>().isMatched = true;
                /*
                if (board.allDots[column, i].GetComponent<DotController>().isAdjacentBomb)
                {
                    GetAdjacentDots(board.allDots[column, i].GetComponent<DotController>().column, board.allDots[column, i].GetComponent<DotController>().row);
                }
                if (board.allDots[column, i].GetComponent<DotController>().isRowBomb || board.allDots[column, i].GetComponent<DotController>().isColumnBomb)
                {
                    GetRowDots(board.allDots[column, i].GetComponent<DotController>().row);
                }
                */
            }
        }
    }
    private void GetRowDots(int row)
    {
        for (int i = 0; i < board.Width; i++)
        {
            if (board.allDots[i,row] != null && board.allDots[i, row].tag != "key")
            {
                board.allDots[i, row].GetComponent<DotController>().isMatched = true;

                /*
                if (board.allDots[i, row].GetComponent<DotController>().isAdjacentBomb)
                {
                    GetAdjacentDots(board.allDots[i, row].GetComponent<DotController>().column, board.allDots[i, row].GetComponent<DotController>().row);
                }
                if (board.allDots[i, row].GetComponent<DotController>().isRowBomb || board.allDots[i, row].GetComponent<DotController>().isColumnBomb)
                {
                    GetColumnDots(board.allDots[i, row].GetComponent<DotController>().column);
                }
                */
            }
        }
    }
    public void BombChecker()
    {
        if (board.currentDot != null)
        {
            if (board.currentDot.isMatched)
            {
                board.currentDot.isMatched = false;
                int randBomb = Random.Range(0, 100);
                if (!board.currentDot.isRowBomb && !board.currentDot.isColumnBomb)
                {
                    if (randBomb >=0 && randBomb <50)
                    {
                        board.currentDot.MakeColumnBomb();
                    }
                    else if(randBomb >=50 && randBomb <=100)
                    {
                        board.currentDot.MakeRowBomb();
                    }
                }
            }
        }
    }
}
