using UnityEngine;
using hiscene;

public class TartgetDynamicBehaviour : TartgetDynamic
{
    public override void OnDynamicReco(RecoResult recoResult)
    {
        GameObject gameObject = null;
        gameObject = new GameObject();
        if (recoResult.keyType == KeyType.IMAGE)
        {
            gameObject.AddComponent<ImageTargetBehaviour>();
        }

        Target target = gameObject.GetComponent<Target>();
        target.PixelWidth = recoResult.Width * 0.01f;
        target.PixelHeight = recoResult.Height * 0.01f;
        gameObject.transform.parent = transform.parent;
        gameObject.SetActive(true);

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.parent = gameObject.transform;
        cube.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        bindingGameObject(gameObject, recoResult.KeyId);
    }

    GameObject CreateVideoObject(string videoFile, int imgWidth, int imgHeight)
    {
        GameObject videoObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        videoObject.name = "VideoPlayer";
        videoObject.transform.localScale = new Vector3(imgWidth * 0.001f, 0, imgHeight * 0.001f);
        videoObject.SetActive(false);
        videoObject.AddComponent<VideoPlayerBehaviour>();
        VideoPlayerBehaviour videoPlayer = videoObject.GetComponent<VideoPlayerBehaviour>();
        //videoPlayer.VideoUrl = videoFile;
        //videoPlayer.LocalPath = videoFile;
        videoPlayer.AbsolutePath = videoFile;
        videoPlayer.IsLocal = true;
        videoPlayer.IsTransparent = true;
        videoObject.SetActive(true);
        return videoObject;
    }

}
