using System;
using UnityEngine;

public class ElevatorDoor : MonoBehaviour
{
	public void SetOpen(bool b)
	{
		if (b != this.isOpen)
		{
			base.GetComponent<AudioSource>().Play();
		}
		this.isOpen = b;
	}

	private void Update()
	{
		base.transform.localPosition = Vector3.LerpUnclamped(base.transform.localPosition, (!this.isOpen) ? this.closePos : this.openPos, Time.deltaTime * 3f);
	}

	private bool isOpen;

	public Vector3 openPos;

	public Vector3 closePos;
}
