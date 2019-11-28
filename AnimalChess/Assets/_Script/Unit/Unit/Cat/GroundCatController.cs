using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCatController : PlayerUnitController
{
    public override void SkillAction()
    {
        base.SkillAction();
    }


    public override void SetSynergyParam()
    {
        tribeSynergy = new CatSynergy();
        attributeSynergy = new GroundSynergy();
        base.SetSynergyParam();
    }
}
