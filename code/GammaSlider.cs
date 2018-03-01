using System;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.UI;

public class GammaSlider : MonoBehaviour
{
	public GammaSlider()
	{
	}

	private void Start()
	{
		if (this.slider != null)
		{
			this.slider.value = PlayerPrefs.GetFloat("gamma", 0f);
			this.SetValue(this.slider.value);
		}
	}

	public void SetValue(float f)
	{
		this.warningText.enabled = (f > 0.5f);
		PlayerPrefs.SetFloat("gamma", f);
		ColorGradingModel.Settings settings = default(ColorGradingModel.Settings);
		settings = this.profile.colorGrading.settings;
		settings.basic.postExposure = f;
		this.profile.colorGrading.settings = settings;
	}

	public PostProcessingProfile profile;

	public Slider slider;

	public Text warningText;
}
