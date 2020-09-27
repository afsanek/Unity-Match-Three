using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move,
    lose,
    win,
    pause
}
public enum TileType
{
    Breakable,
    Blank,
    Basket,
    Rock,
    Wood
}
[Serializable]
public class TileDef
{
   public int x;
   public int y;
   public TileType tileType;
   [Header("0 for blank and basket !\n")]
   public int hitPoint = 0;
}
public class Board : MonoBehaviour
{
    [Header("Scriptable Objects")]
    public World world;
    public int level;

    [Header("Board Informations")]
    public int Width;
    public int Height;
    public int offset;

    [Header("In-Game Delays")]
    [Range(0, 1)]
    public float DelayTime = 0.5f;
    [Range(0, 2)]
    public float DelayTimeForParticles = 0.6f;

    public DotController currentDot;
    public TileDef[] BoardTiles;
    public List<GameObject> selectedItems;
    public GameObject[,] allDots;
    public bool[,] blankSpaces;
    public bool[,] EmptySpaces;
    private BackgroundTile[,] breakableTiles;
    public HardThingsController[,] HardTiles;
    public GameObject[,] basketTiles;
    private GameObject[,] BoxTiles;

    [Header("Prefabs")]
    public GameObject[] destroyEffect; //each friut destroy effect
    public GameObject RCdestroyEffect; // rpwcolumn Destroy effect
    public GameObject BackgroundTile; // blackbg
    public GameObject Background2Tile; //white bg
    public GameObject[] Dots; // fruits
    public GameObject BreakableTilePrefab;
    public GameObject BasketPrefab;
    public GameObject BoxPrefab;
    public GameObject GlowingDot;
    public GameObject AdjacentDot;
    public GameObject KeyPrefab;
    public GameObject RockPrefab;
    public GameObject WoodPrefab;
        
    public GameState CurrentState = GameState.move;
    public const int maxIterations = 4;
    private FindMatches findMatches;
    private LineRenderer lineRenderer;

    [Header("Goal Info")]
    public List<string> targetTags = new List<string>();
    private int dotScore = 25;
    public int[] ScoreTargets;
    public int[] KeyRoom;
    public int KeyNeeded;
    public int tmpKeyNeeded;
    public int KeyCollected;
    private bool pauseOff;
    private ScoreManager scoreManager;
    private GoalManager goalManager;
    private AnimationAndButtonController UiAnimCo;
    public MusicController musicController;

    private void Awake()
    {
        if(PlayerPrefs.HasKey("Current Level"))
        {
            level = PlayerPrefs.GetInt("Current Level");
        }
        if (world != null)
        {
            if (level < world.levels.Length)
            {
                if (world.levels[level] != null)
                {
                    Width = world.levels[level].Width;
                    Height = world.levels[level].Height;
                    Dots = world.levels[level].Dots;
                    BoardTiles = world.levels[level].BoardLayout;
                    ScoreTargets = world.levels[level].scoreTarget;
                    KeyRoom = world.levels[level].KeyRoom;
                    for (int i = 0; i < world.levels[level].levelTargets.Length; i++)
                    {
                        world.levels[level].levelTargets[i].NumberOfCollected = 0;
                        targetTags.Add(world.levels[level].levelTargets[i].TargetTag);
                        if(world.levels[level].levelTargets[i].TargetTag == "key")
                        {
                            KeyNeeded = world.levels[level].levelTargets[i].NumberOfNeeded;
                            tmpKeyNeeded = KeyNeeded;
                        }
                    }
                }
            }
        }
    }
    void Start()
    {
        selectedItems = new List<GameObject>();
        lineRenderer = GetComponent<LineRenderer>();
        findMatches = FindObjectOfType<FindMatches>();
        scoreManager = GameObject.FindWithTag("ScoreManager").GetComponent<ScoreManager>();
        musicController = FindObjectOfType<MusicController>();
        goalManager = GameObject.FindWithTag("GoalManager").GetComponent<GoalManager>();
        UiAnimCo = GameObject.FindWithTag("uiandanimcontroller").GetComponent<AnimationAndButtonController>();
        blankSpaces = new bool[Width, Height];
        EmptySpaces = new bool[Width, Height];
        allDots = new GameObject[Width, Height];
        breakableTiles = new BackgroundTile[Width, Height];
        HardTiles = new HardThingsController[Width, Height];
        basketTiles = new GameObject[Width, Height];
        BoxTiles = new GameObject[Width, Height];
        CurrentState = GameState.pause;
        
        SettingTiles();

        int MaxItra = 0;
        while (BoardFailure() && MaxItra < 2)
        {
            DeadLockResolverShuffle();
            MaxItra++;
        }
        if (MaxItra > 1 && BoardFailure())
        {
            DeadLockResolverDec();
        }
        MaxItra = 0;
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            uiManag.instance.RewardPanelIn();
        }
        else
        {
          UiAnimCo.TargetStarterCo();
        }
    }
    public void GenerateBlankTiles()
    {
        for (int i = 0; i < BoardTiles.Length; i++)
        {
            if (BoardTiles[i].tileType == TileType.Blank)
            {
                blankSpaces[BoardTiles[i].x, BoardTiles[i].y] = true;
            }
        }
    }
    public void GenerateBreakableTiles()
    {
        for (int i = 0; i < BoardTiles.Length; i++)
        {
            if (BoardTiles[i].tileType == TileType.Breakable)
            {
                var temp = Instantiate(BreakableTilePrefab, new Vector2(BoardTiles[i].x, BoardTiles[i].y), Quaternion.identity);
                breakableTiles[BoardTiles[i].x, BoardTiles[i].y] = temp.GetComponent<BackgroundTile>();
                breakableTiles[BoardTiles[i].x, BoardTiles[i].y].SetHitPoint(BoardTiles[i].hitPoint);
            }
        }
    }
    public void GenerateBaskets()
    {
        for (int i = 0; i < BoardTiles.Length; i++)
        {
            if (BoardTiles[i].tileType == TileType.Basket)
            {
                if (basketTiles[BoardTiles[i].x, BoardTiles[i].y] == null)
                {
                    var temp = Instantiate(BasketPrefab, new Vector2(BoardTiles[i].x, BoardTiles[i].y), Quaternion.identity);
                    basketTiles[BoardTiles[i].x, BoardTiles[i].y] = temp;
                }
            }
        }
    }
    /*public void GenerateBoxes()
    {
        for (int i = 0; i < BoardTiles.Length; i++)
        {
            if (BoardTiles[i].tileType == TileType.Box)
            {
                var temp = Instantiate(BoxPrefab, new Vector2(BoardTiles[i].x, BoardTiles[i].y), Quaternion.identity);
                BoxTiles[BoardTiles[i].x, BoardTiles[i].y] = temp;
            }
        }
    }*/
    public void GenerateKey()
    {
        for(int i = 0; i < KeyRoom.Length; i++)
        {
            if (allDots[KeyRoom[i], Height - 1] == null && basketTiles[KeyRoom[i], Height - 1] == null)
            {
                if (KeyNeeded > 0)
                {
                    var tmp = Instantiate(KeyPrefab, new Vector3(KeyRoom[i], (Height - 1) + offset, 0), Quaternion.identity);
                    tmp.name = "Key in" + KeyRoom[i];
                    tmp.transform.parent = this.transform;
                    KeyNeeded--;
                    allDots[KeyRoom[i], Height - 1] = tmp;
                    tmp.GetComponent<DotController>().row = Height - 1;
                    tmp.GetComponent<DotController>().column = KeyRoom[i];
                }
            }
        }
    }
    public void GenerateHardTiles()
    {
        for (int i = 0; i < BoardTiles.Length; i++)
        {
            if (BoardTiles[i].tileType == TileType.Rock)
            {
                if (HardTiles[BoardTiles[i].x, BoardTiles[i].y] == null)
                {
                    var temp = Instantiate(RockPrefab, new Vector2(BoardTiles[i].x, BoardTiles[i].y), Quaternion.identity);
                    HardTiles[BoardTiles[i].x, BoardTiles[i].y] = temp.GetComponent<HardThingsController>();
                    HardTiles[BoardTiles[i].x, BoardTiles[i].y].SetHitPoint(BoardTiles[i].hitPoint, BoardTiles[i].x, BoardTiles[i].y);
                }
            }
            if (BoardTiles[i].tileType == TileType.Wood)
            {
                if (HardTiles[BoardTiles[i].x, BoardTiles[i].y] == null)
                {
                    var temp = Instantiate(WoodPrefab, new Vector2(BoardTiles[i].x, BoardTiles[i].y), Quaternion.identity);
                    HardTiles[BoardTiles[i].x, BoardTiles[i].y] = temp.GetComponent<HardThingsController>();
                    HardTiles[BoardTiles[i].x, BoardTiles[i].y].SetHitPoint(BoardTiles[i].hitPoint, BoardTiles[i].x, BoardTiles[i].y);
                }
            }
        }
    }
    private void SettingTiles()
    {
        GenerateHardTiles();
        //GenerateBoxes();
        GenerateBaskets();
        GenerateBlankTiles();
        GenerateBreakableTiles();
        var tempBackground = false;
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                if (!blankSpaces[i, j])
                {
                    var tempPos = new Vector2(i, j);
                    if (tempBackground)
                    {
                        var tile = Instantiate(BackgroundTile, tempPos, Quaternion.identity);
                        tempBackground = !tempBackground;
                        tile.name = "( " + i + " , " + j + " )";
                        tile.transform.parent = this.transform;
                    }
                    else
                    {
                        var tile = Instantiate(Background2Tile, tempPos, Quaternion.identity);
                        tempBackground = !tempBackground;
                        tile.name = "( " + i + " , " + j + " )";
                        tile.transform.parent = this.transform;
                    }
                    tempPos = new Vector2(i, j + offset);
                    var tempKey = false;
                    for (int s = 0; s < KeyRoom.Length; s++)
                    {
                        if (KeyRoom[s] == i && j == Height - 1)
                        {
                            tempKey = true;
                        }
                    }
                    if (!tempKey && HardTiles[i,j] == null)
                    {
                        GenerateRandomDot(i, j, 2, tempPos);
                    }
                    if(HardTiles[i,j] != null)
                    {
                        HardTileEmptykn(i, j);
                    }
                }
            }
            tempBackground = !tempBackground;
        }
        GenerateKey();
    }
    private void HardTileEmptykn(int column,int row)
    {
        for (int j = row -1 ; j >= 0 ; j--)
        {
            if (HardTiles[column, j] == null && !blankSpaces[column, j])
            {
                Destroy(allDots[column, j]);
                allDots[column, j] = null;
                EmptySpaces[column, j] = true;
            }
        }
    }
    public void HardTileFiller(int column, int row)
    {
        for (int j = Height - 1; j >= 0; j--)
        {
            if(HardTiles[column,j] != null)
            {
                HardTileEmptykn(column, j);
                break;
            }
            if (!blankSpaces[column, j] && EmptySpaces[column,j] == true)
            {
                allDots[column, j] = null;
                EmptySpaces[column, j] = false;
            }
        }
        allDots[column, row] = null;
        DecreaseRows();
    }
    private void RefillBoard()
    {
        if ((tmpKeyNeeded - KeyNeeded) == KeyCollected && KeyNeeded > 0)
        {
            GenerateKey();
        }
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                if (allDots[i, j] == null && !blankSpaces[i, j] && !EmptySpaces[i,j] && HardTiles[i,j] == null)
                {
                    var tempPos = new Vector2(i, j + offset);
                    GenerateRandomDot(i, j, 2, tempPos);
                }
            }
        }
    }
    private void GenerateRandomDot(int i, int j, int maxIterations, Vector2 tempPos)
    {
        int randomDot;
        int maxItr = 0;
        do
        {
            randomDot = UnityEngine.Random.Range(0, Dots.Length);
            maxItr++;
        } while (!DotPossibility(i, j, Dots[randomDot]) && maxItr < maxIterations);
        maxItr = 0;

        var dot = Instantiate(Dots[randomDot], tempPos, Quaternion.identity);
        dot.GetComponent<DotController>().row = j;
        dot.GetComponent<DotController>().column = i;
        dot.transform.parent = this.transform;
        dot.name = "Candy " + "( " + i + " , " + j + " )";
        allDots[i, j] = dot;
    }
    private bool DotPossibility(int column, int row, GameObject dot)
    {
       
        if (column > 1 && row > 1)
        {
            if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
            {
                if (allDots[column - 1, row].tag == dot.tag && allDots[column - 2, row].tag == dot.tag)
                {
                    return true;
                }
            }
            if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
            {
                if (allDots[column, row - 1].tag == dot.tag && allDots[column, row - 2].tag == dot.tag)
                {
                    return true;
                }
            }
        }
        else if (dot != null)
        {
            if (targetTags.Contains(dot.tag))
            {
                return true;
            }
        }
        if (row < 2 && column > 1)
        {
            if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
            {
                if (allDots[column - 1, row].tag == dot.tag && allDots[column - 2, row].tag == dot.tag)
                {
                    return true;
                }
            }
        }
        else if (column < 2 && row > 1)
        {
            if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
            {
                if (allDots[column, row - 1].tag == dot.tag && allDots[column, row - 2].tag == dot.tag)
                {
                    return true;
                }
            }
        }
        else if (dot != null)
        {
            if (targetTags.Contains(dot.tag))
            {
                if (UnityEngine.Random.value > 0.6f)
                    return true;
                else
                    return false;
            }
        }
        return false;
    }
    private void DestroyMatchedDot(int column, int row)
    {
       // print("column va row to destroy : " + column + " " + row);
      //  print("tagesh : " + allDots[column, row].tag);
        var tmp = allDots[column, row].GetComponent<DotController>();
        if (tmp.isMatched)
        {
            if (selectedItems.Count > 6)
            {
                currentDot = selectedItems[selectedItems.Count - 1].GetComponent<DotController>();
                MakeBomb();
            }
            if (breakableTiles[column, row] != null && basketTiles[column, row] == null)
            {
                if (PlayerPrefs.HasKey("Sound"))
                {
                    if (PlayerPrefs.GetInt("Sound") == 0)
                        musicController.Sounds[5].Play();
                }
                breakableTiles[column, row].TakeDamage(1);
                if (goalManager != null)
                {
                    goalManager.CompareGoal("ice");
                    goalManager.UpdateTargets();
                }
                if (breakableTiles[column, row].hitPoint <= 0)
                {
                    breakableTiles[column, row] = null;
                }
            }
            if (tmp != currentDot && basketTiles[column,row] == null && allDots[column,row] != null)
            {
                if (goalManager != null)
                {
                        goalManager.CompareGoal(allDots[column, row].tag);
                        goalManager.UpdateTargets();
                }
                if (tmp.isColumnBomb)
                {
                    Destroy(Instantiate(RCdestroyEffect, allDots[column, row].transform.position, Quaternion.Euler(0, 0, 90)), DelayTimeForParticles);
                }
                else if (tmp.isRowBomb)
                {
                    Destroy(Instantiate(RCdestroyEffect, allDots[column, row].transform.position, Quaternion.identity), DelayTimeForParticles);
                }
                else
                {
                    if (allDots[column, row].tag == "banana" || allDots[column, row].tag == "peach")
                    {
                        Destroy(Instantiate(destroyEffect[4], allDots[column, row].transform.position, Quaternion.identity), DelayTimeForParticles);
                    }
                    else if (allDots[column, row].tag == "strawberry" || allDots[column, row].tag == "graps")
                    {
                        Destroy(Instantiate(destroyEffect[3], allDots[column, row].transform.position, Quaternion.identity), DelayTimeForParticles);
                    }
                    else if (allDots[column, row].tag == "watermelon" || allDots[column, row].tag == "pear")
                    {
                        Destroy(Instantiate(destroyEffect[1], allDots[column, row].transform.position, Quaternion.identity), DelayTimeForParticles);
                    }
                    else if (allDots[column, row].tag == "orange ")
                    {
                        Destroy(Instantiate(destroyEffect[2], allDots[column, row].transform.position, Quaternion.identity), DelayTimeForParticles);
                    }
                    else
                    {
                        Destroy(Instantiate(destroyEffect[0], allDots[column, row].transform.position, Quaternion.identity), DelayTimeForParticles);
                    }
                }
                if (PlayerPrefs.HasKey("Sound"))
                {
                    if (PlayerPrefs.GetInt("Sound") == 0)
                        musicController.Sounds[4].Play();
                }
                Destroy(allDots[column, row]);
                scoreManager.IncreaseScore(dotScore);
                allDots[column, row] = null;
                findMatches.GetFourAdjacentDots(column, row);
            }
            if (basketTiles[column, row] != null)
            {
                tmp.isMatched = false;
                if (goalManager != null)
                {
                    goalManager.CompareGoal("basket");
                    goalManager.UpdateTargets();
                }
                Destroy(basketTiles[column, row]);
                basketTiles[column, row] = null;
            }
        }
    }
    private void MakeBomb()
    {
        if (selectedItems.Count == 7)
        {
            findMatches.BombChecker();
        }
        if (selectedItems.Count > 7)
        {
            if (currentDot != null)
            {
                if (currentDot.isMatched)
                {
                    if (!currentDot.isAdjacentBomb)
                    {
                        currentDot.isMatched = false;
                        currentDot.MakeAdjacentBomb();
                    }
                }
            }
        }
    }
    public void DestroyMatches()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                if (allDots[i, j] != null && !blankSpaces[i,j]
                    && HardTiles[i,j] == null)
                {
                    DestroyMatchedDot(i, j);
                }
            }
        }
        DecreaseRows();
    }
    public void DecreaseRows()
    {
        PreDec();
        StartCoroutine(FillBoard());
    }
    public void KeyFound(int i)
    {
        KeyCollected++;
        if (goalManager != null)
        {
            goalManager.CompareGoal("key");
            goalManager.UpdateTargets();
        }
       // Destroy(allDots[KeyRoom[i], 0]);
        allDots[i, 0] = null;
        DecreaseRows();
    }
    private void PreDec()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                if (allDots[i, j] == null && !blankSpaces[i, j] && !EmptySpaces[i, j] && HardTiles[i, j] == null)
                {
                    for (int k = j + 1; k < Height; k++)
                    {
                        if (allDots[i, k] != null)
                        {
                            if (allDots[i, k].tag == "key")
                            {
                                var tmpH = false;
                                for (int d = k - 1; d > j; d--)
                                {
                                    if (d >= 0 && d < Height)
                                    {
                                        if (HardTiles[i, d] != null)
                                        {
                                            tmpH = true;
                                        }
                                    }
                                }
                                if (basketTiles[i, j] == null && !tmpH)
                                {
                                    allDots[i, k].GetComponent<DotController>().row = j;
                                    allDots[i, k] = null;
                                    break;
                                }
                            }else
                            {
                                allDots[i, k].GetComponent<DotController>().row = j;
                                allDots[i, k] = null;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
    private IEnumerator FillBoard()
    {
        if (CurrentState != GameState.lose && CurrentState != GameState.win)
        {
            if (CurrentState != GameState.pause)
            {
                CurrentState = GameState.wait;
            }
            yield return new WaitForSeconds(DelayTime);
            RefillBoard();
            int MaxItra = 0;
            while (BoardFailure() && MaxItra < 3)
            {
                DeadLockResolverShuffle();
                MaxItra++;
            }
            if(MaxItra > 2 && BoardFailure())
            {
                DeadLockResolverDec();
            }
            MaxItra = 0;
            yield return new WaitForSeconds(DelayTime);
            currentDot = null;
            if (CurrentState != GameState.pause)
            {
                CurrentState = GameState.move;
            }
        }
    }
    private void Update()
    {
        if (!goalManager.HammerOn)
        {
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Moved)
                {
                    Vector3 tempTouchPos = Camera.main.ScreenToWorldPoint(touch.position);
                    Vector2 touchPosition = new Vector2(tempTouchPos.x, tempTouchPos.y);
                    RaycastHit2D hitInfo = Physics2D.Raycast(touchPosition, Camera.main.transform.forward);
                    if (hitInfo.collider != null)
                    {
                        hitInfo.transform.gameObject.GetComponent<DotController>().TouchEnter();
                    }
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    if (selectedItems.Count <= 2)
                    {
                        for (int i = 0; i < selectedItems.Count; i++)
                        {
                            selectedItems[i].GetComponent<Animator>().SetBool("isSelected", false);
                        }
                        selectedItems.Clear();
                        DrawLineBetweenFruits();
                    }
                    else
                    {
                        selectedItems[selectedItems.Count - 1].transform.gameObject.GetComponent<DotController>().TouchExit();
                    }
                }
            }
        }
        if(goalManager.HammerOn)
        {
            if(goalManager.NumberofRandHammer > 0)
            {
                if (Input.touchCount > 0)
                {
                    var touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Began)
                    {
                        Vector3 tempTouchPos = Camera.main.ScreenToWorldPoint(touch.position);
                        Vector2 touchPosition = new Vector2(tempTouchPos.x, tempTouchPos.y);
                        RaycastHit2D hitInfo = Physics2D.Raycast(touchPosition, Camera.main.transform.forward);
                        if (hitInfo.collider != null)
                        {
                            if (PlayerPrefs.HasKey("Sound"))
                            {
                                if (PlayerPrefs.GetInt("Sound") == 0)
                                    musicController.Sounds[0].Play();
                            }
                            var tmpHit = hitInfo.transform.gameObject.GetComponent<DotController>();
                            var tmpHammer = goalManager.tmpHammer;
                            tmpHammer[goalManager.NumberofRandHammer-1].transform.position = new Vector3(tmpHit.column + 1, tmpHit.row + 1, 0);
                            tmpHammer[goalManager.NumberofRandHammer - 1].GetComponent<Animator>().SetBool("HammerOn", true);
                            Destroy(tmpHammer[goalManager.NumberofRandHammer - 1], .5f);
                            goalManager.tmpHammer[goalManager.NumberofRandHammer - 1] = null;
                            tmpHit.IsHammer();
                            if (goalManager.NumberofRandHammer <= 1)
                            {
                                goalManager.HammerPanelIn.SetBool("hammerIn", false);
                                goalManager.HammerOn = false;
                            }
                            else
                            {
                                goalManager.NumberofRandHammer--;
                                goalManager.UpdateHammerText();
                            }
                        }
                    }
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (!pauseOff)
            {
                PauseButton();
            }
            else
            {
                UiAnimCo.PauseButtonDown();
            }
            pauseOff = !pauseOff;
        }
    }
    public void PauseButton()
    {
        
        CurrentState = GameState.pause;
        UiAnimCo.PauseButton();
    }
    public void DrawLineBetweenFruits()
    {
        Vector3[] tempPos = new Vector3[selectedItems.Count];
        for (int i = 0; i < selectedItems.Count; i++)
        {
            tempPos[i].x = selectedItems[i].transform.position.x;
            tempPos[i].y = selectedItems[i].transform.position.y;
            tempPos[i].z = 0f;
        }
        lineRenderer.positionCount = selectedItems.Count;
        lineRenderer.SetPositions(tempPos);
    }

    #region Check Failure
    private bool BoardFailure()
    {
        //CurrentState = GameState.wait;
        for (int i = 0; i < Width - 2; i++)
        {
            for (int j = 0; j < Height - 2; j++)
            {
                if (allDots[i, j] != null)
                {
                    if (allDots[i + 1, j + 1] != null && allDots[i, j + 1] != null)
                    {
                        if (allDots[i + 1, j + 1].tag == allDots[i, j].tag && allDots[i, j + 1].tag == allDots[i, j].tag)
                        {

                            return false;
                        }
                    }
                    if (allDots[i, j + 1] != null && allDots[i + 1, j] != null)
                    {
                        if (allDots[i, j + 1].tag == allDots[i, j].tag && allDots[i + 1, j].tag == allDots[i, j].tag)
                        {
                            return false;
                        }
                    }
                    if (allDots[i + 1, j + 1] != null && allDots[i + 1, j] != null)
                    {
                        if (allDots[i + 1, j + 1].tag == allDots[i, j].tag && allDots[i + 1, j].tag == allDots[i, j].tag)
                        {
                            return false;
                        }
                    }
                    if (j > 0)
                    {
                        if (allDots[i + 1, j - 1] != null)
                        {
                            if (allDots[i + 1, j - 1].tag == allDots[i, j].tag)
                            {
                                if (CheckDots(i + 1, j - 1, i, j))
                                {
                                    return false;
                                }
                            }
                        }
                    }
                    if (allDots[i + 1, j] != null)
                    {
                        if (allDots[i + 1, j].tag == allDots[i, j].tag)
                        {
                            if (CheckDots(i + 1, j, i, j))
                            {
                                return false;
                            }
                        }
                    }
                    if (allDots[i, j + 1] != null)
                    {
                        if (allDots[i, j + 1].tag == allDots[i, j].tag)
                        {
                            if (CheckDots(i, j + 1, i, j))
                            {
                                return false;
                            }
                        }
                    }
                    if (allDots[i + 1, j + 1] != null)
                    {
                        if (allDots[i + 1, j + 1].tag == allDots[i, j].tag)
                        {
                            if (CheckDots(i + 1, j + 1, i, j))
                            {
                                return false;
                            }
                        }
                    }
                }
            }
        }
        for (int i = 0; i < Width - 1; i++)
        {
            for (int j = Height - 1; j > Height - 3; j--)
            {
                //print("( " + i + "," + j + ")");
                if (allDots[i, j] != null)
                {
                    if (allDots[i, j - 1] != null && allDots[i + 1, j] != null)
                    {
                        if (allDots[i, j - 1].tag == allDots[i, j].tag && allDots[i, j].tag == allDots[i + 1, j].tag)
                        {
                            return false;
                        }
                    }
                    if (allDots[i + 1, j] != null && allDots[i + 1, j - 1] != null)
                    {
                        if (allDots[i, j].tag == allDots[i + 1, j].tag && allDots[i, j].tag == allDots[i + 1, j - 1].tag)
                        {
                            return false;
                        }
                    }
                    if (allDots[i + 1, j - 1] != null && allDots[i, j - 1] != null)
                    {
                        if (allDots[i, j].tag == allDots[i + 1, j - 1].tag && allDots[i, j].tag == allDots[i, j - 1].tag)
                        {
                            return false;
                        }
                    }
                    if (i > 0)
                    {
                        if (allDots[i - 1, j - 1] != null)
                        {
                            /*if (allDots[i, j - 1] != null)
                            {
                                print("10");
                                if (allDots[i - 1, j - 1].tag == allDots[i, j].tag && allDots[i, j].tag == allDots[i, j - 1].tag)
                                {
                                    return false;
                                }
                            }
                            if (allDots[i + 1, j] != null)
                            {
                                print("11");
                                if (allDots[i+1,j].tag == allDots[i,j].tag && allDots[i-1,j-1].tag == allDots[i, j].tag)
                                {
                                    return false; ;
                                }
                            }
                            if (allDots[i + 1, j - 1] != null)
                            {
                                print("12");
                                if (allDots[i+1,j-1].tag == allDots[i,j].tag && allDots[i,j].tag == allDots[i - 1, j - 1].tag)
                                {
                                    return false;
                                }
                            }*/
                            if (allDots[i - 1, j - 1].tag == allDots[i, j].tag)
                            {
                                if (CheckDotsInLastRows(i - 1, j - 1, i, j))
                                {
                                    return false;
                                }
                            }
                        }
                    }
                    if (allDots[i, j - 1] != null)
                    {
                        if (allDots[i, j - 1].tag == allDots[i, j].tag)
                        {
                            if (CheckDotsInLastRows(i, j - 1, i, j))
                            {
                                return false;
                            }
                        }
                    }
                    if (allDots[i + 1, j] != null)
                    {
                        if (allDots[i + 1, j].tag == allDots[i, j].tag)
                        {
                            if (CheckDotsInLastRows(i + 1, j, i, j))
                            {
                                return false;
                            }
                        }
                    }
                    if (allDots[i + 1, j - 1] != null)
                    {
                        if (allDots[i + 1, j - 1].tag == allDots[i, j].tag)
                        {
                            if (CheckDotsInLastRows(i + 1, j - 1, i, j))
                            {
                                return false;
                            }
                        }
                    }
                }
            }
        }
        for (int i = Width - 1; i > Width - 3; i--)
        {
            for (int j = Height - 1; j > 0; j--)
            {
                //  print("( " + i + "," + j + ")");
                if (allDots[i, j] != null)
                {
                    if (allDots[i, j - 1] != null && allDots[i - 1, j] != null)
                    {
                        if (allDots[i, j - 1].tag == allDots[i, j].tag && allDots[i, j].tag == allDots[i - 1, j].tag)
                        {
                            return false;
                        }
                    }
                    if (allDots[i - 1, j] != null && allDots[i - 1, j - 1] != null)
                    {
                        if (allDots[i, j].tag == allDots[i - 1, j].tag && allDots[i, j].tag == allDots[i - 1, j - 1].tag)
                        {
                            return false;
                        }
                    }
                    if (allDots[i - 1, j - 1] != null && allDots[i, j - 1] != null)
                    {
                        if (allDots[i, j].tag == allDots[i - 1, j - 1].tag && allDots[i, j].tag == allDots[i, j - 1].tag)
                        {
                            return false;
                        }
                    }
                    if (allDots[i, j - 1] != null)
                    {
                        if (allDots[i, j - 1].tag == allDots[i, j].tag)
                        {
                            if (CheckDotsInLastColumns(i, j - 1, i, j))
                            {
                                return false;
                            }
                        }
                    }
                    if (allDots[i - 1, j] != null)
                    {
                        if (allDots[i - 1, j].tag == allDots[i, j].tag)
                        {
                            if (CheckDotsInLastColumns(i - 1, j, i, j))
                            {
                                return false;
                            }
                        }
                    }
                    if (allDots[i - 1, j - 1] != null)
                    {
                        if (allDots[i - 1, j - 1].tag == allDots[i, j].tag)
                        {
                            if (CheckDotsInLastColumns(i - 1, j - 1, i, j))
                            {
                                return false;
                            }
                        }
                    }
                }
            }
        }
        return true;
    }
    private bool CheckDotsInLastColumns(int i, int j, int prevI, int prevJ)
    {
        if (j > 0)
        {
            if (allDots[i, j - 1] != null)
            {
                if (allDots[i, j].tag == allDots[i, j - 1].tag && allDots[i, j - 1] != allDots[prevI, prevJ])
                {
                    return true;
                }
            }
            if (i < Width - 1)
            {
                if (allDots[i + 1, j - 1] != null)
                {
                    if (allDots[i + 1, j - 1].tag == allDots[i, j].tag && allDots[i + 1, j - 1] != allDots[prevI, prevJ])
                    {
                        return true;
                    }
                }
            }
        }
        if (i > 0)
        {
            if (allDots[i - 1, j] != null)
            {
                if (allDots[i, j].tag == allDots[i - 1, j].tag && allDots[i - 1, j] != allDots[prevI, prevJ])
                {
                    return true;
                }
            }
            if (j > 0)
            {
                if (allDots[i - 1, j - 1] != null)
                {
                    if (allDots[i - 1, j - 1].tag == allDots[i, j].tag && allDots[i - 1, j - 1] != allDots[prevI, prevJ])
                    {
                        return true;
                    }
                }
            }
            if (j < Height - 1)
            {
                if (allDots[i - 1, j + 1] != null)
                {
                    if (allDots[i - 1, j + 1].tag == allDots[i, j].tag && allDots[i - 1, j + 1] != allDots[prevI, prevJ])
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
    private bool CheckDotsInLastRows(int i, int j, int prevI, int prevJ)
    {
        if (allDots[i, j - 1] != null)
        {
            if (allDots[i, j].tag == allDots[i, j - 1].tag && allDots[i, j - 1] != allDots[prevI, prevJ])
            {
                return true;
            }
        }
        if (i < Width - 1)
        {
            if (allDots[i + 1, j] != null)
            {
                if (allDots[i, j].tag == allDots[i + 1, j].tag && allDots[i + 1, j] != allDots[prevI, prevJ])
                {
                    return true;
                }
            }
        }
        if (j > 0 && i > 0)
        {
            if (allDots[i - 1, j - 1] != null)
            {
                if (allDots[i - 1, j - 1].tag == allDots[i, j].tag && allDots[i - 1, j - 1] != allDots[prevI, prevJ])
                {
                    return true;
                }
            }

        }
        if (i < Width - 1)
        {
            if (allDots[i + 1, j - 1] != null)
            {
                if (allDots[i + 1, j - 1].tag == allDots[i, j].tag && allDots[i + 1, j - 1] != allDots[prevI, prevJ])
                {
                    return true;
                }
            }
            if (j < Height - 1)
            {
                if (allDots[i + 1, j + 1] != null)
                {
                    if (allDots[i + 1, j + 1].tag == allDots[i, j].tag && allDots[i + 1, j + 1] != allDots[prevI, prevJ])
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    private bool CheckDots(int i, int j, int prevI, int prevJ)
    {
        if (allDots[i + 1, j] != null)
        {
            if (allDots[i + 1, j].tag == allDots[i, j].tag && allDots[i + 1, j] != allDots[prevI, prevJ])
            {
                return true;
            }

        }
        if (allDots[i, j + 1] != null)
        {
            if (allDots[i, j + 1].tag == allDots[i, j].tag && allDots[i, j + 1] != allDots[prevI, prevJ])
            {
                return true;
            }
        }
        if (allDots[i + 1, j + 1] != null)
        {
            if (allDots[i + 1, j + 1].tag == allDots[i, j].tag && allDots[i + 1, j + 1] != allDots[prevI, prevJ])
            {
                return true;
            }
        }
        if (i >= 0 && i < Width - 1 && j > 0 && j < Height - 1)
        {
            if (allDots[i + 1, j - 1] != null)
            {
                if (allDots[i + 1, j - 1].tag == allDots[i, j].tag && allDots[i + 1, j - 1] != allDots[prevI, prevJ])
                {
                    return true;
                }
            }
        }
        if (i > 0 && i < Width - 1 && j >= 0 && j < Height - 1)
        {
            if (allDots[i - 1, j + 1] != null)
            {
                if (allDots[i - 1, j + 1].tag == allDots[i, j].tag && allDots[i - 1, j + 1] != allDots[prevI, prevJ])
                {
                    return true;
                }
            }
        }
        return false;
    }
    private void DeadLockResolverDec()
    {
        //buggy Failur Function !!!!
        int row = UnityEngine.Random.Range(0, Height);
        int column = UnityEngine.Random.Range(0, Width);
        for (int i = 0; i < Width; i++)
        {
            if (!blankSpaces[i, row] && !EmptySpaces[i, row] && HardTiles[i,row] == null)
            {
                var temp = allDots[i, row].GetComponent<DotController>();
                if (temp != null)
                {
                    if (!temp.isAdjacentBomb && !temp.isRowBomb && !temp.isColumnBomb)
                    {
                        Destroy(allDots[i, row]);
                        allDots[i, row] = null;
                    }
                }
            }
        }
        RefillBoard();
        for (int i = 0; i < Height; i++)
        {
            if (!blankSpaces[column, i] && !EmptySpaces[column, i] && !HardTiles[column,i])
            {
                var temp = allDots[column, i].GetComponent<DotController>();
                if (temp != null)
                {
                    if (!temp.isAdjacentBomb && !temp.isRowBomb && !temp.isColumnBomb)
                    {
                        Destroy(allDots[column, i]);
                        allDots[column, i] = null;
                    }
                }
            }
        }
        RefillBoard();
    }
    private void DeadLockResolverShuffle()
    {
        /////////////////////////..... new shuffle Resolver
        List<GameObject> newBoard = new List<GameObject>();
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                if (allDots[i, j] != null && HardTiles[i,j] == null)
                {
                    if (allDots[i, j].tag != "key")
                    {
                        newBoard.Add(allDots[i, j]);
                    }
                }
            }
        }
        print("new  " + newBoard.Count);
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                if (!blankSpaces[i, j] && !EmptySpaces[i, j] &&
                    HardTiles[i,j] == null && allDots[i,j].tag != "key"
                    && newBoard.Count > 0)
                {
                    var r = UnityEngine.Random.Range(0, newBoard.Count);
                   // print("deadlock " + r);
                    var MaxItr = 0;
                    while (!DotPossibility(i, j, newBoard[r]) && MaxItr < maxIterations)
                    {
                        r = UnityEngine.Random.Range(0, newBoard.Count);
                       // print("deadlock " + r);
                        MaxItr++;
                    }
                   // print("bad e in e ");
                    DotController dot = newBoard[r].GetComponent<DotController>();
                    dot.column = i;
                    dot.row = j;
                    allDots[i, j] = newBoard[r];
                    newBoard.Remove(newBoard[r]);
                }
            }
        }
    }
    #endregion
}
