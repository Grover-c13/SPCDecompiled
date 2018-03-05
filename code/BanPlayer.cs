using System;
using System.Collections.Generic;
using System.IO;
using GameConsole;
using UnityEngine;
using UnityEngine.Networking;

public class BanPlayer : NetworkBehaviour
{
	public string NetworkhardwareID
	{
		get
		{
			return this.hardwareID;
		}
		set
		{
			uint dirtyBit = 1u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SyncHwId(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<string>(value, ref this.hardwareID, dirtyBit);
		}
	}

	public BanPlayer()
	{
	}

	private void Start()
	{
		if (base.isLocalPlayer)
		{
			if (base.isServer)
			{
				BanPlayer.dbpath = ConfigFile.GetString("ban_database_folder", "[appdata]/SCP Secret Laboratory/Bans");
				BanPlayer.dbpath = ((!BanPlayer.dbpath.Contains("[appdata]")) ? BanPlayer.dbpath : BanPlayer.dbpath.Replace("[appdata]", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)));
				BanPlayer.ReloadBans();
			}
			this.CallCmdSetHwId(SystemInfo.deviceUniqueIdentifier);
		}
	}

	[Command]
	private void CmdSetHwId(string s)
	{
		this.SyncHwId(s);
	}

	private void SyncHwId(string s)
	{
		this.NetworkhardwareID = s;
	}

	public static void ReloadBans()
	{
		BanPlayer.bans.Clear();
		try
		{
			if (!Directory.Exists(BanPlayer.dbpath))
			{
				Directory.CreateDirectory(BanPlayer.dbpath);
			}
			string[] files = Directory.GetFiles(BanPlayer.dbpath, "*.ban", SearchOption.TopDirectoryOnly);
			foreach (string path in files)
			{
				StreamReader streamReader = File.OpenText(path);
				string[] array2 = streamReader.ReadToEnd().Split(new string[]
				{
					Environment.NewLine
				}, StringSplitOptions.None);
				streamReader.Close();
				BanPlayer.bans.Add(new BanPlayer.Ban(array2[0], array2[1], array2[2], array2[3], false));
			}
		}
		catch
		{
			GameConsole.Console.singleton.AddLog("Ban database directory incorrect.", new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue), false);
		}
	}

	public static void BanConnection(NetworkConnection conn, int duration)
	{
		GameObject gameObject = GameConsole.Console.FindConnectedRoot(conn);
		string nick = "MissingNick";
		string hardware = "MissingHardware";
		string address = conn.address;
		string time = DateTime.Now.AddMinutes((double)duration).ToString();
		if (gameObject != null)
		{
			nick = gameObject.GetComponent<NicknameSync>().myNick;
			hardware = gameObject.GetComponent<BanPlayer>().hardwareID;
		}
		BanPlayer.bans.Add(new BanPlayer.Ban(nick, hardware, address, time, true));
		conn.Disconnect();
	}

	public static bool NotExpired(string _time)
	{
		return Convert.ToDateTime(_time) > DateTime.Now;
	}

	static BanPlayer()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(BanPlayer), BanPlayer.kCmdCmdSetHwId, new NetworkBehaviour.CmdDelegate(BanPlayer.InvokeCmdCmdSetHwId));
		NetworkCRC.RegisterBehaviour("BanPlayer", 0);
	}

	private void UNetVersion()
	{
	}

	protected static void InvokeCmdCmdSetHwId(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSetHwId called on client.");
			return;
		}
		((BanPlayer)obj).CmdSetHwId(reader.ReadString());
	}

	public void CallCmdSetHwId(string s)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdSetHwId called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdSetHwId(s);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)BanPlayer.kCmdCmdSetHwId);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(s);
		base.SendCommandInternal(networkWriter, 0, "CmdSetHwId");
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			writer.Write(this.hardwareID);
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
			writer.Write(this.hardwareID);
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
			this.hardwareID = reader.ReadString();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			this.SyncHwId(reader.ReadString());
		}
	}

	[SyncVar(hook = "SyncHwId")]
	public string hardwareID;

	private static string dbpath;

	public static List<BanPlayer.Ban> bans = new List<BanPlayer.Ban>();

	private static int kCmdCmdSetHwId = -389185796;

	[Serializable]
	public class Ban
	{
		public Ban(string _nick, string _hardware, string _ip, string _time, bool _addToDatabase)
		{
			this.nick = _nick;
			this.hardware = _hardware;
			this.ip = _ip;
			this.time = _time;
			if (_addToDatabase)
			{
				try
				{
					string text = string.Concat(new object[]
					{
						BanPlayer.dbpath,
						"/",
						_nick,
						Directory.GetFiles(BanPlayer.dbpath, "*.ban", SearchOption.TopDirectoryOnly).Length,
						".ban"
					});
					MonoBehaviour.print(text);
					StreamWriter streamWriter = new StreamWriter(text);
					streamWriter.Write(string.Concat(new string[]
					{
						_nick,
						Environment.NewLine,
						_hardware,
						Environment.NewLine,
						_ip,
						Environment.NewLine,
						_time
					}));
					streamWriter.Close();
				}
				catch
				{
					GameConsole.Console.singleton.AddLog("Failed to ban: Database error - no such file.", new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue), false);
				}
			}
		}

		public string nick;

		public string hardware;

		public string ip;

		public string time;
	}
}
