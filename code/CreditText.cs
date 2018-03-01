using System;
using UnityEngine;

public class CreditText : MonoBehaviour
{
	public CreditText()
	{
	}

	private void FixedUpdate()
	{
		if (this.move)
		{
			base.transform.Translate(Vector3.up * this.speed);
		}
	}

	public bool move;

	public float speed;
}
