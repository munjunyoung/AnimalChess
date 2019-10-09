using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestMoveSystem : MonoBehaviour
{
    private bool pickUpObject_Mouse = false;
    private Unit target = null;
    private Vector3 prevPos = Vector3.zero;

    private Vector3 m_curPos;
    private Vector3 m_prevPos;

    private readonly Vector3 NormalSize = Vector3.one;
    private readonly Vector3 PickUpSize = Vector3.one * 1.5f;

    private readonly float yPosPickUP = 3f;

    int unitMask = 1 << LayerMask.NameToLayer("Unit");
    int tileMask = 1 << LayerMask.NameToLayer("BoardTile");


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
            PointerDownObject();

        if (Input.GetMouseButton(0))
            PointerDragObject();

        if (Input.GetMouseButtonUp(0))
            PointerUpObject();
    }

    /// <summary>
    /// NOTE : Click Get Object;
    /// </summary>
    /// <returns></returns>
    private void PointerDownObject()
    {
        RaycastHit targetHit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out targetHit, 50, unitMask);
        if (targetHit.collider != null)
        {
            PickUpUnitEvent(targetHit);
            return;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void PointerDragObject()
    {
        if (pickUpObject_Mouse)
        {
            //추후에 터치로 변경 z값이 스크린의 plane 포인터
            var point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y - yPosPickUP));
            Debug.Log(point);
            point = new Vector3(point.x, yPosPickUP, point.z + (point.y - yPosPickUP));
            target.transform.localPosition = point;

            RaycastHit targetHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out targetHit, 50, tileMask);

            if (targetHit.collider!=null)
            {

            }
        }
    }

    private void PointerUpObject()
    {
        GetBackUnitEvents();
    }

    /// <summary>
    /// NOTE : 오브젝트를 집었을때 이벤트
    /// </summary>
    /// <param name="_targethit"></param>
    private void PickUpUnitEvent(RaycastHit _targethit)
    {
        target = _targethit.transform.GetComponent<Unit>();
        prevPos = _targethit.transform.position;
        pickUpObject_Mouse = true;
        target.transform.localScale = PickUpSize;
    }

    /// <summary>
    /// NOTE : 오브젝트를 놓았을때 이벤트 노말사이즈 변경 및 위치 지정이 안되었을 경우 포지션 지정
    /// </summary>
    private void GetBackUnitEvents()
    {
        if (pickUpObject_Mouse)
        {
            target.transform.localPosition = NormalSize;
            target.transform.localPosition = prevPos;
        }
        target = null;
        pickUpObject_Mouse = false;
    }

    private void DropUnitEvent()
    {

    }

    private void CheckGroundTile(Vector3 point)
    {
        int xPos = (int)point.x;
        int yPos = (int)point.y;
    }
}

public class ChessUnit
{
    
}