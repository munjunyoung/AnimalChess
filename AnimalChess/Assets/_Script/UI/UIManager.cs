using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public static UIManager instance = null;
    
    public GameObject ShopCanvas;
    //Display Panel
    [SerializeField]
    private List<UnitProductPanel> unitPanelList = new List<UnitProductPanel>();
    //Button BackGround
    [SerializeField]
    private Image reRollButtonImage, BuyExpButtonImage;

    [SerializeField]
    private Text levelText, hpText, unitNumberText, goldText, expText, waitCountText, roundNumberText, gameStateText;
    
    private Color impossibleColor  = new Color(1,0,0,0.5f);
    private Color waitStateColor   = new Color(0.6f, 0.9f, 0.6f, 1f);
    private Color battleStateColor = new Color(0.8f, 0, 0, 1);
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    #region SetTextUI
 
    public void SetHpText(int hp)
    {
        hpText.text = hp.ToString();
    }

    public void SetExpText(int _exp, int _requireExp)
    {
        expText.text = _exp + "/" + _requireExp;
    }

    public void SetLevelText(int _level)
    {
        levelText.text = _level.ToString();
    }

    public void SetUnitNumberText(int _unitnumber, int _level)
    {
        unitNumberText.text = _unitnumber + "/" + _level;
    }

    public void SetCountText(int _count)
    {
        if (_count == 0)
        {
            waitCountText.text = " ";
            return;
        }
        waitCountText.text = _count.ToString();
        waitCountText.color = _count <= 10 ? Color.red : Color.white;

    }

    public void SetRoundNumberText(int _roundnumber)
    {
        roundNumberText.text = _roundnumber.ToString();
    }

    public void SetGameStateText(bool _battleStart)
    {
        gameStateText.text = _battleStart == true ? "In Battle" : "Waiting";
        gameStateText.color = _battleStart == true ? battleStateColor : waitStateColor;
    }

    public void SetGoldText(int _gold)
    {
        goldText.text = _gold.ToString().ToString();
    }

    #endregion
    #region SHOP
    /// <summary>
    /// NOTE : UI SHOP 버튼 함수 , 패널 ON/OFF
    /// </summary>
    public void ShopButtonClick()
    {
        if (!ShopCanvas.activeSelf)
            ShopCanvas.SetActive(true);
        else
            ShopCanvas.SetActive(false);
    }
   
    /// <summary>
    /// NOTE : Reroll 버튼 함수 , 상점 다시 섞기
    /// </summary>
    public void RerollButtonClick()
    {
        if (IngameManager.instance.pData.Gold < 2)
            return;
        SetShopPanelCharacter(IngameManager.instance.pData.Level, IngameManager.instance.pData.Gold);
        IngameManager.instance.pData.Gold -= 2;
    }

    public void LevelUpButtonClick()
    {
        if (IngameManager.instance.pData.Gold < 5)
            return;
        if (IngameManager.instance.pData.Level >= 7)
            return;

        IngameManager.instance.pData.ExpValue += 5;
        IngameManager.instance.pData.Gold -= 5;
    }

    /// <summary>
    /// NOTE : 상점 유닛들 데이터 초기화
    /// </summary>
    /// <param name="level"></param>
    /// <param name="gold"></param>
    public void SetShopPanelCharacter(int level, int gold)
    {
        foreach (var upanel in unitPanelList)
            upanel.SetData(level, gold);
    }
    
    /// <summary>
    /// NOTE : 골드에 따라 유닛패널, 레벨업, 리롤 패널 색상 변경 
    /// </summary>
    /// <param name="gold"></param>
    public void SetBackGroundColorButtonPanels(int gold)
    {
        foreach (var upanel in unitPanelList)
            upanel.SetBackGroundByGold(gold);

        BuyExpButtonImage.color = gold >= 4 ? Color.white : impossibleColor;
        reRollButtonImage.color = gold >= 2 ? Color.white : impossibleColor;
    }
    
#endregion
}
