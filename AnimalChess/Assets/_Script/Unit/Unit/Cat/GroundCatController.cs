using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCatController : PlayerUnitController
{
    /// <summary>
    /// NOTE : Mp Delete
    /// </summary>
    public override void SkillActionInAnim()
    {
        base.SkillActionInAnim();
        var target = currentTargetBlock.unitInBattle;
        if (target == null)
            return;
        target.unitController.TakeDamagePhysics(abilityDataInBattle.normalSkillAttackDamage, this);

        target.unitController.StartStun(1f * unitPdata.ratingValue);

        var pos = target.transform.position;
        pos.y += 1;
        skillEffect.transform.position = pos;
        skillEffect.SetActive(true);
    }
    
    public override void SetSynergyParam()
    {
        tribeSynergy = new CatSynergy();
        attributeSynergy = new GroundSynergy();
        base.SetSynergyParam();
    }
}
