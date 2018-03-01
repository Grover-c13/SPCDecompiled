using System;
using UnityEngine;

public class GameMenuButton : MonoBehaviour
{
	public GameMenuButton()
	{
	}

	private void Start()
	{
		this.rectTransform = base.GetComponent<RectTransform>();
	}

	public void Focus(bool b)
	{
		this.isFocused = b;
	}

	private void Update()
	{
		this.status += Time.deltaTime * (float)((!this.isFocused) ? -1 : 1);
		this.status = Mathf.Clamp01(this.status);
		Vector3 a = this.focusedPos - this.normalPos;
		this.rectTransform.localPosition = this.normalPos + a * this.anim.Evaluate(this.status);
	}

	public Vector3 normalPos;

	public Vector3 focusedPos;

	public AnimationCurve anim;

	private bool isFocused;

	private float status;

	private RectTransform rectTransform;
}
