using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBearController : PlayerUnitController
{
    /// <summary>
    /// NOTE : 일정시간 적 슬로우
    /// </summary>
    public override void SkillActionInAnim()
    {
        base.SkillActionInAnim();
        var target = currentTargetBlock.unitInBattle;
        if (target == null)
            return;
        target.unitController.TakeDamagePhysics(abilityDataInBattle.normalSkillAttackDamage, this);
        //MPDelete
        target.unitController.StartWaterSlow(2f * unitPdata.ratingValue);
        skillEffect.transform.position = target.transform.position;
        skillEffect.SetActive(true);
    }


    public override void SetSynergyParam()
    {
        tribeSynergy = new BearSynergy();
        attributeSynergy = new WaterSynergy();
        base.SetSynergyParam();
    }
}
