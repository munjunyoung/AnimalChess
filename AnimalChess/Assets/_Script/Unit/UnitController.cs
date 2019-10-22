using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Animation_State { Idle, Walk, Skill, Die } 
public class UnitController : MonoBehaviour
{
    [HideInInspector]
    public    UnitBlockData         unitblockSc;
    public    Rigidbody             rb;
    protected CapsuleCollider       col;
    protected Animator              anim;
    protected CharacterController   cController;

    protected float pDataAttackRange = 1.5f;

    [HideInInspector]
    public    bool  IsAlive = true;
    protected int   currentHP;
    protected int   currentMP;

    protected UnitPathFinding pathfind = new UnitPathFinding();
    protected List<BlockOnBoard> path = new List<BlockOnBoard>();
    protected BlockOnBoard targetBlock;
    protected int nextBlockIndexCount = 0;

    protected bool isFindPath = false;
    protected bool isMoving = false;
    protected bool isCloseTarget = false;

    public void Init()
    {
        unitblockSc = GetComponentInParent<UnitBlockData>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        
    }

    #region  Move
    
    public virtual bool SetTargetBlock()
    {
        if (targetBlock == null
          || targetBlock.GetUnitInBattle() == null
          || !targetBlock.GetUnitInBattle().unitController.IsAlive)
        {
            var mlist = BoardManager.instance.currentMonsterList;
            float mindistance = 100;
            for (int i = 0; i < mlist.Count; i++)
            {
                //살아있는지 확인
                if (mlist[i].IsAlive)
                {
                    var currentdis = Vector2Int.Distance(unitblockSc.GetCurrentBlockInBattle().groundArrayIndex, mlist[i].unitblockSc.GetCurrentBlockInBattle().groundArrayIndex);
                    if (mindistance > currentdis)
                    {
                        mindistance = currentdis;
                        targetBlock = mlist[i].unitblockSc.GetCurrentBlockInBattle();
                    }
                }
            }
            isFindPath = pathfind.FindPath(unitblockSc.GetCurrentBlockInBattle(), targetBlock, BoardManager.instance.allGroundBlocks, ref path, pDataAttackRange);
            if (isFindPath)
                nextBlockIndexCount = path.Count - 2;
            return false;
        }
        return true;
    }
    
    //정해진 시간동안 한칸씩 이동 
    public virtual bool ResetPath()
    {
        //가는길에 유닛이 존재할 경우, 목적지와 한칸 거리 일 경우
        if (nextBlockIndexCount >= 0)
        {
            if (path[nextBlockIndexCount].GetUnitInBattle() != null)
            {
                if (pathfind.FindPath(unitblockSc.GetCurrentBlockInBattle(), targetBlock, BoardManager.instance.allGroundBlocks, ref path, pDataAttackRange))
                    nextBlockIndexCount = path.Count - 2;
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// NOTE : 이동중이거나, 목적지에 도착했거나, 길을 찾지 못했을 경우 이동실행 X
    /// </summary>
    /// <returns></returns>
    public virtual bool CheckMoveState()
    {
        if (isMoving) 
            return false;
        if (!isFindPath)
            return false;
        //모든 움직임이 끝나고 난후 체크
        isCloseTarget = nextBlockIndexCount >= 0 ? false : true;
        if (isCloseTarget)
            return false;

        return true;
    }
    
    public virtual void StartMoveToNextBlock()
    {
        StartCoroutine(MoveNextBlock(unitblockSc.GetCurrentBlockInBattle(), path[nextBlockIndexCount]));
    }

    IEnumerator MoveNextBlock(BlockOnBoard currentBlock, BlockOnBoard nextblock)
    {
        isMoving = true;
        nextblock.SetUnitInBattle(unitblockSc);
        currentBlock.SetUnitInBattle(null);
        nextBlockIndexCount--;
        float count = 0;
        //lerp포지션
        Vector3 startpos = unitblockSc.transform.position;
        Vector3 nextpos = new Vector3(nextblock.transform.position.x, 0, nextblock.transform.position.z);
        //방향
        var dir = nextpos - startpos;
        dir = dir.normalized;
        Quaternion currentRot = unitblockSc.transform.rotation;
        Quaternion dirRot = Quaternion.LookRotation(dir);
        
        //초기화
        while (!unitblockSc.transform.position.Equals(nextpos))
        {
            count += Time.deltaTime;
            if(dir!=Vector3.zero)
                unitblockSc.transform.rotation = Quaternion.Lerp(this.transform.rotation, dirRot, 1f * count);
            unitblockSc.transform.position = Vector3.Lerp(startpos, nextpos, count);
            yield return new WaitForFixedUpdate();
        }
        isMoving = false;
    }

    #endregion

    //공격
    public virtual bool CheckAttackRangeCondition()
    {
        return isCloseTarget;
    }

    public virtual void LookAtTarget()
    {
        Vector3 dirVector = targetBlock.transform.position - unitblockSc.transform.position;
        dirVector = dirVector.normalized;
        unitblockSc.transform.rotation = Quaternion.LookRotation(dirVector);
    }
    public virtual bool CheckAttackCondition() { return true; }
    public virtual void AttackAction() { }
    //스킬
    public virtual bool CheckSkillCondition () { return true; }
    public virtual void SkillAction() { }
    //죽음
    public virtual bool IsDie() { return false; }
    public virtual void DeadAction() { }

    public virtual void SetAnimation()
    {
        if (isMoving)
            anim.SetFloat ("animState", 1);
        else
            anim.SetFloat("animState", 0);

    }
}
