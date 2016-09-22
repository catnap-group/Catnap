using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public enum LoadingState
{
	DownloadingConfig = 1,
	DownloadingFile,
	DownloadingVersionData,
	//CompareVersion,
	InitializingResource,
	NetworkConnected,
}

public struct LoadingProfiler
{
	public LoadingState state;

	public string filename;

	public int maxSize;

	public int currentSize;
}

public class ResourcesManager : UnityAllSceneSingleton<ResourcesManager>
{

	public static readonly string WriteablePath = Application.persistentDataPath + "/";
	//不同平台下的streamingasset路径
	public static readonly string BasePathURL =
	#if UNITY_EDITOR
		 "file:" + Application.dataPath + "/StreamingAssets";
	#elif UNITY_ANDROID
		BasePathURL = "jar:file://"+ Application.dataPath + "!/assets/";
	


#elif UNITY_IOS
		BasePathURL = "file:" + Application.dataPath + "/Raw";
	


#else
	//Desktop (Mac OS or Windows)
		BasePathURL = "file:"+ Application.dataPath + "/StreamingAssets";
	#endif
	
	

	public LoadingProfiler SystemLoadingState;

	bool _DataLoaded = false;

	// Use this for initialization
	public override void Start ()
	{
		Debug.Log ("WriteablePath/" + WriteablePath);
	}

	void LoadDataObjects (Object[] objects, string[]names = null)
	{
		for (int i = 0; i < objects.Length; ++i) {
			string tablename = objects [i].name;
			TextAsset textAsset = objects [i] as TextAsset;
			string textAssetname = textAsset.name;
			if (names != null) {
				tablename = names [i];
				Debug.Log (tablename);
				textAssetname = names [i].Substring (names [i].LastIndexOf ('/') + 1);
			}
			
			//load xml
			if (tablename.IndexOf (".csv") != -1) {
				string tt = textAsset.text.ToString ();
				char splitType = '\t';
				if (tt.IndexOf (",") != -1) {
					splitType = ',';		
				}
				BaseDataManager.Instance.ReadTableContent (textAssetname, textAsset.bytes, Encoding.UTF8, splitType);
//			} else if (tablename.IndexOf (".xml") != -1) {
//				BaseDataManager.Instance.ReadXmlContent (textAssetname, textAsset.bytes, Encoding.UTF8);
			} else if (tablename.IndexOf (".txt") != -1) {
				BaseDataManager.Instance.ReadTxtContent (textAssetname, textAsset.ToString ());
			} else if (tablename.IndexOf (".byte") != -1 || tablename.IndexOf (".data") != -1) {
				BaseDataManager.Instance.ReadBinaryDataContent (textAssetname, textAsset.bytes);
			} else {
				Debug.LogError ("un supported file format" + tablename);
			}
		}
	}

	string[] dataContentNames = {
		"Data/Config/Notice.txt",
		"Data/DataTable/soundBase.csv",
		"Data/DataTable/characterBase.csv",
		"Data/DataTable/animationBase.csv",

	};

	public IEnumerator LoadSystemBaseDataAysn ()
	{
		SystemLoadingState.state = LoadingState.InitializingResource;

		string path = WriteablePath + ConstantTable.DataAssetBoundleFile;

		if (!IsFileExist (path)) {
			List<Object> objects = new List<Object> ();
			List<string> names = new List<string> ();


			for (int i = 0; i < dataContentNames.Length; ++i) {
				string name = dataContentNames [i];
				name = name.Substring (0, name.IndexOf ('.'));
				Object obj = Resources.Load (name);
				objects.Add (obj);
                yield return null;
			}
			LoadDataObjects (objects.ToArray (), dataContentNames);
		} else {
			//TODO: 使用新的load方式
		}

		_DataLoaded = true;
	}

	public bool IsFileExist (string filename)
	{
		return File.Exists (filename);
	}
	//同步加载的方法
	public void LoadSystemBaseData ()
	{
		if (_DataLoaded)
			return;

		SystemLoadingState.state = LoadingState.InitializingResource;

		string path = WriteablePath + ConstantTable.DataAssetBoundleFile;

		if (!IsFileExist (path)) {
			List<Object> objects = new List<Object> ();
			List<string> names = new List<string> ();

			for (int i = 0; i < dataContentNames.Length; ++i) {
				string name = dataContentNames [i];
				name = name.Substring (0, name.LastIndexOf ('.'));
				Object obj = Resources.Load (name);

				objects.Add (obj);
			}

			LoadDataObjects (objects.ToArray (), dataContentNames);

			_DataLoaded = true;

			return;
		}

		byte[] tempall = BinaryStream.GetFileBinaryData (path);

		AssetBundle assetBundle = AssetBundle.LoadFromMemory (tempall);

		if (assetBundle != null) {
			Object[] objs = assetBundle.LoadAllAssets (typeof(Object));

			LoadDataObjects (objs);

			assetBundle.Unload (false);
		}

		_DataLoaded = true;
	}

	//同步刷新的方法
	public void RefreshSystemBaseData ()
	{
		BaseDataManager.Instance.clearAllSystemTableData ();
		_DataLoaded = false;
		LoadSystemBaseData ();
	}

	public AssetBundle GetAssetBoundle (string relativePath)
	{
		//AssetBundle ab;

		string path = WriteablePath + relativePath;

//		if (IsFileExist (path)) {
//			byte[] tempall = BinaryStream.GetFileBinaryData (path);
//
//			ab = AssetBundle.CreateFromMemoryImmediate (tempall);
//
//			return ab;
//
//		}


		return null;
	}

	public byte[] GetFileData (string path)
	{
		//先查看是否更新目录有文件
		string filePath = WriteablePath + path;
		if (IsFileExist (filePath)) {
			using (FileStream fs = new FileStream (filePath, FileMode.Open)) {


				if (fs == null)
					return null;
				BinaryReader br = new BinaryReader (fs);
				if (br == null)
					return null;
				return br.ReadBytes ((int)fs.Length);
			}
		}
		//看看数据是否assetbundle
		AssetBundle assetBundle = GetAssetBoundle (ConstantTable.DataAssetBoundleFile);
		if (assetBundle != null) {
			TextAsset textAsset = assetBundle.LoadAsset (path) as TextAsset;

			byte[] data = textAsset.bytes;

			assetBundle.Unload (false);
			
			return data;
		} else {
			TextAsset textAsset = Resources.Load<TextAsset> (path);
			if (textAsset == null)
				return null;
			byte[] data = textAsset.bytes;
			return data;
		}

	}
	
	public T LoadAsset<T>(string assetPath) where T:Object
	{
		AssetBundle assetBundle = GetAssetBoundle(assetPath + ".assetbundle");
		if (assetBundle != null) {
			T obj = assetBundle.mainAsset as T;
			assetBundle.Unload (false);
			return obj;
		} else {
			T ob = Resources.Load<T> (assetPath);
			return ob;
		}
	}

	public GameObject LoadGameObject(string objectPath)
	{
		GameObject objectResult;
		AssetBundle assetBundle = GetAssetBoundle (objectPath + ".assetbundle");
		if (assetBundle != null) {
			Object obj = assetBundle.mainAsset;
			objectResult = Instantiate (obj) as GameObject;
			assetBundle.Unload (false);
		} else {
			string path = objectPath;
			object ob = Resources.Load (path);
			if (ob != null) 
			objectResult = Instantiate(Resources.Load(path)) as GameObject;
				else
			objectResult = null;
		}

		if (objectResult != null && objectResult.name.IndexOf("(Clone)") != -1)
		{
			objectResult.name = objectResult.name.Substring(0, objectResult.name.Length - 7);
		}

		if (objectResult == null)
		{
			Debug.Log("GameObject can not find " + objectPath);
		}

		return objectResult;
	}
}