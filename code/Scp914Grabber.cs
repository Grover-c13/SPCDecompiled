using System;
using System.Collections.Generic;
using UnityEngine;

public class Scp914Grabber : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		this.observes.Add(other);
	}

	private void OnTriggerExit(Collider other)
	{
		this.observes.Remove(other);
	}

	public List<Collider> observes = new List<Collider>();
}
