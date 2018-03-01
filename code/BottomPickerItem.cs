using System;
using UnityEngine;

public class BottomPickerItem : MonoBehaviour
{
	public BottomPickerItem()
	{
	}

	public void SetupButton(string k, int i)
	{
		this.key = k;
		this.id = i;
	}

	public void Submit()
	{
		PlayerPrefs.SetInt(this.key, this.id);
	}

	private string key;

	private int id;
}
