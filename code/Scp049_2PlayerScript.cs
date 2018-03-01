using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Dissonance.Integrations.UNet_HLAPI;
using UnityEngine;
using UnityEngine.Networking;

public class Scp049_2PlayerScript : NetworkBehaviour
{
	public Scp049_2PlayerScript()
	{
	}

	private void Start()
	{
		if (base.isLocalPlayer)
		{
			base.StartCoroutine(this.UpdateInput());
		}
	}

	public void Init(int classID, Class c)
	{
		this.sameClass = (c.team == Team.SCP);
		this.iAm049_2 = (classID == 10);
		this.animator.gameObject.SetActive(base.isLocalPlayer && this.iAm049_2);
	}

	private IEnumerator UpdateInput()
	{
		for (;;)
		{
			if (Input.GetButton("Fire1") && this.iAm049_2)
			{
				float mt = this.multiplier.Evaluate(base.GetComponent<PlayerStats>().GetHealthPercent());
				this.CallCmdShootAnim();
				this.animator.SetTrigger("Shoot");
				this.animator.speed = mt;
				yield return new WaitForSeconds(0.65f / mt);
				this.Attack();
				yield return new WaitForSeconds(1f / mt);
			}
			yield return new WaitForEndOfFrame();
		}
		yield break;
	}

	private void Attack()
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(this.plyCam.transform.position, this.plyCam.transform.forward, out raycastHit, this.distance))
		{
			Scp049_2PlayerScript scp049_2PlayerScript = raycastHit.transform.GetComponent<Scp049_2PlayerScript>();
			if (scp049_2PlayerScript == null)
			{
				scp049_2PlayerScript = raycastHit.transform.GetComponentInParent<Scp049_2PlayerScript>();
			}
			if (scp049_2PlayerScript != null && !scp049_2PlayerScript.sameClass)
			{
				Hitmarker.Hit(1f);
				this.CallCmdHurtPlayer(raycastHit.transform.gameObject, base.GetComponent<HlapiPlayer>().PlayerId);
			}
		}
	}

	[Command(channel = 2)]
	private void CmdHurtPlayer(GameObject ply, string id)
	{
		if (Vector3.Distance(base.GetComponent<PlyMovementSync>().position, ply.transform.position) <= this.distance * 1.3f && base.GetComponent<CharacterClassManager>().curClass == 10)
		{
			base.GetComponent<PlayerStats>().HurtPlayer(new PlayerStats.HitInfo((float)this.damage, id, "SCP:0492"), ply);
		}
	}

	[Command(channel = 1)]
	private void CmdShootAnim()
	{
		this.CallRpcShootAnim();
	}

	[ClientRpc]
	private void RpcShootAnim()
	{
		base.GetComponent<AnimationController>().DoAnimation("Shoot");
	}

	private void UNetVersion()
	{
	}

	protected static void InvokeCmdCmdHurtPlayer(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogError("Command CmdHurtPlayer called on client.");
			return;
		}
		((Scp049_2PlayerScript)obj).CmdHurtPlayer(reader.ReadGameObject(), reader.ReadString());
	}

	protected static void InvokeCmdCmdShootAnim(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogError("Command CmdShootAnim called on client.");
			return;
		}
		((Scp049_2PlayerScript)obj).CmdShootAnim();
	}

	public void CallCmdHurtPlayer(GameObject ply, string id)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("Command function CmdHurtPlayer called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdHurtPlayer(ply, id);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)Scp049_2PlayerScript.kCmdCmdHurtPlayer);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(ply);
		networkWriter.Write(id);
		base.SendCommandInternal(networkWriter, 2, "CmdHurtPlayer");
	}

	public void CallCmdShootAnim()
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("Command function CmdShootAnim called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdShootAnim();
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)Scp049_2PlayerScript.kCmdCmdShootAnim);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		base.SendCommandInternal(networkWriter, 1, "CmdShootAnim");
	}

	protected static void InvokeRpcRpcShootAnim(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("RPC RpcShootAnim called on server.");
			return;
		}
		((Scp049_2PlayerScript)obj).RpcShootAnim();
	}

	public void CallRpcShootAnim()
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogError("RPC Function RpcShootAnim called on client.");
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)2));
		networkWriter.WritePackedUInt32((uint)Scp049_2PlayerScript.kRpcRpcShootAnim);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		this.SendRPCInternal(networkWriter, 0, "RpcShootAnim");
	}

	static Scp049_2PlayerScript()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(Scp049_2PlayerScript), Scp049_2PlayerScript.kCmdCmdHurtPlayer, new NetworkBehaviour.CmdDelegate(Scp049_2PlayerScript.InvokeCmdCmdHurtPlayer));
		Scp049_2PlayerScript.kCmdCmdShootAnim = 1794565020;
		NetworkBehaviour.RegisterCommandDelegate(typeof(Scp049_2PlayerScript), Scp049_2PlayerScript.kCmdCmdShootAnim, new NetworkBehaviour.CmdDelegate(Scp049_2PlayerScript.InvokeCmdCmdShootAnim));
		Scp049_2PlayerScript.kRpcRpcShootAnim = 201633926;
		NetworkBehaviour.RegisterRpcDelegate(typeof(Scp049_2PlayerScript), Scp049_2PlayerScript.kRpcRpcShootAnim, new NetworkBehaviour.CmdDelegate(Scp049_2PlayerScript.InvokeRpcRpcShootAnim));
		NetworkCRC.RegisterBehaviour("Scp049_2PlayerScript", 0);
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		bool result;
		return result;
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
	}

	[Header("Player Properties")]
	public Camera plyCam;

	public Animator animator;

	public bool iAm049_2;

	public bool sameClass;

	[Header("Attack")]
	public float distance = 2.4f;

	public int damage = 60;

	[Header("Boosts")]
	public AnimationCurve multiplier;

	private static int kCmdCmdHurtPlayer = 21222532;

	private static int kCmdCmdShootAnim;

	private static int kRpcRpcShootAnim;

	[CompilerGenerated]
	private sealed class <UpdateInput>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <UpdateInput>c__Iterator0()
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
				this.$this.Attack();
				this.$current = new WaitForSeconds(1f / this.<mt>__1);
				if (!this.$disposing)
				{
					this.$PC = 2;
				}
				return true;
			case 2u:
				goto IL_105;
			case 3u:
				break;
			default:
				return false;
			}
			if (Input.GetButton("Fire1") && this.$this.iAm049_2)
			{
				this.<mt>__1 = this.$this.multiplier.Evaluate(this.$this.GetComponent<PlayerStats>().GetHealthPercent());
				this.$this.CallCmdShootAnim();
				this.$this.animator.SetTrigger("Shoot");
				this.$this.animator.speed = this.<mt>__1;
				this.$current = new WaitForSeconds(0.65f / this.<mt>__1);
				if (!this.$disposing)
				{
					this.$PC = 1;
				}
				return true;
			}
			IL_105:
			this.$current = new WaitForEndOfFrame();
			if (!this.$disposing)
			{
				this.$PC = 3;
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

		internal float <mt>__1;

		internal Scp049_2PlayerScript $this;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}
}
