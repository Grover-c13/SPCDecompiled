using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class WeaponCamera : MonoBehaviour
{
	public WeaponCamera()
	{
	}

	private void Start()
	{
		this.myvaca = base.GetComponent<VignetteAndChromaticAberration>();
		this.vaca = base.GetComponentInParent<VignetteAndChromaticAberration>();
	}

	private void Update()
	{
		this.myvaca = this.vaca;
	}

	private VignetteAndChromaticAberration vaca;

	private VignetteAndChromaticAberration myvaca;
}
