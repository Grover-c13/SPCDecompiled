using System;
using UnityEngine;

public class SmokeGrenade : GrenadeInstance
{
	public override void Explode(int thrower)
	{
		base.Explode(thrower);
		UnityEngine.Object.Destroy(UnityEngine.Object.Instantiate<GameObject>(this.gfx, base.transform.position, Quaternion.Euler(Vector3.zero)), this.duration);
	}

	public GameObject gfx;

	public float duration = 20f;
}
