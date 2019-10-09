using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private const int       width                   = 8;
    private const int       height                  = 9;
    private const float     yPos                    = 0.5f;

    private GameObject      ingameGroundParentOb;
    private BoardTile[,]     Groundtiles                    = new BoardTile[width, height];
    public WaitingTile[]    waitingGroundTile              = new WaitingTile[width];  

    // Start is called before the first frame update
    private void Start()
    {
        ingameGroundParentOb = GameObject.Find("1PlayGround");
        DrawChessBoard();
    }

    private void DrawChessBoard()
    {
        var grounddic = LoadDataManager.instance.groundDic;

        for (int z = 0; z <= 8; z++)
        {
            for (int x = 0; x < 8; x++)
            {
                var pos = new Vector3(x * 2f, 0.5f, z * 2f);
                BoardTile tileob = null;
                if (z == 0)
                {
                    pos.y = 1f;
                    tileob = Instantiate(grounddic[Ground_TYPE.WaitingGround.ToString()], pos, Quaternion.identity, ingameGroundParentOb.transform);
                }
                else
                {
                    if (z % 2 != 0)
                    {
                        if (x % 2 != 0)
                            tileob = Instantiate(grounddic[Ground_TYPE.ForestGround1.ToString()], pos, Quaternion.identity, ingameGroundParentOb.transform);
                        else
                            tileob = Instantiate(grounddic[Ground_TYPE.ForestGround2.ToString()], pos, Quaternion.identity, ingameGroundParentOb.transform);
                    }
                    else
                    {
                        if (x % 2 != 0)
                            tileob = Instantiate(grounddic[Ground_TYPE.ForestGround2.ToString()], pos, Quaternion.identity, ingameGroundParentOb.transform);
                        else
                            tileob = Instantiate(grounddic[Ground_TYPE.ForestGround1.ToString()], pos, Quaternion.identity, ingameGroundParentOb.transform);
                    }
                }

                if(z<6)
                Groundtiles[x, z] = tileob;
            }
        }
    }

    public void SelectBoardTile(Vector3 pos)
    {
        int x = (int)(pos.x * 0.5f);
        int y = (int)(pos.y * 0.5f);

        if (x < 0)
            x = 0;
        else if (x > width)
            x = width;

        if (y < 0)
            y = 0;
        else if (y > height)
            y = height;


    }
}   
