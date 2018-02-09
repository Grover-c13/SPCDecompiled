using System;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class SensitivitySlider : MonoBehaviour
{
	private void Start()
	{
		this.OnValueChanged(PlayerPrefs.GetFloat("Sens", 1f));
		this.slider.value = PlayerPrefs.GetFloat("Sens", 1f);
	}

	public void OnValueChanged(float vol)
	{
		PlayerPrefs.SetFloat("Sens", vol);
		Sensitivity.sens = this.slider.value;
	}

	public Slider slider;
}
