using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardTile : MonoBehaviour
{
    [HideInInspector]
    public GameObject   tileOb;
    [HideInInspector]
    public Vector3      pos;
    [HideInInspector]
    public ChessUnit    unit;

    private float       yPosOnClick     = 2;
    private float       yPosOriginal    = 0;

    public bool         SetWaitingTile  = false;
    private bool        OnSelect = false;
    private void Start()
    {
        tileOb = gameObject;
        pos = transform.position;
        unit = null;
        yPosOriginal = transform.position.y;
    }
    
    /// <summary>
    /// NOTE : 유닛이 존재하지 않을 경우 선택된 유닛 설정
    /// </summary>
    /// <param name="_unit"></param>
    public void SetUnit(ChessUnit _unit)
    {
        unit = _unit;
    }
    /// <summary>
    /// NOTE : 기존의 유닛이 존재하고 있을 경우 
    /// </summary>
    public void SwapUnit()
    {

    }

    /// <summary>
    /// NOTE : 유닛을 드래그 할 때 선택된 타일의 상태 설정 변경
    /// </summary>
    public void SetSelectOnEvent()
    {
        if (OnSelect)
            return;
        pos.y = yPosOnClick;
        transform.position = pos;
    }

    /// <summary>
    /// NOTE : 지나갈 경우 다시 설정
    /// </summary>
    public void SetSelectOffEvent()
    {

        pos.y = yPosOriginal;
        transform.position = pos;
    }

}
