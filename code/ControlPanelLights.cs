using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ControlPanelLights : MonoBehaviour
{
	public ControlPanelLights()
	{
	}

	private void Start()
	{
		base.StartCoroutine(this.Animate());
	}

	private IEnumerator Animate()
	{
		int i = this.emissions.Length;
		for (;;)
		{
			this.targetMat.SetTexture("_EmissionMap", this.emissions[UnityEngine.Random.Range(0, i)]);
			yield return new WaitForSeconds(UnityEngine.Random.Range(0.2f, 0.8f));
		}
		yield break;
	}

	public Texture[] emissions;

	public Material targetMat;

	[CompilerGenerated]
	private sealed class <Animate>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <Animate>c__Iterator0()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this.$PC;
			this.$PC = -1;
			switch (num)
			{
			case 0u:
				this.<l>__0 = this.$this.emissions.Length;
				break;
			case 1u:
				break;
			default:
				return false;
			}
			this.$this.targetMat.SetTexture("_EmissionMap", this.$this.emissions[UnityEngine.Random.Range(0, this.<l>__0)]);
			this.$current = new WaitForSeconds(UnityEngine.Random.Range(0.2f, 0.8f));
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

		internal int <l>__0;

		internal ControlPanelLights $this;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}
}
