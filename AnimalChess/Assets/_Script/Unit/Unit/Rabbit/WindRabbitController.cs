using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindRabbitController : PlayerUnitController
{
    WindRabbitSkill windSkillSc;
    public override void SetObjectData()
    {
        base.SetObjectData();
        windSkillSc = skillEffect.GetComponent<WindRabbitSkill>();
        windSkillSc.SetData(this);
    }

    public override void SkillActionInAnim()
    {
        base.SkillActionInAnim();
        windSkillSc.target = currentTargetBlock.unitInBattle;
        skillEffect.SetActive(true);
    }


    public override void SetSynergyParam()
    {
        tribeSynergy = new RabbitSynergy();
        attributeSynergy = new WindSynergy();
        base.SetSynergyParam();
    }
}
