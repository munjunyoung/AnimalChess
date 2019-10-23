using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockOnBoard : MonoBehaviour
{
    //In Battle
    public UnitBlockData unitInBattle  = null;
    //In Wating
    private UnitBlockData unitInWaiting = null;
    //Check WaitingBlock
    public  bool IsWaitingBlock = false;
    //array index 
    public Vector2Int groundArrayIndex = Vector2Int.zero;
    
    #region Waiting Time
    /// <summary>
    /// NOTE : 유닛이 존재하지 않을 경우 선택된 유닛 설정
    /// </summary>
    /// <param name="_unit"></param>
    public void SetUnitaddList(UnitBlockData _unit)
    {
        unitInWaiting = _unit;
        unitInBattle = unitInWaiting;
        if (unitInWaiting != null)
        {
            unitInWaiting.transform.position = new Vector3(transform.position.x, transform.localPosition.y + (int)MAP_INFO.CubeSize, transform.position.z);
            unitInWaiting.SetCurrentBlockInWaiting(this);
            BoardManager.instance.AddBlockOnList(this);
        }
    }
    
    /// <summary>
    /// NOTE : 유닛 리스트 처리 및 UNIT 리턴
    /// </summary>
    /// <returns></returns>
    public UnitBlockData GetUnitRemoveList()
    {
        var tmpunit = unitInWaiting;
        if (tmpunit != null)
        {
            BoardManager.instance.RemoveBlockOnList(this);
            unitInWaiting = null;
            unitInBattle = null;
            tmpunit.SetCurrentBlockInWaiting(null);
        }

        return tmpunit;
    }

    /// <summary>
    /// NOTE : 일반적인 GET
    /// </summary>
    /// <returns></returns>
    public UnitBlockData GetUnitNormal()
    {
        return unitInWaiting;
    }
    #endregion

    #region In Battle
    public void SetUnitEnemy(UnitBlockData _unit)
    {
        unitInBattle = _unit;
        if (unitInBattle != null)
        {
            unitInBattle.transform.position = new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z);
            unitInBattle.SetCurrentBlockInBattle(this);
        }
    }

    public void SetUnitInBattle(UnitBlockData _unit)
    {
        unitInBattle = _unit;
        if(unitInBattle!=null)
            unitInBattle.SetCurrentBlockInBattle(this);
    }

    public UnitBlockData GetUnitInBattle()
    {
        return unitInBattle;
    }
    #endregion

    /// <summary>
    /// NOTE : 레이어 설정 변경
    /// </summary>
    /// <param name="_isBattle"></param>
    public void SetLayerValue(bool _isBattle)
    {
        gameObject.layer = _isBattle == true ? LayerMask.NameToLayer("Default") : LayerMask.NameToLayer("DeploybleBlock");
    }
    
    //추가 이펙트 합성시 합성 이펙트
    //판매시 판매 이펙트
    //생성시 생성 이펙트?
}
