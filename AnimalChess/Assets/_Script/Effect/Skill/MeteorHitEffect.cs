using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorHitEffect : MonoBehaviour
{
    private int attackDamage;
    /// <summary>
    /// Rating Value값에 따른 피격범위 증가 이펙트 사이즈 증가, damage설정
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="ratingValue"></param>
    public void SetData(int damage, int ratingValue)
    {
        attackDamage = damage;
        transform.localScale = Vector3.one * 2 + (Vector3.one * ratingValue);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("EnemyUnit"))
        {
            var target = collision.GetComponent<UnitController>();
            //AttackDamage
            target.TakeDamageSkill(attackDamage);
        }
    }
}
