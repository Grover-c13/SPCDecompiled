using System;
using UnityEngine;

public class DetectorBlink : MonoBehaviour
{
	public DetectorBlink()
	{
	}

	private void Start()
	{
		this.Blink();
	}

	private void Blink()
	{
		this.state = !this.state;
		int num = (!this.state) ? 0 : 2;
		this.mat.SetColor("_EmissionColor", new Color((float)num, (float)num, (float)num));
		base.Invoke("Blink", (!this.state) ? 1.3f : 0.2f);
	}

	public Material mat;

	private bool state;
}
