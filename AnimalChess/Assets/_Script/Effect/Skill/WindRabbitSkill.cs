using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindRabbitSkill : MonoBehaviour
{
    [HideInInspector]
    public UnitController unit;
    [HideInInspector]
    public UnitBlockData target;

    public ParticleSystem ps;

    private Vector3 startpos;
    private Vector3 targetPos;
    protected int attackDamage;
    private float targetdistance;

    protected virtual void OnEnable()
    {
        startpos = unit.unitblockSc.transform.position;
        targetPos = target.transform.position;
        //scale자체는 한칸단위로 설정 
        targetdistance = Vector3.Distance(startpos, targetPos)*0.5f;
        targetdistance = targetdistance <= unit.abilityDataInBattle.attackRange ? unit.abilityDataInBattle.attackRange : targetdistance;
        var main = ps.main;
        main.startSpeed = targetdistance * 2f;

        //collier변경 
        var tmpscale = transform.localScale;
        tmpscale.z = targetdistance;
        transform.localScale = tmpscale;
    }

    /// <summary>
    /// NOTE : 일반 스킬 사용시 데미지 새로 설정
    /// </summary>
    public void SetData(UnitController _unit)
    {
        if (unit == null)
            unit = _unit;
        attackDamage = unit.abilityDataInBattle.totalSkillAttackDamage;
    }
    
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("EnemyUnit"))
        {
            var target = collision.GetComponent<UnitController>();
            //AttackDamage
            target.TakeDamageSkill(attackDamage);
            target.StartFreezing(3f);
        }
    }
}
