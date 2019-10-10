using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public enum Ground_TYPE { DesertBlock1, DesertBlock2, WaitingBlock };

public class LoadDataManager : MonoBehaviour
{
    //싱글톤
    public static LoadDataManager instance = null;

    private readonly string groundPath = "Map/IngameGround";

    public Dictionary<string, BlockOnBoard> groundDic = new Dictionary<string, BlockOnBoard>();


    private void Awake()
    {
        if(instance==null)
            instance = this;
        SetLoadData(groundDic, groundPath);
    }

    /// <summary>
    /// NOTE : 해당 폴더와 타입에 따른 리소스 로드 및 초기화
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_dic"></param>
    /// <param name="_path"></param>
    private void SetLoadData<T>(Dictionary<string, T> _dic, string _path)
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
}
