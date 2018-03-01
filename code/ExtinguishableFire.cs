using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ExtinguishableFire : MonoBehaviour
{
	public ExtinguishableFire()
	{
	}

	private void Start()
	{
		this.m_isExtinguished = true;
		this.smokeParticleSystem.Stop();
		this.fireParticleSystem.Stop();
		base.StartCoroutine(this.StartingFire());
	}

	public void Extinguish()
	{
		if (this.m_isExtinguished)
		{
			return;
		}
		this.m_isExtinguished = true;
		base.StartCoroutine(this.Extinguishing());
	}

	private IEnumerator Extinguishing()
	{
		this.fireParticleSystem.Stop();
		this.smokeParticleSystem.time = 0f;
		this.smokeParticleSystem.Play();
		for (float elapsedTime = 0f; elapsedTime < 2f; elapsedTime += Time.deltaTime)
		{
			float ratio = Mathf.Max(0f, 1f - elapsedTime / 2f);
			this.fireParticleSystem.transform.localScale = Vector3.one * ratio;
			yield return null;
		}
		yield return new WaitForSeconds(2f);
		this.smokeParticleSystem.Stop();
		this.fireParticleSystem.transform.localScale = Vector3.one;
		yield return new WaitForSeconds(4f);
		base.StartCoroutine(this.StartingFire());
		yield break;
	}

	private IEnumerator StartingFire()
	{
		this.smokeParticleSystem.Stop();
		this.fireParticleSystem.time = 0f;
		this.fireParticleSystem.Play();
		for (float elapsedTime = 0f; elapsedTime < 2f; elapsedTime += Time.deltaTime)
		{
			float ratio = Mathf.Min(1f, elapsedTime / 2f);
			this.fireParticleSystem.transform.localScale = Vector3.one * ratio;
			yield return null;
		}
		this.fireParticleSystem.transform.localScale = Vector3.one;
		this.m_isExtinguished = false;
		yield break;
	}

	public ParticleSystem fireParticleSystem;

	public ParticleSystem smokeParticleSystem;

	protected bool m_isExtinguished;

	private const float m_FireStartingTime = 2f;

	[CompilerGenerated]
	private sealed class <Extinguishing>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <Extinguishing>c__Iterator0()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this.$PC;
			this.$PC = -1;
			switch (num)
			{
			case 0u:
				this.$this.fireParticleSystem.Stop();
				this.$this.smokeParticleSystem.time = 0f;
				this.$this.smokeParticleSystem.Play();
				this.<elapsedTime>__0 = 0f;
				break;
			case 1u:
				this.<elapsedTime>__0 += Time.deltaTime;
				break;
			case 2u:
				this.$this.smokeParticleSystem.Stop();
				this.$this.fireParticleSystem.transform.localScale = Vector3.one;
				this.$current = new WaitForSeconds(4f);
				if (!this.$disposing)
				{
					this.$PC = 3;
				}
				return true;
			case 3u:
				this.$this.StartCoroutine(this.$this.StartingFire());
				this.$PC = -1;
				return false;
			default:
				return false;
			}
			if (this.<elapsedTime>__0 >= 2f)
			{
				this.$current = new WaitForSeconds(2f);
				if (!this.$disposing)
				{
					this.$PC = 2;
				}
				return true;
			}
			this.<ratio>__1 = Mathf.Max(0f, 1f - this.<elapsedTime>__0 / 2f);
			this.$this.fireParticleSystem.transform.localScale = Vector3.one * this.<ratio>__1;
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

		internal float <elapsedTime>__0;

		internal float <ratio>__1;

		internal ExtinguishableFire $this;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}

	[CompilerGenerated]
	private sealed class <StartingFire>c__Iterator1 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <StartingFire>c__Iterator1()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this.$PC;
			this.$PC = -1;
			switch (num)
			{
			case 0u:
				this.$this.smokeParticleSystem.Stop();
				this.$this.fireParticleSystem.time = 0f;
				this.$this.fireParticleSystem.Play();
				this.<elapsedTime>__0 = 0f;
				break;
			case 1u:
				this.<elapsedTime>__0 += Time.deltaTime;
				break;
			default:
				return false;
			}
			if (this.<elapsedTime>__0 < 2f)
			{
				this.<ratio>__1 = Mathf.Min(1f, this.<elapsedTime>__0 / 2f);
				this.$this.fireParticleSystem.transform.localScale = Vector3.one * this.<ratio>__1;
				this.$current = null;
				if (!this.$disposing)
				{
					this.$PC = 1;
				}
				return true;
			}
			this.$this.fireParticleSystem.transform.localScale = Vector3.one;
			this.$this.m_isExtinguished = false;
			this.$PC = -1;
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

		internal float <elapsedTime>__0;

		internal float <ratio>__1;

		internal ExtinguishableFire $this;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}
}
