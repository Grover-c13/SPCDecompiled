using System;
using UnityEngine;

public class Scp096PlayerScript : MonoBehaviour
{
	public float rageProgress;

	public bool rage;

	public float rageSpeedMultiplier;

	public float rageDurationMultiplier;

	public float rageCooldownTime;

	public static Scp096PlayerScript instance;

	public bool sameClass;

	public bool iAm096;

	public LayerMask viewMask;

	public AnimationCurve viewAngleTolerance;
}
