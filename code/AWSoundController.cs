using System;
using UnityEngine;
using UnityEngine.Networking;

public class AWSoundController : MonoBehaviour
{
	public AWSoundController()
	{
	}

	private void Awake()
	{
		this.a_source = base.GetComponents<AudioSource>()[0];
		this.m_source = base.GetComponents<AudioSource>()[1];
		this.exp_src = base.GetComponents<AudioSource>()[2];
	}

	public void UpdateSound(float t, bool p)
	{
		if (t == 90f)
		{
			if (p && !this.exploded)
			{
				this.exploded = true;
				foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Player"))
				{
					if (gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
					{
						this.exp_src.PlayOneShot((gameObject.transform.position.y >= 900f) ? this.outside : this.inside);
						UnityEngine.Object.FindObjectOfType<ExplosionCameraShake>().Shake(2f);
					}
				}
			}
			if (this.a_source.isPlaying || this.m_source.isPlaying)
			{
				this.a_source.Stop();
				this.m_source.Stop();
			}
		}
		else
		{
			if (!this.a_source.isPlaying || !this.m_source.isPlaying)
			{
				this.a_source.Play();
				this.m_source.Play();
			}
			if (Mathf.Abs(this.a_source.time - t) > 0.5f)
			{
				this.a_source.time = t;
				this.m_source.time = t;
			}
		}
	}

	private AudioSource a_source;

	private AudioSource m_source;

	private AudioSource exp_src;

	public AudioClip inside;

	public AudioClip outside;

	private bool exploded;
}
