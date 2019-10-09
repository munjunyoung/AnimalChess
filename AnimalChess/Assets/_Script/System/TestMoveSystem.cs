using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestMoveSystem : MonoBehaviour
{
    private bool pickUpObject_Mouse = false;
    private ChessUnit target = null;

    private Vector3 m_curPos;
    private Vector3 m_prevPos;

    private readonly Vector3 NormalSize = Vector3.one;
    private readonly Vector3 PickUpSize = Vector3.one * 1.5f;

    private readonly float yPosPickUP = 3f;
    
    
    private void Update()
    {
        MouseClick();
    }

    /// <summary>
    /// NOTE : Mouse Click
    /// </summary>
    private void MouseClick()
    {
        if (Input.GetMouseButtonDown(0))
            GetClickDownObject();

        if (Input.GetMouseButton(0))
            GetDragObject();
    }

    /// <summary>
    /// NOTE : Click Get Object;
    /// </summary>
    /// <returns></returns>
    private void GetClickDownObject()
    {
        RaycastHit targetHit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out targetHit);
        if (targetHit.collider != null)
        {
            if (targetHit.collider.CompareTag("Unit"))
            {
                target = new ChessUnit(targetHit.transform.gameObject, targetHit.transform.position);
                PickUpObjectEvent();
            }
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    private void GetDragObject()
    {
        if(pickUpObject_Mouse)
        {
            //추후에 터치로 변경 z값이 스크린의 plane 포인터
            var point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y - yPosPickUP));
            Debug.Log(point);
            point = new Vector3(point.x, yPosPickUP, point.z + (point.y - yPosPickUP));
            target.unitOB.transform.localPosition = point;
        }
    }

    /// <summary>
    /// NOTE : 오브젝트를 집었을때 이벤트
    /// </summary>
    private void PickUpObjectEvent()
    {
        pickUpObject_Mouse = true;
        target.unitOB.transform.localScale = PickUpSize;

    }

    /// <summary>
    /// NOTE : 오브젝트를 놓았을때 이벤트 노말사이즈 변경 및 위치 지정이 안되었을 경우 포지션 지정
    /// </summary>
    private void PickDownObjectEvent()
    {
        target.unitOB.transform.localPosition = NormalSize;
        target.unitOB.transform.localPosition = target.prevPos;
    }

    private void CheckGroundTile(Vector3 point)
    {
        int xPos = (int)point.x;
        int yPos = (int)point.y;   
    }
}

public class ChessUnit
{
    public GameObject unitOB;
    public Vector3 prevPos;

    public ChessUnit(GameObject _unitob, Vector3 _prevPos)
    {
        unitOB = _unitob;
        prevPos = _prevPos;
    }

    public ChessUnit()
    {
        unitOB = null;
        prevPos = Vector3.zero;
    }
}