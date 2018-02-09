using System;
using UnityEngine;

public class ToggleableLight : MonoBehaviour
{
	public void SetLights(bool b)
	{
		foreach (GameObject gameObject in this.allLights)
		{
			gameObject.SetActive((!this.isAlarm) ? (!b) : b);
		}
	}

	public GameObject[] allLights;

	public bool isAlarm;
}
