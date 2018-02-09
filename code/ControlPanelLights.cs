using System;
using System.Collections;
using UnityEngine;

public class ControlPanelLights : MonoBehaviour
{
	private void Start()
	{
		base.StartCoroutine(this.Animate());
	}

	private IEnumerator Animate()
	{
		int i = this.emissions.Length;
		for (;;)
		{
			this.targetMat.SetTexture("_EmissionMap", this.emissions[UnityEngine.Random.Range(0, i)]);
			yield return new WaitForSeconds(UnityEngine.Random.Range(0.2f, 0.8f));
		}
		yield break;
	}

	public Texture[] emissions;

	public Material targetMat;
}
