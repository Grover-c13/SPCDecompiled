using System;
using UnityEngine;

[ExecuteInEditMode]
public class NGSS_NoiseTexture : MonoBehaviour
{
	public NGSS_NoiseTexture()
	{
	}

	private void Update()
	{
		Shader.SetGlobalFloat("NGSS_NOISE_TEXTURE_SCALE", this.noiseScale);
		if (this.isTextureSet || this.noiseTex == null)
		{
			return;
		}
		Shader.SetGlobalTexture("NGSS_NOISE_TEXTURE", this.noiseTex);
		this.isTextureSet = true;
	}

	public Texture noiseTex;

	[Range(0f, 1f)]
	public float noiseScale = 1f;

	private bool isTextureSet;
}
