using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttackProjectileSc : MonoBehaviour
{
    [HideInInspector]
    public UnitController unit;
    [HideInInspector]
    public GameObject hiteffect;
    [HideInInspector]
    public UnitBlockData target;

    private Vector3 startpos;
    private Vector3 targetPos;
    private Vector3 currentPos;
    protected int attackDamage;
    private float targetdistance;
    private float projectileSpeed;
   
    protected virtual void OnEnable()
    {
        startpos = unit.unitblockSc.transform.position;
        startpos.y = 0.5f;
        targetPos = target.transform.position;
        targetPos.y = 0.5f;
        currentPos = startpos;
        StartCoroutine(MovePos());
    }

    /// <summary>
    /// NOTE : 일반 스킬 사용시 데미지 새로 설정
    /// </summary>
    public void SetData(int damage)
    {
        attackDamage = damage;
    }
    
    IEnumerator MovePos()
    {
        var count = 0f;
        targetdistance = Vector3.Distance(startpos, targetPos);
        projectileSpeed = 20 / targetdistance;
        while(currentPos!=targetPos)
        {
            count += Time.fixedDeltaTime * projectileSpeed;
            currentPos = Vector3.Lerp(startpos, targetPos, count);
            transform.position = currentPos;
            yield return new WaitForFixedUpdate();
        }
        gameObject.SetActive(false);
    }

    protected virtual void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.Equals(target.unitController.gameObject))
        {
            //AttackDamage
            target.unitController.TakeDamagePhysics(attackDamage, unit);
            //Effect
            hiteffect.transform.position = this.transform.position;
            hiteffect.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
