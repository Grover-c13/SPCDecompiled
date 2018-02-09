using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SensSlider : MonoBehaviour
{
	private void Start()
	{
		this.OnValueChanged((float)PlayerPrefs.GetInt("Volume", 0));
		this.slider.value = (float)PlayerPrefs.GetInt("Volume", 0);
		this.master.SetFloat("volume", (float)PlayerPrefs.GetInt("Volume", 0));
		this.optionalValueText.text = PlayerPrefs.GetInt("Volume", 0).ToString() + " dB";
	}

	public void OnValueChanged(float vol)
	{
		this.master.SetFloat("volume", vol);
		PlayerPrefs.SetInt("Volume", (int)vol);
		if (this.optionalValueText != null)
		{
			this.optionalValueText.text = ((int)vol).ToString() + " dB";
		}
	}

	public AudioMixer master;

	public Slider slider;

	public Text optionalValueText;
}
