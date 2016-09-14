using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;


public class Pool<T>
{
    public List<T> activeList = new List<T>();
    public List<T> inactiveList = new List<T>();

    //
    public T New()
    {
        if (inactiveList.Count == 0)
        {
            T t = (T)CreateNew();
            activeList.Add(t);
            return t;
        }
        else
        {
            T data = inactiveList[inactiveList.Count - 1];
            inactiveList.RemoveAt(inactiveList.Count - 1);
            activeList.Add(data);
            return data;
        }
    }

    public void Delete(T obj)
    {
        for (int i = 0; i < activeList.Count; ++i)
        {
            if (Equal(activeList[i], obj))
            {
                activeList.RemoveAt(i);
            }
        }

        Zero(obj);

        inactiveList.Add(obj);
    }

    protected virtual bool Equal(object a, object b)
    {
        return false;
    }
    protected virtual void Zero(object a)
    {

    }

    protected virtual object CreateNew() { return null; }
}

public class Vector3Pool : Pool<Vector3>
{
    public static Vector3Pool Instance = new Vector3Pool();

    protected override object CreateNew()
    {
        return new Vector3();
    }

    protected override void Zero(object a)
    {
        Vector3 vec = (Vector3)a;
        vec.x = 0;
        vec.y = 0;
        vec.z = 0;
    }
    protected override bool Equal(object a, object b)
    {
        return ((Vector3)a) == ((Vector3)b);
    }
}

public class GameObjectPool
{
    public GameObjectPool(string prefabPath, Transform pa)
    {
        PrefabName = prefabPath;
        parent = pa;
    }
    public bool LoadObject()
    {
        PrefabGameObject = ResourcesManager.Instance.LoadGameObject(PrefabName);
        if (PrefabGameObject == null)
        {
            Debug.LogError("No prefab found at path:" + PrefabName);

            return false;
        }
        else
        {
            PrefabGameObject.SetActive(false);
            PrefabGameObject.transform.parent = parent;

            return true;
        }
    }
    private Transform parent;
    public string PrefabName = "";
    public GameObject PrefabGameObject = null;
    public List<GameObject> activeList = new List<GameObject>();
    public List<GameObject> inactiveList = new List<GameObject>();

    public void Clear()
    {

    }
}

public class MemoryDataManager : UnityAllSceneSingletonVisible<MemoryDataManager>
{
    public Dictionary<string, GameObjectPool> gameObjectPoolList = new Dictionary<string, GameObjectPool>();

    public void ClearAllPoolData()
    {
        foreach (KeyValuePair<string, GameObjectPool> pool in gameObjectPoolList)
        {
            pool.Value.Clear();
        }

    }

    public GameObject LoadGameObject(string prefabPath, Vector3 position = new Vector3(), Quaternion rotation = new Quaternion(),bool isAutoEffect = false,int preFabsNum = 2)
    {
        if (prefabPath == null || prefabPath.Trim() == "")
        {
            return null;
        }

        GameObjectPool pool = null;
        if (!gameObjectPoolList.ContainsKey(prefabPath))
        {
            pool = new GameObjectPool(prefabPath, this.transform);
            gameObjectPoolList.Add(prefabPath, pool);

        }
        else
        {
            pool = gameObjectPoolList[prefabPath];
        }

        GameObject go = null;
        if (pool.inactiveList.Count == 0)
        {
            if (pool.PrefabGameObject == null)
            {
                if (!pool.LoadObject())
                    return null;
            }
			if(!isAutoEffect)
            {
                //预留几个
				for (int i = 0; i < preFabsNum; ++i)
                {
                    GameObject gameObj = MonoBehaviour.Instantiate(pool.PrefabGameObject) as GameObject;
                    gameObj.SetActive(false);
                    
                    if (gameObj.name.IndexOf("(Clone)") != -1)
                    {
                        gameObj.name = gameObj.name.Substring(0, gameObj.name.Length - 7);
                    }
                    gameObj.transform.parent = this.transform;
                    pool.inactiveList.Add(gameObj);
                }
            }
        }

        int fetchIndex = pool.inactiveList.Count - 1;
        go = pool.inactiveList[fetchIndex];
        go.transform.parent = null;
        go.transform.position = position;
        go.transform.rotation = rotation;
        pool.inactiveList.RemoveAt(fetchIndex);
        go.SetActive(true);
        pool.activeList.Add(go);
        return go;
    }

    public GameObject LoadGameObject(GameObject prefabPath, Vector3 position = new Vector3(), Quaternion rotation = new Quaternion(), bool isAutoEffect = false, int preFabsNum = 2)
    {
        if (prefabPath == null || prefabPath.name.Trim() == "")
        {
            return null;
        }

        GameObjectPool pool = null;
        if (!gameObjectPoolList.ContainsKey(prefabPath.name))
        {
            pool = new GameObjectPool(prefabPath.name, this.transform);
            gameObjectPoolList.Add(prefabPath.name, pool);

        }
        else
        {
            pool = gameObjectPoolList[prefabPath.name];
        }

        GameObject go = null;
		if (pool.inactiveList.Count == 0 || pool.inactiveList.Count == 1)
        {   
            //if (pool.PrefabGameObject == null)
            //{
            //    if (!pool.LoadObject())
            //        return null;
            //}
            pool.PrefabGameObject = prefabPath;
            if (!isAutoEffect)
            {
                //预留几个
                for (int i = 0; i < preFabsNum; ++i)
                {
                    GameObject gameObj = MonoBehaviour.Instantiate(pool.PrefabGameObject) as GameObject;
                    gameObj.SetActive(false);

                    if (gameObj.name.IndexOf("(Clone)") != -1)
                    {
                        gameObj.name = gameObj.name.Substring(0, gameObj.name.Length - 7);
                    }
                    gameObj.transform.parent = this.transform;
                    pool.inactiveList.Add(gameObj);
                }
            }
        }

        int fetchIndex = pool.inactiveList.Count - 1;
        go = pool.inactiveList[fetchIndex];//尾拿法
        go.transform.parent = null;
        go.transform.position = position;
        go.transform.rotation = rotation;
        pool.inactiveList.RemoveAt(fetchIndex);
        go.SetActive(true);
        pool.activeList.Add(go);
        return go;
    }

    public void UnloadGameObject(GameObject go)
    {
        if (go == null)
            return;

        foreach (KeyValuePair<string, GameObjectPool> pool in gameObjectPoolList)
        {
            if (pool.Value.activeList.Contains(go))
            {
                go.SetActive(false);
                go.transform.parent = this.transform;
                pool.Value.activeList.Remove(go);
				pool.Value.inactiveList.Insert (0, go);// 头插法
                break;
            }
        }
    }

    public void PreloadGameObjects(List<string> gameObjects)
    {
        foreach (string gameObjectPath in gameObjects)
        {
            if (!gameObjectPoolList.ContainsKey(gameObjectPath))
            {
                GameObjectPool pool = new GameObjectPool(gameObjectPath, this.transform);
                gameObjectPoolList[gameObjectPath] = pool;
            }
        }

    }

    public static void SetActiveRecursively(GameObject target, bool bActive)
    {
#if (!UNITY_3_5)
        for (int n = target.transform.childCount - 1; 0 <= n; n--)
            if (n < target.transform.childCount)
                SetActiveRecursively(target.transform.GetChild(n).gameObject, bActive);
        target.SetActive(bActive);
#else
		target.SetActiveRecursively(bActive);
#endif
        // 		SetActiveRecursivelyEffect(target, bActive);
    }
};
