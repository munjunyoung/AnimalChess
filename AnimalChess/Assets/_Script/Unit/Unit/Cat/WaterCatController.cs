using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCatController : PlayerUnitController
{
    
    /// <summary>
    /// NOTE : Mp 삭제 
    /// </summary>
    public override void SkillActionInAnim()
    {
        base.SkillActionInAnim();
        var target = currentTargetBlock.unitInBattle;
        if (target == null)
            return;
        target.unitController.TakeDamagePhysics(abilityDataInBattle.normalSkillAttackDamage, this);
        //MPDelete
        target.unitController.TakeDamageMp(30 * unitPdata.ratingValue);
        var pos= target.transform.position;
        pos.y += 1;
        skillEffect.transform.position = pos;
        skillEffect.SetActive(true);
    }
    
    public override void SetSynergyParam()
    {
        tribeSynergy = new CatSynergy();
        attributeSynergy = new WaterSynergy();
        base.SetSynergyParam();
    }
}
