using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindRabbitController : PlayerUnitController
{
    public override void SkillAction()
    {
        base.SkillAction();
    }


    public override void SetSynergyParam()
    {
        tribeSynergy = new RabbitSynergy();
        attributeSynergy = new WindSynergy();
        base.SetSynergyParam();
    }
}
