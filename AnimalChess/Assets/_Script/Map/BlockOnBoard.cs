using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockOnBoard : MonoBehaviour
{
    [HideInInspector]
    private Vector3      pos;
    [HideInInspector]
    private Unit         unit            = null;

    private float        yPosOnClick     = 0f;
    private float        yPosOriginal    = 0;

    public  bool         IsWaitingBlock = false;

    private void Start()
    {
        pos = transform.position;
        yPosOriginal = transform.position.y;
        yPosOnClick = yPosOriginal + 0.5f;
    }
    
    /// <summary>
    /// NOTE : 유닛이 존재하지 않을 경우 선택된 유닛 설정
    /// </summary>
    /// <param name="_unit"></param>
    public void SetUnit(Unit _unit)
    {
        unit = _unit;

        if (unit != null)
            unit.transform.position = new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z);
    }

    public Unit GetUnit()
    {
        return unit;
    }

    /// <summary>
    /// NOTE : 레이어 설정 변경
    /// </summary>
    /// <param name="_isBattle"></param>
    public void SetLayerValue(bool _isBattle)
    {
        gameObject.layer = _isBattle == true ? LayerMask.NameToLayer("Default") : LayerMask.NameToLayer("DeploybleBlock");
    }
}
