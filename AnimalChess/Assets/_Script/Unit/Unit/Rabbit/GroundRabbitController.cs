﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundRabbitController : PlayerUnitController
{
    public NormalAttackProjectileSc skillProjectTile;

    public override void SetObjectData()
    {
        base.SetObjectData();
        skillProjectTile = skillParent.transform.Find("SkillProjectile").GetComponent<NormalAttackProjectileSc>();
        if (skillProjectTile.hiteffect == null)
            skillProjectTile.hiteffect = skillEffect;
        if (skillProjectTile.unit == null)
            skillProjectTile.unit = this;
        skillProjectTile.SetData(abilityDataInBattle.totalSkillAttackDamage);
    }

    public override void SkillActionInAnim()
    {
        base.SkillActionInAnim();

        var target = currentTargetBlock.unitInBattle;
        if (target != null)
        {
            skillProjectTile.target = target;
            skillProjectTile.gameObject.SetActive(true);
        }
    }
    
    public override void SetSynergyParam()
    {
        tribeSynergy = new RabbitSynergy();
        attributeSynergy = new GroundSynergy();
        base.SetSynergyParam();
    }
}
