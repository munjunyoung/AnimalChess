using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterRabbitController : PlayerUnitController
{
    List<BlockOnBoard> playerUnits;
   
    public override void SkillActionInAnim()
    {
        base.SkillActionInAnim();
        playerUnits = BoardManager.instance.GetBattleBlockOnUnit();
        foreach(var pu in playerUnits)
        {
            var unit = pu.GetUnitNormal();
            if(unit.unitController.isAlive)
                unit.unitController.StartMPHealing(30 * unitPdata.ratingValue);
        }

    }


    public override void SetSynergyParam()
    {
        tribeSynergy = new RabbitSynergy();
        attributeSynergy = new WaterSynergy();
        base.SetSynergyParam();
    }
}