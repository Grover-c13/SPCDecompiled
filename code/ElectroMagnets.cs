using System;
using UnityEngine;

public class ElectroMagnets : MonoBehaviour
{
	private void Update()
	{
		this.anim.SetBool("isUP", this.lever.GetState());
	}

	public LeverButton lever;

	public Animator anim;
}
