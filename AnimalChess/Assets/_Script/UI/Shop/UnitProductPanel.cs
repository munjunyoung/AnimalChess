using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UnitProductPanel : MonoBehaviour
{
    private UnitPropertyData unitdata;
    [SerializeField]
    private GameObject unitInfoParent;
    [SerializeField]
    private Transform rtCam;
    [SerializeField]
    private Text     nameText;
    [SerializeField]
    private Image    tribeImage;
    [SerializeField]
    private Image    attributeImage;
    [SerializeField]
    private Text     goldText;
    //현재 골드가 구매할 수 있는 골드보다 적을경우 색상변경
    [SerializeField]
    private Image backGroundImage;
    private Color originalColor;
    private Color changeColor;

    public void Awake()
    {
        originalColor = backGroundImage.color;
        changeColor = new Color(1, 0, 0, 0.5F);
    }

    public void SetData(int _currentlevel, int _currentgold)
    {
        unitdata = GetRandomUnitData(_currentlevel);

        rtCam.localPosition = unitdata.camPos;
        nameText.text = unitdata.name;
        tribeImage.sprite = unitdata.tribeSprite;
        attributeImage.sprite = unitdata.attributeSprite;
        goldText.text = unitdata.cost.ToString();

        if(!gameObject.activeSelf)
            gameObject.SetActive(true);
    }
    
    /// <summary>
    /// 현재 골드에 따른 상태 설정
    /// </summary>
    /// <param name="_currentgold"></param>
    public void SetBackGroundByGold(int _currentgold)
    {
        if (unitdata == null)
            return;
        backGroundImage.color = _currentgold >= unitdata.cost ? originalColor : changeColor;
    }

    /// <summary>
    /// NOTE : 전시 될 유닛 랜덤 선택 
    /// </summary>
    /// <param name="_currentlevel"></param>
    /// <returns></returns>
    private UnitPropertyData GetRandomUnitData(int _currentlevel)
    {
        List<UnitPropertyData> unitlist = DataBaseManager.instance.unitTypeListbyGold[GetRandomCostValue(_currentlevel)];
        
        UnitPropertyData selectedUnit = unitlist[Random.Range(0, unitlist.Count)];
        return selectedUnit;
    }

    /// <summary>
    /// NOTE : 1~4 COST 랜덤 선택
    /// </summary>
    /// <param name="_listindex"></param>
    /// <returns></returns>
    private int GetRandomCostValue(int _currentlevel)
    {
        List<int[]> perList = DataBaseManager.instance.ShopUnitPerList;

        var _listindex = _currentlevel;
        int unitcost = 1;
        var randomvalue = Random.Range(0, 100);
        //설정한 값에 따라 코스트 비용 변경
        if (perList[_listindex][0] <= randomvalue && perList[_listindex][1] > randomvalue)
            unitcost = 1;
        else if (perList[_listindex][0] <= randomvalue && perList[_listindex][0] + perList[_listindex][1] > randomvalue)
            unitcost = 2;
        else if (perList[_listindex][0] + perList[_listindex][1] <= randomvalue && perList[_listindex][0] + perList[_listindex][1] + perList[_listindex][2] > randomvalue)
            unitcost = 3;
        else if (perList[_listindex][0] + perList[_listindex][1] + perList[_listindex][2] <= randomvalue && perList[_listindex][0] + perList[_listindex][1] + perList[_listindex][2] + perList[_listindex][3] > randomvalue)
            unitcost = 4;

        return unitcost;
    }

    /// <summary>
    /// NOTE : Button Click Func
    /// </summary>
    public void DisplayUnitPanelClick()
    {
        //구매가 되었을 경우 
        if (BoardManager.instance.BuyUnit(unitdata))       
            gameObject.SetActive(false);
        //UnitPanel Active false
    }
}
