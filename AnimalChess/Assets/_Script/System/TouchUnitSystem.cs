using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchUnitSystem : MonoBehaviour
{
    private UnitBlockData target = null;
    private BlockOnBoard startBlock = null;

    private BlockOnBoard nextBlock = null;
    private bool IsPickUp = false;

    [SerializeField]
    private GameObject hightlightedEffect = null;
    private int blockMask;

    public bool IsPointerOnSellPanel = false;
    [SerializeField]
    SellPanelHandler sellPanel;

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
        if (Input.GetMouseButton(0))
            TouchDragObject();
        if (Input.GetMouseButtonUp(0))
            TouchUpObject();
    }

    /// <summary>
    /// NOTE : Click Get Object;
    /// </summary>
    /// 
    /// <returns></returns>
    private void TouchDownObject()
    {
        if (IsPointerOnSellPanel)
        {
            nextBlock = null;
            return;
        }
        RaycastHit targetHit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out targetHit, 50, blockMask);
        if (targetHit.collider != null)
        {
            //오브젝트를 집었을때 이벤트
            startBlock = targetHit.transform.GetComponent<BlockOnBoard>();
            target = startBlock.GetUnitRemoveList();
            //Target이 존재하지 않으면 return
            if (target == null)
                return;
            //모델이 하위오브젝트에 존재하여 드래그중에 떨어지지 않게 하기 위함
            target.unitController.rb.useGravity = false;
            IsPickUp = true;
            hightlightedEffect.SetActive(true);
            UIManager.instance.ShowUnitProfile(target.unitController);
            return;
        }
    }

    /// <summary>
    /// NOTE : 마우스 포지션을 기준으로 Ray를 통하여 해당 block 체크
    /// </summary>
    private void TouchDragObject()
    {
        if (!IsPickUp)
            return;
        if (sellPanel.IsPointerOnSellPanel)
        {
            nextBlock = null;
            return;
        }

        RaycastHit targetHit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out targetHit, 50, blockMask);

        if (targetHit.collider != null)
        {
            nextBlock = targetHit.transform.GetComponent<BlockOnBoard>();
            target.transform.position = nextBlock.transform.position + (Vector3.up * 4);
        }
        else
        {
            nextBlock = null;
            target.transform.position = startBlock.transform.position + (Vector3.up * 4);
        }
        hightlightedEffect.transform.position = target.transform.position + (Vector3.down * 3);
    }

    /// <summary>
    /// NOTE : 유닛 드랍 이벤트
    /// </summary>
    private void TouchUpObject()
    {
        hightlightedEffect.SetActive(false);
        if (!IsPickUp)
            return;
           
        //NEXTBLOCK이 null일 경우 유닛은 기존자리로 돌아감
        if (nextBlock == null)
        {
            startBlock.SetUnitaddList(target);
            if (sellPanel.IsPointerOnSellPanel)
                BoardManager.instance.SellUnit(startBlock);
        }
        else
        {
            hightlightedEffect.SetActive(false);
            //유닛이 없을 경우 
            if (nextBlock.GetUnitNormal() == null)
            {
                nextBlock.SetUnitaddList(target);
                //보드에 올릴수 있는 유닛수가 최대넘어갈 경우 조건문 추가해야함
            }
            else
            {
                //기존과 같은 block일경우 처리 
                if (startBlock == nextBlock)
                    nextBlock.SetUnitaddList(target);
                //다를 경우 스왑
                else
                {
                    var tmpUnit = nextBlock.GetUnitRemoveList();
                    nextBlock.SetUnitaddList(target);
                    startBlock.SetUnitaddList(tmpUnit);
                }
            }
        }
        SetDataPickDown();
    }

    /// <summary>
    /// NOTE : 전투가 시작될때 드래그중인 배틀보드 위의 유닛을 다시 제자리로 되돌림
    /// </summary>
    public void ReturnPickState()
    {
        //픽업 상태가 아니고, block 이 waiting block 이면 리턴
        if (!IsPickUp)
            return;
        
        startBlock.SetUnitaddList(target);
        SetDataPickDown();
    }
    
    public void SetDataPickDown()
    {
        //겹쳐지지 않게 하기 위함
        target.unitController.rb.useGravity = true;
        UIManager.instance.HideUnitProfile();
        startBlock = null;
        nextBlock = null;
        target = null;
        IsPickUp = false;
        hightlightedEffect.SetActive(false);
    }
    
    public UnitBlockData getTargetUnit()
    {
        return target;
    }
}
