﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Goodies : MonoBehaviour 
{
	private int Price = 0;
	private string Name = "";
	private string IconPath = "";

	// Use this for initialization
	void Start () 
	{
	} 	

	public void SetName(string name) 
	{
		Name = name;
		Text text = transform.FindChild ("Name").GetComponent<Text> ();
		text.text = name;
	}

	public string GetName()
	{
		return Name;
	}

	public void SetPrice(int price)
	{
		Price = price;
	}

	public int GetPrice()
	{
		return Price;
	}

	public void SetIconPath(string iconPath)
	{
		IconPath = iconPath;
		Image icon = transform.FindChild ("Icon").GetComponent<Image> ();
		icon.sprite = UGUIUtility.LoadSprite (iconPath);
	}
}