using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance = null;
    //Board
    private const int width = 9;
    private const int height = 10;
    private const int battleboardHeightIndex = 5;
    private const float yPos = 0.5f;
    private const float yWaitblockPos = 1f;

    private GameObject groundParentOb;
    private BlockOnBoard[,] allGroundBlocks = new BlockOnBoard[width, height];
    public List<BlockOnBoard> waitingBlockList = new List<BlockOnBoard>();
    //Unit
    public GameObject unitOBParent = null;

    private List<BlockOnBoard> BattleBlockOnUnitList = new List<BlockOnBoard>();
    private List<BlockOnBoard> waitBlockOnUnitList = new List<BlockOnBoard>();
    //합성 체크하기 위함
    private List<BlockOnBoard> allBlockOnUnitList = new List<BlockOnBoard>();
    //public List<Unit> unitListOnBattleBoard = new List<Unit>();
    //public List<Unit> waitingBoardUnitList = new List<Unit>();
    public List<EnemyUnit> monsterListOnBattleBoard = new List<EnemyUnit>();

    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        groundParentOb = GameObject.Find("1PlayGround");
        DrawChessBoard();
    }


    #region Draw
    /// <summary>
    /// NOTE : Drawblock
    /// </summary>
    private void DrawChessBoard()
    {
        var grounddic = DataBaseManager.instance.groundDic;

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                var pos = new Vector3(x * 2f, 0.5f, z * 2f);
                BlockOnBoard blockob = null;
                if (z == 0)
                {
                    blockob = Instantiate(grounddic[Ground_TYPE.WaitingBlock.ToString()], pos, Quaternion.identity, groundParentOb.transform);
                }
                else
                {
                    if (z % 2 != 0)
                    {
                        if (x % 2 != 0)
                            blockob = Instantiate(grounddic[Ground_TYPE.DesertBlock1.ToString()], pos, Quaternion.identity, groundParentOb.transform);
                        else
                            blockob = Instantiate(grounddic[Ground_TYPE.DesertBlock2.ToString()], pos, Quaternion.identity, groundParentOb.transform);
                    }
                    else
                    {
                        if (x % 2 != 0)
                            blockob = Instantiate(grounddic[Ground_TYPE.DesertBlock2.ToString()], pos, Quaternion.identity, groundParentOb.transform);
                        else
                            blockob = Instantiate(grounddic[Ground_TYPE.DesertBlock1.ToString()], pos, Quaternion.identity, groundParentOb.transform);
                    }
                }

                if (z < 5)
                    blockob.gameObject.layer = 10;
                if (z == 0)
                {
                    blockob.IsWaitingBlock = true;
                    waitingBlockList.Add(blockob);
                }
                allGroundBlocks[x, z] = blockob;
            }
        }
    }

    #endregion

    #region BlockList
    /// <summary>
    /// NOTE : 유닛이 존재하는 블럭 리스트 초기화
    /// </summary>
    /// <param name="_block"></param>
    public void AddBlockOnList(BlockOnBoard _block)
    {
        if (_block.IsWaitingBlock)
        {
            waitBlockOnUnitList.Add(_block);
        }
        else
        {
            BattleBlockOnUnitList.Add(_block);
            IngameManager.instance.pData.CurrentFieldUnitNumber = BattleBlockOnUnitList.Count;
        }
        allBlockOnUnitList.Add(_block);
        //unitListOnBattleBoard.Add(_unit);
        //UIManager.instance.SetUnitNumberText(unitListOnBattleBoard.Count, IngameManager.instance.pData.Level);
    }

    /// <summary>
    /// NOTE : 리스트 삭제
    /// </summary>
    /// <param name="_block"></param>
    public void RemoveBlockOnList(BlockOnBoard _block)
    {
        if (_block.IsWaitingBlock)
        {
            waitBlockOnUnitList.Remove(_block);
        }
        else
        {
            BattleBlockOnUnitList.Remove(_block);
            IngameManager.instance.pData.CurrentFieldUnitNumber = BattleBlockOnUnitList.Count;
        }
        allBlockOnUnitList.Remove(_block);
        //unitListOnBattleBoard.Remove(_unit);
        //UIManager.instance.SetUnitNumberText(unitListOnBattleBoard.Count, IngameManager.instance.pData.Level);
    }

    /// <summary>
    /// NOTE : Layer를 변경하여 전투중 이동 불가 하도록 하기 위함
    /// </summary>
    /// <param name="_isbattle"></param>
    public void SetBattleBlockLayer(bool _isbattle)
    {
        for (int j = 1; j < battleboardHeightIndex; j++)
        {
            for (int i = 0; i < width; i++)
                allGroundBlocks[i, j].SetLayerValue(_isbattle);
        }
    }
    #endregion

    #region  Unit

    /// <summary>
    /// NOTE : Round
    /// </summary>
    public void ReturnUnitOnWaitingBoard(int _level)
    {
        //watingblock 리스트를 체크하여 넘어간 숫자만큼 리스트 0번부터 이동
        if (BattleBlockOnUnitList.Count <= _level)
            return;

        foreach (var wb in waitingBlockList)
        {
            //레벨이 넘어가면 break;
            if (wb.GetUnitNormal() == null)
            {
                var target = BattleBlockOnUnitList[0].GetUnitRemoveList();
                wb.SetUnitaddList(target);
                if (BattleBlockOnUnitList.Count > _level)
                    continue;
                else
                    break;
            }
        }
        //waitngblock을 순회했음에도 아직 유닛이 많다면(wt블락이 꽉차서) 유닛 판매
        while (BattleBlockOnUnitList.Count > _level)
            SellUnit(BattleBlockOnUnitList[0]);

        //if ->전투를 시작 할 때 가능한 말의 수보다 많을경우
        //코스트가 낮은 유닛을 기준으로 board에 되돌아감
        //만약 waiting board가 가득 차있다면 판매되고 골드가 갱신
    }


    /// <summary>
    /// NOTE : 유닛 판매
    /// </summary>
    /// <param name="_block"></param>
    public void SellUnit(BlockOnBoard _block)
    {
        var target = _block.GetUnitRemoveList();
        target.gameObject.SetActive(false);
        IngameManager.instance.pData.Gold += target.unitPdata.cost;
        RemoveUnit(_block);
        _block.SetUnitaddList(null);
        //판매 애니매이션 사운드
    }

    /// <summary>
    /// NOTE : 구매한 유닛 대기 블록라인에 배치 
    /// </summary>
    /// <param name="_unitpdata"></param>
    public bool BuyUnit(UnitPropertyData _unitpdata)
    {
        if (IngameManager.instance.pData.Gold < _unitpdata.cost)
            return false;
        
        //합성 가능한것이 있을 경우 대기보드 자리가 있던 말던 바로 구매
        if (CheckComposeUnitByBuy(_unitpdata))
        {
            IngameManager.instance.pData.Gold -= _unitpdata.cost;
            return true;
        }

        //합성할 것이 없을 경우 
        //아닐경우 아래를 유지
        foreach (var wb in waitingBlockList)
        {
            if (wb.GetUnitNormal() == null)
            {
                CreateUnit((_unitpdata.unitType), 1, wb);
                IngameManager.instance.pData.Gold -= _unitpdata.cost;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// NOTE : 유닛 합성 , 구매할 때 생성되는 block데이터를 통해서 처리 
    /// </summary>
    /// <param name="_unitpdata"></param>
    private bool CheckComposeUnitByBuy(UnitPropertyData _unitpdata)
    {
        if (_unitpdata.ratingValue >= 3)
            return false;

        List<BlockOnBoard> checkBlockList = new List<BlockOnBoard>();

        //전투중일 경우에는 대기 블록에서만처리 , 전투중이 아닐경우에는 모든 block에서 처리
        checkBlockList = IngameManager.instance.IsBattleState ? waitBlockOnUnitList : allBlockOnUnitList;

        List<BlockOnBoard> blockOnUnit = new List<BlockOnBoard>();
        foreach (var block in checkBlockList)
        {
            if (block.GetUnitNormal().unitPdata.unitType == _unitpdata.unitType
                && block.GetUnitNormal().unitPdata.ratingValue == _unitpdata.ratingValue)
            {
                blockOnUnit.Add(block);
            }
        }

        if (blockOnUnit.Count >= 2)
        {
            foreach (var block in blockOnUnit)
                RemoveUnit(block);
            CreateUnit(_unitpdata.unitType, _unitpdata.ratingValue + 1, blockOnUnit[0]);
            return true;
        }
        return false;
    }

    /// <summary>
    /// NOTE : 유닛 합성 , 구매할 때 생성되는 block데이터를 통해서 처리 
    /// </summary>
    /// <param name="_unitpdata"></param>
    private bool CheckComposeUnitNormal(UnitPropertyData _unitpdata)
    {
        if (_unitpdata.ratingValue >= 3)
            return false;

        List<BlockOnBoard> checkBlockList = new List<BlockOnBoard>();

        //전투중일 경우에는 대기 블록에서만처리 , 전투중이 아닐경우에는 모든 block에서 처리
        checkBlockList = IngameManager.instance.IsBattleState ? waitBlockOnUnitList : allBlockOnUnitList;

        List<BlockOnBoard> blockOnUnit = new List<BlockOnBoard>();
        foreach (var block in checkBlockList)
        {
            if (block.GetUnitNormal().unitPdata.unitType == _unitpdata.unitType
                && block.GetUnitNormal().unitPdata.ratingValue == _unitpdata.ratingValue)
            {
                blockOnUnit.Add(block);
            }
        }

        if (blockOnUnit.Count >= 3)
        {
            foreach (var block in blockOnUnit)
                RemoveUnit(block);
            CreateUnit(_unitpdata.unitType, _unitpdata.ratingValue + 1, blockOnUnit[0]);
            return true;
        }
        return false;
    }

    /// <summary>
    /// NOTE : 전투 끝난후 모든 유닛들 합성 체크
    /// </summary>
    private void CheckComposeUnitByBattleEnd()
    {
        var checklist = allBlockOnUnitList;
        for(int i = 0; i<checklist.Count;i++)
        {
            //검색하다가 합성되면 다시 0으로 리셋해서 검색
            if (CheckComposeUnitNormal(checklist[i].GetUnitNormal().unitPdata))
                i = 0;
        }
    }
    
    /// <summary>
    /// NOTE : 유닛 구매 // block을 포함한 이유는 만들어질때마다 유닛합성 체크를 해야하고 그러기 위해선 블럭을 여기 에서 설정해야함
    /// </summary>
    private void CreateUnit(Unit_Type _unitType, int _ratingvalue, BlockOnBoard _blockOnUnit)
    {
        var pdata = DataBaseManager.instance.UnitPropertyDataDic[_ratingvalue - 1][_unitType];
        var unit = Instantiate(DataBaseManager.instance.unitObDic[pdata.ObId], Vector3.zero, Quaternion.identity, unitOBParent.transform);
        unit.GetComponent<Unit>().unitPdata = pdata;
        unit.transform.eulerAngles = new Vector3(0, 180, 0);
        _blockOnUnit.SetUnitaddList(unit);
        CheckComposeUnitNormal(unit.unitPdata);
    }

    /// <summary>
    /// NOTE : 해당 오브젝트 유닛 삭제
    /// </summary>
    /// <param name="_block"></param>
    private void RemoveUnit(BlockOnBoard _block)
    {
        var target = _block.GetUnitRemoveList();
        _block.SetUnitaddList(null);
        target.gameObject.SetActive(false);
    }


    #endregion
}

