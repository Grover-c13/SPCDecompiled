using System;
using UnityEngine;

public class Rid : MonoBehaviour
{
	public Rid()
	{
	}

	private void Start()
	{
		if (string.IsNullOrEmpty(this.id))
		{
			this.id = base.GetComponentInChildren<MeshRenderer>().material.mainTexture.name;
		}
	}

	public string id;
}
