using System;
using System.Collections;
using UnityEngine;

public class TextureAnimator : MonoBehaviour
{
	private void Start()
	{
		base.StartCoroutine(this.Animate());
	}

	private IEnumerator Animate()
	{
		for (;;)
		{
			for (int i = 0; i < this.textures.Length; i++)
			{
				this.optionalLight.enabled = (i < this.lightRange);
				this.targetRenderer.material = this.textures[i];
				yield return new WaitForSeconds(this.cooldown);
			}
		}
		yield break;
	}

	public Material[] textures;

	public Renderer targetRenderer;

	public float cooldown;

	public Light optionalLight;

	public int lightRange;
}
