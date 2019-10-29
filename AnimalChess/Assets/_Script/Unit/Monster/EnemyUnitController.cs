using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnitController : UnitController
{

    public override void StartUnitInBattle(HpMpSlider sliderdata, float waitingTime)
    {
        base.StartUnitInBattle(sliderdata, waitingTime);
        if (targetList.Count == 0)
        {
            var tmplist = BoardManager.instance.GetBattleBlockOnUnit();
            foreach (var targetunit in tmplist)
                targetList.Add(targetunit.GetUnitInBattle().unitController);
        }
    }

    public override void DeadAction()
    {
        base.DeadAction();
        //모든 적이 죽었는지 체크
        IngameManager.instance.CheckEnemyUnitAlive();
    }
}
