using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundBearController : PlayerUnitController
{
    public override void SkillAction()
    {
        base.SkillAction();
    }


    public override void SetSynergyParam()
    {
        tribeSynergy = new BearSynergy();
        attributeSynergy = new GroundSynergy();
        base.SetSynergyParam();
    }
}
