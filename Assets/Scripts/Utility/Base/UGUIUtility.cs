using UnityEngine;
using System.Collections;

public static class UGUIUtility 
{
	static public Sprite LoadSprite(string iconPath)
	{
		Texture2D texture = Resources.Load<Texture2D> (iconPath);
		Sprite sprite = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), new UnityEngine.Vector2 (0.5f, 0.5f));
		return sprite;
	}
}
