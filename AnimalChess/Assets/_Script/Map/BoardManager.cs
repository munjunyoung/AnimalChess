using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance = null;
    //Board
    private const int        width                   = 8;
    private const int        height                  = 9;
    private const int        battleboardHeightIndex  = 5;
    private const float      yPos                    = 0.5f;
    private const float      yWaitblockPos           = 1f;

    private GameObject         ingameGroundParentOb;
    private BlockOnBoard[,]    GroundBlocks      = new BlockOnBoard[width, height];
    private List<BlockOnBoard> waitingBlockList  = new List<BlockOnBoard>();
    //Unit
    public GameObject unitOBParent = null;

    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        ingameGroundParentOb = GameObject.Find("1PlayGround");
        DrawChessBoard();
    }

    /// <summary>
    /// NOTE : Drawblock
    /// </summary>
    private void DrawChessBoard()
    {
        var grounddic = DataBaseManager.instance.groundDic;

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
                if (z == 0)
                {
                    blockob.IsWaitingBlock = true;
                    waitingBlockList.Add(blockob);
                }
                GroundBlocks[x, z] = blockob;
            }
        }
    }

    /// <summary>
    /// NOTE : 구매한 유닛 대기 블록라인에 배치 
    /// </summary>
    /// <param name="_unitType"></param>
    public bool DropPurchasedUnit(Unit_Type _unitType)
    {
        foreach(var wb in waitingBlockList)
        {
            if(wb.GetUnit()==null)
            {
                wb.SetUnit(CreateUnit(DataBaseManager.instance.unitObDic[_unitType.ToString()]));
                return true;
            }
        }
        return false;
    }
    
    public void SetBattleBlockLayer(bool _isbattle)
    {
        for (int j = 1; j < battleboardHeightIndex; j++)
        {
            for (int i = 0; i < width; i++)
                GroundBlocks[i, j].SetLayerValue(_isbattle);
        }
    }

    /// <summary>
    /// NOTE : 유닛 구매 
    /// </summary>
    
    private Unit CreateUnit(Unit _unit)
    {
        var unit = Instantiate(_unit, Vector3.zero, Quaternion.identity, unitOBParent.transform);
        unit.transform.eulerAngles = new Vector3(0, 180, 0);
        return unit;
    }
}   
