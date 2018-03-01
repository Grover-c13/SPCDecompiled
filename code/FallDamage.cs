using System;
using UnityEngine;
using UnityEngine.Networking;

public class FallDamage : NetworkBehaviour
{
	public FallDamage()
	{
	}

	private void Start()
	{
		this.ccm = base.GetComponent<CharacterClassManager>();
	}

	private void Update()
	{
		if (base.isLocalPlayer)
		{
			this.CalculateGround();
		}
	}

	private void CalculateGround()
	{
		if (TutorialManager.status)
		{
			return;
		}
		RaycastHit raycastHit;
		bool flag = Physics.Raycast(new Ray(base.transform.position, Vector3.down), out raycastHit, this.groundMaxDistance, this.groundMask);
		if (flag && this.zone != raycastHit.transform.root.name)
		{
			this.zone = raycastHit.transform.root.name;
			if (this.zone.Contains("Heavy"))
			{
				SoundtrackManager.singleton.mainIndex = 1;
			}
			else if (this.zone.Contains("Out"))
			{
				SoundtrackManager.singleton.mainIndex = 2;
			}
			else
			{
				SoundtrackManager.singleton.mainIndex = 0;
			}
		}
		if (flag != this.isGrounded)
		{
			this.isGrounded = flag;
			if (this.isGrounded)
			{
				this.OnTouchdown();
			}
			else
			{
				this.OnLoseContactWithGround();
			}
		}
	}

	private void OnLoseContactWithGround()
	{
		this.previousHeight = base.transform.position.y;
	}

	private void OnTouchdown()
	{
		float num = this.damageOverDistance.Evaluate(this.previousHeight - base.transform.position.y);
		if (num > 5f && this.ccm.klasy[this.ccm.curClass].team != Team.SCP)
		{
			this.CallCmdFall(num);
		}
	}

	[Command(channel = 2)]
	private void CmdFall(float dmg)
	{
		base.GetComponent<PlayerStats>().HurtPlayer(new PlayerStats.HitInfo(Mathf.Abs(dmg), "WORLD", "FALLDOWN"), base.gameObject);
		this.CallRpcDoSound();
	}

	[ClientRpc]
	private void RpcDoSound()
	{
		this.sfxsrc.PlayOneShot(this.sound);
	}

	private void UNetVersion()
	{
	}

	protected static void InvokeCmdCmdFall(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdFall called on client.");
			return;
		}
		((FallDamage)obj).CmdFall(reader.ReadSingle());
	}

	public void CallCmdFall(float dmg)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdFall called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdFall(dmg);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)FallDamage.kCmdCmdFall);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(dmg);
		base.SendCommandInternal(networkWriter, 2, "CmdFall");
	}

	protected static void InvokeRpcRpcDoSound(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcDoSound called on server.");
			return;
		}
		((FallDamage)obj).RpcDoSound();
	}

	public void CallRpcDoSound()
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("RPC Function RpcDoSound called on client.");
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)2));
		networkWriter.WritePackedUInt32((uint)FallDamage.kRpcRpcDoSound);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		this.SendRPCInternal(networkWriter, 0, "RpcDoSound");
	}

	static FallDamage()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(FallDamage), FallDamage.kCmdCmdFall, new NetworkBehaviour.CmdDelegate(FallDamage.InvokeCmdCmdFall));
		FallDamage.kRpcRpcDoSound = 675793188;
		NetworkBehaviour.RegisterRpcDelegate(typeof(FallDamage), FallDamage.kRpcRpcDoSound, new NetworkBehaviour.CmdDelegate(FallDamage.InvokeRpcRpcDoSound));
		NetworkCRC.RegisterBehaviour("FallDamage", 0);
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		bool result;
		return result;
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
	}

	public bool isGrounded = true;

	public LayerMask groundMask;

	[SerializeField]
	private float groundMaxDistance = 1.3f;

	public AudioClip sound;

	public AudioSource sfxsrc;

	private float previousHeight;

	public AnimationCurve damageOverDistance;

	private CharacterClassManager ccm;

	public string zone;

	private static int kCmdCmdFall = -1476756283;

	private static int kRpcRpcDoSound;
}
