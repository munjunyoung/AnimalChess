using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRabbitController : PlayerUnitController
{
    /// </summary>
    private GameObject skillEffect2;
    private List<MeteorEffect> meteorSkill = new List<MeteorEffect>();
    private int meteorCount = 0;
    
    /// <summary>
    /// NOTE : 스킬 실행시간이 크므로 2개를 풀링하는 형식
    public override void SetObjectData()
    {
        base.SetObjectData();
        skillEffect2 = skillParent.transform.Find("SkillEffect2").gameObject;
        meteorSkill.Add(skillEffect.GetComponent<MeteorEffect>());
        meteorSkill.Add(skillEffect2.GetComponent<MeteorEffect>());
        foreach (var mt in meteorSkill)
            mt.SetData(this);
    }

    /// <summary>
    /// NOTE : 메테오
    /// </summary>
    public override void SkillActionInAnim()
    {
        base.SkillActionInAnim();
        
        meteorSkill[meteorCount].target = currentTargetBlock.unitInBattle;
        meteorSkill[meteorCount].gameObject.SetActive(true);
        meteorCount++;
        meteorCount = meteorCount % 2;
    }
    
    public override void SetSynergyParam()
    {
        tribeSynergy = new RabbitSynergy();
        attributeSynergy = new FireSynergy();
        base.SetSynergyParam();
    }
}