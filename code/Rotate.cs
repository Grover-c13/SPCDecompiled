using System;
using UnityEngine;

public class Rotate : MonoBehaviour
{
	private void Update()
	{
		base.transform.Rotate(this.speed * Time.deltaTime);
	}

	private Vector3 speed;
}
