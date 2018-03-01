using System;
using UnityEngine;

public class Noclip : MonoBehaviour
{
	public Noclip()
	{
	}

	public void Switch()
	{
		this.isOn = !this.isOn;
	}

	private void Update()
	{
		if (!this.isOn)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.N))
		{
			this.activated = !this.activated;
			base.GetComponent<FirstPersonController>().noclip = this.activated;
		}
		if (this.activated)
		{
			base.transform.position += this.cam.transform.forward * Input.GetAxis("Vertical") + this.cam.transform.right * Input.GetAxis("Horizontal");
		}
	}

	public bool Get()
	{
		return this.isOn;
	}

	private bool isOn;

	private bool activated;

	public GameObject cam;
}
