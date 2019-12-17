using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindBearController : PlayerUnitController
{
    //KnockBack
    public override void SkillActionInAnim()
    {
        base.SkillActionInAnim();
        var target = currentTargetBlock.unitInBattle;
        if (target == null)
            return;
        target.unitController.TakeDamagePhysics(abilityDataInBattle.normalSkillAttackDamage, this);
        target.unitController.StartKnockBack(unitblockSc.transform.position);
        skillEffect.SetActive(true);
    }
    
    public override void SetSynergyParam()
    {
        tribeSynergy = new BearSynergy();
        attributeSynergy = new WindSynergy();
        base.SetSynergyParam();
    }
}
