using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class VolumetricLightRenderer : MonoBehaviour
{
	public VolumetricLightRenderer()
	{
	}

	public static event Action<VolumetricLightRenderer, Matrix4x4> PreRenderEvent
	{
		add
		{
			Action<VolumetricLightRenderer, Matrix4x4> action = VolumetricLightRenderer.PreRenderEvent;
			Action<VolumetricLightRenderer, Matrix4x4> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange<Action<VolumetricLightRenderer, Matrix4x4>>(ref VolumetricLightRenderer.PreRenderEvent, (Action<VolumetricLightRenderer, Matrix4x4>)Delegate.Combine(action2, value), action);
			}
			while (action != action2);
		}
		remove
		{
			Action<VolumetricLightRenderer, Matrix4x4> action = VolumetricLightRenderer.PreRenderEvent;
			Action<VolumetricLightRenderer, Matrix4x4> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange<Action<VolumetricLightRenderer, Matrix4x4>>(ref VolumetricLightRenderer.PreRenderEvent, (Action<VolumetricLightRenderer, Matrix4x4>)Delegate.Remove(action2, value), action);
			}
			while (action != action2);
		}
	}

	public CommandBuffer GlobalCommandBuffer
	{
		get
		{
			return this._preLightPass;
		}
	}

	public static Material GetLightMaterial()
	{
		return VolumetricLightRenderer._lightMaterial;
	}

	public static Mesh GetPointLightMesh()
	{
		return VolumetricLightRenderer._pointLightMesh;
	}

	public static Mesh GetSpotLightMesh()
	{
		return VolumetricLightRenderer._spotLightMesh;
	}

	public RenderTexture GetVolumeLightBuffer()
	{
		if (this.Resolution == VolumetricLightRenderer.VolumtericResolution.Quarter)
		{
			return this._quarterVolumeLightTexture;
		}
		if (this.Resolution == VolumetricLightRenderer.VolumtericResolution.Half)
		{
			return this._halfVolumeLightTexture;
		}
		return this._volumeLightTexture;
	}

	public RenderTexture GetVolumeLightDepthBuffer()
	{
		if (this.Resolution == VolumetricLightRenderer.VolumtericResolution.Quarter)
		{
			return this._quarterDepthBuffer;
		}
		if (this.Resolution == VolumetricLightRenderer.VolumtericResolution.Half)
		{
			return this._halfDepthBuffer;
		}
		return null;
	}

	public static Texture GetDefaultSpotCookie()
	{
		return VolumetricLightRenderer._defaultSpotCookie;
	}

	private void Awake()
	{
		this._camera = base.GetComponent<Camera>();
		if (this._camera.actualRenderingPath == RenderingPath.Forward)
		{
			this._camera.depthTextureMode = DepthTextureMode.Depth;
		}
		this._currentResolution = this.Resolution;
		Shader shader = Shader.Find("Hidden/BlitAdd");
		if (shader == null)
		{
			throw new Exception("Critical Error: \"Hidden/BlitAdd\" shader is missing. Make sure it is included in \"Always Included Shaders\" in ProjectSettings/Graphics.");
		}
		this._blitAddMaterial = new Material(shader);
		shader = Shader.Find("Hidden/BilateralBlur");
		if (shader == null)
		{
			throw new Exception("Critical Error: \"Hidden/BilateralBlur\" shader is missing. Make sure it is included in \"Always Included Shaders\" in ProjectSettings/Graphics.");
		}
		this._bilateralBlurMaterial = new Material(shader);
		this._preLightPass = new CommandBuffer();
		this._preLightPass.name = "PreLight";
		this.ChangeResolution();
		if (VolumetricLightRenderer._pointLightMesh == null)
		{
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			VolumetricLightRenderer._pointLightMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
			UnityEngine.Object.Destroy(gameObject);
		}
		if (VolumetricLightRenderer._spotLightMesh == null)
		{
			VolumetricLightRenderer._spotLightMesh = this.CreateSpotLightMesh();
		}
		if (VolumetricLightRenderer._lightMaterial == null)
		{
			shader = Shader.Find("Sandbox/VolumetricLight");
			if (shader == null)
			{
				throw new Exception("Critical Error: \"Sandbox/VolumetricLight\" shader is missing. Make sure it is included in \"Always Included Shaders\" in ProjectSettings/Graphics.");
			}
			VolumetricLightRenderer._lightMaterial = new Material(shader);
		}
		if (VolumetricLightRenderer._defaultSpotCookie == null)
		{
			VolumetricLightRenderer._defaultSpotCookie = this.DefaultSpotCookie;
		}
		this.LoadNoise3dTexture();
		this.GenerateDitherTexture();
	}

	private void OnEnable()
	{
		if (this._camera.actualRenderingPath == RenderingPath.Forward)
		{
			this._camera.AddCommandBuffer(CameraEvent.AfterDepthTexture, this._preLightPass);
		}
		else
		{
			this._camera.AddCommandBuffer(CameraEvent.BeforeLighting, this._preLightPass);
		}
	}

	private void OnDisable()
	{
		if (this._camera.actualRenderingPath == RenderingPath.Forward)
		{
			this._camera.RemoveCommandBuffer(CameraEvent.AfterDepthTexture, this._preLightPass);
		}
		else
		{
			this._camera.RemoveCommandBuffer(CameraEvent.BeforeLighting, this._preLightPass);
		}
	}

	private void ChangeResolution()
	{
		int pixelWidth = this._camera.pixelWidth;
		int pixelHeight = this._camera.pixelHeight;
		if (this._volumeLightTexture != null)
		{
			UnityEngine.Object.Destroy(this._volumeLightTexture);
		}
		this._volumeLightTexture = new RenderTexture(pixelWidth, pixelHeight, 0, RenderTextureFormat.ARGBHalf);
		this._volumeLightTexture.name = "VolumeLightBuffer";
		this._volumeLightTexture.filterMode = FilterMode.Bilinear;
		if (this._halfDepthBuffer != null)
		{
			UnityEngine.Object.Destroy(this._halfDepthBuffer);
		}
		if (this._halfVolumeLightTexture != null)
		{
			UnityEngine.Object.Destroy(this._halfVolumeLightTexture);
		}
		if (this.Resolution == VolumetricLightRenderer.VolumtericResolution.Half || this.Resolution == VolumetricLightRenderer.VolumtericResolution.Quarter)
		{
			this._halfVolumeLightTexture = new RenderTexture(pixelWidth / 2, pixelHeight / 2, 0, RenderTextureFormat.ARGBHalf);
			this._halfVolumeLightTexture.name = "VolumeLightBufferHalf";
			this._halfVolumeLightTexture.filterMode = FilterMode.Bilinear;
			this._halfDepthBuffer = new RenderTexture(pixelWidth / 2, pixelHeight / 2, 0, RenderTextureFormat.RFloat);
			this._halfDepthBuffer.name = "VolumeLightHalfDepth";
			this._halfDepthBuffer.Create();
			this._halfDepthBuffer.filterMode = FilterMode.Point;
		}
		if (this._quarterVolumeLightTexture != null)
		{
			UnityEngine.Object.Destroy(this._quarterVolumeLightTexture);
		}
		if (this._quarterDepthBuffer != null)
		{
			UnityEngine.Object.Destroy(this._quarterDepthBuffer);
		}
		if (this.Resolution == VolumetricLightRenderer.VolumtericResolution.Quarter)
		{
			this._quarterVolumeLightTexture = new RenderTexture(pixelWidth / 4, pixelHeight / 4, 0, RenderTextureFormat.ARGBHalf);
			this._quarterVolumeLightTexture.name = "VolumeLightBufferQuarter";
			this._quarterVolumeLightTexture.filterMode = FilterMode.Bilinear;
			this._quarterDepthBuffer = new RenderTexture(pixelWidth / 4, pixelHeight / 4, 0, RenderTextureFormat.RFloat);
			this._quarterDepthBuffer.name = "VolumeLightQuarterDepth";
			this._quarterDepthBuffer.Create();
			this._quarterDepthBuffer.filterMode = FilterMode.Point;
		}
	}

	public void OnPreRender()
	{
		Matrix4x4 matrix4x = Matrix4x4.Perspective(this._camera.fieldOfView, this._camera.aspect, 0.01f, this._camera.farClipPlane);
		matrix4x = GL.GetGPUProjectionMatrix(matrix4x, true);
		this._viewProj = matrix4x * this._camera.worldToCameraMatrix;
		this._preLightPass.Clear();
		bool flag = SystemInfo.graphicsShaderLevel > 40;
		if (this.Resolution == VolumetricLightRenderer.VolumtericResolution.Quarter)
		{
			Texture source = null;
			this._preLightPass.Blit(source, this._halfDepthBuffer, this._bilateralBlurMaterial, (!flag) ? 10 : 4);
			this._preLightPass.Blit(source, this._quarterDepthBuffer, this._bilateralBlurMaterial, (!flag) ? 11 : 6);
			this._preLightPass.SetRenderTarget(this._quarterVolumeLightTexture);
		}
		else if (this.Resolution == VolumetricLightRenderer.VolumtericResolution.Half)
		{
			Texture source2 = null;
			this._preLightPass.Blit(source2, this._halfDepthBuffer, this._bilateralBlurMaterial, (!flag) ? 10 : 4);
			this._preLightPass.SetRenderTarget(this._halfVolumeLightTexture);
		}
		else
		{
			this._preLightPass.SetRenderTarget(this._volumeLightTexture);
		}
		this._preLightPass.ClearRenderTarget(false, true, new Color(0f, 0f, 0f, 1f));
		this.UpdateMaterialParameters();
		if (VolumetricLightRenderer.PreRenderEvent != null)
		{
			VolumetricLightRenderer.PreRenderEvent(this, this._viewProj);
		}
	}

	[ImageEffectOpaque]
	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (this.Resolution == VolumetricLightRenderer.VolumtericResolution.Quarter)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(this._quarterDepthBuffer.width, this._quarterDepthBuffer.height, 0, RenderTextureFormat.ARGBHalf);
			temporary.filterMode = FilterMode.Bilinear;
			Graphics.Blit(this._quarterVolumeLightTexture, temporary, this._bilateralBlurMaterial, 8);
			Graphics.Blit(temporary, this._quarterVolumeLightTexture, this._bilateralBlurMaterial, 9);
			Graphics.Blit(this._quarterVolumeLightTexture, this._volumeLightTexture, this._bilateralBlurMaterial, 7);
			RenderTexture.ReleaseTemporary(temporary);
		}
		else if (this.Resolution == VolumetricLightRenderer.VolumtericResolution.Half)
		{
			RenderTexture temporary2 = RenderTexture.GetTemporary(this._halfVolumeLightTexture.width, this._halfVolumeLightTexture.height, 0, RenderTextureFormat.ARGBHalf);
			temporary2.filterMode = FilterMode.Bilinear;
			Graphics.Blit(this._halfVolumeLightTexture, temporary2, this._bilateralBlurMaterial, 2);
			Graphics.Blit(temporary2, this._halfVolumeLightTexture, this._bilateralBlurMaterial, 3);
			Graphics.Blit(this._halfVolumeLightTexture, this._volumeLightTexture, this._bilateralBlurMaterial, 5);
			RenderTexture.ReleaseTemporary(temporary2);
		}
		else
		{
			RenderTexture temporary3 = RenderTexture.GetTemporary(this._volumeLightTexture.width, this._volumeLightTexture.height, 0, RenderTextureFormat.ARGBHalf);
			temporary3.filterMode = FilterMode.Bilinear;
			Graphics.Blit(this._volumeLightTexture, temporary3, this._bilateralBlurMaterial, 0);
			Graphics.Blit(temporary3, this._volumeLightTexture, this._bilateralBlurMaterial, 1);
			RenderTexture.ReleaseTemporary(temporary3);
		}
		this._blitAddMaterial.SetTexture("_Source", source);
		Graphics.Blit(this._volumeLightTexture, destination, this._blitAddMaterial, 0);
	}

	private void UpdateMaterialParameters()
	{
		this._bilateralBlurMaterial.SetTexture("_HalfResDepthBuffer", this._halfDepthBuffer);
		this._bilateralBlurMaterial.SetTexture("_HalfResColor", this._halfVolumeLightTexture);
		this._bilateralBlurMaterial.SetTexture("_QuarterResDepthBuffer", this._quarterDepthBuffer);
		this._bilateralBlurMaterial.SetTexture("_QuarterResColor", this._quarterVolumeLightTexture);
		Shader.SetGlobalTexture("_DitherTexture", this._ditheringTexture);
		Shader.SetGlobalTexture("_NoiseTexture", this._noiseTexture);
	}

	private void Update()
	{
		if (this._currentResolution != this.Resolution)
		{
			this._currentResolution = this.Resolution;
			this.ChangeResolution();
		}
		if (this._volumeLightTexture.width != this._camera.pixelWidth || this._volumeLightTexture.height != this._camera.pixelHeight)
		{
			this.ChangeResolution();
		}
	}

	private void LoadNoise3dTexture()
	{
		TextAsset textAsset = Resources.Load("NoiseVolume") as TextAsset;
		byte[] bytes = textAsset.bytes;
		uint num = BitConverter.ToUInt32(textAsset.bytes, 12);
		uint num2 = BitConverter.ToUInt32(textAsset.bytes, 16);
		uint num3 = BitConverter.ToUInt32(textAsset.bytes, 20);
		uint num4 = BitConverter.ToUInt32(textAsset.bytes, 24);
		uint num5 = BitConverter.ToUInt32(textAsset.bytes, 80);
		uint num6 = BitConverter.ToUInt32(textAsset.bytes, 88);
		if (num6 == 0u)
		{
			num6 = num3 / num2 * 8u;
		}
		this._noiseTexture = new Texture3D((int)num2, (int)num, (int)num4, TextureFormat.RGBA32, false);
		this._noiseTexture.name = "3D Noise";
		Color[] array = new Color[num2 * num * num4];
		uint num7 = 128u;
		if (textAsset.bytes[84] == 68 && textAsset.bytes[85] == 88 && textAsset.bytes[86] == 49 && textAsset.bytes[87] == 48 && (num5 & 4u) != 0u)
		{
			uint num8 = BitConverter.ToUInt32(textAsset.bytes, (int)num7);
			if (num8 >= 60u && num8 <= 65u)
			{
				num6 = 8u;
			}
			else if (num8 >= 48u && num8 <= 52u)
			{
				num6 = 16u;
			}
			else if (num8 >= 27u && num8 <= 32u)
			{
				num6 = 32u;
			}
			num7 += 20u;
		}
		uint num9 = num6 / 8u;
		num3 = (num2 * num6 + 7u) / 8u;
		int num10 = 0;
		while ((long)num10 < (long)((ulong)num4))
		{
			int num11 = 0;
			while ((long)num11 < (long)((ulong)num))
			{
				int num12 = 0;
				while ((long)num12 < (long)((ulong)num2))
				{
					checked
					{
						float num13 = (float)bytes[(int)((IntPtr)(unchecked((ulong)num7 + (ulong)((long)num12 * (long)((ulong)num9)))))] / 255f;
						array[(int)((IntPtr)(unchecked((long)num12 + (long)num11 * (long)((ulong)num2) + (long)num10 * (long)((ulong)num2) * (long)((ulong)num))))] = new Color(num13, num13, num13, num13);
					}
					num12++;
				}
				num7 += num3;
				num11++;
			}
			num10++;
		}
		this._noiseTexture.SetPixels(array);
		this._noiseTexture.Apply();
	}

	private void GenerateDitherTexture()
	{
		if (this._ditheringTexture != null)
		{
			return;
		}
		int num = 8;
		this._ditheringTexture = new Texture2D(num, num, TextureFormat.Alpha8, false, true);
		this._ditheringTexture.filterMode = FilterMode.Point;
		Color32[] array = new Color32[num * num];
		int num2 = 0;
		byte b = 3;
		array[num2++] = new Color32(b, b, b, b);
		b = 192;
		array[num2++] = new Color32(b, b, b, b);
		b = 51;
		array[num2++] = new Color32(b, b, b, b);
		b = 239;
		array[num2++] = new Color32(b, b, b, b);
		b = 15;
		array[num2++] = new Color32(b, b, b, b);
		b = 204;
		array[num2++] = new Color32(b, b, b, b);
		b = 62;
		array[num2++] = new Color32(b, b, b, b);
		b = 251;
		array[num2++] = new Color32(b, b, b, b);
		b = 129;
		array[num2++] = new Color32(b, b, b, b);
		b = 66;
		array[num2++] = new Color32(b, b, b, b);
		b = 176;
		array[num2++] = new Color32(b, b, b, b);
		b = 113;
		array[num2++] = new Color32(b, b, b, b);
		b = 141;
		array[num2++] = new Color32(b, b, b, b);
		b = 78;
		array[num2++] = new Color32(b, b, b, b);
		b = 188;
		array[num2++] = new Color32(b, b, b, b);
		b = 125;
		array[num2++] = new Color32(b, b, b, b);
		b = 35;
		array[num2++] = new Color32(b, b, b, b);
		b = 223;
		array[num2++] = new Color32(b, b, b, b);
		b = 19;
		array[num2++] = new Color32(b, b, b, b);
		b = 207;
		array[num2++] = new Color32(b, b, b, b);
		b = 47;
		array[num2++] = new Color32(b, b, b, b);
		b = 235;
		array[num2++] = new Color32(b, b, b, b);
		b = 31;
		array[num2++] = new Color32(b, b, b, b);
		b = 219;
		array[num2++] = new Color32(b, b, b, b);
		b = 160;
		array[num2++] = new Color32(b, b, b, b);
		b = 98;
		array[num2++] = new Color32(b, b, b, b);
		b = 145;
		array[num2++] = new Color32(b, b, b, b);
		b = 82;
		array[num2++] = new Color32(b, b, b, b);
		b = 172;
		array[num2++] = new Color32(b, b, b, b);
		b = 109;
		array[num2++] = new Color32(b, b, b, b);
		b = 156;
		array[num2++] = new Color32(b, b, b, b);
		b = 94;
		array[num2++] = new Color32(b, b, b, b);
		b = 11;
		array[num2++] = new Color32(b, b, b, b);
		b = 200;
		array[num2++] = new Color32(b, b, b, b);
		b = 58;
		array[num2++] = new Color32(b, b, b, b);
		b = 247;
		array[num2++] = new Color32(b, b, b, b);
		b = 7;
		array[num2++] = new Color32(b, b, b, b);
		b = 196;
		array[num2++] = new Color32(b, b, b, b);
		b = 54;
		array[num2++] = new Color32(b, b, b, b);
		b = 243;
		array[num2++] = new Color32(b, b, b, b);
		b = 137;
		array[num2++] = new Color32(b, b, b, b);
		b = 74;
		array[num2++] = new Color32(b, b, b, b);
		b = 184;
		array[num2++] = new Color32(b, b, b, b);
		b = 121;
		array[num2++] = new Color32(b, b, b, b);
		b = 133;
		array[num2++] = new Color32(b, b, b, b);
		b = 70;
		array[num2++] = new Color32(b, b, b, b);
		b = 180;
		array[num2++] = new Color32(b, b, b, b);
		b = 117;
		array[num2++] = new Color32(b, b, b, b);
		b = 43;
		array[num2++] = new Color32(b, b, b, b);
		b = 231;
		array[num2++] = new Color32(b, b, b, b);
		b = 27;
		array[num2++] = new Color32(b, b, b, b);
		b = 215;
		array[num2++] = new Color32(b, b, b, b);
		b = 39;
		array[num2++] = new Color32(b, b, b, b);
		b = 227;
		array[num2++] = new Color32(b, b, b, b);
		b = 23;
		array[num2++] = new Color32(b, b, b, b);
		b = 211;
		array[num2++] = new Color32(b, b, b, b);
		b = 168;
		array[num2++] = new Color32(b, b, b, b);
		b = 105;
		array[num2++] = new Color32(b, b, b, b);
		b = 153;
		array[num2++] = new Color32(b, b, b, b);
		b = 90;
		array[num2++] = new Color32(b, b, b, b);
		b = 164;
		array[num2++] = new Color32(b, b, b, b);
		b = 102;
		array[num2++] = new Color32(b, b, b, b);
		b = 149;
		array[num2++] = new Color32(b, b, b, b);
		b = 86;
		array[num2++] = new Color32(b, b, b, b);
		this._ditheringTexture.SetPixels32(array);
		this._ditheringTexture.Apply();
	}

	private Mesh CreateSpotLightMesh()
	{
		Mesh mesh = new Mesh();
		Vector3[] array = new Vector3[50];
		Color32[] array2 = new Color32[50];
		array[0] = new Vector3(0f, 0f, 0f);
		array[1] = new Vector3(0f, 0f, 1f);
		float num = 0f;
		float num2 = 0.3926991f;
		float num3 = 0.9f;
		for (int i = 0; i < 16; i++)
		{
			array[i + 2] = new Vector3(-Mathf.Cos(num) * num3, Mathf.Sin(num) * num3, num3);
			array2[i + 2] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			array[i + 2 + 16] = new Vector3(-Mathf.Cos(num), Mathf.Sin(num), 1f);
			array2[i + 2 + 16] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);
			array[i + 2 + 32] = new Vector3(-Mathf.Cos(num) * num3, Mathf.Sin(num) * num3, 1f);
			array2[i + 2 + 32] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			num += num2;
		}
		mesh.vertices = array;
		mesh.colors32 = array2;
		int[] array3 = new int[288];
		int num4 = 0;
		for (int j = 2; j < 17; j++)
		{
			array3[num4++] = 0;
			array3[num4++] = j;
			array3[num4++] = j + 1;
		}
		array3[num4++] = 0;
		array3[num4++] = 17;
		array3[num4++] = 2;
		for (int k = 2; k < 17; k++)
		{
			array3[num4++] = k;
			array3[num4++] = k + 16;
			array3[num4++] = k + 1;
			array3[num4++] = k + 1;
			array3[num4++] = k + 16;
			array3[num4++] = k + 16 + 1;
		}
		array3[num4++] = 2;
		array3[num4++] = 17;
		array3[num4++] = 18;
		array3[num4++] = 18;
		array3[num4++] = 17;
		array3[num4++] = 33;
		for (int l = 18; l < 33; l++)
		{
			array3[num4++] = l;
			array3[num4++] = l + 16;
			array3[num4++] = l + 1;
			array3[num4++] = l + 1;
			array3[num4++] = l + 16;
			array3[num4++] = l + 16 + 1;
		}
		array3[num4++] = 18;
		array3[num4++] = 33;
		array3[num4++] = 34;
		array3[num4++] = 34;
		array3[num4++] = 33;
		array3[num4++] = 49;
		for (int m = 34; m < 49; m++)
		{
			array3[num4++] = 1;
			array3[num4++] = m + 1;
			array3[num4++] = m;
		}
		array3[num4++] = 1;
		array3[num4++] = 34;
		array3[num4++] = 49;
		mesh.triangles = array3;
		mesh.RecalculateBounds();
		return mesh;
	}

	[CompilerGenerated]
	private static Action<VolumetricLightRenderer, Matrix4x4> PreRenderEvent;

	private static Mesh _pointLightMesh;

	private static Mesh _spotLightMesh;

	private static Material _lightMaterial;

	private Camera _camera;

	private CommandBuffer _preLightPass;

	private Matrix4x4 _viewProj;

	private Material _blitAddMaterial;

	private Material _bilateralBlurMaterial;

	private RenderTexture _volumeLightTexture;

	private RenderTexture _halfVolumeLightTexture;

	private RenderTexture _quarterVolumeLightTexture;

	private static Texture _defaultSpotCookie;

	private RenderTexture _halfDepthBuffer;

	private RenderTexture _quarterDepthBuffer;

	private VolumetricLightRenderer.VolumtericResolution _currentResolution;

	private Texture2D _ditheringTexture;

	private Texture3D _noiseTexture;

	public VolumetricLightRenderer.VolumtericResolution Resolution;

	public Texture DefaultSpotCookie;

	public enum VolumtericResolution
	{
		Full,
		Half,
		Quarter
	}
}
