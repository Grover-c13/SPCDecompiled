using System;
using UnityEngine;
using UnityEngine.Networking;

public class FootstepSync : NetworkBehaviour
{
	public FootstepSync()
	{
	}

	private void Start()
	{
		this.ccm = base.GetComponent<CharacterClassManager>();
		this.controller = base.GetComponent<AnimationController>();
	}

	public void SyncFoot()
	{
		if (base.isLocalPlayer)
		{
			this.CallCmdSyncFoot();
			AudioClip[] stepClips = this.ccm.klasy[this.ccm.curClass].stepClips;
			this.controller.footstepSource.PlayOneShot(stepClips[UnityEngine.Random.Range(0, stepClips.Length)]);
		}
	}

	public void SetLoundness(Team t)
	{
		if (t == Team.SCP || t == Team.CHI)
		{
			this.controller.footstepSource.maxDistance = 50f;
		}
		else if (t == Team.CDP || t == Team.RSC)
		{
			this.controller.footstepSource.maxDistance = 18f;
		}
		else
		{
			this.controller.footstepSource.maxDistance = 30f;
		}
	}

	[Command(channel = 1)]
	private void CmdSyncFoot()
	{
		this.CallRpcSyncFoot();
	}

	[ClientRpc(channel = 1)]
	private void RpcSyncFoot()
	{
		if (!base.isLocalPlayer)
		{
			AudioClip[] stepClips = this.ccm.klasy[this.ccm.curClass].stepClips;
			this.controller.footstepSource.PlayOneShot(stepClips[UnityEngine.Random.Range(0, stepClips.Length)]);
		}
	}

	private void UNetVersion()
	{
	}

	protected static void InvokeCmdCmdSyncFoot(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSyncFoot called on client.");
			return;
		}
		((FootstepSync)obj).CmdSyncFoot();
	}

	public void CallCmdSyncFoot()
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdSyncFoot called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdSyncFoot();
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)FootstepSync.kCmdCmdSyncFoot);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		base.SendCommandInternal(networkWriter, 1, "CmdSyncFoot");
	}

	protected static void InvokeRpcRpcSyncFoot(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcSyncFoot called on server.");
			return;
		}
		((FootstepSync)obj).RpcSyncFoot();
	}

	public void CallRpcSyncFoot()
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("RPC Function RpcSyncFoot called on client.");
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)2));
		networkWriter.WritePackedUInt32((uint)FootstepSync.kRpcRpcSyncFoot);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		this.SendRPCInternal(networkWriter, 1, "RpcSyncFoot");
	}

	static FootstepSync()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(FootstepSync), FootstepSync.kCmdCmdSyncFoot, new NetworkBehaviour.CmdDelegate(FootstepSync.InvokeCmdCmdSyncFoot));
		FootstepSync.kRpcRpcSyncFoot = -840565516;
		NetworkBehaviour.RegisterRpcDelegate(typeof(FootstepSync), FootstepSync.kRpcRpcSyncFoot, new NetworkBehaviour.CmdDelegate(FootstepSync.InvokeRpcRpcSyncFoot));
		NetworkCRC.RegisterBehaviour("FootstepSync", 0);
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		bool result;
		return result;
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
	}

	private AnimationController controller;

	private CharacterClassManager ccm;

	private static int kCmdCmdSyncFoot = -789180642;

	private static int kRpcRpcSyncFoot;
}
