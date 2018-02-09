using System;
using GameConsole;
using UnityEngine;
using UnityEngine.Networking;

public class ServerTime : NetworkBehaviour
{
	public static bool CheckSynchronization(int myTime)
	{
		int num = Mathf.Abs(myTime - ServerTime.time);
		if (num > 2)
		{
			GameConsole.Console.singleton.AddLog("Damage sync error.", new Color32(byte.MaxValue, 200, 0, byte.MaxValue), false);
		}
		return num <= 2;
	}

	private void Update()
	{
		if (base.name == "Host")
		{
			ServerTime.time = this.timeFromStartup;
		}
	}

	private void Start()
	{
		if (base.isLocalPlayer && base.isServer)
		{
			base.InvokeRepeating("IncreaseTime", 1f, 1f);
		}
	}

	private void IncreaseTime()
	{
		this.TransmitData(this.timeFromStartup + 1);
	}

	[ClientCallback]
	private void TransmitData(int timeFromStartup)
	{
		if (!NetworkClient.active)
		{
			return;
		}
		this.CallCmdSetTime(timeFromStartup);
	}

	[Command(channel = 12)]
	private void CmdSetTime(int t)
	{
		this.NetworktimeFromStartup = t;
	}

	private void UNetVersion()
	{
	}

	public int NetworktimeFromStartup
	{
		get
		{
			return this.timeFromStartup;
		}
		set
		{
			base.SetSyncVar<int>(value, ref this.timeFromStartup, 1u);
		}
	}

	protected static void InvokeCmdCmdSetTime(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSetTime called on client.");
			return;
		}
		((ServerTime)obj).CmdSetTime((int)reader.ReadPackedUInt32());
	}

	public void CallCmdSetTime(int t)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdSetTime called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdSetTime(t);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)ServerTime.kCmdCmdSetTime);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.WritePackedUInt32((uint)t);
		base.SendCommandInternal(networkWriter, 12, "CmdSetTime");
	}

	static ServerTime()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(ServerTime), ServerTime.kCmdCmdSetTime, new NetworkBehaviour.CmdDelegate(ServerTime.InvokeCmdCmdSetTime));
		NetworkCRC.RegisterBehaviour("ServerTime", 0);
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			writer.WritePackedUInt32((uint)this.timeFromStartup);
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
			writer.WritePackedUInt32((uint)this.timeFromStartup);
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
			this.timeFromStartup = (int)reader.ReadPackedUInt32();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			this.timeFromStartup = (int)reader.ReadPackedUInt32();
		}
	}

	[SyncVar]
	public int timeFromStartup;

	public static int time;

	private const int allowedDeviation = 2;

	private static int kCmdCmdSetTime = 648282655;
}
