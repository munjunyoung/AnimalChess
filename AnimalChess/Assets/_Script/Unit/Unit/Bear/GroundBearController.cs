using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundBearController : PlayerUnitController
{
    /// <summary>
    /// NOTE : 일정시간 자신의 체력회복
    /// </summary>
    public override void SkillActionInAnim()
    {
        base.SkillActionInAnim();
        StartCoroutine(HealingProcess(10f));
        
    }

    IEnumerator HealingProcess(float time)
    {
        isRunningSkill = true;
        skillEffect.SetActive(true);
        int count = 0;

        while(count<=time && isAlive)
        {
            count++;
            CurrentHp += 50 * unitPdata.ratingValue;
            yield return new WaitForSeconds(1f);
        }
        isRunningSkill = false;
        skillEffect.SetActive(false);
    }


    public override void SetSynergyParam()
    {
        tribeSynergy = new BearSynergy();
        attributeSynergy = new GroundSynergy();
        base.SetSynergyParam();
    }
}
