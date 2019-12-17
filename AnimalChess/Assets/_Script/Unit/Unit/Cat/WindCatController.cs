using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindCatController : PlayerUnitController
{
    public GameObject handParent = null;
    public override void SetObjectData()
    {
        base.SetObjectData();
        skillEffect.transform.SetParent(handParent.transform);
        skillEffect.transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// NOTE : 공속증가
    /// </summary>
    public override void SkillActionInAnim()
    {
        base.SkillActionInAnim();
        StartCoroutine(SpeedUpProcess(10));
    }

    IEnumerator SpeedUpProcess(float time)
    {
        isRunningSkill = true;
        skillEffect.SetActive(true);
        abilityDataInBattle.attackSpeedValueNormal += 0.5f * unitPdata.ratingValue;
        anim.SetFloat("attackSpeed", abilityDataInBattle.totalAttackSpeedRate);
        yield return new WaitForSeconds(time);
        skillEffect.SetActive(false);
        abilityDataInBattle.attackSpeedValueNormal -= 0.5f;
        anim.SetFloat("attackSpeed", abilityDataInBattle.totalAttackSpeedRate);
        isRunningSkill = false; 
    }


    public override void SetSynergyParam()
    {
        tribeSynergy = new CatSynergy();
        attributeSynergy = new WindSynergy();
        base.SetSynergyParam();
    }
}
