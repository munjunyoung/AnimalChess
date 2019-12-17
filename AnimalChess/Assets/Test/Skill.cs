using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    protected PlayerUnitController unit;
    protected GameObject effectModel;

    public virtual void SetParam(PlayerUnitController unitc, GameObject effectmodel)
    {
        unit = unitc;
        effectModel = effectmodel;
    }

    public virtual void StartSkillAnim(ref bool startskill)
    {
        startskill = true;
    }

    public virtual void ExecuteSkill()
    {

    }
}

public class AttackStrike : Skill
{
    public override void ExecuteSkill()
    {
        base.ExecuteSkill();

    }
}