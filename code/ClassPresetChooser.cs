using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClassPresetChooser : MonoBehaviour
{
	public ClassPresetChooser()
	{
	}

	public void RefreshBottomItems(string key)
	{
		this.curKey = key;
		int num = 0;
		foreach (ClassPresetChooser.PickerPreset pickerPreset in this.presets)
		{
			if (pickerPreset.classID == key)
			{
				num++;
				this.curPresets.Add(pickerPreset);
			}
		}
		IEnumerator enumerator = this.bottomMenuHolder.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				Transform transform = (Transform)obj;
				UnityEngine.Object.Destroy(transform.gameObject);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		for (int j = 0; j < num; j++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.bottomMenuItem, this.bottomMenuHolder);
			gameObject.GetComponent<BottomPickerItem>().SetupButton(key, j);
			gameObject.GetComponentInChildren<Text>().text = "ABCDEFGHIJKL"[j].ToString();
		}
	}

	private void Update()
	{
		if (this.curPresets.Count > 0)
		{
			ClassPresetChooser.PickerPreset pickerPreset = this.curPresets[PlayerPrefs.GetInt(this.curKey, 0)];
			this.health.value = (float)pickerPreset.health;
			this.wSpeed.value = pickerPreset.wSpeed;
			this.rSpeed.value = pickerPreset.rSpeed;
			this.avatar.texture = pickerPreset.icon;
			for (int i = 0; i < this.startItems.Length; i++)
			{
				if (i >= pickerPreset.startingItems.Length)
				{
					this.startItems[i].color = Color.clear;
				}
				else
				{
					this.startItems[i].color = Color.white;
					this.startItems[i].texture = pickerPreset.startingItems[i];
				}
			}
		}
	}

	public GameObject bottomMenuItem;

	public Transform bottomMenuHolder;

	public ClassPresetChooser.PickerPreset[] presets;

	private string curKey;

	private List<ClassPresetChooser.PickerPreset> curPresets = new List<ClassPresetChooser.PickerPreset>();

	public Slider health;

	public Slider wSpeed;

	public Slider rSpeed;

	public RawImage[] startItems;

	public RawImage avatar;

	public TextMeshProUGUI addInfo;

	[Serializable]
	public class PickerPreset
	{
		public PickerPreset()
		{
		}

		public string classID;

		public Texture icon;

		public int health;

		public float wSpeed;

		public float rSpeed;

		public float stamina;

		public Texture[] startingItems;

		public string en_additionalInfo;

		public string pl_additionalInfo;
	}
}
