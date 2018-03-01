using System;
using UnityEngine;

public class MapScroll : MonoBehaviour
{
	public MapScroll()
	{
	}

	private void Start()
	{
		this.rootTransf = base.GetComponent<RectTransform>();
	}

	private void Update()
	{
		this.rootTransf.localScale += Vector3.one * Input.GetAxis("Mouse ScrollWheel") * 2f * this.minZoom;
		this.rootTransf.localScale = Vector3.one * Mathf.Clamp(this.rootTransf.localScale.x, this.minZoom, this.maxZoom);
		if (Input.GetButton("Fire1"))
		{
			this.map.localPosition += new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0f) * this.speed * (2f / this.rootTransf.localScale.x);
		}
		if (Input.GetButton("Zoom"))
		{
			this.rootTransf.localScale = Vector3.one;
			this.map.localPosition = Vector3.zero;
		}
	}

	public RectTransform map;

	public RectTransform rootTransf;

	public float minZoom;

	public float maxZoom;

	public float speed;
}
