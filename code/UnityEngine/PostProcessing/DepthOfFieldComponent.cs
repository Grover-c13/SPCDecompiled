using System;

namespace UnityEngine.PostProcessing
{
	public sealed class DepthOfFieldComponent : PostProcessingComponentRenderTexture<DepthOfFieldModel>
	{
		public DepthOfFieldComponent()
		{
		}

		public override bool active
		{
			get
			{
				return base.model.enabled && SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf) && SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RHalf) && !this.context.interrupted;
			}
		}

		public override DepthTextureMode GetCameraFlags()
		{
			return DepthTextureMode.Depth;
		}

		private float CalculateFocalLength()
		{
			DepthOfFieldModel.Settings settings = base.model.settings;
			if (!settings.useCameraFov)
			{
				return settings.focalLength / 1000f;
			}
			float num = this.context.camera.fieldOfView * 0.0174532924f;
			return 0.012f / Mathf.Tan(0.5f * num);
		}

		private float CalculateMaxCoCRadius(int screenHeight)
		{
			float num = (float)base.model.settings.kernelSize * 4f + 6f;
			return Mathf.Min(0.05f, num / (float)screenHeight);
		}

		public void Prepare(RenderTexture source, Material uberMaterial, bool antialiasCoC)
		{
			DepthOfFieldModel.Settings settings = base.model.settings;
			Material material = this.context.materialFactory.Get("Hidden/Post FX/Depth Of Field");
			material.shaderKeywords = null;
			float num = settings.focusDistance;
			float num2 = this.CalculateFocalLength();
			num = Mathf.Max(num, num2);
			material.SetFloat(DepthOfFieldComponent.Uniforms._Distance, num);
			float num3 = num2 * num2 / (settings.aperture * (num - num2) * 0.024f * 2f);
			material.SetFloat(DepthOfFieldComponent.Uniforms._LensCoeff, num3);
			float num4 = this.CalculateMaxCoCRadius(source.height);
			material.SetFloat(DepthOfFieldComponent.Uniforms._MaxCoC, num4);
			material.SetFloat(DepthOfFieldComponent.Uniforms._RcpMaxCoC, 1f / num4);
			float value = (float)source.height / (float)source.width;
			material.SetFloat(DepthOfFieldComponent.Uniforms._RcpAspect, value);
			RenderTexture renderTexture = this.context.renderTextureFactory.Get(this.context.width / 2, this.context.height / 2, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Default, FilterMode.Bilinear, TextureWrapMode.Clamp, "FactoryTempTexture");
			source.filterMode = FilterMode.Point;
			if (!antialiasCoC)
			{
				Graphics.Blit(source, renderTexture, material, 0);
			}
			else
			{
				bool flag = this.m_CoCHistory == null || !this.m_CoCHistory.IsCreated() || this.m_CoCHistory.width != this.context.width / 2 || this.m_CoCHistory.height != this.context.height / 2;
				RenderTexture temporary = RenderTexture.GetTemporary(this.context.width / 2, this.context.height / 2, 0, RenderTextureFormat.RHalf);
				temporary.filterMode = FilterMode.Point;
				temporary.name = "CoC History";
				this.m_MRT[0] = renderTexture.colorBuffer;
				this.m_MRT[1] = temporary.colorBuffer;
				material.SetTexture(DepthOfFieldComponent.Uniforms._MainTex, source);
				material.SetTexture(DepthOfFieldComponent.Uniforms._HistoryCoC, this.m_CoCHistory);
				material.SetFloat(DepthOfFieldComponent.Uniforms._HistoryWeight, (!flag) ? 0.5f : 0f);
				Graphics.SetRenderTarget(this.m_MRT, renderTexture.depthBuffer);
				GraphicsUtils.Blit(material, 1);
				RenderTexture.ReleaseTemporary(this.m_CoCHistory);
				this.m_CoCHistory = temporary;
			}
			RenderTexture renderTexture2 = this.context.renderTextureFactory.Get(this.context.width / 2, this.context.height / 2, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Default, FilterMode.Bilinear, TextureWrapMode.Clamp, "FactoryTempTexture");
			Graphics.Blit(renderTexture, renderTexture2, material, (int)(2 + settings.kernelSize));
			Graphics.Blit(renderTexture2, renderTexture, material, 6);
			if (this.context.profile.debugViews.IsModeActive(BuiltinDebugViewsModel.Mode.FocusPlane))
			{
				uberMaterial.SetVector(DepthOfFieldComponent.Uniforms._DepthOfFieldParams, new Vector2(num, num3));
				uberMaterial.EnableKeyword("DEPTH_OF_FIELD_COC_VIEW");
				this.context.Interrupt();
			}
			else
			{
				uberMaterial.SetTexture(DepthOfFieldComponent.Uniforms._DepthOfFieldTex, renderTexture);
				uberMaterial.EnableKeyword("DEPTH_OF_FIELD");
			}
			this.context.renderTextureFactory.Release(renderTexture2);
			source.filterMode = FilterMode.Bilinear;
		}

		public override void OnDisable()
		{
			if (this.m_CoCHistory != null)
			{
				RenderTexture.ReleaseTemporary(this.m_CoCHistory);
			}
			this.m_CoCHistory = null;
		}

		private const string k_ShaderString = "Hidden/Post FX/Depth Of Field";

		private RenderTexture m_CoCHistory;

		private RenderBuffer[] m_MRT = new RenderBuffer[2];

		private const float k_FilmHeight = 0.024f;

		private static class Uniforms
		{
			static Uniforms()
			{
				// Note: this type is marked as 'beforefieldinit'.
			}

			internal static readonly int _DepthOfFieldTex = Shader.PropertyToID("_DepthOfFieldTex");

			internal static readonly int _Distance = Shader.PropertyToID("_Distance");

			internal static readonly int _LensCoeff = Shader.PropertyToID("_LensCoeff");

			internal static readonly int _MaxCoC = Shader.PropertyToID("_MaxCoC");

			internal static readonly int _RcpMaxCoC = Shader.PropertyToID("_RcpMaxCoC");

			internal static readonly int _RcpAspect = Shader.PropertyToID("_RcpAspect");

			internal static readonly int _MainTex = Shader.PropertyToID("_MainTex");

			internal static readonly int _HistoryCoC = Shader.PropertyToID("_HistoryCoC");

			internal static readonly int _HistoryWeight = Shader.PropertyToID("_HistoryWeight");

			internal static readonly int _DepthOfFieldParams = Shader.PropertyToID("_DepthOfFieldParams");
		}
	}
}
