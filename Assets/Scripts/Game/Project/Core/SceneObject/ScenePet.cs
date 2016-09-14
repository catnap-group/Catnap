using UnityEngine;
using System.Collections;

public class ScenePet : SceneUnit
{

	public float baseSpeed = 3;

	public AIStateManager GetStateManager() { return _AIStateManager; }

	protected AIStateManager _AIStateManager = null;
}

