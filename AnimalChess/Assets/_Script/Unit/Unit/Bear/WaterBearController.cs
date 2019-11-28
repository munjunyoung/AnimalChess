using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBearController : PlayerUnitController
{
    public override void SkillAction()
    {
        base.SkillAction();
    }


    public override void SetSynergyParam()
    {
        tribeSynergy = new BearSynergy();
        attributeSynergy = new WaterSynergy();
        base.SetSynergyParam();
    }
}
