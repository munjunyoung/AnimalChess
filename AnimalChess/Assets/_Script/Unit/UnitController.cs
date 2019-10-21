using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Animation_State { Idle, Walk, Skill, Die } 
public class UnitController : MonoBehaviour
{
    [HideInInspector]
    public    UnitBlockData         unitblockSc;
    protected Rigidbody             rb;
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
    protected int nextBlockCount = 0;
    protected BlockOnBoard targetBlock;
    protected bool isFindPath = false;
    protected bool isMoving = false;
    

    public void Init()
    {
        unitblockSc = GetComponent<UnitBlockData>();

        rb =GetComponent<Rigidbody>();
    }

    #region  Move

    public virtual void SetTargetBlock()
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
                nextBlockCount = path.Count - 2;
        }
    }
    
    //정해진 시간동안 한칸씩 이동 
    public virtual void ResetPath()
    {
        //가는길에 유닛이 존재할 경우, 목적지와 한칸 거리 일 경우
        if (nextBlockCount > 0)
        {
            if (path[nextBlockCount].GetUnitInBattle() != null)
            {
                if (pathfind.FindPath(unitblockSc.GetCurrentBlockInBattle(), targetBlock, BoardManager.instance.allGroundBlocks, ref path, pDataAttackRange))
                    nextBlockCount = path.Count - 2;
            }
        }
    }

    /// <summary>
    /// NOTE : 이동중이거나, 목적지에 도착했거나, 길을 찾지 못했을 경우 이동실행 X
    /// </summary>
    /// <returns></returns>
    public virtual bool CheckMoveState()
    {
        if (isMoving||nextBlockCount<0||!isFindPath)
            return false;

        return true;
    }
    
    public virtual void StartMoveToNextBlock()
    {
        StartCoroutine(MoveNextBlock(unitblockSc.GetCurrentBlockInBattle(), path[nextBlockCount]));
    }

    IEnumerator MoveNextBlock(BlockOnBoard currentBlock, BlockOnBoard nextblock)
    {
        isMoving = true;
        nextBlockCount--;
        float count = 0;
        //lerp포지션
        Vector3 startpos = rb.position;
        Vector3 nextpos = new Vector3(nextblock.transform.position.x, transform.position.y, nextblock.transform.position.z);
        //방향
        var dir = nextpos - startpos;
        transform.eulerAngles = dir;

        Quaternion dirRot = Quaternion.LookRotation(dir);
        Quaternion currentRot = this.transform.rotation;
        //초기화
        nextblock.SetUnitInBattle(unitblockSc);
        while (Vector3.Distance(transform.position,nextpos)>0.1f)
        {
            count += Time.deltaTime;
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, dirRot, 1f * count);
            rb.position = Vector3.Lerp(startpos, nextpos, count);
            yield return new WaitForFixedUpdate();
        }
        isMoving = false;
    }

    private void CompareXZPos()
    {

    }
    #endregion

    //공격
    public virtual bool CheckAttackRangeCondition() { return true; }
    //public virtual bool LookAtTarget() { }
    public virtual bool CheckAttackCondition() { return true; }
    public virtual void AttackAction() { }
    //스킬
    public virtual bool CheckSkillCondition () { return true; }
    public virtual void SkillAction() { }
    //죽음
    public virtual bool IsDie() { return false; }
    public virtual void DeadAction() { }
}
