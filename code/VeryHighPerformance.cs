using System;
using UnityEngine;

public class VeryHighPerformance : MonoBehaviour
{
	private void Start()
	{
		if (PlayerPrefs.GetInt("gfxsets_hp", 0) == 0)
		{
			return;
		}
		Light[] array = UnityEngine.Object.FindObjectsOfType<Light>();
		foreach (Light light in array)
		{
			UnityEngine.Object.Destroy(light.transform.gameObject);
		}
		RenderSettings.ambientEquatorColor = new Color(0.5f, 0.5f, 0.5f);
		RenderSettings.ambientGroundColor = new Color(0.5f, 0.5f, 0.5f);
		RenderSettings.ambientSkyColor = new Color(0.5f, 0.5f, 0.5f);
	}
}
