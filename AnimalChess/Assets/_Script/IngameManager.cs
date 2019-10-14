using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Game_State { WaitState , BattleState }
public class IngameManager : MonoBehaviour
{
    public static IngameManager instance = null;
    public PlayerData pData;
    private TouchUnitSystem unitTouchSystem;

    private List<Unit> unitListOnBattleBoard = new List<Unit>();
    
    private readonly int waitingTime = 3;
    private readonly int goldwhenRoundFinish = 1;
    private readonly int expWhenRoundFinish = 1;

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
    private void Awake()
    {
        if (instance == null)
            instance = this;
        //대기장소에선 이동가능 보드에 올릴수 없음
    }

    private void Start()
    {
        pData = new PlayerData();
        GameStart();
    }
    
    private void GameStart()
    {
        StartWaitState(waitingTime);
    }

    #region Round
    /// <summary>
    /// NOTE : 대기 카운터 실행
    /// </summary>
    /// <param name="_count"></param>
    private void StartWaitState(int _count)
    {
        IsBattleState = false;//골드 정산
        //샵 데이터 초기화(리롤)
        UIManager.instance.SetShopPanelCharacter(pData.Level, pData.Gold);
        //체력 정산
        //라운드 설정 변경 (졌으면 유지, 이겼으면 다음라운드 진행)
        //해당 라운드 유닛 생성
        //유닛들 체력 마나 리셋
        //경험치 +1



        //대기시간 카운팅
        StartCoroutine(WaitCounter(_count));
        //유닛 이동 상태 가능
    }

    /// <summary>
    /// NOTE : 대기 카운터 코루틴
    /// </summary>
    /// <param name="_count"></param>
    /// <returns></returns>
    IEnumerator WaitCounter(int _count)
    {

        int tmpcounter = _count;
        while (tmpcounter>=0)
        {
            yield return new WaitForSecondsRealtime(1);
            tmpcounter--;
            tmpcounter = tmpcounter < 0 ? 0 : tmpcounter;
            UIManager.instance.SetCountText(tmpcounter);

            if (tmpcounter <= 0)
                StartBattleState();
        }
    }

    private void StartBattleState()
    {
        IsBattleState = true;
        //전투시작
        //returnUnitOnwaitingBoard실행
        //실행하고 어느정도 카운트 후에 전투 시작
        //유닛, 적 ai 실행
        //유닛 구매는 가능하지만 이동 배치는 불가 처리
        StartCoroutine(testBattleFinsih());
    }
    
    /// <summary>
     /// NOTE : Round
     /// </summary>
    private void ReturnUnitOnWaitingBoard()
    {
        //if -> 전투를 시작 할 때 가능한 말의 수보다 많을경우 
        //코스트가 낮은 유닛을 기준으로 board에 되돌아감
        //만약 waiting board가 가득 차있다면 판매되고 골드가 갱신
    }
    
    IEnumerator testBattleFinsih()
    {
        int count = 10;
        while(count<=0)
        {
            count--;
            yield return new WaitForSeconds(1);
            if (count <= 0)
                IsBattleState = false;
        }
    }
    
    //유닛들이 죽을때마다, 적이 죽을때마다 함수

    #endregion
}

public class PlayerData
{
    //플레이어 데이터
    private int _level;

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

    public int Level
    {
        get { return _level; }
        set
        {
            _level = value;
            UIManager.instance.SetLevelText(_level);
            UIManager.instance.SetUnitNumberText(CurrentFieldUnitNumber, _level);
        }
    }
    private int _expValue;
    public int ExpValue
    {
        get { return _expValue; }
        set
        {
            _expValue = value;
            while(ExpValue>=DataBaseManager.instance.expRequireValueList[Level-1])
            {
                _expValue = _expValue - DataBaseManager.instance.expRequireValueList[Level - 1];
                Level++;
            }
            UIManager.instance.SetExpText(_expValue, DataBaseManager.instance.expRequireValueList[Level - 1]);
        }
    }
    
    
    private int _currentFieldUnitNumber;
    public int CurrentFieldUnitNumber
    {
        get { return _currentFieldUnitNumber; }
        set
        {
            _currentFieldUnitNumber = value;
            UIManager.instance.SetUnitNumberText(_currentFieldUnitNumber, Level);
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
        Level = 1;
        ExpValue = 0;
        Gold = 10;
        HpValue = 100;
        CurrentFieldUnitNumber = 0;
    
    }
}
