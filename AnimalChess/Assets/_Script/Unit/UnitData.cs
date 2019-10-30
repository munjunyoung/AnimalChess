using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAbilityData
{
    public int maxHP;
    public int maxMP;

    public int attackDamage;
    public int attackRange;
    public float attackCooltime;

    public int SkillAttackDamage;
    
    public UnitAbilityData(Tribe_Type tribe, Attribute_Type attribute, int gold, int ratingValue)
    {
        maxHP = 300;
        maxMP = 100;
        attackDamage = 50;
        attackRange = 1;
        attackCooltime = 2;
        SkillAttackDamage = 100;

        switch(tribe)
        {
            case Tribe_Type.Cat:
                attackCooltime -= 1f;
                attackDamage += 50;
                break;
            case Tribe_Type.Bear:
                maxHP += 100;
                attackCooltime += 0.5f;
                break;
            case Tribe_Type.Rabbit:
                attackRange += 3;
                SkillAttackDamage += 50;
                break;
        }

        switch(attribute)
        {
            case Attribute_Type.Fire:
                attackDamage += 10;
                SkillAttackDamage += 50;
                break;
            case Attribute_Type.Water:
                maxMP -= 10;
                break;
            case Attribute_Type.Ground:
                maxHP += 50;
                break;
            case Attribute_Type.Wind:
                attackCooltime -= 0.1f;
                break;
        }

        maxHP += gold * 100;
        attackDamage += gold * 10;
        SkillAttackDamage += 50;

        maxHP += ratingValue * 200;
        attackDamage += ratingValue * 20;
        SkillAttackDamage += ratingValue * 100;
    }
}