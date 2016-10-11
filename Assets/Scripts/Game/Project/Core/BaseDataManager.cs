using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LumenWorks.Framework.IO.Csv;
using System.Reflection;
using System.Security;

public class ObjectBaseData
{
	public readonly int id;
	public int type;
	public readonly string name = "";
	public readonly string icon = "";
	public readonly string desp;
	public ObjectBaseData()
	{
	}
	public ObjectBaseData(int iId, int iType, string strName, string strIcon, string strDesp)
	{
		id = iId;
		type = iType;
		name = strName;
		icon = strIcon;
		desp = strDesp;
	}
}
public class soundBase
{
	public readonly int sound_id;
	public readonly string file_name;
	public bool is_loop;
	public bool is2d;
}
public class animationBase
{
	public readonly int id;
	public readonly int unit_id;
	public readonly string name;
	public readonly float speed_rate;
	public readonly float move_speed;
}
public class characterBase
{
	public readonly int id;

	public  int type;

	public readonly string name = "";

	public readonly string icon = "";

	public readonly string desp;

	public readonly string[] idle_list = null;

	public readonly int[] sound_list = null;

	public readonly string prefab;
	public readonly float[] pos=null;
	public readonly float[] quaternion = null;
	public readonly string[] effname = null;
	public characterBase()
	{
		
	}

	public characterBase(int iId, int iType, string strName, string strIcon, string strDesp, string strPrefab, string[] strIdle_list, int[] intSound_list, float[] vPos, float[] vQuaternion, string [] vEffname)
	{
		id      = iId;
		type    = iType;
		name    = strName;
		icon    = strIcon;
		desp    = strDesp;
		idle_list = strIdle_list;
		sound_list = intSound_list;
		prefab = strPrefab;
		pos = vPos;
		quaternion = vQuaternion;
		effname = vEffname;
	}
	public UnitClassType GetCreepClassType()
	{
		switch (type)
		{
		case 1:
			return UnitClassType.SceneCat;
		case 2:
			return UnitClassType.SceneCatLitter;
		
		default:
			return UnitClassType.ScenePet;
		}

	}
};
//

public class BaseDataManager : UnityAllSceneSingleton<BaseDataManager>
{
	private Dictionary<string , Dictionary<int ,object>> TableDataList = new Dictionary<string, Dictionary<int, object>>();
	private Dictionary<string , string>  				 noticeConfig  = new Dictionary<string, string>();
	public  Dictionary<string , byte[]>					 SceneObjectList = new Dictionary<string, byte[]>();

	//read table by ID
	public T GetTableDataByID<T>(int id) where T : class
	{
		System.Type tableType = typeof(T);
		if (TableDataList.ContainsKey (tableType.Name)) {
			Dictionary<int , object> tb = TableDataList [tableType.Name];
			if (tb.ContainsKey (id)) {
				T data = tb [id] as T;
				return data;
			}
		}
		return default(T);
	}
	private string empty2number(string val)
	{
		return val == "" ? "0" : val;
	}
	public Dictionary<int , object> GetTableDatas<T>() where T: class
	{
		System.Type tableType = typeof(T);
		if (TableDataList.ContainsKey (tableType.Name)) {
			return TableDataList [tableType.Name];
		}
		return null;
	}   
	private int GetFieldIndexByName(string fieldname, string[] list)
	{
		for (int i = 0; i < list.Length; ++i)
		{
			if (fieldname == list[i])
				return i;
		}
		return -1;
	}
	//TODO: 需要以二进制方式来写和读场景数据
	public void ReadBinaryDataContent(string filename, byte[] content)
	{
//		//map data
//		if (filename.IndexOf (ConstantTable.MapNameType) != -1) {
//			SceneObjectList.Add (filename, content);
//			return;
//		}
	}
//	public void ReadXmlContent(string filename, byte[] content, Encoding coding)
//	{
//		SecurityElement rootElement = MiniXmlParser.Instance.ParseXml(content, coding);
//		if (rootElement.Children == null || rootElement.Children.Count == 0)
//			return;
//	}
	public void ReadTxtContent(string  filename, string content)
	{
		CyrillicTextReader reader = new CyrillicTextReader (content);
		if (filename == "Notice.txt") {
			string line = reader.ReadLine ();
			while (line != null) {
				string[] str = line.Split (new char[] { '=' }, System.StringSplitOptions.RemoveEmptyEntries);
				noticeConfig [str [0]] = str [1];
				line = reader.ReadLine ();
			}
		}
		//language
	}
	public Dictionary<int , soundBase> GetSoundBase(int baseID)
	{
		System.Type tableType = typeof(soundBase);

		if (TableDataList.ContainsKey (tableType.Name)) {

			Dictionary<int, object> datas = TableDataList[tableType.Name];
			Dictionary<int, soundBase> filterData = new Dictionary<int, soundBase>();
			characterBase cb = GetTableDataByID<characterBase> (baseID);
			int[] soundIds = cb.sound_list;
			if (soundIds == null)
				return filterData;
			for (int i = 0; i < soundIds.Length; i++) {
				if (datas.ContainsKey (soundIds [i]))
					filterData.Add (soundIds [i], datas [soundIds [i]] as soundBase );
			}
			return filterData;

		}
		return null;
		
	}
	public Dictionary<string, animationBase> GetAnimationBase(int baseID)
	{
		System.Type tableType = typeof(animationBase);

		if (TableDataList.ContainsKey(tableType.Name))
		{
			Dictionary<int, object> datas = TableDataList[tableType.Name];

			Dictionary<string, animationBase> filterData = new Dictionary<string, animationBase>();

			foreach (KeyValuePair<int, object> pair in datas)
			{
				animationBase va = pair.Value as animationBase;
				if (va.unit_id == baseID && !filterData.ContainsKey(va.name))
				{
					filterData.Add(va.name, va);
				}
			}

			return filterData;

		}
		return null;
	}

	public void ReadTableContent(string tablename, byte[] content, Encoding coding, char splitType = ',')
	{
		if (TableDataList.ContainsKey (tablename))
			return;

		MemoryStream ms = new MemoryStream (content);
		if (ms == null) {
			Debug.LogWarning ("convert csv failed");
			return;
		}
		StreamReader sr = new StreamReader (ms, coding, true);
		TextReader tr = sr as TextReader;
		if (tr == null) {
			Debug.LogWarning ("text reader is null");
			return;
		}
		LumenWorks.Framework.IO.Csv.CsvReader cr = new CsvReader (tr, true, splitType);
		if (cr == null) {
			Debug.LogWarning ("csvreader is null");
			return;
		}
		int fieldCount = 0;

		fieldCount = cr.FieldCount;

		string[] headers = cr.GetFieldHeaders();
		tablename = tablename.Substring(0, tablename.IndexOf("."));
		System.Type tableType = System.Type.GetType(tablename);
		Dictionary<int, object> dataList = new Dictionary<int, object>();

		while (cr.ReadNextRecord())
		{
			if(cr.CurrentRecordIndex <= 1)            
			{
				continue;
			}

			//第一次字段作为索引
			int index = int.Parse(cr[0]);
	
			object obj = System.Activator.CreateInstance(tableType);
			FieldInfo[] fields = obj.GetType().GetFields();
			foreach (FieldInfo fi in fields)//针对每一个属性进行循环 
			{

				string typename = fi.FieldType.Name;

				int fieldIndex = GetFieldIndexByName(fi.Name, headers);
				if (fieldIndex == -1)
					continue;

				if (cr[fieldIndex] == null)
					continue;      


				if (typename == "Int32")
				{
					string intString = cr[fieldIndex];
					int a = 0;
					if (intString.Trim() != "")
					{
						a = int.Parse(intString);       
					}
					fi.SetValue(obj, a);

				}
				else if (typename == "Int32[]")
				{
					string txt = cr[fieldIndex];
					if (txt.Trim() != "")
					{
						string[] stringList = txt.Split(';');

						int[] arryList = new int[stringList.Length];
						for (int k = 0; k < arryList.Length; ++k)
						{
							arryList[k] = int.Parse(stringList[k]);
						}

						fi.SetValue(obj, arryList);
					}

				}
				else if (typename == "Boolean")
				{
					if (cr[fieldIndex].Trim() != "")
					{

						string a = cr[fieldIndex];
						if (a != null && a != "")
						{
							bool value = int.Parse(a) == 1 ? true : false;
							fi.SetValue(obj, value);
						}
					}
				}
				else if (typename == "Single")
				{
					if ( cr[fieldIndex].Trim() != "")
					{
						float value = float.Parse(empty2number(cr[fieldIndex]));
						fi.SetValue(obj, value);
					}


				}
				else if (typename == "Single[]")
				{
					if (cr[fieldIndex].Trim() != "")
					{
						string[] stringList = cr[fieldIndex].Split(';');

						float[] arryList = new float[stringList.Length];
						for (int k = 0; k < arryList.Length; ++k)
						{
							arryList[k] = float.Parse(empty2number(stringList[k]));
						}

						fi.SetValue(obj, arryList);
					}
				}
				else if (typename == "Char")
				{
					if (cr[fieldIndex].Trim() != "")
					{
						fi.SetValue(obj, char.Parse(empty2number(cr[fieldIndex])));
					}
				}
				else if (typename == "Char[]")
				{
					if (cr[fieldIndex].Trim() != "")
					{
						string[] stringList = cr[fieldIndex].Split(';');

						char[] arryList = new char[stringList.Length];
						for (int k = 0; k < arryList.Length; ++k)
						{
							arryList[k] = char.Parse(empty2number(stringList[k]));
						}

						fi.SetValue(obj, arryList);
					}

				}
				else if (typename == "String")
				{
					string a = cr[fieldIndex];
					fi.SetValue(obj, a);

				}
				else if (typename == "String[]")
				{
					if (cr[fieldIndex].Trim() != "")
					{
						string[] stringList = cr[fieldIndex].Split(';');

						fi.SetValue(obj, stringList);
					}
				}

				else if (typename == "Vector3")
				{
					if (cr[fieldIndex].Trim() != "")
					{

						string[] stringList = cr[fieldIndex].Split(';');
						Vector3 v = new Vector3();
						v.x = float.Parse(stringList[0]);
						v.y = float.Parse(stringList[1]);
						v.z = float.Parse(stringList[2]);
						fi.SetValue(obj, v);
					}


				}
				else if (typename == "Vector3[]")
				{
					if (cr[fieldIndex].Trim() != "")
					{
						string[] stringList = cr[fieldIndex].Split('|');

						Vector3[] arryList = new Vector3[stringList.Length];
						for (int k = 0; k < stringList.Length; ++k)
						{
							string[] str = stringList[k].Split(';');
							Vector3 v = new Vector3();
							v.x = float.Parse(str[0]);
							v.y = float.Parse(str[1]);
							v.z = float.Parse(str[2]);
							arryList[k] = v;

						}
						fi.SetValue(obj, arryList);
					}

				}
                else if (typename == "Vector2")
                {
                    if (cr[fieldIndex].Trim() != "")
                    {
                        string[] stringList = cr[fieldIndex].Split(';');
                        Vector2 v = new Vector2();
                        v.x = float.Parse(stringList[0]);
                        v.y = float.Parse(stringList[1]);
                        fi.SetValue(obj, v);
                    }
                }

			}

			//tableType.InvokeMember("Done", BindingFlags.Default | BindingFlags.InvokeMethod, null, obj, new object[]);
			if (!dataList.ContainsKey(index))
				dataList.Add(index, obj);
			else
			{
				Debug.Log("Data index " + index + " already exist");
				continue;
			}
		}

        if (!TableDataList.ContainsKey(tablename))
            TableDataList.Add(tablename, dataList);
	}

	public int generateNPCPointID()
	{
		
		int NPCPointID = PlayerPrefs.GetInt ("NPCPointID");
		int result = NPCPointID;
		NPCPointID++;
		PlayerPrefs.SetInt ("NPCPointID", NPCPointID);

		return result;
	}

	public void clearAllSystemTableData()
	{
		if (TableDataList != null) {
			TableDataList.Clear();
		}
	}

}

