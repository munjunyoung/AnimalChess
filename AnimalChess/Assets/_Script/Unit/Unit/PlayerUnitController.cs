using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnitController : UnitController
{
    //Effect
    private GameObject tribeEffect;
    private GameObject attributeEffect;


    public override void ResetUnitDataToWatingBoard()
    {
        base.ResetUnitDataToWatingBoard();
        SetTribeEffect(0);
        SetAttributeEffect(0);
    }
    // Start is called before the first frame update
    public override void Init()
    {
        base.Init();
        tribeEffect = unitblockSc.transform.Find("SynergyEffect").transform.Find("TribeEffect").gameObject;
        attributeEffect = unitblockSc.transform.Find("SynergyEffect").transform.Find("AttributeEffect").gameObject;
    }

    public override void StartUnitInBattle(HpMpSlider sliderdata, float waitingTime)
    {
        
        base.StartUnitInBattle(sliderdata, waitingTime);
        if (targetList.Count == 0)
        {
            var tmplist = BoardManager.instance.currentEnemyUnitList;
            foreach (var targetunit in tmplist)
                targetList.Add(targetunit);
        }
        
    }

    /// <summary>
    /// NOTE : 플레이어 유닛이 모두 죽었는지 체크
    /// </summary>
    public override void DeadAction()
    {
        base.DeadAction();
        //모든 적이 죽었는지 체크
        IngameManager.instance.CheckUnitAlive();
    }

    //플레이어 유닛에만 적용
    #region  SynergyEffect
    public void SetEffectObject()
    {
        unitPdata.SetEffect(tribeEffect.transform, attributeEffect.transform);
    }
    #endregion

    public override void SetTribeEffect(int level)
    {
        if (level == 0)
            tribeEffect.SetActive(false);
        else
            tribeEffect.SetActive(true);
    }
    

    public override void SetAttributeEffect(int level)
    {
        if (level == 0)
            attributeEffect.SetActive(false);
        else
            attributeEffect.SetActive(true);
    }
}
