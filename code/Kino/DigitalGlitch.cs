using System;
using UnityEngine;

namespace Kino
{
	[ExecuteInEditMode]
	[AddComponentMenu("Kino Image Effects/Digital Glitch")]
	[RequireComponent(typeof(Camera))]
	public class DigitalGlitch : MonoBehaviour
	{
		public float intensity
		{
			get
			{
				return this._intensity;
			}
			set
			{
				this._intensity = value;
			}
		}

		private static Color RandomColor()
		{
			return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
		}

		private void SetUpResources()
		{
			if (this._material != null)
			{
				return;
			}
			this._material = new Material(this._shader);
			this._material.hideFlags = HideFlags.DontSave;
			this._noiseTexture = new Texture2D(64, 32, TextureFormat.ARGB32, false);
			this._noiseTexture.hideFlags = HideFlags.DontSave;
			this._noiseTexture.wrapMode = TextureWrapMode.Clamp;
			this._noiseTexture.filterMode = FilterMode.Point;
			this._trashFrame1 = new RenderTexture(Screen.width, Screen.height, 0);
			this._trashFrame2 = new RenderTexture(Screen.width, Screen.height, 0);
			this._trashFrame1.hideFlags = HideFlags.DontSave;
			this._trashFrame2.hideFlags = HideFlags.DontSave;
			this.UpdateNoiseTexture();
		}

		private void UpdateNoiseTexture()
		{
			Color color = DigitalGlitch.RandomColor();
			for (int i = 0; i < this._noiseTexture.height; i++)
			{
				for (int j = 0; j < this._noiseTexture.width; j++)
				{
					if (UnityEngine.Random.value > 0.89f)
					{
						color = DigitalGlitch.RandomColor();
					}
					this._noiseTexture.SetPixel(j, i, color);
				}
			}
			this._noiseTexture.Apply();
		}

		private void Update()
		{
			if (UnityEngine.Random.value > Mathf.Lerp(0.9f, 0.5f, this._intensity))
			{
				this.SetUpResources();
				this.UpdateNoiseTexture();
			}
		}

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			this.SetUpResources();
			int frameCount = Time.frameCount;
			if (frameCount % 13 == 0)
			{
				Graphics.Blit(source, this._trashFrame1);
			}
			if (frameCount % 73 == 0)
			{
				Graphics.Blit(source, this._trashFrame2);
			}
			this._material.SetFloat("_Intensity", this._intensity);
			this._material.SetTexture("_NoiseTex", this._noiseTexture);
			RenderTexture value = (UnityEngine.Random.value <= 0.5f) ? this._trashFrame2 : this._trashFrame1;
			this._material.SetTexture("_TrashTex", value);
			Graphics.Blit(source, destination, this._material);
		}

		[SerializeField]
		[Range(0f, 1f)]
		private float _intensity;

		[SerializeField]
		private Shader _shader;

		private Material _material;

		private Texture2D _noiseTexture;

		private RenderTexture _trashFrame1;

		private RenderTexture _trashFrame2;
	}
}
