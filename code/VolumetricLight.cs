using System;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Light))]
public class VolumetricLight : MonoBehaviour
{
	public Light Light
	{
		get
		{
			return this._light;
		}
	}

	public Material VolumetricMaterial
	{
		get
		{
			return this._material;
		}
	}

	public VolumetricLight()
	{
	}

	private void Start()
	{
		if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D11 || SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D12 || SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal || SystemInfo.graphicsDeviceType == GraphicsDeviceType.PlayStation4 || SystemInfo.graphicsDeviceType == GraphicsDeviceType.Vulkan || SystemInfo.graphicsDeviceType == GraphicsDeviceType.XboxOne)
		{
			this._reversedZ = true;
		}
		this._commandBuffer = new CommandBuffer();
		this._commandBuffer.name = "Light Command Buffer";
		this._cascadeShadowCommandBuffer = new CommandBuffer();
		this._cascadeShadowCommandBuffer.name = "Dir Light Command Buffer";
		this._cascadeShadowCommandBuffer.SetGlobalTexture("_CascadeShadowMapTexture", new RenderTargetIdentifier(BuiltinRenderTextureType.CurrentActive));
		this._light = base.GetComponent<Light>();
		if (this._light.type == LightType.Directional)
		{
			this._light.AddCommandBuffer(LightEvent.BeforeScreenspaceMask, this._commandBuffer);
			this._light.AddCommandBuffer(LightEvent.AfterShadowMap, this._cascadeShadowCommandBuffer);
		}
		else
		{
			this._light.AddCommandBuffer(LightEvent.AfterShadowMap, this._commandBuffer);
		}
		Shader shader = Shader.Find("Sandbox/VolumetricLight");
		if (shader == null)
		{
			throw new Exception("Critical Error: \"Sandbox/VolumetricLight\" shader is missing. Make sure it is included in \"Always Included Shaders\" in ProjectSettings/Graphics.");
		}
		this._material = new Material(shader);
	}

	private void OnEnable()
	{
		VolumetricLightRenderer.PreRenderEvent += this.VolumetricLightRenderer_PreRenderEvent;
	}

	private void OnDisable()
	{
		VolumetricLightRenderer.PreRenderEvent -= this.VolumetricLightRenderer_PreRenderEvent;
	}

	public void OnDestroy()
	{
		UnityEngine.Object.Destroy(this._material);
	}

	private void VolumetricLightRenderer_PreRenderEvent(VolumetricLightRenderer renderer, Matrix4x4 viewProj)
	{
		if (!this._light.gameObject.activeInHierarchy || !this._light.enabled)
		{
			return;
		}
		this._material.SetVector("_CameraForward", Camera.current.transform.forward);
		this._material.SetInt("_SampleCount", this.SampleCount);
		this._material.SetVector("_NoiseVelocity", new Vector4(this.NoiseVelocity.x, this.NoiseVelocity.y) * this.NoiseScale);
		this._material.SetVector("_NoiseData", new Vector4(this.NoiseScale, this.NoiseIntensity, this.NoiseIntensityOffset));
		this._material.SetVector("_MieG", new Vector4(1f - this.MieG * this.MieG, 1f + this.MieG * this.MieG, 2f * this.MieG, 0.07957747f));
		this._material.SetVector("_VolumetricLight", new Vector4(this.ScatteringCoef, this.ExtinctionCoef, this._light.range, 1f - this.SkyboxExtinctionCoef));
		this._material.SetTexture("_CameraDepthTexture", renderer.GetVolumeLightDepthBuffer());
		this._material.SetFloat("_ZTest", 8f);
		if (this.HeightFog)
		{
			this._material.EnableKeyword("HEIGHT_FOG");
			this._material.SetVector("_HeightFog", new Vector4(this.GroundLevel, this.HeightScale));
		}
		else
		{
			this._material.DisableKeyword("HEIGHT_FOG");
		}
		if (this._light.type == LightType.Point)
		{
			this.SetupPointLight(renderer, viewProj);
		}
		else if (this._light.type == LightType.Spot)
		{
			this.SetupSpotLight(renderer, viewProj);
		}
		else if (this._light.type == LightType.Directional)
		{
			this.SetupDirectionalLight(renderer, viewProj);
		}
	}

	private void Update()
	{
		this._commandBuffer.Clear();
	}

	private void SetupPointLight(VolumetricLightRenderer renderer, Matrix4x4 viewProj)
	{
		int num = 0;
		if (!this.IsCameraInPointLightBounds())
		{
			num = 2;
		}
		this._material.SetPass(num);
		Mesh pointLightMesh = VolumetricLightRenderer.GetPointLightMesh();
		float num2 = this._light.range * 2f;
		Matrix4x4 matrix4x = Matrix4x4.TRS(base.transform.position, this._light.transform.rotation, new Vector3(num2, num2, num2));
		this._material.SetMatrix("_WorldViewProj", viewProj * matrix4x);
		this._material.SetMatrix("_WorldView", Camera.current.worldToCameraMatrix * matrix4x);
		if (this.Noise)
		{
			this._material.EnableKeyword("NOISE");
		}
		else
		{
			this._material.DisableKeyword("NOISE");
		}
		this._material.SetVector("_LightPos", new Vector4(this._light.transform.position.x, this._light.transform.position.y, this._light.transform.position.z, 1f / (this._light.range * this._light.range)));
		this._material.SetColor("_LightColor", this._light.color * this._light.intensity);
		if (this._light.cookie == null)
		{
			this._material.EnableKeyword("POINT");
			this._material.DisableKeyword("POINT_COOKIE");
		}
		else
		{
			Matrix4x4 inverse = Matrix4x4.TRS(this._light.transform.position, this._light.transform.rotation, Vector3.one).inverse;
			this._material.SetMatrix("_MyLightMatrix0", inverse);
			this._material.EnableKeyword("POINT_COOKIE");
			this._material.DisableKeyword("POINT");
			this._material.SetTexture("_LightTexture0", this._light.cookie);
		}
		bool flag = false;
		if ((this._light.transform.position - Camera.current.transform.position).magnitude >= QualitySettings.shadowDistance)
		{
			flag = true;
		}
		if (this._light.shadows != LightShadows.None && !flag)
		{
			this._material.EnableKeyword("SHADOWS_CUBE");
			this._commandBuffer.SetGlobalTexture("_ShadowMapTexture", BuiltinRenderTextureType.CurrentActive);
			this._commandBuffer.SetRenderTarget(renderer.GetVolumeLightBuffer());
			this._commandBuffer.DrawMesh(pointLightMesh, matrix4x, this._material, 0, num);
			if (this.CustomRenderEvent != null)
			{
				this.CustomRenderEvent(renderer, this, this._commandBuffer, viewProj);
			}
		}
		else
		{
			this._material.DisableKeyword("SHADOWS_CUBE");
			renderer.GlobalCommandBuffer.DrawMesh(pointLightMesh, matrix4x, this._material, 0, num);
			if (this.CustomRenderEvent != null)
			{
				this.CustomRenderEvent(renderer, this, renderer.GlobalCommandBuffer, viewProj);
			}
		}
	}

	private void SetupSpotLight(VolumetricLightRenderer renderer, Matrix4x4 viewProj)
	{
		int shaderPass = 1;
		if (!this.IsCameraInSpotLightBounds())
		{
			shaderPass = 3;
		}
		Mesh spotLightMesh = VolumetricLightRenderer.GetSpotLightMesh();
		float range = this._light.range;
		float num = Mathf.Tan((this._light.spotAngle + 1f) * 0.5f * 0.0174532924f) * this._light.range;
		Matrix4x4 matrix4x = Matrix4x4.TRS(base.transform.position, base.transform.rotation, new Vector3(num, num, range));
		Matrix4x4 inverse = Matrix4x4.TRS(this._light.transform.position, this._light.transform.rotation, Vector3.one).inverse;
		Matrix4x4 lhs = Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0f), Quaternion.identity, new Vector3(-0.5f, -0.5f, 1f));
		Matrix4x4 rhs = Matrix4x4.Perspective(this._light.spotAngle, 1f, 0f, 1f);
		this._material.SetMatrix("_MyLightMatrix0", lhs * rhs * inverse);
		this._material.SetMatrix("_WorldViewProj", viewProj * matrix4x);
		this._material.SetVector("_LightPos", new Vector4(this._light.transform.position.x, this._light.transform.position.y, this._light.transform.position.z, 1f / (this._light.range * this._light.range)));
		this._material.SetVector("_LightColor", this._light.color * this._light.intensity);
		Vector3 position = base.transform.position;
		Vector3 forward = base.transform.forward;
		Vector3 lhs2 = position + forward * this._light.range;
		float value = -Vector3.Dot(lhs2, forward);
		this._material.SetFloat("_PlaneD", value);
		this._material.SetFloat("_CosAngle", Mathf.Cos((this._light.spotAngle + 1f) * 0.5f * 0.0174532924f));
		this._material.SetVector("_ConeApex", new Vector4(position.x, position.y, position.z));
		this._material.SetVector("_ConeAxis", new Vector4(forward.x, forward.y, forward.z));
		this._material.EnableKeyword("SPOT");
		if (this.Noise)
		{
			this._material.EnableKeyword("NOISE");
		}
		else
		{
			this._material.DisableKeyword("NOISE");
		}
		if (this._light.cookie == null)
		{
			this._material.SetTexture("_LightTexture0", VolumetricLightRenderer.GetDefaultSpotCookie());
		}
		else
		{
			this._material.SetTexture("_LightTexture0", this._light.cookie);
		}
		bool flag = false;
		if ((this._light.transform.position - Camera.current.transform.position).magnitude >= QualitySettings.shadowDistance)
		{
			flag = true;
		}
		if (this._light.shadows != LightShadows.None && !flag)
		{
			lhs = Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, new Vector3(0.5f, 0.5f, 0.5f));
			if (this._reversedZ)
			{
				rhs = Matrix4x4.Perspective(this._light.spotAngle, 1f, this._light.range, this._light.shadowNearPlane);
			}
			else
			{
				rhs = Matrix4x4.Perspective(this._light.spotAngle, 1f, this._light.shadowNearPlane, this._light.range);
			}
			Matrix4x4 lhs3 = lhs * rhs;
			lhs3[0, 2] = lhs3[0, 2] * -1f;
			lhs3[1, 2] = lhs3[1, 2] * -1f;
			lhs3[2, 2] = lhs3[2, 2] * -1f;
			lhs3[3, 2] = lhs3[3, 2] * -1f;
			this._material.SetMatrix("_MyWorld2Shadow", lhs3 * inverse);
			this._material.SetMatrix("_WorldView", lhs3 * inverse);
			this._material.EnableKeyword("SHADOWS_DEPTH");
			this._commandBuffer.SetGlobalTexture("_ShadowMapTexture", BuiltinRenderTextureType.CurrentActive);
			this._commandBuffer.SetRenderTarget(renderer.GetVolumeLightBuffer());
			this._commandBuffer.DrawMesh(spotLightMesh, matrix4x, this._material, 0, shaderPass);
			if (this.CustomRenderEvent != null)
			{
				this.CustomRenderEvent(renderer, this, this._commandBuffer, viewProj);
			}
		}
		else
		{
			this._material.DisableKeyword("SHADOWS_DEPTH");
			renderer.GlobalCommandBuffer.DrawMesh(spotLightMesh, matrix4x, this._material, 0, shaderPass);
			if (this.CustomRenderEvent != null)
			{
				this.CustomRenderEvent(renderer, this, renderer.GlobalCommandBuffer, viewProj);
			}
		}
	}

	private void SetupDirectionalLight(VolumetricLightRenderer renderer, Matrix4x4 viewProj)
	{
		int pass = 4;
		this._material.SetPass(pass);
		if (this.Noise)
		{
			this._material.EnableKeyword("NOISE");
		}
		else
		{
			this._material.DisableKeyword("NOISE");
		}
		this._material.SetVector("_LightDir", new Vector4(this._light.transform.forward.x, this._light.transform.forward.y, this._light.transform.forward.z, 1f / (this._light.range * this._light.range)));
		this._material.SetVector("_LightColor", this._light.color * this._light.intensity);
		this._material.SetFloat("_MaxRayLength", this.MaxRayLength);
		if (this._light.cookie == null)
		{
			this._material.EnableKeyword("DIRECTIONAL");
			this._material.DisableKeyword("DIRECTIONAL_COOKIE");
		}
		else
		{
			this._material.EnableKeyword("DIRECTIONAL_COOKIE");
			this._material.DisableKeyword("DIRECTIONAL");
			this._material.SetTexture("_LightTexture0", this._light.cookie);
		}
		this._frustumCorners[0] = Camera.current.ViewportToWorldPoint(new Vector3(0f, 0f, Camera.current.farClipPlane));
		this._frustumCorners[2] = Camera.current.ViewportToWorldPoint(new Vector3(0f, 1f, Camera.current.farClipPlane));
		this._frustumCorners[3] = Camera.current.ViewportToWorldPoint(new Vector3(1f, 1f, Camera.current.farClipPlane));
		this._frustumCorners[1] = Camera.current.ViewportToWorldPoint(new Vector3(1f, 0f, Camera.current.farClipPlane));
		this._material.SetVectorArray("_FrustumCorners", this._frustumCorners);
		Texture source = null;
		if (this._light.shadows != LightShadows.None)
		{
			this._material.EnableKeyword("SHADOWS_DEPTH");
			this._commandBuffer.Blit(source, renderer.GetVolumeLightBuffer(), this._material, pass);
			if (this.CustomRenderEvent != null)
			{
				this.CustomRenderEvent(renderer, this, this._commandBuffer, viewProj);
			}
		}
		else
		{
			this._material.DisableKeyword("SHADOWS_DEPTH");
			renderer.GlobalCommandBuffer.Blit(source, renderer.GetVolumeLightBuffer(), this._material, pass);
			if (this.CustomRenderEvent != null)
			{
				this.CustomRenderEvent(renderer, this, renderer.GlobalCommandBuffer, viewProj);
			}
		}
	}

	private bool IsCameraInPointLightBounds()
	{
		float sqrMagnitude = (this._light.transform.position - Camera.current.transform.position).sqrMagnitude;
		float num = this._light.range + 1f;
		return sqrMagnitude < num * num;
	}

	private bool IsCameraInSpotLightBounds()
	{
		float num = Vector3.Dot(this._light.transform.forward, Camera.current.transform.position - this._light.transform.position);
		float num2 = this._light.range + 1f;
		if (num > num2)
		{
			return false;
		}
		float f = Vector3.Dot(base.transform.forward, (Camera.current.transform.position - this._light.transform.position).normalized);
		return Mathf.Acos(f) * 57.29578f <= (this._light.spotAngle + 3f) * 0.5f;
	}

	public event Action<VolumetricLightRenderer, VolumetricLight, CommandBuffer, Matrix4x4> CustomRenderEvent;

	private Light _light;

	private Material _material;

	private CommandBuffer _commandBuffer;

	private CommandBuffer _cascadeShadowCommandBuffer;

	[Range(1f, 64f)]
	public int SampleCount = 12;

	[Range(0f, 1f)]
	public float ScatteringCoef = 0.1f;

	[Range(0f, 0.1f)]
	public float ExtinctionCoef = 0.01f;

	[Range(0f, 1f)]
	public float SkyboxExtinctionCoef = 0.9f;

	[Range(0f, 0.999f)]
	public float MieG = 0.1f;

	public bool HeightFog;

	[Range(0f, 0.5f)]
	public float HeightScale = 0.1f;

	public float GroundLevel;

	public bool Noise;

	public float NoiseScale = 0.015f;

	public float NoiseIntensity = 1f;

	public float NoiseIntensityOffset = 0.3f;

	public Vector2 NoiseVelocity = new Vector2(3f, 3f);

	[Tooltip("")]
	public float MaxRayLength = 400f;

	private Vector4[] _frustumCorners = new Vector4[4];

	private bool _reversedZ;
}
