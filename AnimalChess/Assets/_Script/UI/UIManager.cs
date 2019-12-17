using System.Collections;
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

    [Header("TopProfile"), SerializeField]
    private GameObject PlayerHpPanel;
    bool isRunningShakePanel = false;
    float count = 0;
    [SerializeField]
    private Text levelText, playerHPText, unitNumberText, goldText, expText, waitCountText, roundNumberText, gameStateText;
    //TAKE Damage Playerhp
    public Vector3 hpTextPosition;
    private Color impossibleColor = new Color(1, 0, 0, 0.5f);
    private Color waitStateColor = new Color(0.6f, 0.9f, 0.6f, 1f);
    private Color battleStateColor = new Color(0.8f, 0, 0, 1);

    [Header("Shop")]
    public RectTransform ShopPanel;
    [SerializeField]
    private Toggle lockToggle;
    private readonly Vector2 ShopHidePos = new Vector2(0, 700);
    private readonly Vector2 ShopShowPos = new Vector2(0, 70);
    private Vector2 ShopTargetPos = Vector2.zero;
    private Vector2 currentShopPos = Vector2.zero;
    private bool isRunningShopCoroutine = false;
    private bool isLockShop = false;
    private bool isViewShop = false;

    [Header("ProfilePanel")]
    //UnitProfilePanel
    [SerializeField]
    private RectTransform profilePanel;
    [SerializeField]
    private Transform rtCam;
    [SerializeField]
    private Text unitID, profileHpText, profileMpText, atkText, defText, atkSpeedText;
    [SerializeField]
    private Image tribeImage, attributeImage, item;
    [SerializeField]
    private GameObject[] ratingImages;
    private readonly Vector2 profileHidePos = new Vector2(250, -5);
    private readonly Vector2 profileShowPos = new Vector2(-10, -5);
    private Vector2 profileTargetPos = Vector2.zero;
    private Vector2 currentProfilePos = Vector2.zero;
    private bool isRunningProfileCoroutine = false;

    [Header("SliderBar")]
    [SerializeField]
    private Transform playerUnitSliderParent;
    [SerializeField]
    private Transform enemyUnitSliderParent;
    
    public List<HpMpSlider> playerUnitSliderList;
    public List<HpMpSlider> enemyUnitSliderList;
    
    [SerializeField]
    private Text catTribeText, rabbitTribeText, bearTribeText;
    [SerializeField]
    private Text fireAttributeText, waterAttributeText, windAttributeText, groundAttributeText;  


    private void Awake()
    {
        if (instance == null)
            instance = this;
        SetSliderList();

        var pos = playerHPText.transform.position + new Vector3(0, 0, 10);
        hpTextPosition = Camera.main.ScreenToWorldPoint(pos);
    }


    
    #region SetTextUI

    public void SetPlayerHpText(int hp)
    {
        playerHPText.text = hp.ToString();
        if (hp != 100)
            TakeDamagePlayerHP();
    }

    public void TakeDamagePlayerHP()
    {
        count = 0;
        if (!isRunningShakePanel)
            StartCoroutine(ShakePlayerHpPanelProcess(2f, 1f, PlayerHpPanel.transform.localPosition));
    }

    /// <summary>
    /// NOTE : 이펙트로 적에게 공격당하는 흔들림
    /// </summary>
    /// <param name="_shakeAmount"></param>
    /// <param name="_duration"></param>
    /// <param name="originalPos"></param>
    /// <returns></returns>
    IEnumerator ShakePlayerHpPanelProcess(float _shakeAmount, float _duration, Vector3 originalPos)
    {
        isRunningShakePanel = true;
        while (count <= _duration)
        {
            count += Time.deltaTime;
            PlayerHpPanel.transform.localPosition = (Vector3)Random.insideUnitCircle * _shakeAmount + originalPos;
            yield return null;
        }
        isRunningShakePanel = false;
        transform.localPosition = originalPos;

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
    public void ShowUnitProfile(UnitController unit)
    {
        rtCam.localPosition = unit.unitPdata.camPos;
        unitID.text = unit.unitPdata.name;

        switch (unit.unitPdata.originalCost)
        {
            case 1:
                unitID.color = Color.white;
                break;
            case 2:
                unitID.color = new Color(0.6f, 1, 0.4f);
                break;
            case 3:
                unitID.color = new Color(0.4f, 0.8f, 1f);
                break;
            case 4:
                unitID.color = new Color(1, 0.2f, 0.4f);
                break;
            //현재 레벨5는 존재하지 않음
            case 5:
                unitID.color = new Color(1, 0.8f, 0.4f);
                break;
            default:
                break;
        }

        tribeImage.sprite = unit.unitPdata.tribeSprite;
        attributeImage.sprite = unit.unitPdata.attributeSprite;
        profileTargetPos = profileShowPos;
        currentProfilePos = profilePanel.anchoredPosition;
        SetRatingImage(unit.unitPdata.ratingValue);
        profileHpText.text = unit.abilityDataInBattle.totalMaxHp.ToString();
        profileMpText.text = unit.abilityDataInBattle.maxMP.ToString();
        atkText.text = unit.abilityDataInBattle.totalAttackDamage.ToString();
        defText.text = unit.abilityDataInBattle.PhysicalDefense.ToString();
        atkSpeedText.text = unit.abilityDataInBattle.totalAttackSpeedRate.ToString();

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
        if (isLockShop)
            return;
        if (IngameManager.instance.playerData.Gold < 2)
            return;
        SetShopPanelCharacter(IngameManager.instance.playerData.Level, IngameManager.instance.playerData.Gold);
        IngameManager.instance.playerData.Gold -= 2;
    }

    public void LevelUpButtonClick()
    {
        if (IngameManager.instance.playerData.Gold < 5)
            return;
        if (IngameManager.instance.playerData.Level >= 7)
            return;

        IngameManager.instance.playerData.ExpValue += 5;
        IngameManager.instance.playerData.Gold -= 5;
    }

    public void SetLockShop()
    {
        isLockShop = !lockToggle.isOn;
    }

    /// <summary>
    /// NOTE : 상점 유닛들 데이터 초기화
    /// </summary>
    /// <param name="level"></param>
    /// <param name="gold"></param>
    public void SetShopPanelCharacter(int level, int gold)
    {
        if (isLockShop)
            return;
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

    #region Hp/MpSlider
    /// <summary>
    /// NOTE : 슬라이더 리스트들 초기화
    /// </summary>
    private void SetSliderList()
    {
        for (int i = 0; i < playerUnitSliderParent.transform.childCount; i++)
            playerUnitSliderList.Add(new HpMpSlider(playerUnitSliderParent.transform.GetChild(i).gameObject));
        for (int i = 0; i < enemyUnitSliderParent.transform.childCount; i++)
            enemyUnitSliderList.Add(new HpMpSlider(enemyUnitSliderParent.transform.GetChild(i).gameObject));
    }
    
    public void SetOffSliderList()
    {
        foreach(var slider in enemyUnitSliderList)
            slider.panel.SetActive(false);
        foreach (var slider in playerUnitSliderList)
            slider.panel.SetActive(false);
    }
    #endregion

    #region Synergy
    //Tribe
    public void SetCatSynergyText(int number)
    {
        if (number == 0)
        {
            catTribeText.text = "0";
            return;
        }
        var maxnumber = number < 2 ? 2 : 4;
        catTribeText.text = number.ToString() + "/" + maxnumber;
    }
    public void SetRabbitSynergyText(int number)
    {
        if (number == 0)
        {
            rabbitTribeText.text = "0";
            return;
        }
        var maxnumber = number < 2 ? 2 : 4;
        rabbitTribeText.text = number.ToString() + "/" + maxnumber;
    }
    public void SetBearSynergyText(int number)
    {
        if (number == 0)
        {
            bearTribeText.text = "0";
            return;
        }
        var maxnumber = number < 2 ? 2 : 4;
        bearTribeText.text = number.ToString() + "/" + maxnumber;
    }

    //Attribute
    public void SetFireSynergyText(int number)
    {
        if (number == 0)
        {
            fireAttributeText.text = "0";
            return;
        }
        int maxnumber = 3;
        fireAttributeText.text = number.ToString() + "/" + maxnumber;
    }
    public void SetWaterSynergyText(int number)
    {
        if (number == 0)
        {
            waterAttributeText.text = "0";
            return;
        }
        int maxnumber = 3;
        waterAttributeText.text = number.ToString() + "/" + maxnumber;
    }
    public void SetWindSynergyText(int number)
    {
        if (number == 0)
        {
            windAttributeText.text = "0";
            return;
        }
        int maxnumber = 3;
        windAttributeText.text = number.ToString() + "/" + maxnumber;
    }
    public void SetGroundSynergyText(int number)
    {
        if (number == 0)
        {
            groundAttributeText.text = "0";
            return;
        }
        int maxnumber = 3;
        groundAttributeText.text = number.ToString() + "/" + maxnumber;
    }

    #endregion
}

[System.Serializable]
public class HpMpSlider
{
    public GameObject panel;
    public Slider hpSlider;
    public Slider mpSlider;
    public Text hpText;
    public Text mpText;

    public HpMpSlider(GameObject _panel)
    {
        panel = _panel;
        hpSlider = panel.transform.Find("UnitHpSlider").GetComponent<Slider>();
        mpSlider = panel.transform.Find("UnitMpSlider").GetComponent<Slider>();
        hpText = panel.transform.Find("UnitHpText").GetComponent<Text>();
        mpText = panel.transform.Find("UnitMpText").GetComponent<Text>();
    }
}
