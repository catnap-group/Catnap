using System;
using hiscene;
using UnityEngine;

//[RequireComponent(typeof(HiARBaseObjectMovement))]
public class ImageTargetBehaviour : ImageTarget, ITrackableEventHandler, ILoadBundleEventHandler, IRelativeMoveTarget
{

    private bool isRelativeMove = false;

    private HiARBaseObjectMovement _movement;

    private HiARBaseObjectMovement Movement
    {
        get
        {
            return _movement ?? (_movement = gameObject.GetComponent<HiARBaseObjectMovement>() ?? gameObject.AddComponent<HiARBaseObjectMovement>());
        }
    }

    private void Start()
    {
        if (Application.isPlaying)
        {
            for (var i = 0; i < transform.childCount; i++) transform.GetChild(i).gameObject.SetActive(false);
        }
        else
        {
            for (var i = 0; i < transform.childCount; i++) transform.GetChild(i).gameObject.SetActive(true);
        }
        RegisterTrackableEventHandler(this);
        RegisterILoadBundleEventHandler(this);
    }


    #region called by global setting

    public void enableRelativeMove(bool isCenter = true)
    {
        //        this.disableTargetMovement();
        //        var movement = GetComponent<HiARBaseObjectMovement>()
        //            ?? gameObject.AddComponent<HiARBaseObjectMovement>();
        //        isRelativeMove = true;
        //        Movement.enabled = true;
        //        if (isCenter) Movement.isPovObject = true;
    }

    public void disableRelativeMove()
    {
        //        this.enableTargetMovement();
        //        isRelativeMove = false;
        //        Movement.enabled = false;
        //        DestroyImmediate(gameObject.GetComponent<HiARBaseObjectMovement>());
    }

    #endregion

    public void OnLoadBundleStart(string url)
    {
        Debug.Log("load bundle start: " + url);
    }

    public void OnLoadBundleProgress(float progress)
    {
        Debug.Log("load bundle progress: " + progress);
    }

    public void OnLoadBundleComplete() { }

    public virtual void OnTargetFound(RecoResult recoResult)
    {
        if (recoResult.IsCloudReco)
        {
            downloadBundleFromHiAR(recoResult);
        }
        for (var i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public virtual void OnTargetTracked(RecoResult recoResult, Matrix4x4 pose)
    {
        if (isRelativeMove)
        {
            Movement.setTransform(pose, 1.0f / unityPixelRatio);
            Movement.applyTransform();
        }
    }

    public virtual void OnTargetLost(RecoResult recoResult)
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void OnLoadBundleError(Exception error)
    {
        Debug.Log("load bundle error: " + error.ToString());
    }

    public override void ConfigCloudObject(IHsGameObject target)
    {
        try
        {//兼容老版本识别包
            HiARObjectMonoBehaviour oldScript = target.HsGameObjectInstance.GetComponent<HiARObjectMonoBehaviour>();
            if (oldScript != null && target.HsGameObjectInstance.transform.childCount > 0)
            {
                GameObject child = target.HsGameObjectInstance.transform.GetChild(0).gameObject;
                VideoPlayerMonoBehaviour oldVideo = child.GetComponent<VideoPlayerMonoBehaviour>();
                if (oldVideo != null)
                {
                    child.AddComponent<VideoPlayerBehaviour>();
                    VideoPlayerBehaviour player = child.GetComponent<VideoPlayerBehaviour>();
                    player.m_isLocal = false;
                    player.m_webUrl = oldVideo.m_webUrl;
                    if (string.IsNullOrEmpty(player.m_webUrl))
                    {
                        player.m_isLocal = true;
                        player.m_localPath = oldVideo.m_localPath;
                    }
                }
                target.HsGameObjectInstance = child;
            }
        }
        catch (Exception e)
        {
            LogUtil.Log(e.ToString());
        }

        VideoPlayerBehaviour playerSrc = target.HsGameObjectInstance.GetComponent<VideoPlayerBehaviour>();
        if (playerSrc != null)
        {
            target.HsGameObjectInstance.name = "VideoPlayer";
        }

        Transform trans = target.HsGameObjectInstance.transform;
        Vector3 scale = trans.localScale;
        Vector3 position = trans.position;
        Quaternion rotation = trans.rotation;

        trans.position = transform.position;
        trans.rotation = transform.rotation;

        trans.SetParent(transform);

        trans.localPosition = position;
        trans.localRotation = rotation;

        trans.gameObject.SetActive(true);
    }
}
