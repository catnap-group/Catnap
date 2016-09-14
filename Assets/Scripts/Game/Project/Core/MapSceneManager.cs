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
public enum PetClassType
{
	ScenePet =0 ,
	SceneCat,
}
public class MapSceneManager : UnityAllSceneSingleton<MapSceneManager>
{

	protected int _CurrentObjectID = 0;
	Dictionary<int, ScenePet> _ScenePetList = new Dictionary<int, ScenePet>();
	int GenerateObjectID()
	{
		++_CurrentObjectID;
		while (_ScenePetList.ContainsKey(_CurrentObjectID))
			++_CurrentObjectID;
		return _CurrentObjectID;
	}
	public ScenePet AddPetComponentByType(GameObject go, PetClassType type)
	{
		switch (type)
		{

		case PetClassType.SceneCat:
			return go.AddComponent<SceneCat> ();

		default:
			return go.AddComponent<ScenePet>();
		}
	}

	public SceneCat CreateSceneCat(int baseID, Vector3 position, Quaternion rotation)
		
	{
		ScenePet pet = CreateScenePet(baseID, position, rotation);

		return pet as SceneCat;
	}
	public ScenePet CreateScenePet(int baseID, Vector3 position, Quaternion rotation)
	{




		characterBase baseData = BaseDataManager.Instance.GetTableDataByID<characterBase>(baseID);
		if (baseData == null)
		{
			//int iiii = 0;
		}

		PetClassType classType = BaseDataManager.Instance.GetTableDataByID<characterBase>(baseID).GetCreepClassType();

		GameObject go = new GameObject();
		go.name = "pet";
		ScenePet pet = AddPetComponentByType(go, classType);
		pet.id = GenerateObjectID();

		pet.Init(baseID);

		go.transform.position = position;
	
		go.transform.rotation = rotation;
		go.transform.parent = this.transform;

		//cat.OnCreated();

		_ScenePetList.Add(pet.id, pet);

		return pet;
	}
	public bool CheckCreepsExits()
	{
		return _ScenePetList.Count <= 1;
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
		if (!_ScenePetList.ContainsKey(id))
			return;

		ScenePet unit = _ScenePetList[id];

		unit.OnUnInit(immediatly);

		_ScenePetList.Remove(id);

	}
}

