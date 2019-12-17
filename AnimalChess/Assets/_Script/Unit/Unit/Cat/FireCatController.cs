using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCatController : PlayerUnitController
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
    /// <summary>
    /// NOTE : FireCat 스킬 : PowerStrike, 애니매이션 실행 및 타겟 구조체 설정
    /// </summary>
    public override void SkillActionInAnim()
    {
        base.SkillActionInAnim();
        if (skillProjectTile.hiteffect == null)
            skillProjectTile.hiteffect = skillEffect;
        if (skillProjectTile.unit == null)
            skillProjectTile.unit = this;

        var target = currentTargetBlock.unitInBattle;
        if (target != null)
        {
            skillProjectTile.target = target;
            skillProjectTile.gameObject.SetActive(true);
        }
    }
    
    /// <summary>
    /// NOTE : SetSynergy Data
    /// </summary>
    public override void SetSynergyParam()
    {
        tribeSynergy = new CatSynergy();
        attributeSynergy = new FireSynergy();
        base.SetSynergyParam();
    }
}
