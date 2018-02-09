using System;
using Dissonance.Integrations.UNet_HLAPI;
using GameConsole;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerStats : NetworkBehaviour
{
	private void Start()
	{
		this.ccm = base.GetComponent<CharacterClassManager>();
		this.ui = UserMainInterface.singleton;
	}

	public float GetHealthPercent()
	{
		return Mathf.Clamp01(1f - (float)this.health / (float)this.ccm.klasy[this.ccm.curClass].maxHP);
	}

	[Command(channel = 2)]
	public void CmdSelfDeduct(PlayerStats.HitInfo info)
	{
		this.HurtPlayer(info, base.gameObject);
	}

	public void Explode()
	{
		bool flag = this.health >= 1 && base.transform.position.y < 900f;
		if (this.ccm.curClass == 3)
		{
			Scp106PlayerScript component = base.GetComponent<Scp106PlayerScript>();
			component.DeletePortal();
			if (component.goingViaThePortal)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			foreach (LiftIdentity liftIdentity in UnityEngine.Object.FindObjectsOfType<LiftIdentity>())
			{
				if (liftIdentity.InArea(base.transform.position))
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			this.HurtPlayer(new PlayerStats.HitInfo(999999f, "WORLD", "NUKE"), base.gameObject);
		}
	}

	private void Update()
	{
		if (base.isLocalPlayer && this.ccm.curClass != 2)
		{
			this.ui.SetHP(this.health, this.maxHP);
			GameConsole.Console.singleton.UpdateValue("info", this.lastHitInfo.tool);
		}
		if (base.isLocalPlayer)
		{
			this.ui.hpOBJ.SetActive(this.ccm.curClass != 2);
		}
	}

	[Command(channel = 2)]
	public void CmdTesla()
	{
		this.HurtPlayer(new PlayerStats.HitInfo((float)UnityEngine.Random.Range(100, 200), base.GetComponent<HlapiPlayer>().PlayerId, "TESLA"), base.gameObject);
	}

	public void SetHPAmount(int hp)
	{
		this.Networkhealth = hp;
	}

	public void HurtPlayer(PlayerStats.HitInfo info, GameObject go)
	{
		PlayerStats component = go.GetComponent<PlayerStats>();
		CharacterClassManager component2 = go.GetComponent<CharacterClassManager>();
		PlayerStats playerStats = component;
		playerStats.Networkhealth = playerStats.health - Mathf.CeilToInt(info.amount);
		if (component.health < 1 && component2.curClass != 2)
		{
			if (component2.curClass == 3)
			{
				go.GetComponent<Scp106PlayerScript>().CallRpcAnnounceContaining();
			}
			if (info.amount != 999799f)
			{
				base.GetComponent<RagdollManager>().SpawnRagdoll(go.transform.position, go.transform.rotation, component2.curClass, info, component2.klasy[component2.curClass].team != Team.SCP, go.GetComponent<HlapiPlayer>().PlayerId, go.GetComponent<NicknameSync>().myNick);
			}
			component2.NetworkdeathPosition = go.transform.position;
			component.SetHPAmount(100);
			component2.SetClassID(2);
			if (TutorialManager.status)
			{
				PlayerManager.localPlayer.GetComponent<TutorialManager>().KillNPC();
			}
		}
	}

	[ServerCallback]
	private void CmdRoundrestart()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		this.CallRpcRoundrestart();
	}

	[ClientRpc(channel = 7)]
	private void RpcRoundrestart()
	{
		if (!base.isServer)
		{
			CustomNetworkManager customNetworkManager = UnityEngine.Object.FindObjectOfType<CustomNetworkManager>();
			customNetworkManager.reconnect = true;
			base.Invoke("ChangeLevel", 0.5f);
		}
	}

	public void Roundrestart()
	{
		this.CmdRoundrestart();
		base.Invoke("ChangeLevel", 2.5f);
	}

	private void ChangeLevel()
	{
		if (base.isServer)
		{
			NetworkManager.singleton.ServerChangeScene(NetworkManager.singleton.onlineScene);
		}
		else
		{
			NetworkManager.singleton.StopClient();
		}
	}

	private void UNetVersion()
	{
	}

	public int Networkhealth
	{
		get
		{
			return this.health;
		}
		set
		{
			uint dirtyBit = 1u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SetHPAmount(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<int>(value, ref this.health, dirtyBit);
		}
	}

	protected static void InvokeCmdCmdSelfDeduct(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSelfDeduct called on client.");
			return;
		}
		((PlayerStats)obj).CmdSelfDeduct(GeneratedNetworkCode._ReadHitInfo_PlayerStats(reader));
	}

	protected static void InvokeCmdCmdTesla(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdTesla called on client.");
			return;
		}
		((PlayerStats)obj).CmdTesla();
	}

	public void CallCmdSelfDeduct(PlayerStats.HitInfo info)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdSelfDeduct called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdSelfDeduct(info);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)PlayerStats.kCmdCmdSelfDeduct);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		GeneratedNetworkCode._WriteHitInfo_PlayerStats(networkWriter, info);
		base.SendCommandInternal(networkWriter, 2, "CmdSelfDeduct");
	}

	public void CallCmdTesla()
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdTesla called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdTesla();
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)PlayerStats.kCmdCmdTesla);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		base.SendCommandInternal(networkWriter, 2, "CmdTesla");
	}

	protected static void InvokeRpcRpcRoundrestart(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcRoundrestart called on server.");
			return;
		}
		((PlayerStats)obj).RpcRoundrestart();
	}

	public void CallRpcRoundrestart()
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("RPC Function RpcRoundrestart called on client.");
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)2));
		networkWriter.WritePackedUInt32((uint)PlayerStats.kRpcRpcRoundrestart);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		this.SendRPCInternal(networkWriter, 7, "RpcRoundrestart");
	}

	static PlayerStats()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(PlayerStats), PlayerStats.kCmdCmdSelfDeduct, new NetworkBehaviour.CmdDelegate(PlayerStats.InvokeCmdCmdSelfDeduct));
		PlayerStats.kCmdCmdTesla = -1109720487;
		NetworkBehaviour.RegisterCommandDelegate(typeof(PlayerStats), PlayerStats.kCmdCmdTesla, new NetworkBehaviour.CmdDelegate(PlayerStats.InvokeCmdCmdTesla));
		PlayerStats.kRpcRpcRoundrestart = 907411477;
		NetworkBehaviour.RegisterRpcDelegate(typeof(PlayerStats), PlayerStats.kRpcRpcRoundrestart, new NetworkBehaviour.CmdDelegate(PlayerStats.InvokeRpcRpcRoundrestart));
		NetworkCRC.RegisterBehaviour("PlayerStats", 0);
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			writer.WritePackedUInt32((uint)this.health);
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
			writer.WritePackedUInt32((uint)this.health);
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
			this.health = (int)reader.ReadPackedUInt32();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			this.SetHPAmount((int)reader.ReadPackedUInt32());
		}
	}

	public PlayerStats.HitInfo lastHitInfo = new PlayerStats.HitInfo(0f, "NONE", "NONE");

	[SyncVar(hook = "SetHPAmount")]
	public int health;

	public int maxHP;

	private UserMainInterface ui;

	private CharacterClassManager ccm;

	private static int kCmdCmdSelfDeduct = -2147454163;

	private static int kCmdCmdTesla;

	private static int kRpcRpcRoundrestart;

	[Serializable]
	public struct HitInfo
	{
		public HitInfo(float amnt, string plyID, string weapon)
		{
			this.amount = amnt;
			this.tool = weapon;
			this.time = ServerTime.time;
		}

		public float amount;

		public string tool;

		public int time;
	}
}
