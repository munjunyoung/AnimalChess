using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockOnBoard : MonoBehaviour
{
    [HideInInspector]
    private Unit         unit            = null;
    public  bool         IsWaitingBlock = false;
    
    /// <summary>
    /// NOTE : 유닛이 존재하지 않을 경우 선택된 유닛 설정
    /// </summary>
    /// <param name="_unit"></param>
    public void SetUnitByTouch(Unit _unit)
    {
        unit = _unit;

        if (unit != null)
        {
            unit.transform.position = new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z);
            unit.SetCurrentBlock(this);
            BoardManager.instance.AddBlockOnList(this);
        }
    }

    public Unit GetUnitByTouch()
    {
        if (unit != null)
            BoardManager.instance.RemoveBlockOnList(this);
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
