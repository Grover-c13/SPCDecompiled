using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MusicSlider : MonoBehaviour
{
	private void Start()
	{
		this.OnValueChanged((float)PlayerPrefs.GetInt(this.keyName, 0));
		this.slider.value = (float)PlayerPrefs.GetInt(this.keyName, 0);
		this.master.SetFloat(this.keyName, (float)PlayerPrefs.GetInt(this.keyName, 0));
		if (this.optionalValueText != null)
		{
			this.optionalValueText.text = PlayerPrefs.GetInt(this.keyName, 0).ToString() + " dB";
		}
	}

	public void OnValueChanged(float vol)
	{
		this.master.SetFloat(this.keyName, vol);
		PlayerPrefs.SetInt(this.keyName, (int)vol);
		if (this.optionalValueText != null)
		{
			this.optionalValueText.text = ((int)vol).ToString() + " dB";
		}
	}

	public AudioMixer master;

	public Slider slider;

	public Text optionalValueText;

	public string keyName = "Volume";
}
