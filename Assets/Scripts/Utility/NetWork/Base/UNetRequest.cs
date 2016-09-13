using UnityEngine;

using System;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

public abstract class UNetArg
{
    public abstract object GetArg(string key);
    public abstract void Add(string key, object value);
    public abstract void Remove(string key);
    public abstract void Clear();
    public override string ToString()
    {
        return base.ToString();
    }
    public abstract string GetMergedArgs();
}

public class UWebArg : UNetArg
{
    protected SortedDictionary<string, object> m_ArgDict = new SortedDictionary<string, object>();
    protected object m_LocalArg = null;

    public object localArg
    {
        get { return m_LocalArg; }
        set { m_LocalArg = value; }
    }

    public override object GetArg(string key)
    {
        object arg = null;
        m_ArgDict.TryGetValue(key, out arg);

        return arg;
    }

    public override void Add(string key, object value)
    {
        if (m_ArgDict.ContainsKey(key))
            return;

        m_ArgDict.Add(key, value.ToString());
    }

    public override void Remove(string key)
    {
        m_ArgDict.Remove(key);
    }

    public override void Clear()
    {
        m_ArgDict.Clear();
        m_LocalArg = null;
    }

    public void Merge(UWebArg arg)
    {
        if (arg == null)
            return;

        foreach(var pair in arg.m_ArgDict)
        {
            Add(pair.Key, pair.Value);
        }

        if (m_LocalArg != null && arg.localArg != null)
            m_LocalArg = arg.localArg;
    }

    public override string ToString()
    {
        System.Text.StringBuilder builder = null;

        foreach (var kv in m_ArgDict)
        {
            if (builder != null)
                builder = UUtil.BuildString(builder, "&");

            builder = UUtil.BuildString(builder, kv.Key, "=", WWW.EscapeURL(kv.Value.ToString()));
        }

        if (builder != null)
            return builder.ToString();
        else
            return string.Empty;
    }

    public override string GetMergedArgs()
    {
        System.Text.StringBuilder builder = null;

        foreach (var kv in m_ArgDict)
        {
            builder = UUtil.BuildString(builder, kv.Value.ToString());
        }

        if (builder != null)
            return builder.ToString();
        else
            return string.Empty;
    }

    public virtual WWWForm ToForm()
    {
        WWWForm form = new WWWForm();
        foreach (var keyVal in m_ArgDict)
        {
            form.AddField(keyVal.Key, keyVal.Value.ToString());
        }
        return form;
    }
}

#region Request
public abstract class UNetRequest
{
    protected uint      m_Id;

    #region Properties
    public uint id
    {
        get { return m_Id; }
        set { m_Id = value; }
    }
    #endregion
}

public abstract class UWebRequest : UNetRequest
{
    protected string    m_RemoteClass;
    protected string    m_RemoteMethod;

    protected string    m_RemoteName;
    protected string    m_RuntimeClass;
    protected bool      m_isCompressed = false;

    protected UWebArg m_Args = null;

    #region Properties
    public string runtimeClass
    {
        get { return m_RuntimeClass; }
        set { m_RuntimeClass = value; }
    }

    public string remoteClass
    {
        get { return m_RemoteClass; }
        set { m_RemoteClass = value; }
    }

    public string remoteMethod
    {
        get { return m_RemoteMethod; }
        set { m_RemoteMethod = value; }
    }

    public string remoteName
    {
        get { return m_RemoteName; }
        set { m_RemoteName = value; }
    }

    public bool isCompressed
    {
        get { return m_isCompressed; }
        set { m_isCompressed = value; }
    }

    public object localArg
    {
        get { return m_Args != null ? m_Args.localArg : null; }
    }

    public UWebArg args
    {
        get { return m_Args; }
    }

    protected System.Type runtimeClassType
    {
        get { return System.Type.GetType(m_RuntimeClass); }
    }
    #endregion

    public void AddArgument(string key, object value)
    {
        if (m_Args == null)
            m_Args = new UWebArg();

        m_Args.Add(key, value);
    }

    public string GetArgsAsString()
    {
        return m_Args.ToString();
    }

    public abstract object ToDataObject(string strData);
}

public class UWebJsonRequest : UWebRequest
{
    public override object ToDataObject(string strData)
    {
       // LitJsonUtil.JsonMapper.ClearErrorStack();

		object data = JsonConvert.DeserializeObject( strData, runtimeClassType);
		if (data == null)
        {
            Debug.LogWarning("ToDataObject error:" + System.Environment.NewLine +"json deserialize error");
        }

        return data;
    }
}

#endregion
