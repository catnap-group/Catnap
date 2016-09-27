using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class UWebRequestRoutine : MonoBehaviour
{
    class UWebRequestProxy
    {
        public UWebRequest              m_Request;
        public WWWForm                  m_Form;
        public string                   m_UrlArgs;
        public string                   m_MergedArgs;
        public UWebMgr.OnWebResponse    m_OnResponse;
        public UWebMgr.OnWebError       m_OnError;
        public float                    m_TimeOutInterval;

        public object                   m_LocalArg;
    }
    string      m_SessionToken;
    string      m_UrlRoot;
    string      m_PostFailureMsg;
    bool        m_isUrlArgs = true;
    //bool        m_isUseSignature = false;
    //string      m_Salt = "";

    Queue<UWebRequestProxy> m_RequestQueue = new Queue<UWebRequestProxy>();
    UWebRequestProxy        m_ActiveRequestProxy;
    IEnumerator             m_PostCoroutine;

    UWebMgr.OnWebTextResponse   m_ResponseHook;
    UWebMgr.OnWebError          m_ErrorHook;

    Action                      m_OnPostBegin;
    Action<object>              m_OnPostEnd;
    Action<uint, object>        m_ErrMsgReporter;

    void Awake()
    {
        GameObject.DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (m_ActiveRequestProxy == null && m_RequestQueue.Count > 0)
        {
            //temp: optimize
            m_ActiveRequestProxy = m_RequestQueue.Dequeue();
            m_PostCoroutine = DoPost();
            StartCoroutine(m_PostCoroutine);
        }
    }

    void OnDestroy()
    {
        if (m_PostCoroutine != null)
            StopCoroutine(m_PostCoroutine);
    }

    public void Init(string urlRoot, string networkFailureMsg, bool isURLArgs, bool isUseSignature, string salt)
    {
        m_UrlRoot = urlRoot;
        m_PostFailureMsg = networkFailureMsg;
        m_isUrlArgs = isURLArgs;

        //m_isUseSignature = isUseSignature;
        //m_Salt = salt;
    }

	public void SetSessionToken(string token)
    {
		m_SessionToken = token;
    }

    public void SetHooks(UWebMgr.OnWebTextResponse responseHook, UWebMgr.OnWebError errorHook)
    {
        m_ResponseHook = responseHook;
        m_ErrorHook = errorHook;
    }

    public void SetErrorMessageReporter(Action<uint, object> reporter)
    {
        m_ErrMsgReporter = reporter;
    }

    public void SetPostCallbacks(Action onPostBegin, Action<object> onPostEnd)
    {
        m_OnPostBegin = onPostBegin;
        m_OnPostEnd = onPostEnd;
    }

    public void Post(UWebRequest request, UWebArg customArgs, UWebMgr.OnWebResponse onResponse, UWebMgr.OnWebError onError, float timeOutInterval)
    {
        UWebRequestProxy proxy = new UWebRequestProxy();
        proxy.m_Request = request;

        if (request.isCompressed)
        {
            customArgs.Add("compress", 1);
        }

        if (m_isUrlArgs)
        {
            proxy.m_UrlArgs = customArgs.ToString();
            proxy.m_MergedArgs = customArgs.GetMergedArgs();
        }
        else
        {
            proxy.m_Form = customArgs.ToForm();
        }

        proxy.m_OnResponse = onResponse;
        proxy.m_OnError = onError;
        proxy.m_TimeOutInterval = timeOutInterval;
        proxy.m_LocalArg = customArgs.localArg;

        m_RequestQueue.Enqueue(proxy);
    }

    IEnumerator DoPost()
    {
        //var headers = m_ActiveRequestProxy.m_Form.headers;
        var headers = new Dictionary<string, string>();
        headers["Accept-Encoding"] = "identity";

        //string url = m_UrlRoot + "?ctl=" + m_ActiveRequestProxy.m_Request.remoteClass + "&act=" + m_ActiveRequestProxy.m_Request.remoteMethod;
        string url = m_UrlRoot + m_ActiveRequestProxy.m_Request.remoteName +"/"
			+ m_ActiveRequestProxy.m_Request.remoteFolder + "/" + m_ActiveRequestProxy.m_Request.remoteMethod + "?";

        if (!string.IsNullOrEmpty(m_SessionToken))
            url += "token=" + m_SessionToken;

        if (!string.IsNullOrEmpty(m_ActiveRequestProxy.m_UrlArgs))
			url +="&" +  m_ActiveRequestProxy.m_UrlArgs;

        //if (m_isUseSignature)
        //{
        //    string merged = m_ActiveRequestProxy.m_Request.remoteClass + m_ActiveRequestProxy.m_Request.remoteMethod + m_SessionToken + m_ActiveRequestProxy.m_MergedArgs + m_Salt;
        //    string md5 = UUtil.Md5Sum(merged);
        //    url += "&" + "sign=" + md5;
        //}

        float timeOut = Time.realtimeSinceStartup + m_ActiveRequestProxy.m_TimeOutInterval;
        //using (WWW www = new WWW(url, m_ActiveRequestProxy.m_Form.data, headers))

        Debug.Log("Web Request:" + url);

        if (m_OnPostBegin != null)
            m_OnPostBegin();

        using (WWW www = new WWW(url, (m_ActiveRequestProxy.m_Form != null) ? m_ActiveRequestProxy.m_Form.data : null, headers))
        {
            while (Time.realtimeSinceStartup < timeOut)
            {
                if (!www.isDone)
                    yield return null;
                else
                    break;
            }

            object data = null;

            if (www.isDone)
            {
                string strData = string.Empty;

                //iOS decompress packets by default
#if !UNITY_IPHONE
                if (m_ActiveRequestProxy.m_Request.isCompressed)
                {
                    //Debuger.LogError("Bytes downloaded:" + www.bytesDownloaded + "," + www.bytes.Length);
                    //Debuger.LogError("Bytes:" + ByteArrayToString(www.bytes));
                    //Debuger.LogError("String:" + www.text);
                    strData = UZipUtil.UnzipMemoryData(www.bytes);
                }
                else
#endif
                {
                    strData = www.text;
                }

                Debug.Log("Web Response:" + strData);

                data = m_ActiveRequestProxy.m_Request.ToDataObject(strData);

                if (m_ActiveRequestProxy.m_OnResponse != null)
                {
                    m_ActiveRequestProxy.m_OnResponse(m_ActiveRequestProxy.m_Request.id, data, m_ActiveRequestProxy.m_LocalArg);
                }

                if (m_ErrMsgReporter != null)
                {
                    m_ErrMsgReporter(m_ActiveRequestProxy.m_Request.id, data);
                }

                //hook
                if (m_ResponseHook != null)
                    m_ResponseHook(m_ActiveRequestProxy.m_Request.id, strData, m_ActiveRequestProxy.m_LocalArg);
            }
            else
            {
                if (m_ActiveRequestProxy.m_OnError != null)
                    m_ActiveRequestProxy.m_OnError(m_ActiveRequestProxy.m_Request.id, m_PostFailureMsg);

                //hook
                if (m_ErrorHook != null)
                    m_ErrorHook(m_ActiveRequestProxy.m_Request.id, m_PostFailureMsg);
            }

            m_ActiveRequestProxy = null;

            if (m_OnPostEnd != null)
                m_OnPostEnd(data);
        }
    }

    public static string ByteArrayToString(byte[] ba)
    {
        System.Text.StringBuilder hex = new System.Text.StringBuilder(ba.Length * 2);
        foreach (byte b in ba)
            hex.AppendFormat("{0:x2}", b);
        return hex.ToString();
    }

    void ResendRequest()
    {

    }
}
