using System;
using UnityEngine;
using UnityEngine.Networking;

public class DisableUselessComponents : NetworkBehaviour
{
	private void OnDestroy()
	{
		if (!base.isLocalPlayer)
		{
			PlayerManager.singleton.RemovePlayer(base.gameObject);
		}
	}

	private void Start()
	{
		if (!base.isLocalPlayer)
		{
			UnityEngine.Object.DestroyImmediate(base.GetComponent<FirstPersonController>());
			foreach (Behaviour behaviour in this.uselessComponents)
			{
				behaviour.enabled = false;
			}
			UnityEngine.Object.Destroy(base.GetComponent<CharacterController>());
		}
		else
		{
			PlayerManager.localPlayer = base.gameObject;
			this.CallCmdSetName((!base.isServer) ? "Player" : "Host", ServerStatic.isDedicated);
			base.GetComponent<FirstPersonController>().enabled = false;
		}
	}

	private void FixedUpdate()
	{
		if (!this.isDedicated && !this.added && !string.IsNullOrEmpty(base.GetComponent<BanPlayer>().hardwareID))
		{
			this.added = true;
			try
			{
				if (GameObject.Find("Host").GetComponent<NetworkIdentity>().isLocalPlayer)
				{
					foreach (BanPlayer.Ban ban in BanPlayer.bans)
					{
						if (ban.hardware == base.GetComponent<BanPlayer>().hardwareID && BanPlayer.NotExpired(ban.time))
						{
							base.GetComponent<NetworkIdentity>().connectionToClient.Disconnect();
							this.added = false;
							return;
						}
					}
				}
			}
			catch
			{
			}
			PlayerManager.singleton.AddPlayer(base.gameObject);
		}
		base.name = this.label;
	}

	[Command(channel = 2)]
	private void CmdSetName(string n, bool b)
	{
		this.SetName(n);
		this.SetServer(b);
	}

	private void SetName(string n)
	{
		this.Networklabel = n;
	}

	private void SetServer(bool b)
	{
		this.NetworkisDedicated = b;
	}

	private void UNetVersion()
	{
	}

	public string Networklabel
	{
		get
		{
			return this.label;
		}
		set
		{
			uint dirtyBit = 1u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SetName(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<string>(value, ref this.label, dirtyBit);
		}
	}

	public bool NetworkisDedicated
	{
		get
		{
			return this.isDedicated;
		}
		set
		{
			uint dirtyBit = 2u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SetServer(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<bool>(value, ref this.isDedicated, dirtyBit);
		}
	}

	protected static void InvokeCmdCmdSetName(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSetName called on client.");
			return;
		}
		((DisableUselessComponents)obj).CmdSetName(reader.ReadString(), reader.ReadBoolean());
	}

	public void CallCmdSetName(string n, bool b)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdSetName called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdSetName(n, b);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)DisableUselessComponents.kCmdCmdSetName);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(n);
		networkWriter.Write(b);
		base.SendCommandInternal(networkWriter, 2, "CmdSetName");
	}

	static DisableUselessComponents()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(DisableUselessComponents), DisableUselessComponents.kCmdCmdSetName, new NetworkBehaviour.CmdDelegate(DisableUselessComponents.InvokeCmdCmdSetName));
		NetworkCRC.RegisterBehaviour("DisableUselessComponents", 0);
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			writer.Write(this.label);
			writer.Write(this.isDedicated);
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
			writer.Write(this.label);
		}
		if ((base.syncVarDirtyBits & 2u) != 0u)
		{
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
				flag = true;
			}
			writer.Write(this.isDedicated);
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
			this.label = reader.ReadString();
			this.isDedicated = reader.ReadBoolean();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			this.SetName(reader.ReadString());
		}
		if ((num & 2) != 0)
		{
			this.SetServer(reader.ReadBoolean());
		}
	}

	[SerializeField]
	private Behaviour[] uselessComponents;

	[SyncVar(hook = "SetName")]
	private string label = "Player";

	[SyncVar(hook = "SetServer")]
	public bool isDedicated = true;

	private bool added;

	private static int kCmdCmdSetName = -306933249;
}
