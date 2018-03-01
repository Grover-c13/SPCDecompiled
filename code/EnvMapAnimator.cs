using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class EnvMapAnimator : MonoBehaviour
{
	public EnvMapAnimator()
	{
	}

	private void Awake()
	{
		this.m_textMeshPro = base.GetComponent<TMP_Text>();
		this.m_material = this.m_textMeshPro.fontSharedMaterial;
	}

	private IEnumerator Start()
	{
		Matrix4x4 matrix = default(Matrix4x4);
		for (;;)
		{
			matrix.SetTRS(Vector3.zero, Quaternion.Euler(Time.time * this.RotationSpeeds.x, Time.time * this.RotationSpeeds.y, Time.time * this.RotationSpeeds.z), Vector3.one);
			this.m_material.SetMatrix("_EnvMatrix", matrix);
			yield return null;
		}
		yield break;
	}

	public Vector3 RotationSpeeds;

	private TMP_Text m_textMeshPro;

	private Material m_material;

	[CompilerGenerated]
	private sealed class <Start>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <Start>c__Iterator0()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this.$PC;
			this.$PC = -1;
			switch (num)
			{
			case 0u:
				this.<matrix>__0 = default(Matrix4x4);
				break;
			case 1u:
				break;
			default:
				return false;
			}
			this.<matrix>__0.SetTRS(Vector3.zero, Quaternion.Euler(Time.time * this.$this.RotationSpeeds.x, Time.time * this.$this.RotationSpeeds.y, Time.time * this.$this.RotationSpeeds.z), Vector3.one);
			this.$this.m_material.SetMatrix("_EnvMatrix", this.<matrix>__0);
			this.$current = null;
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

		internal Matrix4x4 <matrix>__0;

		internal EnvMapAnimator $this;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}
}
