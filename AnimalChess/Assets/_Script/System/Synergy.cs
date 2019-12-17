using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Synergy
{
    protected PlayerUnitController unit;
    protected GameObject effectModel;

    public virtual void SetParam(PlayerUnitController unitcontroller, GameObject effectmodel)
    {
        unit = unitcontroller;
        effectModel = effectmodel; 
    }
    public virtual void SetSynergy(int level)
    {
        if(level==0)
        {
            effectModel.SetActive(false);
        }
        else
        {
            effectModel.SetActive(false);
            effectModel.SetActive(true);
        }
    }
}

public class CatSynergy : Synergy
{
    public override void SetSynergy(int level)
    {
        if (level == 0)
            unit.abilityDataInBattle.avoidanceRate = 0;
        else if (level == 1)
            unit.abilityDataInBattle.avoidanceRate = 20;
        else if (level == 2)
            unit.abilityDataInBattle.avoidanceRate = 50;

        base.SetSynergy(level);

    }
}
public class RabbitSynergy : Synergy
{
    public override void SetSynergy(int level)
    {
        if (level == 0)
            unit.abilityDataInBattle.plusSkillAttackRate = 0;
        else if (level == 1)
            unit.abilityDataInBattle.plusSkillAttackRate = 0.3f;
        else if (level == 2)
            unit.abilityDataInBattle.plusSkillAttackRate = 1;

        base.SetSynergy(level);
    }
}
public class BearSynergy : Synergy
{
    public override void SetSynergy(int level)
    {
        if (level == 0)
            unit.abilityDataInBattle.PhysicalDefense = 0;
        else if (level == 1)
            unit.abilityDataInBattle.PhysicalDefense = 0.15f;
        else if (level == 2)
            unit.abilityDataInBattle.PhysicalDefense = 0.4f;

        base.SetSynergy(level);
    }
}

public class FireSynergy : Synergy
{
    public override void SetSynergy(int level)
    {
        if (level == 0)
            unit.abilityDataInBattle.plusAttackDamageRate = 0;
        else if (level == 1)
            unit.abilityDataInBattle.plusAttackDamageRate = 0.3f;

        base.SetSynergy(level);
    }
}

public class WaterSynergy : Synergy
{
    public override void SetSynergy(int level)
    {
        if (level == 0)
            unit.abilityDataInBattle.drainHp = 0;
        else if (level == 1)
            unit.abilityDataInBattle.drainHp = 0.3f;

        base.SetSynergy(level);
    }
}
public class WindSynergy : Synergy
{
    public override void SetSynergy(int level)
    {
        if (level == 0)
            unit.abilityDataInBattle.attackSpeedValueSynergy = 0;
        else if (level == 1)
            unit.abilityDataInBattle.attackSpeedValueSynergy = 0.3f;

        base.SetSynergy(level);
    }
}
public class GroundSynergy : Synergy
{
    public override void SetSynergy(int level)
    {
        if (level == 0)
            unit.abilityDataInBattle.plusHpRate = 0;
        else if (level == 1)
            unit.abilityDataInBattle.plusHpRate = 0.5f;

        base.SetSynergy(level);
    }
}