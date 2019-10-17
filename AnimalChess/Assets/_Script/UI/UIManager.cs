﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public static UIManager instance = null;
    
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

    [Header("Shop")]
    public RectTransform ShopPanel;
    private readonly Vector2 ShopHidePos = new Vector2(0, 700);
    private readonly Vector2 ShopShowPos = new Vector2(0, 30);
    private Vector2 ShopTargetPos = Vector2.zero;
    private Vector2 currentShopPos = Vector2.zero;
    private bool isRunningShopCoroutine = false;
    private bool isViewShop = false;

    [Header("ProfilePanel")]
    //UnitProfilePanel
    [SerializeField]
    private RectTransform profilePanel;
    [SerializeField]
    private Transform rtCam;
    [SerializeField]
    private Text     unitID;
    [SerializeField]
    private Image tribeImage, attributeImage, item1, item2, itme3;
    [SerializeField]
    private GameObject[] ratingImages;
    private readonly Vector2 profileHidePos = new Vector2(250, -5);
    private readonly Vector2 profileShowPos = new Vector2(-10, -5);
    private Vector2 profileTargetPos = Vector2.zero;
    private Vector2 currentProfilePos = Vector2.zero;
    private bool isRunningProfileCoroutine = false;
    

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

    public void SetUnitNumberText(int _unitnumber)
    {
        unitNumberText.text = _unitnumber.ToString();
    }

    public void SetWaitingCountText(int _count)
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

    #region  Profile
    public void ShowUnitProfile(UnitPropertyData _pdata)
    {
        rtCam.localPosition = _pdata.camPos;
        unitID.text = _pdata.name;
        tribeImage.sprite = _pdata.tribeSprite;
        attributeImage.sprite = _pdata.attributeSprite;
        profileTargetPos = profileShowPos;
        currentProfilePos = profilePanel.anchoredPosition;
        SetRatingImage(_pdata.ratingValue);
        if (!isRunningProfileCoroutine)
            StartCoroutine(SetProfilePos());
    }

    public void HideUnitProfile()
    {
        profileTargetPos = profileHidePos;
        currentProfilePos = profilePanel.anchoredPosition;
        if (!isRunningProfileCoroutine)
            StartCoroutine(SetProfilePos());
    }

    IEnumerator SetProfilePos()
    {
        float count = 0;
        isRunningProfileCoroutine = true;
        while(!profilePanel.anchoredPosition.Equals(profileTargetPos))
        {
            count += Time.fixedDeltaTime * 5;
            profilePanel.anchoredPosition = Vector2.Lerp(currentProfilePos,profileTargetPos,count);
            yield return new WaitForFixedUpdate();
        }
        isRunningProfileCoroutine = false;
    }

    private void SetRatingImage(int _ratingvalue)
    {
        for(int i = 0; i<3; i++)
        {
            if (i < _ratingvalue)
            {
                ratingImages[i].SetActive(true);
                continue;
            }
            ratingImages[i].SetActive(false);
        }
    }
    #endregion

    #region SHOP
    /// <summary>
    /// NOTE : UI SHOP 버튼 함수 , 패널 ON/OFF
    /// </summary>
    public void ShopButtonClick()
    {
        isViewShop = isViewShop ? false : true;
        ShopTargetPos = isViewShop ? ShopShowPos : ShopHidePos;
        currentShopPos = ShopPanel.anchoredPosition;
        if(!isRunningShopCoroutine)
            StartCoroutine(SetShopPanelPos());
    }

    IEnumerator SetShopPanelPos()
    {
        float count = 0;
        isRunningShopCoroutine = true;
        while(!ShopPanel.anchoredPosition.Equals(ShopTargetPos))
        {
            count += Time.fixedDeltaTime * 5f;
            ShopPanel.anchoredPosition = Vector2.Lerp(currentShopPos, ShopTargetPos, count);
            yield return new WaitForFixedUpdate();
        }
        isRunningShopCoroutine = false;
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

        BuyExpButtonImage.color = gold >= 5 ? Color.white : impossibleColor;
        reRollButtonImage.color = gold >= 2 ? Color.white : impossibleColor;
    }
    
#endregion
}
