using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnitController : UnitController
{
    public override void StartUnitInBattle(HpMpSlider sliderdata, float waitingTime)
    {
        base.StartUnitInBattle(sliderdata, waitingTime);
        if (targetList.Count == 0)
        {
            var tmplist = BoardManager.instance.currentEnemyUnitList;
            foreach (var targetunit in tmplist)
                targetList.Add(targetunit);
        }
    }

    /// <summary>
    /// NOTE : 플레이어 유닛이 모두 죽었는지 체크
    /// </summary>
    public override void DeadAction()
    {
        base.DeadAction();
        //모든 적이 죽었는지 체크
        IngameManager.instance.CheckUnitAlive();
    }

}
