using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    BoardTile[] tiles;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void DrawChessBoard()
    {
        Vector3 widthLine = Vector3.right * 8;
        Vector3 heightLine = Vector3.up * 8;
        
    }
}   


public class BoardTile
{
    Vector2Int pos;
    GameObject onUnit;
    
    public BoardTile(Vector2Int _pos)
    {
        pos = _pos;
        onUnit = null;
    }
}

public class WaitingTile
{
}
