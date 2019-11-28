using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameManager : MonoBehaviour
{
    public static IngameManager instance = null;
    public PlayerData playerData;
    
    private TouchUnitSystem unitTouchSystem;

    private readonly int waitingTime = 10;
    private readonly int roundFinishGold = 1;
    private readonly int roundFinishEXP = 1;

    private bool _IsBattleState = false;
    public bool IsBattleState
    {
        get { return _IsBattleState; }
        set
        {
            _IsBattleState = value;
            UIManager.instance.SetGameStateText(IsBattleState);
            BoardManager.instance.SetBattleBlockLayer(IsBattleState);
        }
    }

    private int _CurrentRoundNum = 0;
    public int CurrentRoundNum
    {
        get { return _CurrentRoundNum; }
        set
        {
            _CurrentRoundNum = value;
            UIManager.instance.SetRoundNumberText(_CurrentRoundNum);
        }
    }
    
    public int winsNumber = 0;
    public int defeatsNumber = 0;
    
    private void Awake()
    {
        if (instance == null)
            instance = this;

        unitTouchSystem = GetComponent<TouchUnitSystem>();
        //대기장소에선 이동가능 보드에 올릴수 없음
    }

    private void Start()
    {
        playerData = new PlayerData();
        GameStart();
    }
    
    private void GameStart()
    {
        BoardManager.instance.BoardStart();
        CurrentRoundNum = 1;
        StartWaitState(waitingTime, false);
       
    }

    #region Round Waiting
    /// <summary>
    /// NOTE : 대기 카운터 실행
    /// </summary>
    /// <param name="_count"></param>
    private void StartWaitState(int _count, bool _iswin)
    {
        //Round종료 체크 유닛리스트 카운트로 처리 

        IsBattleState = false;//골드 정산
        //라운드 설정 변경 (졌으면 유지, 이겼으면 다음라운드 진행)
        if (_iswin)
        {
            //라운드 증가 
            CurrentRoundNum++;
            winsNumber++;
            defeatsNumber = 0;
        }
        else
        {
            //플레이어 체력 정산 
            defeatsNumber++;
            winsNumber = 0;
        }

        //슬라이더 PANEL OFF
        UIManager.instance.SetOffSliderList();
        //해당 라운드 유닛 생성
        //SetMonster[RoundNumber]; 
        //경험치
        playerData.ExpValue += 1;
        //골드 정산
        SetRoundGold(_iswin);

        //레벨 적용후 샵 데이터 초기화(리롤)
        UIManager.instance.SetShopPanelCharacter(playerData.Level, playerData.Gold);
        //유닛 데이터 및 포지션 다시 설정 
        ReDeployUnitOB();
        //적유닛 설정
        DeployEnemyUnitOB(CurrentRoundNum, _iswin);
        //대기시간 카운팅
        StartCoroutine(WaitCounterProcess(_count));
        //전투시간떄 layer변경은 변수 property에서 처리
        
    }
    
    /// <summary>
    /// NOTE : 라운드 종료후 골드 값 설정
    /// </summary>
    private void SetRoundGold(bool _isWin)
    {
        int plusgold = 0;
        
        //라운드 당 골드  (라운드 당  2,3,4,5)
        if (CurrentRoundNum < 4)
            plusgold += CurrentRoundNum;
        else
            plusgold += 5;

        //해당 라운드 승리당 골드 
        if (_isWin)
            plusgold += 1;

        //연승 추가 골드, 
        //연패 추가 골드(3연승 이후부터 1골드씩  최대 5골드 ) 
        if (winsNumber >= 3)
            plusgold += winsNumber > 5  ? 3 : winsNumber - 2;
        if (defeatsNumber >= 3)
            plusgold += defeatsNumber> 5 ? 3 : defeatsNumber - 2;


        // 현재 골드에 따른 추가 골드
        // 10골드 간격에 따라 +1
        plusgold += playerData.Gold >= 10 ? 1 : 0;
        plusgold += playerData.Gold >= 20 ? 1 : 0;
        plusgold += playerData.Gold >= 30 ? 1 : 0;
        plusgold += playerData.Gold >= 40 ? 1 : 0;
        plusgold += playerData.Gold >= 50 ? 1 : 0;

        playerData.Gold += plusgold;
        
    }

    /// <summary>
    /// NOTE : 유닛 전투후 포지션 및 데이터 다시 설정, 처음 시작시 유닛이 없을떈 RETURN
    /// </summary>
    private void ReDeployUnitOB()
    {
        var blockListOnUnit = BoardManager.instance.GetBattleBlockOnUnit();
        if (blockListOnUnit == null)
            return;
        foreach (var blockOnList in blockListOnUnit)
        {
            blockOnList.GetUnitNormal().unitController.ResetUnitDataInWaiting();
        }
        //시너지도 다시적용
        BoardManager.instance.synergyManager.SetSynergy(blockListOnUnit);
    }


    /// <summary>
    /// NOTE : 라운드별 대기시간에 해당 라운드의 적 유닛 배치
    /// </summary>
    private void DeployEnemyUnitOB(int currentround, bool iswin)
    {
        if (iswin)
        {
            var prevRoundEnemyUnitList = BoardManager.instance.currentEnemyUnitList;
            foreach (var unit in  prevRoundEnemyUnitList)
            {
                unit.unitblockSc.GetCurrentBlockInWaiting().SetUnitaddList(null);
                unit.unitblockSc.gameObject.SetActive(false);
            }
        }

        var enemyunitlist = BoardManager.instance.SetCurrentEnemyUnit(currentround);
        foreach (var tmpunit in enemyunitlist)
            tmpunit.ResetUnitDataInWaiting();

    }


    /// <summary>
    /// NOTE : 대기 카운터 코루틴
    /// </summary>
    /// <param name="_count"></param>
    /// <returns></returns>
    IEnumerator WaitCounterProcess(int _count)
    {
        int tmpcounter = _count;
        
        UIManager.instance.SetWaitingCountText(tmpcounter);
        while (tmpcounter>0)
        {
            tmpcounter--;
            UIManager.instance.SetWaitingCountText(tmpcounter);
            yield return new WaitForSecondsRealtime(1);
        }
        StartBattleState();
    }

    #endregion

    #region StartBattle
    /// <summary>
    /// NOTE : 전투 상태 시작 ( UI 변경 및 BLOCK LAYER처리는 iSBATTLE PROPERT 내부에서 처리 )
    /// </summary>
    private void StartBattleState()
    {
        IsBattleState = true;
        //전투시작
        //배틀보드에 있는 유닛을 드래그중인경우 초기 자리로 되돌리고 픽업 상태를 변경
        unitTouchSystem.ReturnPickState();
        //레벨보다 많은 숫자의 유닛이 올라갈경우 유닛처리
        BoardManager.instance.ReturnUnitOnWaitingBoard(playerData.Level);
        //유닛, 적 ai 실행 (1초~2초정도의 텀을 주도록 실행하도록)
        StartUnitBehavior();
        StartEnemyUnitBehavior();
        //유닛이 없거나 적이 없을때를 체크하여 배틀 처리
        CheckUnitAlive();
        CheckEnemyUnitAlive();
    }

    /// <summary>
    /// NOTE : 유닛 슬라이더 UI 설정, BTAI 실행
    /// </summary>
    private void StartUnitBehavior()
    {
        var battleblockOnUnitlist = BoardManager.instance.GetBattleBlockOnUnit();
        //Sort
        SortBlcokListByPos(ref battleblockOnUnitlist);

        for (int i = 0; i < battleblockOnUnitlist.Count; i++)
        {
            battleblockOnUnitlist[i].GetUnitNormal().unitController.StartUnitInBattle(UIManager.instance.playerUnitSliderList[i],1.5f);
        }
    }

    /// <summary>
    /// NOTE
    /// </summary>
    private void StartEnemyUnitBehavior()
    {
        var enemyunitlist = BoardManager.instance.currentEnemyUnitList;

        for(int i = 0;i<enemyunitlist.Count; i++)
        {
            enemyunitlist[i].StartUnitInBattle(UIManager.instance.enemyUnitSliderList[i], 1.5f);
        }
    }

    /// <summary>
    /// NOTE : 좀더 적에 가까이 있는 유닛이 먼져 행동하도록 하기 위함
    /// </summary>
    /// <param name="blocklist"></param>
    private void SortBlcokListByPos(ref List<BlockOnBoard> blocklist)
    {
        if (blocklist.Count < 2)
            return;
        bool sortcontinue = true;

        while (sortcontinue)
        {
            sortcontinue = false;
            for(int i = 0; i< blocklist.Count -1; i++)
            {
                if(CompareBlockPos(blocklist[i], blocklist[i+1]))
                {
                    BlockOnBoard tmpblock = blocklist[i];
                    blocklist[i] = blocklist[i + 1];
                    blocklist[i+1] = tmpblock;
                    sortcontinue = true;
                }
            }
        }
    }
    /// <summary>
    /// NOTE : Y값은 높은순, X값은 낮은순
    /// </summary>
    /// <param name="block1"></param>
    /// <param name="block2"></param>
    /// <returns></returns>
    private bool CompareBlockPos(BlockOnBoard block1, BlockOnBoard block2)
    {
        if (block1.groundArrayIndex.y > block2.groundArrayIndex.y)
            return false;
        if(block1.groundArrayIndex.y < block2.groundArrayIndex.y)
            return true;

        if (block1.groundArrayIndex.x < block2.groundArrayIndex.x)
            return false;
        if (block1.groundArrayIndex.x > block2.groundArrayIndex.x)
            return true;

        return false;
    }
    #endregion

    #region EndBattle
    /// <summary>
    /// NOTE : Unit이 죽었을때 체크
    /// </summary>
    public void CheckUnitAlive()
    {
        bool allDie = true;
        var blockonUnitList = BoardManager.instance.GetBattleBlockOnUnit();

        //유닛이 없었을 경우 도 체크 하기 위해서 
        if (blockonUnitList.Count != 0)
        {
            foreach (var blockOnUnit in blockonUnitList)
            {
                if (blockOnUnit.GetUnitNormal().unitController.isAlive)
                    allDie = false;
            }
        }
        if (allDie)
            EndBattle(false);
        
    }
    
    /// <summary>
    /// NOTE : EnemyUnit이 죽었을 떄 체크
    /// </summary>
    public void CheckEnemyUnitAlive()
    {
        bool allDie = true;
        //유닛이 없었을 경우 도 체크 하기 위해서 
        var enemyunitlist = BoardManager.instance.currentEnemyUnitList;
        if(enemyunitlist.Count!=0)
        {
            foreach (var unit in enemyunitlist)
            {
                if (unit.unitblockSc.unitController.isAlive)
                    allDie = false;
            }
        }

        if (allDie)
            EndBattle(true);
    }

    /// <summary>
    /// NOTE : EndBattle 프로세스 실행
    /// </summary>
    /// <param name="iswin"></param>
    public void EndBattle(bool iswin)
    {
        
        if (iswin)
        {
            foreach (var blockOnUnit in BoardManager.instance.GetBattleBlockOnUnit())
            {
                //살아있는 유닛들 Victory 애니매이션 실행
                if (blockOnUnit.GetUnitNormal().unitController.isAlive)
                    blockOnUnit.GetUnitNormal().unitController.SetVictory();
            }
        }
        else
        {
            foreach (var enemyUnit in BoardManager.instance.currentEnemyUnitList)
            {
                //살아있는 유닛들 Victory 애니매이션 실행
                if (enemyUnit.isAlive)
                    enemyUnit.SetVictory();
            }
        }
       

        if (!isRunningBattleEnd)
            StartCoroutine(EndBattleProcess(iswin, 3f));
    }

    protected bool isRunningBattleEnd = false;

    IEnumerator EndBattleProcess(bool iswin, float time)
    {
        //Round 승리 패배 이미지 띄우기
        isRunningBattleEnd = true;
        yield return new WaitForSeconds(time);
        StartWaitState(waitingTime, iswin);
        isRunningBattleEnd = false;
    }

    /// <summary>
    /// NOTE : 전투가 끝나고 패배했을 경우 남아있는 몬스터마다의 데미지 만큼 체력 감소 
    /// </summary>
    private void TakeDamage()
    {
        var mList = BoardManager.instance.currentEnemyUnitList;

        if (mList.Count == 0)
            return;
        foreach (var m in mList)
        {
            //몬스터가 존재하고 있을경우 데미지 감소 처리
            if (m.gameObject.activeSelf)
                playerData.HpValue -= m.unitPdata.cost;
        }
        //체력 이펙트 발생 및 몬스터 유닛에서 애니매이션 처리
    }

    //유닛들이 죽을때마다, 적이 죽을때마다 실행할 함수 필요
    #endregion
}

public class PlayerData
{
    private int _hpValue;
    public int HpValue
    {
        get { return _hpValue; }
        set
        {
            _hpValue = value;
            if (_hpValue > 100)
                _hpValue = 100;
            if (_hpValue < 0)
                _hpValue = 0;
            UIManager.instance.SetHpText(_hpValue);
        }
    }

    //플레이어 데이터
    private int _level;
    public int Level
    {
        get { return _level; }
        set
        {
            _level = value;
            UIManager.instance.SetLevelText(_level);
        }
    }
    private int _expValue;
    public int ExpValue
    {
        get { return _expValue; }
        set
        {
            _expValue = value;
            while(ExpValue>=DataBaseManager.instance.expRequireValueList[Level])
            {
                _expValue = _expValue - DataBaseManager.instance.expRequireValueList[Level];
                Level++;
            }
            if (Level >= 7)
                _expValue = 0;
            UIManager.instance.SetExpText(_expValue, DataBaseManager.instance.expRequireValueList[Level]);
        }
    }
    
    private int _currentFieldUnitNumber;
    public int CurrentFieldUnitNumber
    {
        get { return _currentFieldUnitNumber; }
        set
        {
            _currentFieldUnitNumber = value;
            UIManager.instance.SetUnitNumberText(_currentFieldUnitNumber);
        }
    }
    private int _Gold;
    public int Gold
    {
        get { return _Gold; }
        set
        {
            _Gold = value;
            UIManager.instance.SetGoldText(_Gold);
            UIManager.instance.SetBackGroundColorButtonPanels(_Gold);
            //골드 UI 설정 함수 추가
            //리롤, 레벨업 또한 색상 변경
            //돈 사용하는 사운드 
            //레벨업, 라운드 끝나고 골드 수급,캐릭터를 구매할때 처리
        }
    }
    public PlayerData ()
    {
        Level = 0;
        ExpValue = 0;
        Gold = 100;
        HpValue = 100;
        CurrentFieldUnitNumber = 0;
    }
}
