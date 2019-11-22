using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Die 는 Trigget로 처리
public enum Anim_State { Idle = 0, Walk, Attack, Skill }
public class UnitController : MonoBehaviour
{
    //UnitData
    [HideInInspector]
    public      UnitBlockData unitblockSc;
    [HideInInspector]
    public      UnitData unitPdata;
    //abilityData In Battle
    public UnitAbilityData abilityDataInBattle;
    //reference TouchSystem
    [HideInInspector] 
    public      Rigidbody rb;
    protected   Animator anim;
    public      Anim_State animState = Anim_State.Idle;

    //HP
    private     float _currentHp = 0;
    private     float _currentMp = 0;
    protected   float CurrentHp
    {
        get { return _currentHp; }
        set
        {
            prevHp = _currentHp;
            _currentHp = value;
            if (_currentHp > abilityDataInBattle.totalMaxHp)
                _currentHp = abilityDataInBattle.totalMaxHp;
            if (_currentHp < 0)
            {
                
                _currentHp = 0;
                isAlive = false;
            }
            StartHpSliderProcess();
        }
    }
    protected   float CurrentMp
    {
        get { return _currentMp; }
        set
        {
            prevMp = _currentMp;
            _currentMp = value;
            if (_currentMp > abilityDataInBattle.maxMP)
                _currentMp = abilityDataInBattle.maxMP;
            if (_currentMp < 0)
                _currentMp = 0;
            StartMpSliderProcess();
        }
    }
    //slider Lerp
    public HpMpSlider hpmpSliderData = null;
    protected   float prevHp;
    protected   float prevMp;
    protected   float sliderLerpSpeed = 5f;

    //EnemyUnit
    protected List<UnitController> targetList = new List<UnitController>();
    //Move
    protected UnitPathFinding pathfind = new UnitPathFinding();
    protected List<BlockOnBoard> path = new List<BlockOnBoard>();
    protected BlockOnBoard prevTargetBlock;
    protected BlockOnBoard currentTargetBlock;
    protected int nextBlockIndexCount = 0;
    protected float currentDistance = 0;
    protected float moveSpeed = 1f;
    //Rotate
    private float rotateSpeed = 5f;
    
    [HideInInspector]
    public    bool isAlive = true;
    protected bool isFindPath = false;
    protected bool isMoving = false;
    protected bool isChangedTaget = true;
    protected bool isCloseTarget = false;
    protected bool isDieAllEnemy = false;
    protected bool isRotating = false;
    protected bool isLookatTarget = false;
    protected bool isStartAttack = false;
    protected bool isAttackCooltimeWaiting = false;
    protected bool isStartSkill = false;
    protected bool isAttacking = false;
    protected bool isVictory = false;

    protected bool IsRunningHpSliderLerp = false;
    protected bool isRunningMpSliderLerp = false;

    #region Set
    public virtual void Init()
    {
        unitblockSc = GetComponentInParent<UnitBlockData>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        
    }

    /// <summary>
    /// NOTE : 슬라이더 데이터 설정
    /// </summary>
    public virtual void StartUnitInBattle(HpMpSlider sliderdata, float waitingTime)
    {
        //ability Data 초기화
        //능력치 리셋 
        SetAbilityDataInBattle(unitPdata.abilityData);
        //시너지 효과 추가 능력치 함수 필요
        //..

        //슬라이더 초기화
        hpmpSliderData = sliderdata;
        //HP
        hpmpSliderData.hpSlider.maxValue = abilityDataInBattle.totalMaxHp;
        SetHpSlideValue(abilityDataInBattle.totalMaxHp);
        CurrentHp = abilityDataInBattle.totalMaxHp;
        //MP
        hpmpSliderData.mpSlider.maxValue = abilityDataInBattle.maxMP;
        SetMpSliderValue(0);
        CurrentMp = 0;
        //Pos
        SetPosSliderBar(unitblockSc.transform.position);
        hpmpSliderData.panel.SetActive(true);
        //AI 실행
        StartCoroutine(WaitingStartAI(waitingTime));
        //공속
        anim.SetFloat("attackSpeed",abilityDataInBattle.totalAttackSpeedRate);
    }

    /// <summary>
    /// NOTE : 전투가 끝난후 모든 데이터 리셋
    /// </summary>
    public void ResetUnitDataInWaiting()
    {
        //능력치 리셋 
        abilityDataInBattle = unitPdata.abilityData;
        // true로
        isAlive = true;
        hpmpSliderData = null;
        isVictory = false;
        isAttacking = false;

        targetList.Clear();
        //애니매이션 설정 
        anim.SetTrigger("ReStart");
        animState = Anim_State.Idle;
        anim.SetFloat("animState", (int)animState);
        
        //Rotation설정
        unitblockSc.transform.eulerAngles = new Vector3(0, 180, 0);

        //위치 설정
        unitblockSc.GetCurrentBlockInWaiting().SetUnitNotList(unitblockSc);
    }

    public void SetAbilityDataInBattle(UnitAbilityData readAbilityData)
    {
        abilityDataInBattle = readAbilityData.DeepCopy();
    }

    /// <summary>
    /// NOTE : 배틀보드 위에서 시너지를 적용받다가 대기석으로 돌아갔을때 데이터를 다시 되돌리기` 위함
    /// </summary>
    public virtual void ResetUnitDataToWatingBoard()
    { 
        SetAbilityDataInBattle(unitPdata.abilityData);
    }
    
    /// <summary>
    /// NOTE : 파라미터 값 이후로 AI실행
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    IEnumerator WaitingStartAI(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        unitblockSc.unitBTAI.StartBT();
    }
    #endregion

    #region  Move

    /// <summary>
    /// NOTE : 타겟이 없거나 타겟의 유닛이 없어졌거나 (타겟이 이동하였을 경우), 타겟이 죽었다면 타겟 다시 설정
    /// </summary>
    /// <returns></returns>
    public virtual bool SetTargetBlock()
    {
        float mindistance = 100;
        //적이 모두 죽었는지 체크
        isDieAllEnemy = true;
        //가장 가까운 거리에 있는 적 검색
        for (int i = 0; i < targetList.Count; i++)
        {
            //살아있는지 확인
            if (targetList[i].isAlive)
            {
                var currentTargetdis = Vector2Int.Distance(unitblockSc.GetCurrentBlockInBattle().groundArrayIndex, targetList[i].unitblockSc.GetCurrentBlockInBattle().groundArrayIndex);
                if (mindistance > currentTargetdis)
                {
                    mindistance = currentTargetdis;                   
                    currentTargetBlock = targetList[i].unitblockSc.GetCurrentBlockInBattle();

                    isDieAllEnemy = false;
                }
            }
        }
        currentDistance = mindistance;
        
        //모든 적이 죽었을 경우 
        if (isDieAllEnemy)
            return false;
        //처음 이전 타겟이 없을 경우 
        if (prevTargetBlock == null)
            prevTargetBlock = currentTargetBlock;
        //타겟이 변경되었을 경우 
        if (isChangedTaget = !prevTargetBlock.Equals(currentTargetBlock))
            prevTargetBlock = currentTargetBlock;
        
        return true;
    }

    /// <summary>
    /// NOTE : 타겟이 변경되었을 경우, PATH가 없을경우, 길을 가는중 길이 막혔을 경우 -> 길찾기 실행, 길을 못찾았을 경우 RETURN FALSE
    /// </summary>
    /// <returns></returns>
    public virtual bool SetPath()
    {
        //타겟이 변경되었을 경우
        if (isChangedTaget)
        {
            if (isFindPath = pathfind.FindPath(unitblockSc.GetCurrentBlockInBattle(), currentTargetBlock, BoardManager.instance.allGroundBlocks, ref path))
                nextBlockIndexCount = path.Count - 2;

        } 
        //길이 없을 경우
        else if (path.Count == 0)
        {
            if (isFindPath = pathfind.FindPath(unitblockSc.GetCurrentBlockInBattle(), currentTargetBlock, BoardManager.instance.allGroundBlocks, ref path))
                nextBlockIndexCount = path.Count - 2;

        }
        //길을 막았을 경우 
        else if (nextBlockIndexCount >= 0)
        {
            if (path[nextBlockIndexCount].GetUnitInBattle() != null)
            {
                if (isFindPath = pathfind.FindPath(unitblockSc.GetCurrentBlockInBattle(), currentTargetBlock, BoardManager.instance.allGroundBlocks, ref path))
                    nextBlockIndexCount = path.Count - 2;
            }
        }
        return isFindPath;
    }

    /// <summary>
    /// NOTE : 이동중이거나, 목적지에 도착했거나, 길을 찾지 못했을 경우 이동실행 X
    /// </summary>
    /// <returns></returns>
    public virtual bool CheckMoveState()
    {
        //움직이는 중일 경우 return
        if (isMoving)
            return false;
        //목표에 접근했을 경우 Target에서 설정할 경우 실제 이동중에 이미 공격 처리중
        if (isCloseTarget = ((abilityDataInBattle.attackRange + 0.5f) > currentDistance))
            return false;
        return true;
    }

    /// <summary>
    /// NOTE : 이동 코루틴 실행
    /// </summary>
    public virtual void StartMoveToNextBlock()
    {
        StartCoroutine(MoveNextBlock(unitblockSc.GetCurrentBlockInBattle(), path[nextBlockIndexCount]));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="currentBlock"></param>
    /// <param name="nextblock"></param>
    /// <returns></returns>
    IEnumerator MoveNextBlock(BlockOnBoard currentBlock, BlockOnBoard nextblock)
    {
        //갈 block 자신의 위치로 변경 이전길 null로 초기화
        isMoving = true;
        nextblock.SetUnitInBattle(unitblockSc);
        currentBlock.SetUnitInBattle(null);
        nextBlockIndexCount--;
        float count = 0;
        //lerp포지션
        Vector3 startpos = unitblockSc.transform.position;
        Vector3 nextpos = nextblock.transform.position;
        nextpos.y = 0;
        //방향
        var dir = nextpos - startpos;
        dir = dir.normalized;
        Quaternion currentRot = unitblockSc.transform.rotation;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        
        //초기화
        while (unitblockSc.transform.position != nextpos&&isAlive)
        {
            count += Time.deltaTime;
            unitblockSc.transform.rotation = Quaternion.Lerp(currentRot, targetRot, count * rotateSpeed);
            unitblockSc.transform.position = Vector3.Lerp(startpos, nextpos, moveSpeed * count);

            SetPosSliderBar(unitblockSc.transform.position);
            yield return new WaitForFixedUpdate();
        }
        isMoving = false;
    }

    #endregion

    #region Attack
    //공격
    public virtual bool CheckAttackRangeCondition()
    {
        return isCloseTarget;
    }

    /// <summary>
    /// NOTE : 타겟을 바라보는 rotation
    /// </summary>
    /// <returns></returns>
    public virtual bool LookAtTarget()
    {
        //공격중일때 회전 되지 않도록
        if (isAttacking)
            return false;

        if (!isRotating)
        {
            //block position y 값은 0으로 초기화 
            var targetpos = currentTargetBlock.transform.position;
            targetpos.y = 0;
            //방향벡터
            Vector3 dirVector = targetpos - unitblockSc.transform.position;
            dirVector = dirVector.normalized;
            //방향벡터를 통한 rotation값
            Quaternion targetRot = Quaternion.LookRotation(dirVector);

            //바라보는 방향이 다를 경우 코루틴실행
            //ROTATION 값은 -값이 서로 다른 축에서 반대 값을 가지는 경우가 발생 eulerAngle값으로 처리
            if (!(isLookatTarget = (Quaternion.Angle(unitblockSc.transform.rotation, targetRot) == 0)))
                StartCoroutine(StartRotate(Quaternion.LookRotation(dirVector)));
        }

        return isLookatTarget;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator StartRotate(Quaternion targetRot)
    {
        isRotating = true;
        Quaternion currentrot = unitblockSc.transform.rotation;

        float count = 0;
        //angle값이 0 이면 rotation값 일치
        while (Quaternion.Angle(unitblockSc.transform.rotation, targetRot) != 0)
        {
            count += Time.deltaTime;
            unitblockSc.transform.rotation = Quaternion.Lerp(currentrot, targetRot, count * rotateSpeed);
            yield return new WaitForFixedUpdate();
        }
        isRotating = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public virtual bool CheckAttackCondition()
    {
        //상대방의 상태가 moving중인 경우 return
        if (currentTargetBlock.unitInBattle.unitController.isMoving)
            return false;
        if (isAttackCooltimeWaiting)
            return false;
        return true;
    }

    /// <summary>
    /// NOTE : 
    /// </summary>
    public virtual void AttackAction()
    {
        isStartAttack = true;
    }

    /// <summary>
    /// Animation event
    /// </summary>
    public virtual void StartAttackInAttackAnim()
    {
        isStartAttack = false;
        isAttacking = true;
        StartCoroutine(SetAttackCoolTime());
        //StartCoroutine(AttackHitProcess());
        
    }
    /// <summary>
    /// NOTE : 근접 공격, 원거리 공격 분류 해야함
    /// </summary>
    public virtual void AttackHit()
    {
        var target = currentTargetBlock.unitInBattle;
        if (target != null)
        {
            //방어력을 제외한 데미지 값
            int resultdamage = (int)(abilityDataInBattle.totalAttackDamage * abilityDataInBattle.physicaldefenseRate);
            //Drain
            CurrentHp += resultdamage * unitPdata.abilityData.drainHp;
            target.unitController.TakeDamagePhysics(resultdamage);
            //원거리 공격 애매한상태
            //..이펙트
        }   
    }
    
    /// <summary>
    /// NOTE : 공격 애니매이션 이벤트 함수
    /// </summary>
    public virtual void EndAttackInAttackAnim()
    {
        isAttacking = false;
    }
    
    /// <summary>
    /// NOTE : 쿨타임 상태 설정
    /// </summary>
    /// <returns></returns>
    IEnumerator SetAttackCoolTime()
    {
        isAttackCooltimeWaiting = true;
        yield return new WaitForSecondsRealtime(abilityDataInBattle.attackCooltime * (1f - (abilityDataInBattle.attackSpeedRateSynergy * 0.01f)));
        isAttackCooltimeWaiting = false;
    }
    #endregion

    #region Skill
    //스킬
    public virtual bool CheckSkillCondition() { return true; }
    public virtual void SkillAction() { }
    #endregion

    #region  Dead
    //죽음
    public virtual bool IsDie() {

        return !isAlive;
    }
    public virtual void DeadAction()
    {
        hpmpSliderData.panel.SetActive(false);
        anim.SetTrigger("isDie");

        unitblockSc.GetCurrentBlockInBattle().SetUnitInBattle(null);
        unitblockSc.unitBTAI.StopBT();
        
        //죽음 
    }

    
    #endregion

    #region Animation
    public virtual void SetAnimation()
    {

        animState = Anim_State.Idle;
        if (isMoving)
            animState = Anim_State.Walk;
        if (isStartAttack)
            animState = Anim_State.Attack;
        if (isStartSkill)
            animState = Anim_State.Skill;
        

        anim.SetFloat("animState", (int)animState);

    }

    #endregion

    #region ETC
    /// <summary>
    /// NOTE : 슬라이더 포지션 변경
    /// </summary>
    /// <param name="pos"></param>
    private void SetPosSliderBar(Vector3 pos)
    {
        pos.y = +2;
        if(hpmpSliderData!=null)
            hpmpSliderData.panel.transform.position = Camera.main.WorldToScreenPoint(pos);
    }
    /// <summary>
    /// NOTE : HP LERP 슬라이더 코루틴 실행
    /// </summary>
    private void StartHpSliderProcess()
    {
        if (hpmpSliderData == null)
            return;
        if (!IsRunningHpSliderLerp)
            StartCoroutine(HpSliderProcess());
    }
    /// <summary>
    /// NOTE : HP UI 값 설정
    /// </summary>
    /// <param name="hp"></param>
    private void SetHpSlideValue(float hp)
    {
        hpmpSliderData.hpSlider.value = hp;
        hpmpSliderData.hpText.text = ((int)hp).ToString();
    }
    
    /// <summary>
    /// NOTE : HP SLIDER VALUE lerp
    /// </summary>
    /// <returns></returns>
    private IEnumerator HpSliderProcess()
    {
        IsRunningHpSliderLerp = true;
        var tmpprevhp = prevHp; 
        float count = 0;
        while (hpmpSliderData.hpSlider.value != CurrentHp)
        {
            count += Time.fixedDeltaTime;
            var hpvalue= Mathf.Lerp(tmpprevhp, CurrentHp, count * sliderLerpSpeed);
            SetHpSlideValue(hpvalue);
            yield return new WaitForFixedUpdate();
        }
        IsRunningHpSliderLerp = false;
    }

    /// <summary>
    /// NOTE : MP LERP 슬라이더 코루틴 실행
    /// </summary>
    private void StartMpSliderProcess()
    {
        if (hpmpSliderData == null)
            return;
        if (!isRunningMpSliderLerp)
            StartCoroutine(MpSliderProcess());
    }

    /// <summary>
    /// NOTE : MP 슬라이더 UI 값 설정
    /// </summary>
    /// <param name="mp"></param>
    private void SetMpSliderValue(float mp)
    {
        hpmpSliderData.mpSlider.value = mp;
        hpmpSliderData.mpText.text = ((int)mp).ToString();
    }

    /// <summary>
    /// NOTE : M P SLIDER VALUE lerp
    /// </summary>
    /// <returns></returns>
    private IEnumerator MpSliderProcess()
    {
        isRunningMpSliderLerp = true;
        var tmpprevmp = prevMp;
        float count = 0;
        while(hpmpSliderData.mpSlider.value != CurrentMp)
        {
            count += Time.fixedDeltaTime;

            var mpvalue = Mathf.Lerp(tmpprevmp, CurrentMp, count * sliderLerpSpeed);
            SetMpSliderValue(mpvalue);
            yield return new WaitForFixedUpdate();
        }
        isRunningMpSliderLerp = false;
    }
    #endregion
    
    /// <summary>
    /// NOTE : 물리데미지
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamagePhysics(int damage)
    {
        //회피 랜덤설정 
        if (Random.Range(0, 100) < abilityDataInBattle.avoidanceRate)
            return;


        CurrentHp -= damage;
        CurrentMp += damage * 0.2f;
        //이펙트 생성
    }

    public virtual bool CheckVictroy()
    {
        return isVictory;
    }

    public virtual void StartVictoryAnimation()
    {
        anim.SetTrigger("IsVictory");
        unitblockSc.GetCurrentBlockInBattle().SetUnitInBattle(null);
        unitblockSc.unitBTAI.StopBT();
    }

    public void SetVictory()
    {
        isVictory = true;
    }

    public virtual void SetTribeEffect(int level) { }
    public virtual void SetAttributeEffect(int level) { }
}


////목적지가 없을때
//if (currentTargetBlock == null
//  || currentTargetBlock.GetUnitInBattle() == null
//  || !currentTargetBlock.GetUnitInBattle().unitController.isAlive)
//{
//    //몬스터 리스트 
//    var mlist = BoardManager.instance.currentMonsterList;

//    float mindistance = 100;
//    //적이 모두 죽었는지 체크
//    isDieAllEnemy = true;
//    //가장 가까운 거리에 있는 적 검색
//    for (int i = 0; i < mlist.Count; i++)
//    {
//        //살아있는지 확인
//        if (mlist[i].isAlive)
//        {
//            var currentdis = Vector2Int.Distance(unitblockSc.GetCurrentBlockInBattle().groundArrayIndex, mlist[i].unitblockSc.GetCurrentBlockInBattle().groundArrayIndex);
//            if (mindistance > currentdis)
//            {
//                mindistance = currentdis;
//                currentTargetBlock = mlist[i].unitblockSc.GetCurrentBlockInBattle();
//                isDieAllEnemy = false;
//            }
//        }
//    }
//}
//if (currentTargetBlock == null)
//    return false;