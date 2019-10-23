using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Anim_State { Idle=0, Walk, Attack , Skill, Die }
public class UnitController : MonoBehaviour
{
    [HideInInspector]
    public UnitBlockData unitblockSc;
    [HideInInspector] //touch 에서 참조 
    public Rigidbody rb;
    protected CapsuleCollider col;
    protected Animator anim;
    protected CharacterController cController;

    public Anim_State animState = Anim_State.Idle;

    protected float pDataAttackRange = 1.5f;

    protected int currentHP;
    protected int currentMP;
    //Move
    protected UnitPathFinding pathfind = new UnitPathFinding();
    protected List<BlockOnBoard> path = new List<BlockOnBoard>();
    protected BlockOnBoard targetBlock;
    protected float moveSpeed = 1f;
    protected int nextBlockIndexCount = 0;
    //Rotate
    private float rotateSpeed = 5f;
    //Attack

    [HideInInspector]
    public    bool isAlive = true;
    protected bool isFindPath = false;
    protected bool isMoving = false;
    protected bool isCloseTarget = false;
    protected bool isDieAllEnemy = false;
    protected bool isRotating = false;
    protected bool isLookatTarget = false;
    protected bool isStartAttack = false;
    protected bool isAttackCooltimeWaiting = false;
    protected bool isStartSkill = false;
    

    public void Init()
    {
        unitblockSc = GetComponentInParent<UnitBlockData>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    #region  Move

    /// <summary>
    /// NOTE : 타겟이 없거나 타겟의 유닛이 없어졌거나 (타겟이 이동하였을 경우), 타겟이 죽었다면 타겟 다시 설정
    /// </summary>
    /// <returns></returns>
    public virtual bool SetTargetBlock()
    {
        //목적지가 없을때
        if (targetBlock == null
          || targetBlock.GetUnitInBattle() == null
          || !targetBlock.GetUnitInBattle().unitController.isAlive)
        {
            //몬스터 리스트 
            var mlist = BoardManager.instance.currentMonsterList;
            
            float mindistance = 100;
            //적이 모두 죽었는지 체크
            isDieAllEnemy = true;
            //가장 가까운 거리에 있는 적 검색
            for (int i = 0; i < mlist.Count; i++)
            {
                //살아있는지 확인
                if (mlist[i].isAlive)
                {
                    var currentdis = Vector2Int.Distance(unitblockSc.GetCurrentBlockInBattle().groundArrayIndex, mlist[i].unitblockSc.GetCurrentBlockInBattle().groundArrayIndex);
                    if (mindistance > currentdis)
                    {
                        mindistance = currentdis;
                        targetBlock = mlist[i].unitblockSc.GetCurrentBlockInBattle();
                        isDieAllEnemy = false;
                    }
                }
            }
        }
        return isDieAllEnemy;
    }
    
    /// <summary>
    /// NOTE : PATH가 없을경우, 길을 가는중 길이 막혔을 경우 -> 길찾기 실행, 길을 못찾았을 경우 RETURN FALSE
    /// </summary>
    /// <returns></returns>
    public virtual bool SetPath()
    {
        //길이 없을 경우
        if (path.Count == 0)
        {
            if (isFindPath = pathfind.FindPath(unitblockSc.GetCurrentBlockInBattle(), targetBlock, BoardManager.instance.allGroundBlocks, ref path))
                nextBlockIndexCount = path.Count - 2;
        }
        //길이 존재 하지만 원래 길의 목적지에 방해물이 있을 경우 
        else
        {
            if (nextBlockIndexCount >= 0)
            {
                if (path[nextBlockIndexCount].GetUnitInBattle() != null)
                {
                    if (isFindPath = pathfind.FindPath(unitblockSc.GetCurrentBlockInBattle(), targetBlock, BoardManager.instance.allGroundBlocks, ref path))
                        nextBlockIndexCount = path.Count - 2;
                }
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
        if (isMoving)
            return false;
        //모든 움직임이 끝나고 난후 체크
        //지정한 사거리와 현재 타겟과의 거리 비교
        isCloseTarget = (unitblockSc.unitPdata.abilityData.attackRange + 0.5f)> Vector2Int.Distance(unitblockSc.GetCurrentBlockInBattle().groundArrayIndex, targetBlock.groundArrayIndex) ? true : false;
        //isCloseTarget = nextBlockIndexCount >= 0 ? false : true;
        if (isCloseTarget)
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
        while (unitblockSc.transform.position!=nextpos)
        {
            count += Time.deltaTime;
            if (dir != Vector3.zero)
                unitblockSc.transform.rotation = Quaternion.Lerp(currentRot, targetRot, count * rotateSpeed );
            unitblockSc.transform.position = Vector3.Lerp(startpos, nextpos, moveSpeed * count);
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
        if (!isRotating)
        {
            //block position y 값은 0으로 초기화 
            var targetpos = targetBlock.transform.position;
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
        while(Quaternion.Angle(unitblockSc.transform.rotation, targetRot) != 0)
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
        if (isAttackCooltimeWaiting)
            return false;
        return true;
    }
    
    /// <summary>
    /// 
    /// </summary>
    public virtual void AttackAction()
    {
        isStartAttack = true;
    }

    public void StartAttackInAttackAnim()
    {
        Debug.Log("AttackAnim Start");
        isStartAttack = false;
        StartCoroutine(SetAttackCoolTime());
    }

    /// <summary>
    /// NOTE : 쿨타임 상태 설정
    /// </summary>
    /// <returns></returns>
    IEnumerator SetAttackCoolTime()
    {
        isAttackCooltimeWaiting = true;
        yield return new WaitForSecondsRealtime(unitblockSc.unitPdata.abilityData.attackCooltime);
        isAttackCooltimeWaiting = false;
    }
    #endregion
    //스킬
    public virtual bool CheckSkillCondition() { return true; }
    public virtual void SkillAction() { }
    //죽음
    public virtual bool IsDie() { return false; }
    public virtual void DeadAction() { }

    public virtual void SetAnimation()
    {
        animState = Anim_State.Idle;
        if (isMoving)
            animState = Anim_State.Walk;
        if (isStartAttack)
            animState = Anim_State.Attack;
        if (isStartSkill)
            animState = Anim_State.Skill;
        if (!isAlive)
            animState = Anim_State.Die;

        anim.SetFloat("animState", (int)animState);

    }
}
