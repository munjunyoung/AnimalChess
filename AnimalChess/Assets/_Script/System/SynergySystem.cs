using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynergySystem
{
    List<unitSynergyData> uSynergyDataList = new List<unitSynergyData>();

    private int _catNumber = 0;
    public int catNumber
    {
        get
        {
            return _catNumber;
        }
        set
        {
            _catNumber = value;
            UIManager.instance.SetCatSynergyText(_catNumber);
        }
    }
    private int _rabbitNumber = 0;
    public int rabbitNumber
    {
        get
        {
            return _rabbitNumber;
        }
        set
        {
            _rabbitNumber = value;
            UIManager.instance.SetRabbitSynergyText(_rabbitNumber);
        }
    }
    private int _bearNumber = 0;
    public int bearNumber
    {
        get
        {
            return _bearNumber;
        }
        set
        {
            _bearNumber = value;
            UIManager.instance.SetBearSynergyText(_bearNumber);
        }
    }

    private int _fireNumber = 0;
    public int FireNumber
    {
        get
        {
            return _fireNumber;
        }
        set
        {
            _fireNumber = value;
            UIManager.instance.SetFireSynergyText(_fireNumber);
        }
    }
    private int _waterNumber = 0;
    public int WaterNumber
    {
        get
        {
            return _waterNumber;
        }
        set
        {
            _waterNumber = value;
            UIManager.instance.SetWaterSynergyText(_waterNumber);
        }
    }
    private int _windNumber = 0;
    public int WindNumber
    {
        get
        {
            return _windNumber;
        }
        set
        {
            _windNumber = value;
            UIManager.instance.SetWindSynergyText(_windNumber);
        }
    }
    private int _groundNumber = 0;
    public int GroundNumber
    {
        get
        {
            return _groundNumber;
        }
        set
        {
            _groundNumber = value;
            UIManager.instance.SetGroundSynergyText(_groundNumber);
        }
    }

    private int catSynergy = 0;
    private int rabbitSynergy = 0;
    private int bearSynergy = 0;

    private int fireSynergy = 0;
    private int waterSynergy = 0;
    private int windSynergy = 0;
    private int groundSynergy = 0;


    /// <summary>
    /// NOTE : 하나로 통일  -> 원래는 각각 속성에 하나씩 추가해도 가능할것 같다
    /// </summary>
    /// <param name="unitblocklist"></param>
    public void SetSynergy(List<BlockOnBoard> unitblocklist)
    {
        catSynergy = catNumber / 2;
        rabbitSynergy = rabbitNumber / 2;
        bearSynergy = bearNumber / 2;

        fireSynergy = FireNumber/ 3;
        waterSynergy = WaterNumber / 3;
        windSynergy = WindNumber / 3;
        groundSynergy = GroundNumber / 3;
        
        foreach(var block in unitblocklist)
        {
            var tmpunit = block.GetUnitNormal();
            switch (tmpunit.unitController.unitPdata.tribe)
            {
                case Tribe_Type.Cat:
                    tmpunit.unitController.TribeSynergyLevel = catSynergy;
                    break;
                //스킬공격데미지 
                case Tribe_Type.Rabbit:
                    tmpunit.unitController.TribeSynergyLevel = rabbitSynergy;
                    break;

                //물리방어력
                case Tribe_Type.Bear:
                    tmpunit.unitController.TribeSynergyLevel = bearSynergy;
                    break;
            }

            switch (tmpunit.unitController.unitPdata.attribute)
            {
                //공격력 % 완
                case Attribute_Type.Fire:
                    tmpunit.unitController.AttributeSynergyLevel = fireSynergy;
                    break;
                //생명력 흡수
                case Attribute_Type.Water:
                    tmpunit.unitController.AttributeSynergyLevel= waterSynergy;
                    break;
                //공속
                case Attribute_Type.Wind:
                    tmpunit.unitController.AttributeSynergyLevel= windSynergy;
                    break;
                //체력
                case Attribute_Type.Ground:
                    tmpunit.unitController.AttributeSynergyLevel = groundSynergy;
                    break;
            }
        }
    }
    

    public void AddSynergy(UnitBlockData unitblock)
    {
        unitSynergyData tmpdata = new unitSynergyData(unitblock.unitController.unitPdata.tribe, unitblock.unitController.unitPdata.attribute);

        //같은 종류의 내용이 이미 존재할경우 리스트에 추가하고 숫자는 변경하지 않음 return
        if (uSynergyDataList.Contains(tmpdata))
        {
            uSynergyDataList.Add(tmpdata);
            SetSynergy(BoardManager.instance.GetBattleBlockOnUnit());
            return;
        }
        uSynergyDataList.Add(tmpdata);

        switch (tmpdata.tribe)
        {
            case Tribe_Type.Cat:
                catNumber++;
                break;
            case Tribe_Type.Rabbit:
                rabbitNumber++;
                break;
            case Tribe_Type.Bear:
                bearNumber++;
                break;
        }

        switch (tmpdata.attribute)
        {
            case Attribute_Type.Fire:
                FireNumber++;
                break;
            case Attribute_Type.Water:
                WaterNumber++;
                break;
            case Attribute_Type.Wind:
                WindNumber++;
                break;
            case Attribute_Type.Ground:
                GroundNumber++;
                break;
        }

        SetSynergy(BoardManager.instance.GetBattleBlockOnUnit());
    }

    public void RemoveSynergy(UnitBlockData unitblock)
    {
        unitSynergyData tmpdata = new unitSynergyData(unitblock.unitController.unitPdata.tribe, unitblock.unitController.unitPdata.attribute);

        uSynergyDataList.Remove(tmpdata);
        //같은 종류의 내용이 이미 존재할경우 리스트에 추가하고 숫자는 변경하지 않음 return
        if (uSynergyDataList.Contains(tmpdata))
        {

            return;
        }
        switch (unitblock.unitController.unitPdata.tribe)
        {
            case Tribe_Type.Cat:
                catNumber--;
                break;
            case Tribe_Type.Rabbit:
                rabbitNumber--;
                break;
            case Tribe_Type.Bear:
                bearNumber--;
                break;
        }

        switch (unitblock.unitController.unitPdata.attribute)
        {
            case Attribute_Type.Fire:
                FireNumber--;
                break;
            case Attribute_Type.Water:
                WaterNumber--;
                break;
            case Attribute_Type.Wind:
                WindNumber--;
                break;
            case Attribute_Type.Ground:
                GroundNumber--;
                break;
        }
        SetSynergy(BoardManager.instance.GetBattleBlockOnUnit());
    }

    /// <summary>
    /// NOTE : 중복 시너지를 관리하기위해서 생성
    /// </summary>
    private struct unitSynergyData
    {
        public Tribe_Type tribe;
        public Attribute_Type attribute;

        public unitSynergyData(Tribe_Type _tribe, Attribute_Type _attribute)
        {
            tribe = _tribe;
            attribute = _attribute;
        }
    }
}

