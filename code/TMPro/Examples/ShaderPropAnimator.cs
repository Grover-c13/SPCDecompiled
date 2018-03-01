using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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

		[CompilerGenerated]
		private sealed class <AnimateProperties>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
		{
			[DebuggerHidden]
			public <AnimateProperties>c__Iterator0()
			{
			}

			public bool MoveNext()
			{
				uint num = (uint)this.$PC;
				this.$PC = -1;
				switch (num)
				{
				case 0u:
					this.$this.m_frame = UnityEngine.Random.Range(0f, 1f);
					break;
				case 1u:
					break;
				default:
					return false;
				}
				this.<glowPower>__1 = this.$this.GlowCurve.Evaluate(this.$this.m_frame);
				this.$this.m_Material.SetFloat(ShaderUtilities.ID_GlowPower, this.<glowPower>__1);
				this.$this.m_frame += Time.deltaTime * UnityEngine.Random.Range(0.2f, 0.3f);
				this.$current = new WaitForEndOfFrame();
				if (!this.$disposing)
				{
					this.$PC = 1;
				}
				return true;
			}

			object IEnumerator<object>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.$current;
				}
			}

			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.$current;
				}
			}

			[DebuggerHidden]
			public void Dispose()
			{
				this.$disposing = true;
				this.$PC = -1;
			}

			[DebuggerHidden]
			public void Reset()
			{
				throw new NotSupportedException();
			}

			internal float <glowPower>__1;

			internal ShaderPropAnimator $this;

			internal object $current;

			internal bool $disposing;

			internal int $PC;
		}
	}
}
