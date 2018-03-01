using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

public class LeverButton : NetworkBehaviour
{
	public LeverButton()
	{
	}

	private void SetSwitch(bool b)
	{
		this.NetworkdefaultState = b;
		this.SetupLights();
	}

	public void Switch()
	{
		if (this.cooldown <= 0f)
		{
			this.cooldown = 0.6f;
			this.lever.GetComponent<AudioSource>().Play();
			this.SetSwitch(!this.defaultState);
		}
	}

	private void SetupLights()
	{
		if (this.onLights.Length == 0 || this.offLights.Length == 0)
		{
			return;
		}
		for (int i = 0; i < this.onLights.Length; i++)
		{
			base.StartCoroutine(this.SetupLights(this.onLights[i], (!this.GetState()) ? 0f : this.intensity));
		}
		for (int j = 0; j < this.offLights.Length; j++)
		{
			base.StartCoroutine(this.SetupLights(this.offLights[j], this.GetState() ? 0f : this.intensity));
		}
	}

	private IEnumerator SetupLights(Light l, float targetIntens)
	{
		bool increase = l.intensity < targetIntens;
		if (increase)
		{
			while (l.intensity < targetIntens)
			{
				l.intensity += Time.deltaTime * 5f;
				yield return new WaitForEndOfFrame();
			}
		}
		else
		{
			while (l.intensity > 0f)
			{
				l.intensity -= Time.deltaTime * 5f;
				yield return new WaitForEndOfFrame();
			}
		}
		yield break;
	}

	public bool GetState()
	{
		return (this.orientation != LeverButton.LeverOrientation.OnIsDown) ? (!this.defaultState) : this.defaultState;
	}

	private void Update()
	{
		if (this.cooldown > 0f)
		{
			this.cooldown -= Time.deltaTime;
		}
		this.targetQuaternion = Quaternion.Euler((!this.defaultState) ? new Vector3(92f, 0f, 0f) : new Vector3(268f, 0f, 0f));
		this.lever.transform.localRotation = Quaternion.LerpUnclamped(this.lever.transform.localRotation, this.targetQuaternion, Time.deltaTime * 5f);
	}

	private void UNetVersion()
	{
	}

	public bool NetworkdefaultState
	{
		get
		{
			return this.defaultState;
		}
		set
		{
			uint dirtyBit = 1u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SetSwitch(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<bool>(value, ref this.defaultState, dirtyBit);
		}
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			writer.Write(this.defaultState);
			return true;
		}
		bool flag = false;
		if ((base.syncVarDirtyBits & 1u) != 0u)
		{
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
				flag = true;
			}
			writer.Write(this.defaultState);
		}
		if (!flag)
		{
			writer.WritePackedUInt32(base.syncVarDirtyBits);
		}
		return flag;
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
		if (initialState)
		{
			this.defaultState = reader.ReadBoolean();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			this.SetSwitch(reader.ReadBoolean());
		}
	}

	[SyncVar(hook = "SetSwitch")]
	public bool defaultState;

	public Transform lever;

	private Quaternion targetQuaternion;

	[SerializeField]
	private LeverButton.LeverOrientation orientation;

	private float cooldown;

	public Light[] onLights;

	public Light[] offLights;

	public float intensity = 1.4f;

	public enum LeverOrientation
	{
		OnIsDown,
		OnIsUp
	}

	[CompilerGenerated]
	private sealed class <SetupLights>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <SetupLights>c__Iterator0()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this.$PC;
			this.$PC = -1;
			switch (num)
			{
			case 0u:
				this.<increase>__0 = (this.l.intensity < this.targetIntens);
				if (!this.<increase>__0)
				{
					goto IL_E6;
				}
				break;
			case 1u:
				break;
			case 2u:
				goto IL_E6;
			default:
				return false;
			}
			if (this.l.intensity >= this.targetIntens)
			{
				goto IL_FB;
			}
			this.l.intensity += Time.deltaTime * 5f;
			this.$current = new WaitForEndOfFrame();
			if (!this.$disposing)
			{
				this.$PC = 1;
			}
			return true;
			IL_E6:
			if (this.l.intensity > 0f)
			{
				this.l.intensity -= Time.deltaTime * 5f;
				this.$current = new WaitForEndOfFrame();
				if (!this.$disposing)
				{
					this.$PC = 2;
				}
				return true;
			}
			IL_FB:
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

		internal Light l;

		internal float targetIntens;

		internal bool <increase>__0;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}
}
