using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Level",menuName ="Level")]
public class Level : ScriptableObject
{
    [Header("Board Dimentions")]
    public int Width;
    public int Height;

    [Header("un jaha k ye model dgas")]
    public TileDef[] BoardLayout;
    
    [Header("level fruits")]
    public GameObject[] Dots;

    [Header("Score Target")]
    public int[] scoreTarget;

    [Header("End Game Manager")]
    public EndGameRequirements EndGameReq;
    public LevelTarget[] levelTargets;
    public TargetType targetType;
    public int ScoreTarget;

    [Header("Key Room")]
    public int[] KeyRoom;
}
