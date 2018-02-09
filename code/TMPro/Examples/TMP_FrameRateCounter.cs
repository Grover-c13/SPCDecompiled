using System;
using UnityEngine;

namespace TMPro.Examples
{
	public class TMP_FrameRateCounter : MonoBehaviour
	{
		private void Awake()
		{
			if (!base.enabled)
			{
				return;
			}
			this.m_camera = Camera.main;
			Application.targetFrameRate = -1;
			GameObject gameObject = new GameObject("Frame Counter");
			this.m_TextMeshPro = gameObject.AddComponent<TextMeshPro>();
			this.m_TextMeshPro.font = (Resources.Load("Fonts & Materials/LiberationSans SDF", typeof(TMP_FontAsset)) as TMP_FontAsset);
			this.m_TextMeshPro.fontSharedMaterial = (Resources.Load("Fonts & Materials/LiberationSans SDF - Overlay", typeof(Material)) as Material);
			this.m_frameCounter_transform = gameObject.transform;
			this.m_frameCounter_transform.SetParent(this.m_camera.transform);
			this.m_frameCounter_transform.localRotation = Quaternion.identity;
			this.m_TextMeshPro.enableWordWrapping = false;
			this.m_TextMeshPro.fontSize = 24f;
			this.m_TextMeshPro.isOverlay = true;
			this.Set_FrameCounter_Position(this.AnchorPosition);
			this.last_AnchorPosition = this.AnchorPosition;
		}

		private void Start()
		{
			this.m_LastInterval = Time.realtimeSinceStartup;
			this.m_Frames = 0;
		}

		private void Update()
		{
			if (this.AnchorPosition != this.last_AnchorPosition)
			{
				this.Set_FrameCounter_Position(this.AnchorPosition);
			}
			this.last_AnchorPosition = this.AnchorPosition;
			this.m_Frames++;
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			if (realtimeSinceStartup > this.m_LastInterval + this.UpdateInterval)
			{
				float num = (float)this.m_Frames / (realtimeSinceStartup - this.m_LastInterval);
				float arg = 1000f / Mathf.Max(num, 1E-05f);
				if (num < 30f)
				{
					this.htmlColorTag = "<color=yellow>";
				}
				else if (num < 10f)
				{
					this.htmlColorTag = "<color=red>";
				}
				else
				{
					this.htmlColorTag = "<color=green>";
				}
				this.m_TextMeshPro.SetText(this.htmlColorTag + "{0:2}</color> FPS \n{1:2} <#8080ff>MS", num, arg);
				this.m_Frames = 0;
				this.m_LastInterval = realtimeSinceStartup;
			}
		}

		private void Set_FrameCounter_Position(TMP_FrameRateCounter.FpsCounterAnchorPositions anchor_position)
		{
			this.m_TextMeshPro.margin = new Vector4(1f, 1f, 1f, 1f);
			switch (anchor_position)
			{
			case TMP_FrameRateCounter.FpsCounterAnchorPositions.TopLeft:
				this.m_TextMeshPro.alignment = TextAlignmentOptions.TopLeft;
				this.m_TextMeshPro.rectTransform.pivot = new Vector2(0f, 1f);
				this.m_frameCounter_transform.position = this.m_camera.ViewportToWorldPoint(new Vector3(0f, 1f, 100f));
				break;
			case TMP_FrameRateCounter.FpsCounterAnchorPositions.BottomLeft:
				this.m_TextMeshPro.alignment = TextAlignmentOptions.BottomLeft;
				this.m_TextMeshPro.rectTransform.pivot = new Vector2(0f, 0f);
				this.m_frameCounter_transform.position = this.m_camera.ViewportToWorldPoint(new Vector3(0f, 0f, 100f));
				break;
			case TMP_FrameRateCounter.FpsCounterAnchorPositions.TopRight:
				this.m_TextMeshPro.alignment = TextAlignmentOptions.TopRight;
				this.m_TextMeshPro.rectTransform.pivot = new Vector2(1f, 1f);
				this.m_frameCounter_transform.position = this.m_camera.ViewportToWorldPoint(new Vector3(1f, 1f, 100f));
				break;
			case TMP_FrameRateCounter.FpsCounterAnchorPositions.BottomRight:
				this.m_TextMeshPro.alignment = TextAlignmentOptions.BottomRight;
				this.m_TextMeshPro.rectTransform.pivot = new Vector2(1f, 0f);
				this.m_frameCounter_transform.position = this.m_camera.ViewportToWorldPoint(new Vector3(1f, 0f, 100f));
				break;
			}
		}

		public float UpdateInterval = 5f;

		private float m_LastInterval;

		private int m_Frames;

		public TMP_FrameRateCounter.FpsCounterAnchorPositions AnchorPosition = TMP_FrameRateCounter.FpsCounterAnchorPositions.TopRight;

		private string htmlColorTag;

		private const string fpsLabel = "{0:2}</color> FPS \n{1:2} <#8080ff>MS";

		private TextMeshPro m_TextMeshPro;

		private Transform m_frameCounter_transform;

		private Camera m_camera;

		private TMP_FrameRateCounter.FpsCounterAnchorPositions last_AnchorPosition;

		public enum FpsCounterAnchorPositions
		{
			TopLeft,
			BottomLeft,
			TopRight,
			BottomRight
		}
	}
}
