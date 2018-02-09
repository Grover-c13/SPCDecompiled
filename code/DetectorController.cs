using System;
using UnityEngine;

public class DetectorController : MonoBehaviour
{
	private void Start()
	{
		base.InvokeRepeating("RefreshDetectorsList", 10f, 10f);
	}

	public void RefreshDetectorsList()
	{
		this.detectors = GameObject.FindGameObjectsWithTag("Detector");
	}

	private void Update()
	{
		if (this.detectors.Length == 0)
		{
			return;
		}
		bool flag = false;
		foreach (GameObject gameObject in this.detectors)
		{
			if (Vector3.Distance(gameObject.transform.position, base.transform.position) > this.viewRange)
			{
				Vector3 normalized = (base.transform.position - gameObject.transform.position).normalized;
				RaycastHit raycastHit;
				if (Vector3.Dot(gameObject.transform.forward, normalized) < this.fov && Physics.Raycast(gameObject.transform.position, normalized, out raycastHit) && raycastHit.transform.tag == "Detector")
				{
					flag = true;
					break;
				}
			}
		}
		this.detectionProgress += Time.deltaTime * ((!flag) ? -0.5f : 0.3f);
		this.detectionProgress = Mathf.Clamp01(this.detectionProgress);
	}

	public float detectionProgress;

	public float viewRange = 30f;

	public float fov = -0.75f;

	public GameObject[] detectors;
}
