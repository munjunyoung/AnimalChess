using System.Linq;
using System.Collections.Generic;
using UnityEngine;

//Map
public enum Ground_TYPE { DesertBlock1, DesertBlock2, WaitingBlock }
////Unit
public enum Tribe_Type { Cat, Bear, Rabbit }
//public enum Attribute_Type  { Fire, Water, Wind, Ground }
//종족, 속성, 가격
public enum Unit_Type
{
    Cat_Fire_1, Cat_Water_2, Cat_Ground_3, Cat_Wind_4,
    Bear_Water_1, Bear_Fire_2, Bear_Wind_3, Bear_Ground_4,
    Rabbit_Ground_1, Rabbit_Water_2, Rabbit_Wind_3, Rabbit_Fire_4
}

public class DataBaseManager : MonoBehaviour
{
    public static DataBaseManager instance = null;
    //싱글톤
    //public static LoadDataManager instance = null;

    private readonly string groundPrefabPath = "Map/IngameGround";
    private readonly string unitRTPath = "UI/UnitRendererTexture";
    private readonly string tribeSpritePath = "UI/Tribe";
    private readonly string attributeSpritePath = "UI/Attribute";
    private readonly string unitObPath = "Unit/";

    //Map
    public Dictionary<string, BlockOnBoard> groundDic = new Dictionary<string, BlockOnBoard>();

    //Shop UI
    public Dictionary<string, Texture> UnitRTDic = new Dictionary<string, Texture>();
    public Dictionary<string, Sprite> TribeSpriteDic = new Dictionary<string, Sprite>();
    public Dictionary<string, Sprite> AttributeSpriteDic = new Dictionary<string, Sprite>();

    //ShopUnit 
    public Dictionary<Unit_Type, UnitPropertyData> disPlayUnitDataDic = new Dictionary<Unit_Type, UnitPropertyData>();
    public Dictionary<int, List<Unit_Type>> unitTypeListbyGold = new Dictionary<int, List<Unit_Type>>();
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
        SetLoadDataOnDictionary(UnitRTDic, unitRTPath);
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
        int unitcount = System.Enum.GetValues(typeof(Unit_Type)).Length;


        for (int i = 0; i < unitcount; i++)
            disPlayUnitDataDic.Add((Unit_Type)i, new UnitPropertyData((Unit_Type)i));

        List<Unit_Type> g1 = new List<Unit_Type>();
        List<Unit_Type> g2 = new List<Unit_Type>();
        List<Unit_Type> g3 = new List<Unit_Type>();
        List<Unit_Type> g4 = new List<Unit_Type>();

        for (int i = 0; i < unitcount; i++)
        {
            var tmp = (Unit_Type)i;
            string unittext = tmp.ToString();
            string gold = unittext.Substring(unittext.Length - 1);
            switch (gold)
            {
                case "1":
                    g1.Add(tmp);
                    break;
                case "2":
                    g2.Add(tmp);
                    break;
                case "3":
                    g3.Add(tmp);
                    break;
                case "4":
                    g4.Add(tmp);
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
        int[] level7 = { 15, 35, 45, 15 };

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
    public Unit_Type unitType;
    public int cost;
    public Texture rTexture;
    public Sprite tribeSprite;
    public Sprite attributeSprite;

    public UnitPropertyData(Unit_Type _id)
    {
        unitType = _id;

        string[] namedata = _id.ToString().Split('_');

        var tribe = namedata[0];
        var attribute = namedata[1];
        cost = int.Parse(namedata[2]);
        rTexture = DataBaseManager.instance.UnitRTDic[tribe + "_" + attribute];
        tribeSprite = DataBaseManager.instance.TribeSpriteDic[tribe];
        attributeSprite = DataBaseManager.instance.AttributeSpriteDic[attribute];
    }
}
