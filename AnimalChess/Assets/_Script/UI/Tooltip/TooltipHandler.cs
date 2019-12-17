using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TooltipHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject tooltipPanel;
    protected Text tooltipTitleText;
    protected Text tooltipContentsText;

    private void Start()
    {
        if(tooltipPanel.activeSelf)
            tooltipPanel.SetActive(false);
        tooltipTitleText = tooltipPanel.transform.Find("TitleText").GetComponent<Text>();
        tooltipContentsText = tooltipPanel.transform.Find("ContentsText").GetComponent<Text>();
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        var pos = transform.position;
        
        pos.x += 35;
        tooltipPanel.transform.position = pos;
        tooltipPanel.SetActive(true);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        tooltipPanel.SetActive(false);
    }
}
