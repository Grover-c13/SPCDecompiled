using System;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
	private void Update()
	{
		float num = (float)Screen.width * (this.borderWidthPercent / 100f);
		Vector3 vector = Vector3.zero;
		Vector3 mousePosition = Input.mousePosition;
		if (mousePosition.x < num && base.transform.localRotation.eulerAngles.y > 41f)
		{
			vector += Vector3.down;
		}
		if (mousePosition.x > (float)Screen.width - num && base.transform.localRotation.eulerAngles.y < 74f)
		{
			vector += Vector3.up;
		}
		if (vector == Vector3.zero)
		{
			this.rotSpeed = 0f;
		}
		else
		{
			this.rotSpeed += Time.deltaTime * 200f;
			this.rotSpeed = Mathf.Clamp(this.rotSpeed, 0f, 120f);
		}
		vector.Normalize();
		base.transform.localRotation = Quaternion.Euler(base.transform.localRotation.eulerAngles + vector * Time.deltaTime * this.rotSpeed);
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			this.Raycast();
		}
	}

	private void Raycast()
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(base.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition), out raycastHit))
		{
			this.ElementChoosen(raycastHit.transform.name);
		}
	}

	public void ElementChoosen(string id)
	{
		if (id == "EXIT")
		{
			Application.Quit();
		}
		if (id == "PLAY")
		{
			UnityEngine.Object.FindObjectOfType<NetManagerValueSetter>().HostGame();
		}
	}

	public float borderWidthPercent;

	private float rotSpeed;
}
