using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatController : UnitController
{
    //대기
    public override void IdleAction()
    {
        base.IdleAction();
    }

    public override bool DetectTarget()
    {
        return base.DetectTarget();
    }

    //추적
    public override void CheckTargetCondition()
    {
        base.CheckTargetCondition();
    }

    public override void ChaseAction()
    {
        base.ChaseAction();
    }
    //공격
    public override bool CheckAttackRangeCondition()
    {
        return base.CheckAttackRangeCondition();
    }

    public override bool CheckAttackCondition()
    {
        return base.CheckAttackCondition();
    }

    //스킬
    public override bool CheckSkillCondition()
    {
        return base.CheckSkillCondition();
    }

    public override void SkillAction()
    {
        base.SkillAction();
    }
    
}
