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

	private void SetRage(Scp096PlayerScript.RageState b)
	{
		this.Networkenraged = b;
	}

	public void IncreaseRage(float amount)
	{
		if (this.enraged != Scp096PlayerScript.RageState.NotEnraged)
		{
			return;
		}
		this.rageProgress += amount;
		this.rageProgress = Mathf.Clamp01(this.rageProgress);
		if (this.rageProgress == 1f)
		{
			this.SetRage(Scp096PlayerScript.RageState.Panic);
			base.Invoke("StartRage", 5f);
		}
	}

	private void StartRage()
	{
		this.SetRage(Scp096PlayerScript.RageState.Enraged);
	}

	private void Update()
	{
		this.ExecuteClientsideCode();
		this.Animator();
		this.UpdateAudios();
	}

	private void UpdateAudios()
	{
		for (int i = 0; i < this.tracks.Length; i++)
		{
			this.tracks[i].playing = (i == (int)this.enraged && this.iAm096);
			this.tracks[i].Update();
		}
	}

	private void Animator()
	{
		if (!base.isLocalPlayer && this.animationController.animator != null)
		{
			this.animationController.animator.SetBool("Rage", this.enraged == Scp096PlayerScript.RageState.Enraged || this.enraged == Scp096PlayerScript.RageState.Panic);
		}
	}

	private void ExecuteClientsideCode()
	{
		if (base.isLocalPlayer && this.iAm096)
		{
			this.fpc.m_WalkSpeed = (this.fpc.m_RunSpeed = this.normalSpeed * ((this.enraged != Scp096PlayerScript.RageState.Panic) ? ((this.enraged != Scp096PlayerScript.RageState.Enraged) ? 1f : 2.8f) : 0f));
			if (this.enraged == Scp096PlayerScript.RageState.Enraged && Input.GetButton("Fire1"))
			{
				this.Shoot();
			}
		}
	}

	public void DeductRage()
	{
		if (this.enraged != Scp096PlayerScript.RageState.Enraged)
		{
			return;
		}
		this.rageProgress -= Time.fixedDeltaTime * this.ragemultiplier_deduct;
		this.rageProgress = Mathf.Clamp01(this.rageProgress);
		if (this.rageProgress == 0f)
		{
			this.cooldown = this.ragemultiplier_coodownduration;
			this.SetRage(Scp096PlayerScript.RageState.Cooldown);
		}
	}

	public void DeductCooldown()
	{
		if (this.enraged != Scp096PlayerScript.RageState.Cooldown)
		{
			return;
		}
		this.cooldown -= Time.fixedDeltaTime;
		this.cooldown = Mathf.Clamp(this.cooldown, 0f, this.ragemultiplier_coodownduration);
		if (this.cooldown == 0f)
		{
			this.SetRage(Scp096PlayerScript.RageState.NotEnraged);
		}
	}

	[ServerCallback]
	[DebuggerHidden]
	private IEnumerator ExecuteServersideCode_Looking()
	{
		if (!NetworkServer.active)
		{
			return null;
		}
		Scp096PlayerScript.<ExecuteServersideCode_Looking>c__Iterator0 <ExecuteServersideCode_Looking>c__Iterator = new Scp096PlayerScript.<ExecuteServersideCode_Looking>c__Iterator0();
		<ExecuteServersideCode_Looking>c__Iterator.$this = this;
		return <ExecuteServersideCode_Looking>c__Iterator;
	}

	[ServerCallback]
	[DebuggerHidden]
	private IEnumerator ExecuteServersideCode_RageHandler()
	{
		if (!NetworkServer.active)
		{
			return null;
		}
		Scp096PlayerScript.<ExecuteServersideCode_RageHandler>c__Iterator1 <ExecuteServersideCode_RageHandler>c__Iterator = new Scp096PlayerScript.<ExecuteServersideCode_RageHandler>c__Iterator1();
		<ExecuteServersideCode_RageHandler>c__Iterator.$this = this;
		return <ExecuteServersideCode_RageHandler>c__Iterator;
	}

	private void Shoot()
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(this.camera.transform.position, this.camera.transform.forward, out raycastHit, 1.5f))
		{
			CharacterClassManager component = raycastHit.transform.GetComponent<CharacterClassManager>();
			if (component != null && component.klasy[component.curClass].team != Team.SCP)
			{
				Hitmarker.Hit(1f);
				this.CallCmdHurtPlayer(raycastHit.transform.gameObject);
			}
		}
	}

	[Command(channel = 2)]
	private void CmdHurtPlayer(GameObject target)
	{
		if (this.ccm.curClass == 9 && Vector3.Distance(base.GetComponent<PlyMovementSync>().position, target.transform.position) < 3f && this.enraged == Scp096PlayerScript.RageState.Enraged)
		{
			target.GetComponent<PlayerStats>().HurtPlayer(new PlayerStats.HitInfo(999999f, "SCP096", "SCP:096"), target);
		}
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
		this.animationController = base.GetComponent<AnimationController>();
		this.fpc = base.GetComponent<FirstPersonController>();
		this.ccm = base.GetComponent<CharacterClassManager>();
		this.normalSpeed = this.ccm.klasy[9].runSpeed;
		if (base.isLocalPlayer && base.isServer)
		{
			base.StartCoroutine(this.ExecuteServersideCode_Looking());
			base.StartCoroutine(this.ExecuteServersideCode_RageHandler());
		}
	}

	private void UNetVersion()
	{
	}

	public Scp096PlayerScript.RageState Networkenraged
	{
		get
		{
			return this.enraged;
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
			base.SetSyncVar<Scp096PlayerScript.RageState>(value, ref this.enraged, dirtyBit);
		}
	}

	protected static void InvokeCmdCmdHurtPlayer(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogError("Command CmdHurtPlayer called on client.");
			return;
		}
		((Scp096PlayerScript)obj).CmdHurtPlayer(reader.ReadGameObject());
	}

	public void CallCmdHurtPlayer(GameObject target)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("Command function CmdHurtPlayer called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdHurtPlayer(target);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)Scp096PlayerScript.kCmdCmdHurtPlayer);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(target);
		base.SendCommandInternal(networkWriter, 2, "CmdHurtPlayer");
	}

	static Scp096PlayerScript()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(Scp096PlayerScript), Scp096PlayerScript.kCmdCmdHurtPlayer, new NetworkBehaviour.CmdDelegate(Scp096PlayerScript.InvokeCmdCmdHurtPlayer));
		NetworkCRC.RegisterBehaviour("Scp096PlayerScript", 0);
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			writer.Write((int)this.enraged);
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
			writer.Write((int)this.enraged);
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
			this.enraged = (Scp096PlayerScript.RageState)reader.ReadInt32();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			this.SetRage((Scp096PlayerScript.RageState)reader.ReadInt32());
		}
	}

	public static Scp096PlayerScript instance;

	public GameObject camera;

	public bool sameClass;

	public bool iAm096;

	public LayerMask layerMask;

	private AnimationController animationController;

	private float cooldown;

	public SoundtrackManager.Track[] tracks;

	[SyncVar(hook = "SetRage")]
	[Space]
	public Scp096PlayerScript.RageState enraged;

	public float rageProgress;

	[Space]
	public float ragemultiplier_looking;

	[Space]
	public float ragemultiplier_deduct = 0.08f;

	public float ragemultiplier_coodownduration = 20f;

	public AnimationCurve lookingTolerance;

	private CharacterClassManager ccm;

	private FirstPersonController fpc;

	private float normalSpeed;

	private static int kCmdCmdHurtPlayer = 787420137;

	public enum RageState
	{
		NotEnraged,
		Panic,
		Enraged,
		Cooldown
	}

	[CompilerGenerated]
	private sealed class <ExecuteServersideCode_Looking>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <ExecuteServersideCode_Looking>c__Iterator0()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this.$PC;
			this.$PC = -1;
			switch (num)
			{
			case 0u:
				goto IL_249;
			case 1u:
				this.$locvar1++;
				break;
			case 2u:
				goto IL_249;
			default:
				return false;
			}
			IL_212:
			if (this.$locvar1 < this.$locvar0.Length)
			{
				this.<item>__2 = this.$locvar0[this.$locvar1];
				if (this.<item>__2 != null && this.<item>__2.GetComponent<CharacterClassManager>().IsHuman())
				{
					Transform transform = this.<item>__2.GetComponent<Scp096PlayerScript>().camera.transform;
					float num2 = this.$this.lookingTolerance.Evaluate(Vector3.Distance(transform.position, Scp096PlayerScript.instance.camera.transform.position));
					RaycastHit raycastHit;
					if (((double)num2 < 0.75 || Vector3.Dot(transform.forward, (transform.position - Scp096PlayerScript.instance.camera.transform.position).normalized) < -num2) && Physics.Raycast(transform.transform.position, (Scp096PlayerScript.instance.camera.transform.position - transform.position).normalized, out raycastHit, 20f, this.$this.layerMask) && raycastHit.collider.gameObject.layer == 24 && raycastHit.collider.GetComponentInParent<Scp096PlayerScript>() == Scp096PlayerScript.instance)
					{
						Scp096PlayerScript.instance.IncreaseRage(Time.fixedDeltaTime * this.$this.ragemultiplier_looking * (float)this.<plys>__1.Length);
					}
				}
				this.$current = new WaitForFixedUpdate();
				if (!this.$disposing)
				{
					this.$PC = 1;
				}
				return true;
			}
			IL_249:
			if (!this.$this.isServer)
			{
				this.$PC = -1;
			}
			else
			{
				if (Scp096PlayerScript.instance != null && Scp096PlayerScript.instance.iAm096)
				{
					this.<plys>__1 = PlayerManager.singleton.players;
					this.$locvar0 = this.<plys>__1;
					this.$locvar1 = 0;
					goto IL_212;
				}
				this.$current = new WaitForEndOfFrame();
				if (!this.$disposing)
				{
					this.$PC = 2;
				}
				return true;
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

		internal GameObject[] <plys>__1;

		internal GameObject[] $locvar0;

		internal int $locvar1;

		internal GameObject <item>__2;

		internal Scp096PlayerScript $this;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}

	[CompilerGenerated]
	private sealed class <ExecuteServersideCode_RageHandler>c__Iterator1 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <ExecuteServersideCode_RageHandler>c__Iterator1()
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
				break;
			default:
				return false;
			}
			if (this.$this.isServer)
			{
				if (Scp096PlayerScript.instance != null && Scp096PlayerScript.instance.iAm096)
				{
					if (Scp096PlayerScript.instance.enraged == Scp096PlayerScript.RageState.Enraged)
					{
						Scp096PlayerScript.instance.DeductRage();
					}
					if (Scp096PlayerScript.instance.enraged == Scp096PlayerScript.RageState.Cooldown)
					{
						Scp096PlayerScript.instance.DeductCooldown();
					}
				}
				this.$current = new WaitForFixedUpdate();
				if (!this.$disposing)
				{
					this.$PC = 1;
				}
				return true;
			}
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

		internal Scp096PlayerScript $this;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}
}
