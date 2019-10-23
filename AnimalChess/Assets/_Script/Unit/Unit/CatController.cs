using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatController : UnitController
{   //대기

    public override bool SetTargetBlock()
    {
        return base.SetTargetBlock();
    }
    
    public override bool SetPath()
    {
        return base.SetPath();
    }

    public override void StartMoveToNextBlock()
    {

        base.StartMoveToNextBlock();
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
