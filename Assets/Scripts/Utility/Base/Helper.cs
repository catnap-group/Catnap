using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Reflection;



public class Helper
{
    public static void SetMemberValue(object obj, string fieldname, object value)
    {
       
        FieldInfo[] fields = obj.GetType().GetFields();
        foreach (FieldInfo fi in fields)
        {
            string typename = fi.FieldType.Name;
            //Debug.Log("Read field "+fi.Name);
            if (fi.Name != fieldname )
            {
                continue;
            }
            fi.SetValue(obj, value);
            break;
        }
    }

    public static void SetMemberZero(object obj)
    {
        FieldInfo[] fields      = obj.GetType().GetFields();
        string         strType  = string.Empty;
        foreach (FieldInfo fi in fields)
        {
            strType = fi.FieldType.Name;

            if (strType == "Int32")
                fi.SetValue(obj, 0);
            else if (strType == "Int32[]" ||
                        strType == "Single[]" ||
                        strType == "Char[]"   ||
                        strType == "String[]" ||
                        strType == "Vector3[]")
                fi.SetValue(obj, null);
            else if (strType == "Boolean")
                fi.SetValue(obj, false);
            else if (strType == "Single")
                fi.SetValue(obj, 0.0f);
            else if (strType == "Char")
                fi.SetValue(obj, '\0');
            else if (strType == "String")
                fi.SetValue(obj, string.Empty);
            else if (strType == "Vector3")
                fi.SetValue(obj, new Vector3(0.0f, 0.0f, 0.0f));
            else
            {
                Debug.LogError("FieldType Not Found Type:" + fi.FieldType);
            }
        }
    }

    public static Vector3 GetWorldScale(Transform transform)
    {
        Vector3 worldScale = transform.localScale;
        Transform parent = transform.parent;

        while (parent != null)
        {
            worldScale = Vector3.Scale(worldScale, parent.localScale);
            parent = parent.parent;
        }

        return worldScale;
    }




    public static void DestroyColliderRecursively(Transform root)
    {
        foreach (Transform child in root)
        {
            if (child.GetComponent<Collider>() != null)
            {
                GameObject.Destroy(child.GetComponent<Collider>());
            }
            DestroyColliderRecursively(child);
        }
    }

    //
    public static List<string> GetAllChildrenOfComponent<T>(GameObject obj) where T : Component
    {
        List<string> list = new List<string>();
        //
        T[] data = obj.GetComponentsInChildren<T>();

        for (int i = 0; i < data.Length; ++i)
        {
            string relativeName = GetRelativeName(data[i].transform, obj.transform);

            list.Add(relativeName);
        }

        return list;
    }

    public static string GetRelativeName(Transform children, Transform root)
    {
        if (children == root)
            return children.gameObject.name;

        string name = "";
        Transform current = children;
        while( true )
        {
            name = current.gameObject.name + "/" + name;
            current = current.parent;
            if (current == root)
            {
                break;
            }
        }

        name = name.Substring(0, name.Length - 1);

        return name;
    }

   public static Transform FindChild(string name, Transform objTrans)
    {
        if (objTrans == null)
            return null;

        if (objTrans.name == name)
        {
            return objTrans;
        }

        Transform[] transforms = objTrans.GetComponentsInChildren<Transform>();
        foreach (Transform trans in transforms)
        {
            if (trans == objTrans)
                continue;

            Transform childRet = FindChild(name, trans);
            if (childRet)
            {
                return childRet;
            }

        }

        return null;
    }

    public static GameObject FindGameObjectByPath(string path, GameObject root = null)
    {
        GameObject ret = null;
        string[] goNames = path.Split(new char[] { '/' }, System.StringSplitOptions.RemoveEmptyEntries);
        if (goNames.Length < 1)
        {
            return null;
        }

        if (root)
        {
            ret = root;
            for (int i = 0; i < goNames.Length; i++)
            {
                string goName = goNames[i];
                ret = FindChildGameObjectByName(goName, ret);
                if (ret == null)
                    break;
            }
        }
        else
        {
            ret = GameObject.Find(goNames[0]);
            for (int i = 1; i < goNames.Length; i++)
            {
                string goName = goNames[i];
                ret = FindChildGameObjectByName(goName, ret);
                if (ret == null)
                    break;
            }
        }

        return ret;
    }

    public static GameObject FindChildGameObjectByName(string name, GameObject parent)
    {
        Transform trans = parent.transform.Find(name);
        if (trans)
            return trans.gameObject;
        else
            return null;

        //GameObject ret = null;

        //foreach (Transform child in parent.transform)
        //{
        //    //Debug.Log("childName:" + child.name);
        //    if (child.name.Equals(name))
        //    {
        //        ret = child.gameObject;
        //        break;
        //    }
        //}
        //return ret;
    }

}
