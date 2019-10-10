using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public Unit testUnit;
    public Unit testUnit2;

    public static BoardManager instance = null;
    //Board
    private const int        width                   = 8;
    private const int        height                  = 9;
    private const float      yPos                    = 0.5f;
    private const float      yWaitblockPos           = 1f;

    private GameObject       ingameGroundParentOb;
    private BlockOnBoard[,]    GroundBlocks      = new BlockOnBoard[width, height];

    // Start is called before the first frame update
    private void Start()
    {
        if (instance == null)
            instance = this;

        ingameGroundParentOb = GameObject.Find("1PlayGround");
        DrawChessBoard();

        GroundBlocks[0, 0].SetUnit(testUnit);
        GroundBlocks[2, 2].SetUnit(testUnit2);
    }

    /// <summary>
    /// NOTE : Drawblock
    /// </summary>
    private void DrawChessBoard()
    {
        var grounddic = LoadDataManager.instance.groundDic;

        for (int z = 0; z <= 8; z++)
        {
            for (int x = 0; x < 8; x++)
            {
                var pos = new Vector3(x * 2f, 0.5f, z * 2f);
                BlockOnBoard blockob = null;
                if (z == 0)
                {
                    blockob = Instantiate(grounddic[Ground_TYPE.WaitingBlock.ToString()], pos, Quaternion.identity, ingameGroundParentOb.transform);
                }
                else
                {
                    if (z % 2 != 0)
                    {
                        if (x % 2 != 0)
                            blockob = Instantiate(grounddic[Ground_TYPE.DesertBlock1.ToString()], pos, Quaternion.identity, ingameGroundParentOb.transform);
                        else
                            blockob = Instantiate(grounddic[Ground_TYPE.DesertBlock2.ToString()], pos, Quaternion.identity, ingameGroundParentOb.transform);
                    }
                    else
                    {
                        if (x % 2 != 0)
                            blockob = Instantiate(grounddic[Ground_TYPE.DesertBlock2.ToString()], pos, Quaternion.identity, ingameGroundParentOb.transform);
                        else
                            blockob = Instantiate(grounddic[Ground_TYPE.DesertBlock1.ToString()], pos, Quaternion.identity, ingameGroundParentOb.transform);
                    }
                }

                if (z < 5)
                    blockob.gameObject.layer = 10;

                GroundBlocks[x, z] = blockob;
            }
        }
    }
    
    public void MoveUnit(BlockOnBoard _block)
    {
        
    }

    /// <summary>
    /// NOTE : 기존의 유닛이 존재하고 있을 경우 
    /// </summary>
    public void SwapUnit()
    {

    }

    //public void SelectBoardblock(Vector3 point)
    //{
    //    if (point.x < 0 || point.x > width * 2 || point.y < 0 || point.y > height * 2)
    //        return;

    //    int xindex = (int)(point.x * 0.5f);
    //    int yindex = (int)(point.y * 0.5f);




    //}
}   
