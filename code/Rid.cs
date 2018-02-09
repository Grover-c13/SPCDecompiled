using System;
using UnityEngine;

public class Rid : MonoBehaviour
{
	private void Start()
	{
		this.id = base.GetComponentInChildren<MeshRenderer>().material.mainTexture.name;
	}

	public string id;
}
