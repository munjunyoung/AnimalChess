using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class BearSynergyTooltip : TooltipHandler
{
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        tooltipTitleText.text = "PhysicalDefense";
        tooltipContentsText.text = "2 : +15%  4 : +40% ";
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }
}
