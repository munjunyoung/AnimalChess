using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnitController : UnitController
{
    //Effect
    protected GameObject tribeEffect;
    protected GameObject attributeEffect;

    
    #region SetData
    public override void SetUnitAbilityDataToNormalData()
    {
        base.SetUnitAbilityDataToNormalData();
        TribeSynergyLevel = 0;
        AttributeSynergyLevel = 0;
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
    #endregion


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
    /// <summary>
    /// NOTE : 캐릭터 생성시 실행
    /// </summary>
    public override void SetObjectData()
    {
        base.SetObjectData();
        unitPdata.SetEffect(tribeEffect.transform, attributeEffect.transform);
        SetSynergyParam();
    }

    /// <summary>
    /// 시너지 클래스 클래스 변수 초기화
    /// </summary>
    public virtual void SetSynergyParam()
    {
        tribeSynergy.SetParam(this, tribeEffect);
        attributeSynergy.SetParam(this, attributeEffect);
    }
    
    #endregion
}
