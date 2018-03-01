using System;
using UnityEngine;

public class Skybox : MonoBehaviour
{
	public Skybox()
	{
	}

	private void Start()
	{
		this.cam = UnityEngine.Object.FindObjectOfType<SpectatorCamera>().cam.transform;
	}

	private void Update()
	{
		base.transform.position = new Vector3(this.cam.position.x, 1150f, this.cam.position.z);
	}

	private Transform cam;
}
