using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class Outside : MonoBehaviour
{
	public Outside()
	{
	}

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
		GameObject gameObject = GameObject.Find("Directional light");
		if (gameObject != null)
		{
			gameObject.GetComponent<Light>().enabled = b;
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
			globalFog.startDistance = (float)((!b) ? 5 : 50);
		}
	}

	private bool isOutside = true;

	private Transform listenerPos;
}
