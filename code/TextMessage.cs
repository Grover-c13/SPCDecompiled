using System;
using UnityEngine;

public class TextMessage : MonoBehaviour
{
	private Vector3 GetPosition()
	{
		return new Vector3(this.xOffset, this.spacing * this.position, 0f);
	}

	private void Start()
	{
		this.r = base.GetComponent<CanvasRenderer>();
		base.transform.localPosition = this.GetPosition() + Vector3.down * this.spacing;
	}

	private void Update()
	{
		this.remainingLife -= Time.deltaTime;
		this.r.SetAlpha(Mathf.Clamp01(this.remainingLife * 2f));
		base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, this.GetPosition(), Time.deltaTime * this.lerpSpeed);
	}

	public float spacing = 15.5f;

	public float xOffset = 3f;

	public float lerpSpeed = 3f;

	public float position;

	public float remainingLife;

	private CanvasRenderer r;
}
