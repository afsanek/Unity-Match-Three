using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Board board;
    public float boardPadding = 1f;
    [SerializeField]
    private float aspectRatio = Screen.width / (float) Screen.height;
    void Start()
    {
        board = GameObject.FindWithTag("Board").GetComponent<Board>();
        if (board != null)
        {
            cameraRepositioner(board.Width - 1, board.Height - 1);
        }
    }

    private void cameraRepositioner(int width, int height)
    {
        transform.position = new Vector3(width / 2f, height / 2f,transform.position.z);
        if (width >= height)
        {
            Camera.main.orthographicSize = (board.Width / 2f + boardPadding) / aspectRatio;
        }
        else if (height - width < 8) 
        {
            Camera.main.orthographicSize = (board.Height /1.1f + boardPadding) ;
        }
        else
        {
            Camera.main.orthographicSize = (board.Height / 2f + boardPadding);
        }
    }
}
