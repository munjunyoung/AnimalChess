using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAbilityData
{
    public int totalMaxHp;
    public int normalMaxHP;
    public float _plusHpRate;
    public float plusHpRate
    {
        get { return _plusHpRate; }
        set
        {
            _plusHpRate = value;
            totalMaxHp = (int)(normalMaxHP * (1 + _plusHpRate));
        }
    }

    public int maxMP;

    public int attackDamage;
    public int normalAttackDamage;
    private float _plusattackDamageRate;
    public float plusAttackDamageRate
    {
        get { return _plusattackDamageRate; }
        set
        {
            _plusattackDamageRate = value;
            attackDamage = (int)(normalAttackDamage * (1 + (_plusattackDamageRate * 0.01f)));
        }
    }

    public int attackRange;

    public int skillAttackDamage;
    public int normalSkillAttackDamage;
    public float _plusSkillAttackRate;
    public float plusSkillAttackRate
    {
        get { return _plusSkillAttackRate; }
        set
        {
            _plusSkillAttackRate = value;
            skillAttackDamage = (int)(normalSkillAttackDamage * (1 + (_plusSkillAttackRate * 0.01f)));
        }
    }

    //공속
    public float attackCooltime
    {
        get { return 2 / totalAttackSpeedRate; }
    }
    public float totalAttackSpeedRate
    {
        get { return 1 + attackSpeedRateData + attackSpeedRateSynergy; }
    }
    //시너지에서 처리 
    public float attackSpeedRateSynergy;
    //데이터에서 처리
    private float attackSpeedRateData;
    


    public float physicaldefenseRate = 1;
    private int _physicaldefense;
    public int PhysicalDefense
    {
        get { return _physicaldefense; }
        set
        {
            _physicaldefense = value;
            physicaldefenseRate = 1 - (_physicaldefense * 0.01f);
        }
    }

    //회피
    public int avoidanceRate;
    //생흡
    public float drainHp;

    public UnitAbilityData(Tribe_Type tribe, Attribute_Type attribute, int gold, int ratingValue)
    {
        normalMaxHP = 300;
        maxMP = 100;
        normalAttackDamage = 50;
        attackRange = 1;
        normalSkillAttackDamage = 100;


        switch (tribe)
        {
            case Tribe_Type.Cat:
                attackSpeedRateData += 0.1f;
                normalAttackDamage += 50;
                break;
            case Tribe_Type.Bear:
                normalMaxHP += 100;
                attackSpeedRateData -= 20;
                break;
            case Tribe_Type.Rabbit:
                attackRange += 3;
                normalSkillAttackDamage += 50;
                break;
        }

        switch(attribute)
        {
            case Attribute_Type.Fire:
                normalAttackDamage += 10;
                normalSkillAttackDamage += 50;
                break;
            case Attribute_Type.Water:
                maxMP -= 10;
                break;
            case Attribute_Type.Ground:
                normalMaxHP += 50;
                break;
            case Attribute_Type.Wind:
                attackSpeedRateData += 0.1f;
                break;
        }

        normalMaxHP += gold * 100;
        normalAttackDamage += gold * 10;
        normalSkillAttackDamage += 50;

        normalMaxHP += ratingValue * 200;
        normalAttackDamage += ratingValue * 20;
        normalSkillAttackDamage += ratingValue * 100;

        plusHpRate = 0;
        PhysicalDefense = 0;
        attackSpeedRateSynergy = 0;
        plusAttackDamageRate = 0;
        avoidanceRate = 0;
        drainHp = 0;
    }
}