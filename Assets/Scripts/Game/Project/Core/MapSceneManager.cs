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
public class MapSceneManager : UnityAllSceneSingleton<MapSceneManager>
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
		SceneUnit unit = go.GetComponent<SceneUnit> ();
		unit.m_Type = type;
		switch (type)
		{

		case UnitClassType.SceneCat:
			return go.AddComponent<SceneCat> ();
		case UnitClassType.SceneCatLitter:
			return go.AddComponent<SceneCatLittle> ();
		default:
			return go.AddComponent<ScenePet>();
		}
	}

	public SceneCat CreateSceneCat(int baseID, Vector3 position, Quaternion rotation)
		
	{
		ScenePet pet = CreateScenePet(baseID, position, rotation);

		return pet as SceneCat;
	}
	public SceneCatLittle CreateSceneCatLittle(int baseID, Vector3 position, Quaternion rotation)
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
		go.transform.parent = this.transform;

		//cat.OnCreated();

		_SceneUnitList.Add(catlittle.id, catlittle);

		return catlittle;
	}
	public ScenePet CreateScenePet(int baseID, Vector3 position, Quaternion rotation)
	{




		characterBase baseData = BaseDataManager.Instance.GetTableDataByID<characterBase>(baseID);
		if (baseData == null)
		{
			//int iiii = 0;
		}

		UnitClassType classType = BaseDataManager.Instance.GetTableDataByID<characterBase>(baseID).GetCreepClassType();

		GameObject go = new GameObject();
		go.name = "pet";
		ScenePet pet = AddUnitComponentByType(go, classType) as ScenePet;
		pet.id = GenerateObjectID();

		pet.Init(baseID);

		go.transform.position = position;
	
		go.transform.rotation = rotation;
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
	public void RemoveSceneUnit(SceneUnit unit, bool immediatly = true)
	{
		if (unit == null)
			return;

		if (unit.IsCat())
		{
			RemoveSceneCat(unit.id, immediatly);
			return;
		}

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

