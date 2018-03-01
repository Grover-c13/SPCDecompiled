using System;
using Kino;
using UnityEngine;

public class ExplosionCameraShake : MonoBehaviour
{
	public ExplosionCameraShake()
	{
	}

	private void Update()
	{
		this.glitch.enabled = (this.glitch.horizontalShake > 0f);
		this.force -= Time.deltaTime / this.deductSpeed;
		this.force = Mathf.Clamp01(this.force);
		this.glitch.scanLineJitter = this.force;
		this.glitch.horizontalShake = this.force;
		this.glitch.colorDrift = this.force;
	}

	private void Awake()
	{
		ExplosionCameraShake.singleton = this;
	}

	public void Shake(float explosionForce)
	{
		if (explosionForce > this.force)
		{
			this.force = explosionForce;
		}
	}

	public float force;

	public float deductSpeed;

	public AnalogGlitch glitch;

	public static ExplosionCameraShake singleton;
}
