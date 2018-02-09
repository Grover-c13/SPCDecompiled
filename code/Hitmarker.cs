using System;
using UnityEngine;

public class Hitmarker : MonoBehaviour
{
	private void Awake()
	{
		Hitmarker.singleton = this;
	}

	public static void Hit(float size = 1f)
	{
		Hitmarker.singleton.Trigger(size);
	}

	private void Trigger(float size = 1f)
	{
		this.t = 0f;
		this.multiplier = size;
	}

	private void Update()
	{
		if (this.t < 10f)
		{
			this.t += Time.deltaTime;
			this.targetImage.SetAlpha(this.opacity.Evaluate(this.t));
			this.targetImage.transform.localScale = Vector3.one * this.size.Evaluate(this.t) * this.multiplier;
		}
	}

	public static Hitmarker singleton;

	public AnimationCurve size;

	public AnimationCurve opacity;

	private float t = 10f;

	public CanvasRenderer targetImage;

	private float multiplier;
}
