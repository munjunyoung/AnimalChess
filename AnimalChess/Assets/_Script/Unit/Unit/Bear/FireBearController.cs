using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBearController : PlayerUnitController
{
    /// <summary>
    /// NOTE : 자신을 공격하도록 도발하고 적의 방어력 깎기
    /// </summary>
    public override void SkillActionInAnim()
    {
        base.SkillActionInAnim();
        var target = currentTargetBlock.unitInBattle;
        if (target == null)
            return;
        target.unitController.StartProvoked(5f * unitPdata.ratingValue, this);
        StartCoroutine(ProvokeProcess(5f * unitPdata.ratingValue, target.unitController));

    }
    
    /// <summary>
    /// NOTE : 상대를 일정시간 도발하고 방어력을 감소시킴
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    IEnumerator ProvokeProcess(float time, UnitController target)
    {
        isRunningSkill = true;
        skillEffect.SetActive(true);
        target.abilityDataInBattle.PhysicalDefense -= 0.1f;
        int count = 0;

        while (count <= time &&isAlive)
        {
            count++;
            yield return new WaitForSeconds(1f);
        }
        isRunningSkill = false;
        skillEffect.SetActive(false);
        target.abilityDataInBattle.PhysicalDefense += 0.1f;        
    }

    public override void SetSynergyParam()
    {
        tribeSynergy = new BearSynergy();
        attributeSynergy = new FireSynergy();
        base.SetSynergyParam();
    }
}
