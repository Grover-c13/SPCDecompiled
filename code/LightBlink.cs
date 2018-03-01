using System;
using UnityEngine;

public class LightBlink : MonoBehaviour
{
	public LightBlink()
	{
	}

	private void Start()
	{
		if (QualitySettings.shadows != ShadowQuality.Disable)
		{
			this.noshadowIntensMultiplier = 1f;
		}
		this.startIntes = base.GetComponent<Light>().intensity * 1.2f;
		this.outerVaration = this.startIntes * this.noshadowIntensMultiplier / 10f;
		this.innerVariation = this.startIntes * this.noshadowIntensMultiplier * (this.innerVariationPercent / 100f);
		this.l = base.GetComponent<Light>();
		this.RandomOuter();
		if (this.innerVariationPercent < 100f)
		{
			base.InvokeRepeating("RefreshLight", 0f, 1f / this.FREQ);
		}
	}

	private void FixedUpdate()
	{
		if (!this.disabled && this.innerVariationPercent == 100f)
		{
			this.i++;
			if (this.i > 3)
			{
				this.i = 0;
				this.l.enabled = !this.l.enabled;
			}
		}
		else
		{
			this.l.enabled = true;
		}
	}

	private void RandomOuter()
	{
		this.curOut = UnityEngine.Random.Range(-this.outerVaration, this.outerVaration);
		base.Invoke("RandomOuter", (float)UnityEngine.Random.Range(1, 3));
	}

	private void RefreshLight()
	{
		if (!this.disabled)
		{
			this.curIn = UnityEngine.Random.Range(this.startIntes * this.noshadowIntensMultiplier + this.innerVariation, this.startIntes * this.noshadowIntensMultiplier - this.innerVariation);
			this.l.intensity = this.curIn + this.curOut;
		}
		else
		{
			this.l.enabled = true;
			this.l.intensity = this.startIntes;
		}
	}

	private float startIntes;

	public float noshadowIntensMultiplier = 1f;

	public float innerVariationPercent = 10f;

	private float outerVaration = 0.1f;

	private float curOut;

	private float curIn;

	private float innerVariation;

	public float FREQ = 12f;

	private Light l;

	public bool disabled;

	private int i;
}
