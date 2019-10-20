using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SellPanelHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{ 
    [HideInInspector]
    public bool IsPointerOnSellPanel = false;
    public void OnPointerEnter(PointerEventData eventData)
    {
        IsPointerOnSellPanel = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsPointerOnSellPanel = false;
    }
}
