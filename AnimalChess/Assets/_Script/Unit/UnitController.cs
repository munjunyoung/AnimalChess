using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Animation_State { Idle, Walk, Skill, Die } 
public class UnitController : MonoBehaviour
{
    protected Rigidbody             rb;
    protected CapsuleCollider       col;
    protected Animator              anim;

    protected int currentHP;
    protected int currentMP;
    
    //대기 -필요 없을것 같다
    public virtual void IdleAction(){ }
    public virtual bool DetectTarget() { return true; }
    //추적 //정해진 시간동안 한칸씩 이동 
    public virtual void CheckTargetCondition() { }
    public virtual void ChaseAction() { }
    //공격
    public virtual bool CheckAttackRangeCondition() { return true; }
    //public virtual bool LookAtTarget() { }
    public virtual bool CheckAttackCondition() { return true; }
    public virtual void AttackAction() { }
    //스킬
    public virtual bool CheckSkillCondition () { return true; }
    public virtual void SkillAction() { }
    //죽음
    public virtual bool IsDie() { return true; }
    public virtual void DeadAction() { }
}
