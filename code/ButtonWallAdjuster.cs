using System;
using UnityEngine;

public class ButtonWallAdjuster : MonoBehaviour
{
	public ButtonWallAdjuster()
	{
	}

	private void Start()
	{
		if (this.onAwake)
		{
			this.Adjust();
		}
	}

	public void Adjust()
	{
		if (this.adjusted && !this.onAwake)
		{
			return;
		}
		this.adjusted = true;
		base.transform.position += base.transform.up;
		RaycastHit raycastHit;
		if (Physics.Raycast(new Ray(base.transform.position, -base.transform.up), out raycastHit, 2f))
		{
			base.transform.position = raycastHit.point;
			base.transform.position -= base.transform.up * this.offset * 0.1f;
		}
	}

	public float offset = 0.1f;

	private bool adjusted;

	public bool onAwake;
}
