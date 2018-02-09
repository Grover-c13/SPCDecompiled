using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ProjectorInitializer : MonoBehaviour
{
	private IEnumerator StartProjector()
	{
		this.src.Stop();
		this.src.PlayOneShot(this.c_st);
		base.Invoke("InitLoop", 4f);
		yield return new WaitForSeconds(1f);
		this.dir = true;
		yield break;
	}

	private IEnumerator StopProjector()
	{
		this.src.Stop();
		this.src.PlayOneShot(this.c_sp);
		yield return new WaitForSeconds(1f);
		this.dir = false;
		yield break;
	}

	private void InitLoop()
	{
		this.src.Stop();
		this.src.PlayOneShot(this.c_lp);
	}

	private void Update()
	{
		if (this.started != this.prevStarted)
		{
			if (this.started)
			{
				base.StartCoroutine(this.StartProjector());
				this.prevStarted = true;
			}
			else
			{
				base.StartCoroutine(this.StopProjector());
				this.prevStarted = false;
			}
		}
		this.time += Time.deltaTime * (float)((!this.dir) ? -2 : 2);
		this.time = Mathf.Clamp01(this.time / 4f) * 4f;
		foreach (Transform transform in this.spools)
		{
			transform.Rotate(Vector3.up * this.time / 4f);
		}
		foreach (ProjectorInitializer.LightStruct lightStruct in this.lights)
		{
			lightStruct.SetLight(this.time);
		}
	}

	public ProjectorInitializer.LightStruct[] lights;

	public TextMeshProUGUI projector_label;

	public AudioSource src;

	public AudioClip c_st;

	public AudioClip c_lp;

	public AudioClip c_sp;

	public Transform[] spools;

	private float time;

	public bool started;

	private bool prevStarted;

	private bool dir;

	[Serializable]
	public class LightStruct
	{
		public void SetLight(float time)
		{
			this.targetLight.color = Color.Lerp(Color.black, this.normalColor, this.curve.Evaluate(time));
		}

		public string label;

		public Color normalColor;

		public Light targetLight;

		public AnimationCurve curve;
	}
}
