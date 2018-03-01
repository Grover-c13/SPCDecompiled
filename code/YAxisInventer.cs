using System;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class YAxisInventer : MonoBehaviour
{
	public YAxisInventer()
	{
	}

	private void Start()
	{
		this.toggle.isOn = (PlayerPrefs.GetInt("y_invert", 0) == 1);
		this.ChangeState(this.toggle.isOn);
	}

	public void ChangeState(bool b)
	{
		PlayerPrefs.SetInt("y_invert", (!b) ? 0 : 1);
		MouseLook.invert = b;
	}

	public Toggle toggle;
}
