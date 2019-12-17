using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorEffect : MonoBehaviour
{
    [HideInInspector]
    public UnitController unit;
    [HideInInspector]
    public UnitBlockData target;

    private Vector3 startpos;


    public MeteorHitEffect HitEffect;
    protected int attackDamage;

    private void OnEnable()
    {
        //타겟 머리 위에 생성
        startpos = target.transform.position;
        startpos.y += 20;

        //파이어볼 사이즈
        transform.localScale = Vector3.one * unit.unitPdata.ratingValue;
        this.transform.position = startpos;
        StartCoroutine(DropProcess());
    }

    public void SetData(UnitController _unit)
    {
        if(unit==null)
            unit = _unit;
        attackDamage = unit.abilityDataInBattle.totalSkillAttackDamage;
        HitEffect.SetData(attackDamage, unit.unitPdata.ratingValue);
    }

    IEnumerator DropProcess()
    {
        float count = 0;
        var pos = startpos;
        while(transform.position.y>0)
        {
            count += Time.deltaTime * 0.1f;
            //pos.y = Mathf.Lerp(-1, startpos.y, count);
            pos.y -= 1f * count;
            this.transform.position = pos;
            yield return new WaitForFixedUpdate();
        }
        gameObject.SetActive(false);
        var hiteffectpos = transform.position;
        hiteffectpos.y = 0.5f;


        HitEffect.transform.position = hiteffectpos;
        HitEffect.gameObject.SetActive(true);
    }
}
