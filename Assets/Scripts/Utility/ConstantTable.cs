using UnityEngine;
using System.Collections;

public partial class ConstantTable {

	public static readonly string serverIp = "192.168.1.70";//"192.168.119.178";// 
	public static readonly int port = 8088;
	public static readonly string FILEPATH = Application.persistentDataPath+"\\Catnap\\";
	public static readonly string DataAssetBoundleFile = "Data.assetbundle";
	public static readonly string ResourcesPath =  Application.dataPath+"/Resources/";
	public static readonly string UserDataPath = ResourcesManager.WriteablePath + "/playerInfo.txt";
	// Use this for initialization

}
public enum MsgType
{
	SceneBeginLoad,//场景准备加载
	SceneLoaded,//场景加载完毕
	SceneAssetsLoaded,//场景资源加载完毕
	SceneUnLoaded,//场景卸载完毕

	ApplicationPaused,//程序中断了
	ApplicationActived,//程序激活了

	SceneUnitDead,//场景内对象死亡
	SceneWillLoad,//场景将要加载
	CatSetReq,//选择猫回应了
	ReturnMainScene,//回到主场景

}