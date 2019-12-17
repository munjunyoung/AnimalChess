using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHPHitEffect : MonoBehaviour
{
    [SerializeField]
    public EnemyUnitController enemy;
    private Vector3 startpos;
    private Vector3 targetpos;
    
    private int damage;
    // Start is called before the first frame update
    private void OnEnable()
    {
        startpos = enemy.transform.position;
        transform.position = startpos;
        StartCoroutine(MovePos());
    }

    public void SetData(EnemyUnitController _enemy)
    {
        enemy = _enemy;
        damage = _enemy.unitPdata.cost;
        targetpos = UIManager.instance.hpTextPosition;
    }

    IEnumerator MovePos()
    {
        var count = 0f;
        var currentpos = startpos;
        while (currentpos != targetpos)
        {
            count += Time.fixedDeltaTime;
            currentpos = Vector3.Lerp(startpos, targetpos, count);
            transform.position = currentpos;
            yield return new WaitForFixedUpdate();
        }
        IngameManager.instance.TakeDamage(enemy.unitPdata.cost);
        gameObject.SetActive(false);
    }
}
