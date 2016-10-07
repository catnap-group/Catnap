using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Reflection;
using System;
using System.Text;
using System.Security.Permissions;
public enum UnitClassType
{
	SceneCat,
	ScenePet,//以上都是pet
	SceneCatLitter,//猫砂
	SceneObj,//以上都是场景物件
}
public class MapSceneManager : UnityAllSceneSingletonVisible<MapSceneManager>
{

	protected int _CurrentObjectID = 0;
	Dictionary<int, SceneUnit> _SceneUnitList = new Dictionary<int, SceneUnit>();
	int GenerateObjectID()
	{
		++_CurrentObjectID;
		while (_SceneUnitList.ContainsKey(_CurrentObjectID))
			++_CurrentObjectID;
		return _CurrentObjectID;
	}
	public List<SceneUnit> GetAllUnit()
	{
		return new List<SceneUnit>(_SceneUnitList.Values);
	}
	public SceneUnit AddUnitComponentByType(GameObject go, UnitClassType type)
	{
		SceneUnit unit = null;
		switch (type)
		{

		case UnitClassType.SceneCat:
			unit = go.AddComponent<SceneCat> ();
			break;
		case UnitClassType.SceneCatLitter:
			unit = go.AddComponent<SceneCatLittle> ();
			break;
		default:
			unit = go.AddComponent<ScenePet>();
			break;
		}

		unit.m_Type = type;
		return unit;
	}

	public SceneCat CreateSceneCat(int baseID, Vector3 position, Quaternion rotation)
		
	{
		ScenePet pet = CreateScenePet(baseID, position, rotation);

		return pet as SceneCat;
	}
	public SceneCatLittle CreateSceneCatLittle(int baseID, Vector3 position, Quaternion rotation, bool defaultLocal = true)
	{
		characterBase baseData = BaseDataManager.Instance.GetTableDataByID<characterBase>(baseID);
		if (baseData == null)
		{
			//int iiii = 0;
		}

		UnitClassType classType = BaseDataManager.Instance.GetTableDataByID<characterBase>(baseID).GetCreepClassType();

		GameObject go = new GameObject();
		go.name = "catlittle";
		SceneCatLittle catlittle = AddUnitComponentByType(go, classType) as SceneCatLittle;
		catlittle.id = GenerateObjectID();

		catlittle.Init(baseID);

		go.transform.position = position;

		go.transform.rotation = rotation;
		if (defaultLocal) {
			go.transform.position = new Vector3(baseData.pos[0],baseData.pos[1],baseData.pos[2]);

			go.transform.rotation = Quaternion.Euler(new Vector3(baseData.quaternion[0],baseData.quaternion[1],baseData.quaternion[2]));
		}		
		go.transform.parent = this.transform;

		//cat.OnCreated();

		_SceneUnitList.Add(catlittle.id, catlittle);

		return catlittle;
	}
	public ScenePet CreateScenePet(int baseID, Vector3 position, Quaternion rotation, bool defaultLocal = true/*从表格里读，如果是false则直接设置*/)
	{
		characterBase baseData = BaseDataManager.Instance.GetTableDataByID<characterBase>(baseID);
		if (baseData == null)
		{
			//int iiii = 0;
		}

		UnitClassType classType = baseData.GetCreepClassType();

		GameObject go = new GameObject();
		go.name = "pet";
		ScenePet pet = AddUnitComponentByType(go, classType) as ScenePet;
		pet.id = GenerateObjectID();

		pet.Init(baseID);
		go.transform.position = position;
	
		go.transform.rotation = rotation;

		if (defaultLocal) {
			go.transform.position = new Vector3(baseData.pos[0],baseData.pos[1],baseData.pos[2]);

			go.transform.rotation = Quaternion.Euler(new Vector3(baseData.quaternion[0],baseData.quaternion[1],baseData.quaternion[2]));
		}		
		go.transform.parent = this.transform;

		//cat.OnCreated();

		_SceneUnitList.Add(pet.id, pet);

		return pet;
	}
	public bool CheckPetExits()
	{
		int petCnt = 0;
		for(int  i = 0 ; i < _SceneUnitList.Count ; i++){
			if (_SceneUnitList [i].m_Type <= UnitClassType.ScenePet)
				petCnt++;
			}	
		return petCnt <= 1;
	}
	public void CleanAll()
	{
		List<int> ids =new List<int>( _SceneUnitList.Keys);
		for(int i = ids.Count -1; i>= 0; i--){
		//foreach (int id in _SceneUnitList.Keys) {
			SceneUnit unit = _SceneUnitList[ids[i]];

			unit.OnUnInit(false);

			_SceneUnitList.Remove(ids[i]);
		}
	}
	public void RemoveSceneUnit(SceneUnit unit, bool immediatly = true)
	{
		if (unit == null)
			return;

		if (unit.IsCat())
		{
			RemoveSceneCat(unit.id, immediatly);
			return;
		}
		if (!unit.IsCat())
		{
			RemoveSceneItem(unit.id, immediatly);
			return;
		}

	}
	public void RemoveSceneItem(int id, bool immediatly = true)
	{
		//
		if (!_SceneUnitList.ContainsKey(id))
			return;

		SceneUnit unit = _SceneUnitList[id] ;

		unit.OnUnInit(immediatly);

		_SceneUnitList.Remove(id);

	}
	public void RemoveSceneCat(int id, bool immediatly = true)
	{
		//
		if (!_SceneUnitList.ContainsKey(id))
			return;

		SceneUnit unit = _SceneUnitList[id] ;

		unit.OnUnInit(immediatly);

		_SceneUnitList.Remove(id);

	}
}

