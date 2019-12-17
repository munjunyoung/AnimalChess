using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class TooltipTopPanel : TooltipHandler
{
    public override void OnPointerEnter(PointerEventData eventData)
    {
        var pos = transform.position;

        pos.y -= 40;
        tooltipPanel.transform.position = pos;
        tooltipPanel.SetActive(true);
    }
}
