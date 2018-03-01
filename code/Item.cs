using System;
using UnityEngine;

[Serializable]
public class Item
{
	public Item(Item item)
	{
		this.label = item.label;
		this.icon = item.icon;
		this.prefab = item.prefab;
		this.pickingtime = item.pickingtime;
		this.permissions = item.permissions;
		this.firstpersonModel = item.firstpersonModel;
		this.durability = item.durability;
		this.id = item.id;
		this.crosshair = item.crosshair;
		this.crosshairColor = item.crosshairColor;
	}

	public string label;

	public Texture2D icon;

	public GameObject prefab;

	public float pickingtime = 1f;

	public string[] permissions;

	public GameObject firstpersonModel;

	public float durability;

	public bool noEquipable;

	public Texture crosshair;

	public Color crosshairColor;

	[HideInInspector]
	public int id;
}
