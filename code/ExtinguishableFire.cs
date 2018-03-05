using System;
using System.Collections;
using UnityEngine;

public class ExtinguishableFire : MonoBehaviour
{
	public ExtinguishableFire()
	{
	}

	private void Start()
	{
		this.m_isExtinguished = true;
		this.smokeParticleSystem.Stop();
		this.fireParticleSystem.Stop();
		base.StartCoroutine(this.StartingFire());
	}

	public void Extinguish()
	{
		if (this.m_isExtinguished)
		{
			return;
		}
		this.m_isExtinguished = true;
		base.StartCoroutine(this.Extinguishing());
	}

	private IEnumerator Extinguishing()
	{
		this.fireParticleSystem.Stop();
		this.smokeParticleSystem.time = 0f;
		this.smokeParticleSystem.Play();
		for (float elapsedTime = 0f; elapsedTime < 2f; elapsedTime += Time.deltaTime)
		{
			float ratio = Mathf.Max(0f, 1f - elapsedTime / 2f);
			this.fireParticleSystem.transform.localScale = Vector3.one * ratio;
			yield return null;
		}
		yield return new WaitForSeconds(2f);
		this.smokeParticleSystem.Stop();
		this.fireParticleSystem.transform.localScale = Vector3.one;
		yield return new WaitForSeconds(4f);
		base.StartCoroutine(this.StartingFire());
		yield break;
	}

	private IEnumerator StartingFire()
	{
		this.smokeParticleSystem.Stop();
		this.fireParticleSystem.time = 0f;
		this.fireParticleSystem.Play();
		for (float elapsedTime = 0f; elapsedTime < 2f; elapsedTime += Time.deltaTime)
		{
			float ratio = Mathf.Min(1f, elapsedTime / 2f);
			this.fireParticleSystem.transform.localScale = Vector3.one * ratio;
			yield return null;
		}
		this.fireParticleSystem.transform.localScale = Vector3.one;
		this.m_isExtinguished = false;
		yield break;
	}

	public ParticleSystem fireParticleSystem;

	public ParticleSystem smokeParticleSystem;

	protected bool m_isExtinguished;

	private const float m_FireStartingTime = 2f;
}
