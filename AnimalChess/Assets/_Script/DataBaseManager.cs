﻿using System.Linq;
using System.Collections.Generic;
using UnityEngine;

//Map
public enum Ground_TYPE { DesertBlock1, DesertBlock2, WaitingBlock }
////Unit
public enum Tribe_Type { Cat, Bear, Rabbit }
public enum Attribute_Type  { Fire, Water, Wind, Ground }

//종족, 속성, 등급, 가격 , 유닛 종료 숫자는 databasemanager 변수에 수정 해주어야함
public enum Unit_Type
{
    Cat_Fire_1, Cat_Water_1, Cat_Ground_1, Cat_Wind_1,
    Bear_Water_1, Bear_Fire_1, Bear_Wind_1, Bear_Ground_1,
    Rabbit_Ground_1, Rabbit_Water_1, Rabbit_Wind_1, Rabbit_Fire_1,
}

public class DataBaseManager : MonoBehaviour
{
    public static DataBaseManager instance = null;
    //싱글톤
    //public static LoadDataManager instance = null;

    private readonly string groundPrefabPath = "Map/IngameGround";
    private readonly string tribeSpritePath = "UI/Tribe";
    private readonly string attributeSpritePath = "UI/Attribute";
    private readonly string unitObPath = "Unit/";

    //Map
    public Dictionary<string, BlockOnBoard> groundDic = new Dictionary<string, BlockOnBoard>();

    //Shop UI
    public Dictionary<string, Sprite> TribeSpriteDic = new Dictionary<string, Sprite>();
    public Dictionary<string, Sprite> AttributeSpriteDic = new Dictionary<string, Sprite>();

    //ShopUnit  
    //0 -> 1성 리스트 1 -> 2성 리스트 3 -> 3성리스트
    public List<Dictionary<Unit_Type, UnitPropertyData>> UnitPropertyDataDic = new List<Dictionary<Unit_Type, UnitPropertyData>>();
    public Dictionary<int, List<UnitPropertyData>> unitTypeListbyGold = new Dictionary<int, List<UnitPropertyData>>();
    public List<int[]> ShopUnitPerList = new List<int[]>();
    
    //Unit Ob
    public Dictionary<string, Unit> unitObDic = new Dictionary<string, Unit>();
    
    
    //pData
    public readonly int[] expRequireValueList = {1, 1, 1, 1, 5, 10, 20, 40 };

    private void Awake()
    {
        if (instance == null)
            instance = this;
        
        //Map Data
        SetLoadDataOnDictionary(groundDic, groundPrefabPath);
        //ShoUI Data
        SetLoadDataOnDictionary(TribeSpriteDic, tribeSpritePath);
        SetLoadDataOnDictionary(AttributeSpriteDic, attributeSpritePath);
        //Unit
        SetUnitShopData();
        //UnitOB;
        SetLoadDataOnDictionary(unitObDic, unitObPath + Tribe_Type.Cat.ToString());
        SetLoadDataOnDictionary(unitObDic, unitObPath + Tribe_Type.Bear.ToString());
        SetLoadDataOnDictionary(unitObDic, unitObPath + Tribe_Type.Rabbit.ToString());

    }

    /// <summary>
    /// NOTE : 해당 폴더와 타입에 따른 리소스 로드 및 dictionary 초기화 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_dic"></param>
    /// <param name="_path"></param>
    private void SetLoadDataOnDictionary<T>(Dictionary<string, T> _dic, string _path)
    {
        //T 타입 데이터 캐스팅 로드
        var loadob = Resources.LoadAll(_path, typeof(T)).Cast<T>().ToArray();

        foreach (var lo in loadob)
        {
            //key값의 name설정을 위한 object로 타입 변환
            UnityEngine.Object tmp = lo as UnityEngine.Object;
            _dic.Add(tmp.name, lo);
        }
    }

    /// <summary>
    /// NOTE : 해당 폴더와 타입에 따른 리소스 로드 및 list 초기화 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_list"></param>
    /// <param name="_path"></param>
    private void SetLoadDataOnList<T>(List<T> _list, string _path)
    {
        //T 타입 데이터 캐스팅 로드
        var loadob = Resources.LoadAll(_path, typeof(T)).Cast<T>().ToArray();

        foreach (var lo in loadob)
        {
            //key값의 name설정을 위한 object로 타입 변환
            UnityEngine.Object tmp = lo as UnityEngine.Object;
            _list.Add(lo);
        }
    }

    /// <summary>
    /// NOTE : UNIT DATA 속성 값 초기화
    /// </summary>
    private void SetUnitShopData()
    {
        int typeofUnitcount = System.Enum.GetValues(typeof(Unit_Type)).Length;
        //모든 property data 

        for (int ratingvalue = 1; ratingvalue <= 3; ratingvalue++)
        {
            Dictionary<Unit_Type, UnitPropertyData> tmpList = new Dictionary<Unit_Type, UnitPropertyData>();
            for (int i = 0; i < typeofUnitcount; i++)
            {
                tmpList.Add((Unit_Type)i, new UnitPropertyData((Unit_Type)i, ratingvalue));
            }
            UnitPropertyDataDic.Add(tmpList);
        }

        //각 골드별 유닛 데이터 설정
        List<UnitPropertyData> g1 = new List<UnitPropertyData>();
        List<UnitPropertyData> g2 = new List<UnitPropertyData>();
        List<UnitPropertyData> g3 = new List<UnitPropertyData>();
        List<UnitPropertyData> g4 = new List<UnitPropertyData>();
     

        foreach(var pdata in UnitPropertyDataDic[0])
        { 
            int gold = pdata.Value.cost;
            switch (gold)
            {
                case 1:
                    g1.Add(pdata.Value);
                    break;
                case 2:
                    g2.Add(pdata.Value);
                    break;
                case 3:
                    g3.Add(pdata.Value);
                    break;
                case 4:
                    g4.Add(pdata.Value);
                    break;
            }
        }

        unitTypeListbyGold.Add(1, g1);
        unitTypeListbyGold.Add(2, g2);
        unitTypeListbyGold.Add(3, g3);
        unitTypeListbyGold.Add(4, g4);

        SetRandomPercentage();
    }

    /// <summary>
    /// NOTE : UNIT SHOP PERCENTAGE
    /// </summary>
    private void SetRandomPercentage()
    {
        int[] level1 = { 100, 0, 0, 0 };
        int[] level2 = { 70, 30, 0, 0 };
        int[] level3 = { 70, 30, 0, 0 };
        int[] level4 = { 50, 30, 20, 0 };
        int[] level5 = { 30, 35, 30, 5 };
        int[] level6 = { 20, 45, 35, 10 };
        int[] level7 = { 15, 35, 35, 25 };

        ShopUnitPerList.Add(level1);
        ShopUnitPerList.Add(level1);
        ShopUnitPerList.Add(level2);
        ShopUnitPerList.Add(level3);
        ShopUnitPerList.Add(level4);
        ShopUnitPerList.Add(level5);
        ShopUnitPerList.Add(level6);
        ShopUnitPerList.Add(level7);
    }
    
}

public class UnitPropertyData
{
    public string name;
    public Unit_Type unitType;
    public Sprite tribeSprite;
    public Sprite attributeSprite;
    public int cost;
    public int originalCost;

    public int ratingValue;
    public string ObId;
    //RenderTexture 카메라 포지션을 변경하여 보여주기 위함
    public Vector2 camPos;


    public UnitPropertyData(Unit_Type _type, int _ratingvalue)
    {
        unitType = _type;
        string[] namedata = _type.ToString().Split('_');

        var tribe = namedata[0];
        var attribute = namedata[1];
        ratingValue = _ratingvalue;
        //속성 타입만큼 현재 타입 숫자값 + 1 * rating 값 -> Unit_Type 이 1,2,3,4 1,2,3,4 순으로 나열되어있으므로
        cost = (((int)_type% System.Enum.GetValues(typeof(Attribute_Type)).Length)+1) * ratingValue;
        originalCost = (((int)_type % System.Enum.GetValues(typeof(Attribute_Type)).Length) + 1);
        cost = originalCost * ratingValue;
        //가로 5(종류단위), 세로 10(등급단위)
        float xpos = ((int)_type% System.Enum.GetValues(typeof(Unit_Type)).Length) * 5;
        camPos = new Vector3(xpos, (ratingValue-1) * 10, 0);

        tribeSprite = DataBaseManager.instance.TribeSpriteDic[tribe];
        attributeSprite = DataBaseManager.instance.AttributeSpriteDic[attribute];

        name = attribute + tribe;

        ObId = namedata[0] + "_" + namedata[1] +"_" + ratingValue;
    }
}

//public enum Unit_Type
//{
//    Cat_Fire_1_1, Cat_Fire_2_3, Cat_Fire_3_6,
//    Cat_Water_1_2, Cat_Water_2_4, Cat_Water_3_6,
//    Cat_Ground_1_3, Cat_Ground_2_6, Cat_Ground_3_9,
//    Cat_Wind_1_4, Cat_Wind_2_8, Cat_Wind_3_12,

//    Bear_Water_1_1, Bear_Water_2_3, Bear_Water_3_6,
//    Bear_Fire_1_2, Bear_Fire_2_4, Bear_Fire_3_6,
//    Bear_Wind_1_3, Bear_Wind_2_6, Bear_Wind_3_9,
//    Bear_Ground_1_4, Bear_Ground_2_8, Bear_Ground_3_12,

//    Rabbit_Ground_1_1, Rabbit_Ground_2_3, Rabbit_Ground_3_6,
//    Rabbit_Water_1_2, Rabbit_Water_2_4, Rabbit_Water_3_6,
//    Rabbit_Wind_1_3, Rabbit_Wind_2_6, Rabbit_Wind_3_9,
//    Rabbit_Fire_1_4, Rabbit_Fire_2_8, Rabbit_Fire_3_12,
//}

//public enum Unit_ShopData
//{
//    Cat_Fire, Cat_Water, Cat_Ground, Cat_Wind,
//    Bear_Water, Bear_Fire, Bear_Wind, Bear_Ground,
//    Rabbit_Ground, Rabbit_Water, Rabbit_Wind, Rabbit_Fire
//}
//string[,] test ={
//{ "Cat_Fire_1_1", "Cat_Water_1_2", "Cat_Ground_1_3", "Cat_Wind_1_4",
//"Bear_Water_1_1", "Bear_Fire_1_2", "Bear_Wind_1_3", "Bear_Ground_1_4",
//"Rabbit_Ground_1_1", "Rabbit_Water_1_2", "Rabbit_Wind_1_3", "Rabbit_Fire_1_4 "}
//    ,
//{ "Cat_Fire_2_3", "Cat_Water_2_4", "Cat_Ground_2_6", "Cat_Wind_2_8",
//"Bear_Water_2_3", "Bear_Fire_2_4", "Bear_Wind_2_6", "Bear_Ground_2_8",
//"Rabbit_Ground_2_3", "Rabbit_Water_2_4", "Rabbit_Wind_2_6", "Rabbit_Fire_2_8"}
//    ,
//{ "Cat_Fire_3_6", "Cat_Water_3_6", "Cat_Ground_3_9", "Cat_Wind_3_12",
//"Bear_Water_3_6", "Bear_Fire_3_6", "Bear_Wind_3_9", "Bear_Ground_3_12",
//"Rabbit_Ground_3_6", "Rabbit_Water_3_6", "Rabbit_Wind_3_9", "Rabbit_Fire_3_12" }};

//public enum Unit_Type
//{
//    Cat_Fire_1_1, Cat_Water_1_2, Cat_Ground_1_3, Cat_Wind_1_4,
//    Bear_Water_1_1, Bear_Fire_1_2, Bear_Wind_1_3, Bear_Ground_1_4,
//    Rabbit_Ground_1_1, Rabbit_Water_1_2, Rabbit_Wind_1_3, Rabbit_Fire_1_4,

//    Cat_Fire_2_3, Cat_Water_2_4, Cat_Ground_2_6, Cat_Wind_2_8,
//    Bear_Water_2_3, Bear_Fire_2_4, Bear_Wind_2_6, Bear_Ground_2_8,
//    Rabbit_Ground_2_3, Rabbit_Water_2_4, Rabbit_Wind_2_6, Rabbit_Fire_2_8,

//    Cat_Fire_3_6, Cat_Water_3_6, Cat_Ground_3_9, Cat_Wind_3_12,
//    Bear_Water_3_6, Bear_Fire_3_6, Bear_Wind_3_9, Bear_Ground_3_12,
//    Rabbit_Ground_3_6, Rabbit_Water_3_6, Rabbit_Wind_3_9, Rabbit_Fire_3_12
//}