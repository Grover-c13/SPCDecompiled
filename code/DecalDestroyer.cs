using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DecalDestroyer : MonoBehaviour
{
	public DecalDestroyer()
	{
	}

	private IEnumerator Start()
	{
		yield return new WaitForSeconds(this.lifeTime);
		UnityEngine.Object.Destroy(base.gameObject);
		yield break;
	}

	public float lifeTime = 5f;

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
				this.$current = new WaitForSeconds(this.$this.lifeTime);
				if (!this.$disposing)
				{
					this.$PC = 1;
				}
				return true;
			case 1u:
				UnityEngine.Object.Destroy(this.$this.gameObject);
				this.$PC = -1;
				break;
			}
			return false;
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

		internal DecalDestroyer $this;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}
}
