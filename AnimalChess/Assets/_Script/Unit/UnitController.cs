using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Die 는 Trigget로 처리
public enum Anim_State { Idle = 0, Walk, Attack, Skill, Stun }
public class UnitController : MonoBehaviour
{
    //UnitData
    [HideInInspector]
    public UnitBlockData unitblockSc;
    [HideInInspector]
    public UnitData unitPdata;
    //abilityData In Battle
    public UnitAbilityData abilityDataInBattle;
    //reference TouchSystem
    [HideInInspector] 
    public    Rigidbody rb;
    protected Animator anim;
    private   SkinnedMeshRenderer meshR;
    private   Anim_State animState = Anim_State.Idle;

    //HP
    private float _currentHp = 0;
    private float _currentMp = 0;
    protected float CurrentHp
    {
        get { return _currentHp; }
        set
        {
            prevHp = _currentHp;
            _currentHp = value;
            if (_currentHp > abilityDataInBattle.totalMaxHp)
                _currentHp = abilityDataInBattle.totalMaxHp;
            if (_currentHp <= 0)
            {
                
                _currentHp = 0;
                isAlive = false;
            }
            StartHpSliderProcess();
        }
    }
    protected float CurrentMp
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
    public  HpMpSlider hpmpSliderData = null;
    private float prevHp;
    private float prevMp;
    private float sliderLerpSpeed = 5f;

    //EnemyUnit
    protected List<UnitController> targetList = new List<UnitController>();
    //Move
    private   UnitPathFinding pathfind = new UnitPathFinding();
    private   List<BlockOnBoard> path = new List<BlockOnBoard>();
    private   BlockOnBoard prevTargetBlock;
    protected BlockOnBoard currentTargetBlock;
    private   int nextBlockIndexCount = 0;
    private   float currentDistance = 0;
    private   float moveSpeed = 1f;
    //Rotate
    private float rotateSpeed = 5f;
    //Synergy
    private int _tribeSynergyLevel = 0;
    private int _attributeSynergyLevel = 0;
    public  int TribeSynergyLevel
    {
        get { return _tribeSynergyLevel; }
        set
        {
            _tribeSynergyLevel = value;
            tribeSynergy.SetSynergy(_tribeSynergyLevel);
        }
    }
    public  int AttributeSynergyLevel
    {
        get { return _attributeSynergyLevel; }
        set
        {
            _attributeSynergyLevel = value;
            attributeSynergy.SetSynergy(_attributeSynergyLevel);
        }
    }
    protected Synergy tribeSynergy;
    protected Synergy attributeSynergy;
    //Attack
    private  Transform attackProjectileParent;
    private  NormalAttackProjectileSc[] attackProjectileArray;
    private  int projectileCount = 0;
    //Skill
    protected Transform skillParent;
    protected GameObject skillEffect;
    //Debuff
    // KnockBack
    private BlockOnBoard knockBackBlock = null;
    /// Stun
    private Transform debuffEffectParent;
    private GameObject stunEffect;
    private float stunEndTime = 0;
    private float stunCount = 0;
    /// Slow
    private GameObject waterSlowEffect;
    private Color originalColor;
    private Color waterSlowColor = new Color(0.6f, 0.8f, 1f);
    private float waterSlowEndTime = 0;
    private float waterSlowCount = 0;
    /// Provoked
    private GameObject provokedEffect;
    float provokedEndTime = 0;
    float provokedCount = 0;
    //Freezing
    private GameObject freezingEffect;
    float freezingEndTime = 0;
    float freezingCount = 0;
    private Color freezingColor = new Color(0f, 0f, 1f);
    //Bool
    [HideInInspector]
    public  bool isAlive = true;
    private bool isFindPath = false;
    private bool isMoving = false;
    private bool isChangedTaget = true;
    private bool isCloseTarget = false;
    private bool isDieAllEnemy = false;
    private bool isRotating = false;
    private bool isLookatTarget = false;
    private bool isStartAttack = false;
    private bool isAttackCooltimeWaiting = false;
    private bool isAttacking = false;
    private bool isStartSkill = false;
    private bool isCastingSkill = false;
    protected bool isRunningSkill = false;
    private bool isVictory = false;
    private bool isStun = false;
    private bool isWaterSlow = false;
    private bool isProvoked = false;
    private bool isKnockBack = false;
    private bool isFreezing = false;

    private bool IsRunningHpSliderLerp = false;
    private bool isRunningMpSliderLerp = false;
    
    #region Set
    public virtual void Init()
    {
        unitblockSc = GetComponentInParent<UnitBlockData>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        meshR = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    /// <summary>
    /// NOTE : 슬라이더 데이터 설정
    /// </summary>
    public virtual void StartUnitInBattle(HpMpSlider sliderdata, float waitingTime)
    {
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
        SetAbilityDataInBattle(unitPdata.abilityData);
        // true로
        isAlive = true;
        hpmpSliderData = null;
        isVictory = false;
        isAttacking = false;
        isStun = false;
        isCastingSkill = false;
        isKnockBack = false;
        isStartSkill = false;
        isFreezing = false;

        waterSlowEndTime = 0;
        stunEndTime = 0;

        targetList.Clear();
        path.Clear();
        //애니매이션 설정 
        anim.SetTrigger("ReStart");
        animState = Anim_State.Idle;
        anim.SetFloat("animState", (int)animState);
        
        //Rotation설정
        unitblockSc.transform.eulerAngles = new Vector3(0, 180, 0);

        //위치 설정
        unitblockSc.GetCurrentBlockInWaiting().SetUnitNotList(unitblockSc);
    }

   

    /// <summary>
    /// NOTE : 배틀보드 위에서 시너지를 적용받다가 대기석으로 돌아갔을때 데이터를 다시 되돌리기` 위함
    /// </summary>
    public virtual void SetUnitAbilityDataToNormalData()
    { 
        SetAbilityDataInBattle(unitPdata.abilityData);
    }

    public void SetAbilityDataInBattle(UnitAbilityData readAbilityData)
    {
        abilityDataInBattle = readAbilityData.DeepCopy();
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

    /// <summary>
    /// NOTE : 발사체 초기화
    /// </summary>
    public virtual void SetObjectData()
    {
        string parentname = "AttackProjectileParent";

        if (unitPdata.tribe == Tribe_Type.Rabbit)
            parentname = "FireballProjectileParent";

        attackProjectileParent = transform.Find(parentname);
        attackProjectileArray = attackProjectileParent.GetComponentsInChildren<NormalAttackProjectileSc>(true);

        var count = 0;
        while (count != 3)
        {
            var hitob = GameObject.Instantiate(DataBaseManager.instance.hitEffectDic["NormalHitEffect"], this.transform);

            hitob.transform.localPosition = Vector3.zero;
            hitob.gameObject.SetActive(false);
            attackProjectileArray[count].hiteffect = hitob;
            attackProjectileArray[count].unit = this;
            count++;
        }

        //디버프 이펙트
        debuffEffectParent = unitblockSc.transform.Find("DebuffEffect").transform;
        stunEffect = debuffEffectParent.Find("Stun").gameObject;
        provokedEffect = debuffEffectParent.Find("Provoked").gameObject;
        waterSlowEffect = debuffEffectParent.Find("WaterSlow").gameObject;
        mpHealingEffect = debuffEffectParent.Find("MpHealing").gameObject;
        freezingEffect = debuffEffectParent.Find("Freezing").gameObject;
        //Color
        originalColor = meshR.material.color;

        skillParent = unitblockSc.transform.Find("Skill").transform;
        skillEffect = skillParent.transform.Find("SkillEffect").gameObject;
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

        //도발 당했을 경우 거리에 상관없이 타겟은 고정되도록 함 타겟이 죽었을 경우 해제
        if (isProvoked)
        {
            //해당 캐릭터가 죽을 경우 보드는 NULL로 변경되기 때문에 (죽음과 같은 처리)
            if (currentTargetBlock.unitInBattle==null)
            {
                isProvoked = false;
                return false;
            }
            return true;
        }

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

    //MoveNextBlock(unitblockSc.GetCurrentBlockInBattle(), path[nextBlockIndexCount]);
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
        while (unitblockSc.transform.position != nextpos&&isAlive&&!isKnockBack)
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
    /// <summary>
    /// NOTE : 현재 공격상태면 진행하지 않음 CheckAttackRange와 lookatTarget을 skill에서도 사용해야 하므로 설정
    /// </summary>
    /// <returns></returns>
    public virtual bool CheckAttacking()
    {
        return !isAttacking;
    }
    
    /// <summary>
    /// NOTE : 현재 사거리가 맞지 않으면 진행하지 않음
    /// </summary>
    /// <returns></returns>
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
        if (!isRotating)
        {
            //block position y 값은 0으로 초기화 
            var targetpos = currentTargetBlock.transform.position;
            targetpos.y = 0;
            //방향벡터
            var dirVector = targetpos - unitblockSc.transform.position;
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
    public IEnumerator StartRotate(Quaternion targetRot)
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
    /// NOTE : 공격 가능한상태인지 확인
    /// </summary>
    /// <returns></returns>
    public virtual bool CheckAttackCondition()
    {
        if (currentTargetBlock.unitInBattle == null)
            return false;
        if (isAttackCooltimeWaiting)
            return false;
        return true;
    }

    /// <summary>
    /// NOTE : AttackAnimation 상태 체크
    /// </summary>
    public virtual void AttackAction()
    {
        isStartAttack = true;
    }

    /// <summary>
    /// NOTE : Attack Animation event function 공격 애니매이션이 시작할때 실행
    /// </summary>
    public virtual void StartAttackInAttackAnim()
    {
        isStartAttack = false;
        isAttacking = true;
        StartCoroutine(SetAttackCoolTime());
        
    }

    /// <summary>
    /// NOTE : Attack Animation event Function 공격이 피격 할 쯤 실행
    /// </summary>
    public virtual void AttackFire()
    {
        var target = currentTargetBlock.unitInBattle;
        if (target != null)
        {
            attackProjectileArray[projectileCount].target = target;
            attackProjectileArray[projectileCount].gameObject.SetActive(true);
            attackProjectileArray[projectileCount].SetData(abilityDataInBattle.totalAttackDamage);
            projectileCount++;
            projectileCount = projectileCount % 3;
        }
    }

    /// <summary>
    /// NOTE :  Attack Animation event function 공격 애니매이션이 끝날때 실행
    /// </summary>
    public virtual void EndAttackInAttackAnim()
    {
        isAttacking = false;
    }
    
    /// <summary>
    /// NOTE : TakeDamage에서 최종 공격력에 따라 사용
    /// </summary>
    /// <param name="damage"></param>
    public virtual void DrainHp(int damage)
    {
        CurrentHp += damage * unitPdata.abilityData.drainHp;
    }
    
    /// <summary>
    /// NOTE : 쿨타임 상태 설정
    /// </summary>
    /// <returns></returns>
    IEnumerator SetAttackCoolTime()
    {
        isAttackCooltimeWaiting = true;
        yield return new WaitForSecondsRealtime(abilityDataInBattle.attackCooltime * (1f - (abilityDataInBattle.attackSpeedValueSynergy * 0.01f)));
        isAttackCooltimeWaiting = false;
    }
    #endregion

    #region Skill

    /// <summary>
    /// NOTE : 스킬애니매이션이 종료되었는지 체크, 및 스킬사용중 이동하지 않도록
    /// </summary>
    /// <returns></returns>
    public virtual bool CheckCastingSkillCondition()
    {
        return !isCastingSkill;
    }
    /// <summary>
    /// NOTE : 스킬을 사용할수 있는지 상태 체크
    /// </summary>
    /// <returns></returns>
    public virtual bool CheckSkillCondition()
    {
        
        //지속시간이 있는 스킬의 경우에 체크 즉발식의 경우에는 모두 false로 변경되지 않음
        if (isRunningSkill)
            return false;


        return CurrentMp == abilityDataInBattle.maxMP ? true : false;
    }
    
    public virtual void SkillAction()
    {
        isStartSkill = true;
    }

    /// <summary>
    /// NOTE : skill animation Event function , 스킬애니매이션이 시작부분 이벤트
    /// </summary>
    public virtual void StartSkillInAnim()
    {
        CurrentMp = 0;
        isStartSkill = false;
        isCastingSkill = true;
        //CoolTime?
    }

    /// <summary>
    /// NOTE : skill animation Event function . 스킬애니매이션 중간부분 이벤트 
    /// </summary>
    public virtual void SkillActionInAnim()
    {
       
    }

    /// <summary>
    /// NOTE : skill animation Event function . 스킬애니매이션 끝부분 이벤트 
    /// </summary>
    public virtual void EndSkillInAnim()
    {
        isCastingSkill = false;
    }
    
    #endregion

    #region  Dead
    /// <summary>
    /// NOTE : 죽음
    /// </summary>
    /// <returns></returns>
    public virtual bool IsDie() {

        return !isAlive;
    }

    /// <summary>
    /// NOTE : 애니매이션 변경 및 SLIDER 종료,
    /// </summary>
    public virtual void DeadAction()
    {
        hpmpSliderData.panel.SetActive(false);
        anim.SetTrigger("isDie");

        unitblockSc.GetCurrentBlockInBattle().SetUnitInBattle(null);
        unitblockSc.unitBTAI.StopBT();
        //죽음 
    }


    #endregion

    #region Victory
    /// <summary>
    /// NOTE : isVictory 변경
    /// </summary>
    public void SetVictory()
    {
        isVictory = true;
    }
    /// <summary>
    /// NOTE : Victory 체크 
    /// </summary>
    /// <returns></returns>
    public virtual bool CheckVictroy()
    {
        return isVictory;
    }

    /// <summary>
    /// NOTE : 애니매이션 실행 및 BATTILE UNIT NULL 처리, AI 종료
    /// </summary>
    public virtual void StartVictoryAnimation()
    {
        anim.SetTrigger("IsVictory");
        unitblockSc.GetCurrentBlockInBattle().SetUnitInBattle(null);
        unitblockSc.unitBTAI.StopBT();
    }

    #endregion

    #region Animation
    public virtual void SetAnimation()
    {
        if (isStun)
            animState = Anim_State.Stun;
        else if (isStartSkill)
            animState = Anim_State.Skill;
        else if (isStartAttack)
            animState = Anim_State.Attack;
        else if (isMoving)
            animState = Anim_State.Walk;
        else
            animState = Anim_State.Idle;
        
        anim.SetFloat("animState", (int)animState);
    }

    #endregion

    #region Slider
    /// <summary>
    /// NOTE : 슬라이더 포지션 변경
    /// </summary>
    /// <param name="pos"></param>
    protected void SetPosSliderBar(Vector3 pos)
    {
        pos.y = +2;
        if (hpmpSliderData != null)
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
            var hpvalue = Mathf.Lerp(tmpprevhp, CurrentHp, count * sliderLerpSpeed);
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
        while (hpmpSliderData.mpSlider.value != CurrentMp)
        {
            count += Time.fixedDeltaTime;

            var mpvalue = Mathf.Lerp(tmpprevmp, CurrentMp, count * sliderLerpSpeed);
            SetMpSliderValue(mpvalue);
            yield return new WaitForFixedUpdate();
        }
        isRunningMpSliderLerp = false;
    }
    #endregion

    #region Debuff
    
    /// <summary>
    /// NOTE : 타겟으로 받은 넉백
    /// </summary>
    /// <param name="target"></param>
    public void StartKnockBack(Vector3 targetposition)
    {
        isKnockBack = true;
        var startpos = unitblockSc.transform.position;
        var targetpos = targetposition;
        var offsetvaluepos = startpos - targetpos;
        var knockbackpos = startpos + offsetvaluepos;
        
        int x = (int)(knockbackpos.x * 0.5f);
        int z = (int)(knockbackpos.z * 0.5f);

        x = x < 0 ? 0 : x;
        x = x > 9 ? 9 : x;
        z = z < 1 ? 1 : z;
        z = z > 9 ? 9 : z;
        
        knockBackBlock = BoardManager.instance.allGroundBlocks[x, z];
        if (knockBackBlock.GetUnitInBattle() == null)
            StartCoroutine(KnockBackProcess(unitblockSc.GetCurrentBlockInBattle(), knockBackBlock));
        //이동이 불가한 지역이면 위치는이동하지 않고 스턴만 처리 (전투지역이 아닌경우, 넉백위치에 유닛이 있을경우)
        isKnockBack = false;
        StartStun(2F);
    }

    IEnumerator KnockBackProcess(BlockOnBoard currentBlock, BlockOnBoard nextblock)
    {
        isKnockBack = true;
        nextblock.SetUnitInBattle(unitblockSc);
        currentBlock.SetUnitInBattle(null);
        float count = 0;
        //lerp포지션
        Vector3 startpos = unitblockSc.transform.position;
        Vector3 nextpos = nextblock.transform.position;
        nextpos.y = 0;

        //초기화
        while (unitblockSc.transform.position != nextpos)
        {
            count += Time.deltaTime;
            unitblockSc.transform.position = Vector3.Lerp(startpos, nextpos, 10f * count);

            SetPosSliderBar(unitblockSc.transform.position);
            yield return new WaitForFixedUpdate();
        }
        isKnockBack = false;
    }

    /// <summary>
    /// NOTE : 도발당함
    /// </summary>
    public void StartProvoked(float time, UnitController target)
    {
        provokedCount = 0;
        provokedEndTime = time;
        currentTargetBlock = target.unitblockSc.GetCurrentBlockInBattle();
        if (!isProvoked)
            StartCoroutine(BeProvokedProcess());
    }   
    /// <summary>
    /// NOTE : 스턴 프로세스 
    /// </summary>
    /// <returns></returns>
    IEnumerator BeProvokedProcess()
    {
        //애니매이션 상태설정
        isProvoked = true;
        provokedEffect.gameObject.SetActive(true);
        //스턴 이펙트 생성
        //스턴 애니매이션 설정
        while (provokedCount < provokedEndTime && isAlive && !isVictory)
        {
            provokedCount++;
            yield return new WaitForSeconds(1f);
        }
        isProvoked = false;
        provokedEffect.gameObject.SetActive(false);
        provokedEndTime = 0;
    }

    public void StartKnockBack(Transform target)
    {
        
    }

    /// <summary>
    /// NOTE : 스턴 
    /// </summary>
    public void StartStun(float time)
    {
        stunCount = 0;
        stunEndTime = time;
        if (!isStun)
        {
            StartCoroutine(StunProcess());
        }
    }

    /// <summary>
    /// NOTE : 스턴 프로세스 
    /// </summary>
    /// <returns></returns>
    IEnumerator StunProcess()
    {
        //공격, 스킬 중 취소되었을 경우 Moving의 경우 coroutine으로 실행하기 때문에 스턴이 걸려도 한칸을 이동하는건 진행한다
        isAttacking = false;
        isCastingSkill = false;

        //애니매이션 상태설정
        isStun = true;
        stunEffect.gameObject.SetActive(true);

        //스턴 이펙트 생성
        //스턴 애니매이션 설정
        while (stunCount < stunEndTime && isAlive && !isVictory)
        {
            stunCount++;
            yield return new WaitForSeconds(1f);
        }
        isStun = false;
        stunEffect.gameObject.SetActive(false);
        stunEndTime = 0;
    }

    /// <summary>
    /// NOTE : 스턴, 넉백등 상태이상 일 경우 공격등 처리하지 않도록 하기 위함
    /// </summary>
    /// <returns></returns>
    public virtual bool CheckDebuffState()
    {
        return isStun || isKnockBack;
    }

    /// <summary>
    /// NOTE : 빙결상태 슬로우보다 애니매이션속도 0.1로 감소 (SLOW는 0.5)
    /// </summary>
    /// <param name="time"></param>
    public void StartFreezing(float time)
    {
        freezingCount = 0;
        freezingEndTime = time;
        if (!isFreezing)
            StartCoroutine(FreezingProcess());
        
    }

    IEnumerator FreezingProcess()
    {
        isFreezing = true;
        anim.speed = 0.1f;
        moveSpeed = 0.1f;
        freezingEffect.gameObject.SetActive(true);
        meshR.material.color = freezingColor;
        while (freezingCount < freezingEndTime && isAlive && !isVictory)
        {
            freezingCount++;
            yield return new WaitForSeconds(1f);
        }
        freezingEndTime = 0;
        anim.speed = 1f;
        moveSpeed = 1f;
        freezingEffect.gameObject.SetActive(false);
        meshR.material.color = originalColor;
        isFreezing = false;
    }

    /// <summary>
    /// NOTE : 
    /// </summary>
    /// <param name="time"></param>
    public void StartWaterSlow(float time)
    {
        waterSlowCount = 0;
        waterSlowEndTime = time;
        if (!isWaterSlow)
            StartCoroutine(WaterSlowProcess());
    }

    /// <summary>
    /// NOTE : 애니매이션 속도 감소 , MOVESPEED감소
    /// </summary>
    /// <returns></returns>
    IEnumerator WaterSlowProcess()
    {
        isWaterSlow = true;
        waterSlowEffect.gameObject.SetActive(true);
        meshR.material.color = waterSlowColor;
        while (waterSlowCount < waterSlowEndTime && isAlive && !isVictory)
        {
            waterSlowCount++;
            if (!isFreezing)
            {
                anim.speed = 0.5f;
                moveSpeed = 0.5f;
            }
            yield return new WaitForSeconds(1f);
        }
        waterSlowEndTime = 0;
        anim.speed = 1f;
        moveSpeed = 1f;
        waterSlowEffect.gameObject.SetActive(false);
        meshR.material.color = originalColor;
        isWaterSlow = false;
    }

    #endregion

    #region Buff
    private GameObject mpHealingEffect;
    /// <summary>
    /// NOTE : WaterRabbit의 힐링 이펙트
    /// </summary>
    /// <param name="amount"></param>
    public void StartMPHealing(int amount)
    {
        CurrentMp += amount;
        //다시 실행
        mpHealingEffect.SetActive(false);
        mpHealingEffect.SetActive(true);
    }

    #endregion

    #region ETC

    /// <summary>
    /// NOTE : 물리데미지
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamagePhysics(int damage, UnitController attackedUnit)
    {
        //회피 랜덤설정 
        if (Random.Range(0, 100) < abilityDataInBattle.avoidanceRate)
            return;
        //방어력을 제외한 데미지 값
        int resultdamage = (int)(damage * abilityDataInBattle.physicaldefenseRate);
        attackedUnit.DrainHp(resultdamage);
        attackedUnit.CurrentMp += resultdamage * 0.2f;
        CurrentHp -= resultdamage;
        CurrentMp += resultdamage * 0.2f;
        //이펙트 생성
    }

    /// <summary>
    /// NOTE : 스킬은 공격유닛 mp회복 x, 회피 x, 방어력 무시, 추후 마저 감소가 있을경우 파라미터유닛 추가 해당 유닛의 마저 감소양을 통해서 처리
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamageSkill(int damage)
    {
        //방어력을 제외한 데미지 값
        int resultdamage = (int)(damage);
        CurrentHp -= resultdamage;
        CurrentMp += resultdamage * 0.2f;
        //이펙트 생성
    }

    /// <summary>
    /// NOTE : 설정한 MP값만큼 감소
    /// </summary>
    /// <param name="mpAmount"></param>
    public void TakeDamageMp(int mpAmount)
    {
        CurrentMp -= mpAmount;
    }
    #endregion
    
}
