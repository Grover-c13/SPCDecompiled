using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

public class Scp096PlayerScript : NetworkBehaviour
{
	public Scp096PlayerScript()
	{
	}

	public void SetRage(bool b)
	{
		this.Networkrage = b;
	}

	[ServerCallback]
	[DebuggerHidden]
	private IEnumerator ExecuteServersideCode()
	{
		if (!NetworkServer.active)
		{
			return null;
		}
		Scp096PlayerScript.<ExecuteServersideCode>c__Iterator0 <ExecuteServersideCode>c__Iterator = new Scp096PlayerScript.<ExecuteServersideCode>c__Iterator0();
		<ExecuteServersideCode>c__Iterator.$this = this;
		return <ExecuteServersideCode>c__Iterator;
	}

	public void Init(int classID, Class c)
	{
		this.sameClass = (c.team == Team.SCP);
		this.iAm096 = (classID == 9);
		if (this.iAm096)
		{
			Scp096PlayerScript.instance = this;
		}
	}

	private void Start()
	{
		if (base.isLocalPlayer && base.isServer)
		{
			base.StartCoroutine(this.ExecuteServersideCode());
		}
	}

	private void UNetVersion()
	{
	}

	public bool Networkrage
	{
		get
		{
			return this.rage;
		}
		set
		{
			uint dirtyBit = 1u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SetRage(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<bool>(value, ref this.rage, dirtyBit);
		}
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			writer.Write(this.rage);
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
			writer.Write(this.rage);
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
			this.rage = reader.ReadBoolean();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			this.SetRage(reader.ReadBoolean());
		}
	}

	public float rageProgress;

	[SyncVar(hook = "SetRage")]
	public bool rage;

	public float rageSpeedMultiplier;

	public float rageDurationMultiplier;

	public float rageCooldownTime;

	public float rageBoostSpeed;

	public static Scp096PlayerScript instance;

	public bool sameClass;

	public bool iAm096;

	public LayerMask viewMask;

	public AnimationCurve viewAngleTolerance;

	[CompilerGenerated]
	private sealed class <ExecuteServersideCode>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <ExecuteServersideCode>c__Iterator0()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this.$PC;
			this.$PC = -1;
			switch (num)
			{
			case 0u:
				this.<playerCamera>__0 = this.$this.GetComponent<Scp049PlayerScript>().plyCam.transform;
				break;
			case 1u:
				this.$locvar1++;
				goto IL_1D7;
			case 2u:
				IL_2EF:
				this.$current = new WaitForFixedUpdate();
				if (!this.$disposing)
				{
					this.$PC = 3;
				}
				return true;
			case 3u:
				break;
			case 4u:
				break;
			default:
				return false;
			}
			IL_48:
			if (!(Scp096PlayerScript.instance != null) || !Scp096PlayerScript.instance.iAm096)
			{
				this.$current = new WaitForSeconds(5f);
				if (!this.$disposing)
				{
					this.$PC = 4;
				}
				return true;
			}
			this.<playersLooking>__1 = 0;
			this.$locvar0 = PlayerManager.singleton.players;
			this.$locvar1 = 0;
			IL_1D7:
			if (this.$locvar1 < this.$locvar0.Length)
			{
				this.<item>__2 = this.$locvar0[this.$locvar1];
				RaycastHit raycastHit;
				if (this.<item>__2 != null && Vector3.Dot(this.<playerCamera>__0.forward, (this.<playerCamera>__0.position - Scp096PlayerScript.instance.transform.position).normalized) < -this.$this.viewAngleTolerance.Evaluate(Vector3.Distance(this.<playerCamera>__0.position, Scp096PlayerScript.instance.transform.position)) && Physics.Raycast(this.<playerCamera>__0.transform.position, (Scp096PlayerScript.instance.transform.position - this.<playerCamera>__0.transform.position).normalized, out raycastHit, 20f, this.$this.viewMask) && raycastHit.collider.transform.root.GetComponent<Scp096PlayerScript>() == Scp096PlayerScript.instance)
				{
					this.<playersLooking>__1++;
				}
				this.$current = new WaitForFixedUpdate();
				if (!this.$disposing)
				{
					this.$PC = 1;
				}
				return true;
			}
			this.$this.rageProgress += 0.02f * (float)((!this.$this.rage) ? this.<playersLooking>__1 : 1) * (float)PlayerManager.singleton.players.Length * ((!this.$this.rage) ? this.$this.rageSpeedMultiplier : (-this.$this.rageDurationMultiplier));
			this.$this.rageProgress = Mathf.Clamp01(this.$this.rageProgress);
			if (this.$this.rageProgress == 1f)
			{
				Scp096PlayerScript.instance.SetRage(true);
			}
			if (this.$this.rageProgress == 0f && this.$this.rage)
			{
				Scp096PlayerScript.instance.SetRage(true);
				this.$current = new WaitForSeconds(this.$this.rageCooldownTime);
				if (!this.$disposing)
				{
					this.$PC = 2;
				}
				return true;
			}
			goto IL_2EF;
			goto IL_48;
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

		internal Transform <playerCamera>__0;

		internal int <playersLooking>__1;

		internal GameObject[] $locvar0;

		internal int $locvar1;

		internal GameObject <item>__2;

		internal Scp096PlayerScript $this;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}
}
