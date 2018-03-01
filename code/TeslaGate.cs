using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

public class TeslaGate : NetworkBehaviour
{
	public TeslaGate()
	{
	}

	public void Hack()
	{
		this.hack = true;
	}

	private void Update()
	{
		base.transform.localPosition = new Vector3(0f, 1.91f, 5.64f);
		base.transform.localRotation = Quaternion.Euler(new Vector3(0f, -180f, 0f));
	}

	private void Start()
	{
		base.StartCoroutine(this.IUpdate());
	}

	private IEnumerator IUpdate()
	{
		for (;;)
		{
			if (this.hack)
			{
				this.hack = false;
				this.source.PlayOneShot(this.hacksound);
				yield return new WaitForSeconds(0.2f);
				this.effect.SetActive(true);
				foreach (LightBlink lightBlink in this.blinkers)
				{
					lightBlink.disabled = false;
				}
				yield return new WaitForSeconds(4.8f);
				this.effect.SetActive(false);
				foreach (LightBlink lightBlink2 in this.blinkers)
				{
					lightBlink2.disabled = true;
				}
				yield return new WaitForSeconds(5f);
				this.trigger = false;
			}
			else if (this.trigger)
			{
				this.source.PlayOneShot(this.windup);
				yield return new WaitForSeconds(0.4f);
				this.effect.SetActive(true);
				foreach (LightBlink lightBlink3 in this.blinkers)
				{
					lightBlink3.disabled = false;
				}
				this.source.PlayOneShot(this.shock);
				yield return new WaitForSeconds(0.6f);
				this.effect.SetActive(false);
				foreach (LightBlink lightBlink4 in this.blinkers)
				{
					lightBlink4.disabled = true;
				}
				this.trigger = false;
				float f = 0f;
				while (f < 0.67f && !this.hack)
				{
					f += Time.deltaTime;
					yield return new WaitForEndOfFrame();
				}
			}
			yield return new WaitForEndOfFrame();
		}
		yield break;
	}

	public void Trigger(bool isKiller, GameObject other)
	{
		if (isKiller)
		{
			if (other.GetComponentInParent<NetworkIdentity>().isLocalPlayer)
			{
				other.GetComponent<PlayerStats>().CallCmdSelfDeduct(new PlayerStats.HitInfo((float)UnityEngine.Random.Range(700, 900), "WORLD", "TESLA"));
			}
		}
		else
		{
			this.trigger = true;
		}
	}

	private void UNetVersion()
	{
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		bool result;
		return result;
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
	}

	public AudioClip windup;

	public AudioClip shock;

	public AudioClip hacksound;

	public GameObject effect;

	public AudioSource source;

	public LightBlink[] blinkers;

	private float lightIntensity;

	private bool trigger;

	private bool hack;

	[CompilerGenerated]
	private sealed class <IUpdate>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <IUpdate>c__Iterator0()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this.$PC;
			this.$PC = -1;
			switch (num)
			{
			case 0u:
				break;
			case 1u:
				this.$this.effect.SetActive(true);
				this.$locvar0 = this.$this.blinkers;
				this.$locvar1 = 0;
				while (this.$locvar1 < this.$locvar0.Length)
				{
					LightBlink lightBlink = this.$locvar0[this.$locvar1];
					lightBlink.disabled = false;
					this.$locvar1++;
				}
				this.$current = new WaitForSeconds(4.8f);
				if (!this.$disposing)
				{
					this.$PC = 2;
				}
				return true;
			case 2u:
				this.$this.effect.SetActive(false);
				this.$locvar2 = this.$this.blinkers;
				this.$locvar3 = 0;
				while (this.$locvar3 < this.$locvar2.Length)
				{
					LightBlink lightBlink2 = this.$locvar2[this.$locvar3];
					lightBlink2.disabled = true;
					this.$locvar3++;
				}
				this.$current = new WaitForSeconds(5f);
				if (!this.$disposing)
				{
					this.$PC = 3;
				}
				return true;
			case 3u:
				this.$this.trigger = false;
				goto IL_37A;
			case 4u:
				this.$this.effect.SetActive(true);
				this.$locvar4 = this.$this.blinkers;
				this.$locvar5 = 0;
				while (this.$locvar5 < this.$locvar4.Length)
				{
					LightBlink lightBlink3 = this.$locvar4[this.$locvar5];
					lightBlink3.disabled = false;
					this.$locvar5++;
				}
				this.$this.source.PlayOneShot(this.$this.shock);
				this.$current = new WaitForSeconds(0.6f);
				if (!this.$disposing)
				{
					this.$PC = 5;
				}
				return true;
			case 5u:
				this.$this.effect.SetActive(false);
				this.$locvar6 = this.$this.blinkers;
				this.$locvar7 = 0;
				while (this.$locvar7 < this.$locvar6.Length)
				{
					LightBlink lightBlink4 = this.$locvar6[this.$locvar7];
					lightBlink4.disabled = true;
					this.$locvar7++;
				}
				this.$this.trigger = false;
				this.<f>__1 = 0f;
				goto IL_35A;
			case 6u:
				goto IL_35A;
			case 7u:
				break;
			default:
				return false;
			}
			if (this.$this.hack)
			{
				this.$this.hack = false;
				this.$this.source.PlayOneShot(this.$this.hacksound);
				this.$current = new WaitForSeconds(0.2f);
				if (!this.$disposing)
				{
					this.$PC = 1;
				}
				return true;
			}
			if (this.$this.trigger)
			{
				this.$this.source.PlayOneShot(this.$this.windup);
				this.$current = new WaitForSeconds(0.4f);
				if (!this.$disposing)
				{
					this.$PC = 4;
				}
				return true;
			}
			goto IL_37A;
			IL_35A:
			if (this.<f>__1 < 0.67f && !this.$this.hack)
			{
				this.<f>__1 += Time.deltaTime;
				this.$current = new WaitForEndOfFrame();
				if (!this.$disposing)
				{
					this.$PC = 6;
				}
				return true;
			}
			IL_37A:
			this.$current = new WaitForEndOfFrame();
			if (!this.$disposing)
			{
				this.$PC = 7;
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

		internal LightBlink[] $locvar0;

		internal int $locvar1;

		internal LightBlink[] $locvar2;

		internal int $locvar3;

		internal LightBlink[] $locvar4;

		internal int $locvar5;

		internal LightBlink[] $locvar6;

		internal int $locvar7;

		internal float <f>__1;

		internal TeslaGate $this;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}
}
