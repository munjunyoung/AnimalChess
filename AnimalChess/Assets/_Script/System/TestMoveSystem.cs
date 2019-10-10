using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMoveSystem : MonoBehaviour
{
    private Unit target = null;
    private BlockOnBoard prevBlock = null;
    private BlockOnBoard nextBlock = null;
    private bool pickUpObject_Mouse = false;

    private readonly Vector3 pickDownSize = Vector3.one;
    private readonly Vector3 pickUpSize = Vector3.one * 1.5f;

    private readonly float yPosPickUP = 5f;
    [SerializeField]
    private GameObject hightlightedEffect;

    private int blockMask;

    private void Start()
    {
        blockMask = 1 << LayerMask.NameToLayer("DeploybleBlock");
    }

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
            TouchDownObject();

        if (!pickUpObject_Mouse)
            return;
        if (Input.GetMouseButton(0))
            TouchDragObject();
        if (Input.GetMouseButtonUp(0))
            TouchUpObject();
    }

    /// <summary>
    /// NOTE : Click Get Object;
    /// </summary>
    /// <returns></returns>
    private void TouchDownObject()
    {
        RaycastHit targetHit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out targetHit, 50, blockMask);
        if (targetHit.collider != null)
        {
            //오브젝트를 집었을때 이벤트
            prevBlock = targetHit.transform.GetComponent<BlockOnBoard>();
            if (prevBlock.GetUnit() == null)
                return;

            target = prevBlock.GetUnit();
            pickUpObject_Mouse = true;
            return;
        }
    }

    /// <summary>
    /// NOTE : 마우스 포지션을 기준으로 Ray를 통하여 해당 block 체크
    /// </summary>
    private void TouchDragObject()
    {
        //추후에 터치로 변경 z값이 스크린의 plane 포인터
        var point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y - yPosPickUP));
        point = new Vector3(point.x, yPosPickUP, point.z + (point.y - yPosPickUP));
        target.transform.localPosition = point;

        RaycastHit targetHit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out targetHit, 50, blockMask);

        if (targetHit.collider != null)
        {
            nextBlock = targetHit.transform.GetComponent<BlockOnBoard>();
            hightlightedEffect.transform.position = new Vector3(nextBlock.transform.position.x, nextBlock.transform.position.y + 1f, nextBlock.transform.position.z);
            hightlightedEffect.SetActive(true);
        }
        else
        {
            hightlightedEffect.transform.position = Vector3.zero;
            hightlightedEffect.SetActive(false);
            nextBlock = null;
        }

    }

    /// <summary>
    /// NOTE : 유닛 드랍 이벤트
    /// </summary>
    private void TouchUpObject()
    {
        target.transform.localScale = pickDownSize;
        //NEXTBLOCK이 null일 경우 유닛은 기존자리로 돌아감
        if (nextBlock == null)
        {
            prevBlock.SetUnit(target);
        }
        else
        {
            //유닛이 없을 경우 
            if (nextBlock.GetUnit() == null)
            {
                nextBlock.SetUnit(target);
                prevBlock.SetUnit(null);
                //보드에 올릴수 있는 유닛수가 최대넘어갈 경우 조건문 추가해야함
            }
            else
            {
                //Swap
                //기존과 같은 block일경우 처리 
                if (prevBlock == nextBlock)
                {
                    nextBlock.SetUnit(target);
                    return;
                }
                var tmpUnit = nextBlock.GetUnit();
                nextBlock.SetUnit(this.prevBlock.GetUnit());
                this.prevBlock.SetUnit(tmpUnit);
            }
        }

        prevBlock = null;
        nextBlock = null;
        target = null;
        pickUpObject_Mouse = false;
        hightlightedEffect.SetActive(false);
    }
}
