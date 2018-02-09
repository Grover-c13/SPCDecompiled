using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class Outside : MonoBehaviour
{
	private void Update()
	{
		if (this.listenerPos == null)
		{
			SpectatorCamera spectatorCamera = UnityEngine.Object.FindObjectOfType<SpectatorCamera>();
			if (spectatorCamera != null)
			{
				this.listenerPos = spectatorCamera.cam.transform;
			}
		}
		if (this.listenerPos.position.y > 900f && !this.isOutside)
		{
			this.isOutside = true;
			this.SetOutside(true);
		}
		if (this.listenerPos.position.y < 900f && this.isOutside)
		{
			this.isOutside = false;
			this.SetOutside(false);
		}
	}

	private void SetOutside(bool b)
	{
		if (GameObject.Find("Directional light") != null)
		{
			GameObject.Find("Directional light").GetComponent<Light>().enabled = b;
		}
		foreach (Camera camera in base.GetComponentsInChildren<Camera>(true))
		{
			if (camera.farClipPlane == 600f || camera.farClipPlane == 47f)
			{
				camera.farClipPlane = (float)((!b) ? 47 : 600);
				camera.clearFlags = ((!b) ? CameraClearFlags.Color : CameraClearFlags.Skybox);
			}
		}
		foreach (GlobalFog globalFog in base.GetComponentsInChildren<GlobalFog>(true))
		{
			globalFog.startDistance = (float)((!b) ? 0 : 50);
		}
	}

	private bool isOutside = true;

	private Transform listenerPos;
}
