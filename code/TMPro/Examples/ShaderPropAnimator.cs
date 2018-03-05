using System;
using System.Collections;
using UnityEngine;

namespace TMPro.Examples
{
	public class ShaderPropAnimator : MonoBehaviour
	{
		public ShaderPropAnimator()
		{
		}

		private void Awake()
		{
			this.m_Renderer = base.GetComponent<Renderer>();
			this.m_Material = this.m_Renderer.material;
		}

		private void Start()
		{
			base.StartCoroutine(this.AnimateProperties());
		}

		private IEnumerator AnimateProperties()
		{
			this.m_frame = UnityEngine.Random.Range(0f, 1f);
			for (;;)
			{
				float glowPower = this.GlowCurve.Evaluate(this.m_frame);
				this.m_Material.SetFloat(ShaderUtilities.ID_GlowPower, glowPower);
				this.m_frame += Time.deltaTime * UnityEngine.Random.Range(0.2f, 0.3f);
				yield return new WaitForEndOfFrame();
			}
			yield break;
		}

		private Renderer m_Renderer;

		private Material m_Material;

		public AnimationCurve GlowCurve;

		public float m_frame;
	}
}
