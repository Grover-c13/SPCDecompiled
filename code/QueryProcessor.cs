using System;
using System.Collections.Generic;
using GameConsole;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

public class QueryProcessor : NetworkBehaviour
{
	public QueryProcessor()
	{
	}

	private void Update()
	{
		this.session_time += Time.deltaTime;
	}

	[Command(channel = 15)]
	public void CmdSendQuery(string query, string admin_password)
	{
		string @string = ConfigFile.GetString("administrator_password", "none");
		if (@string != "none")
		{
			if (admin_password == @string && admin_password != string.Empty)
			{
				this.ProcessQuery(base.GetComponent<NetworkIdentity>().connectionToClient, query);
			}
			else
			{
				base.GetComponent<NetworkIdentity>().connectionToClient.Disconnect();
			}
		}
	}

	[Command(channel = 15)]
	public void CmdCheckPassword(string admin_password)
	{
		string @string = ConfigFile.GetString("administrator_password", "none");
		if (admin_password == @string && admin_password != string.Empty && @string != "none")
		{
			this.CallTargetReturnValue(base.GetComponent<NetworkIdentity>().connectionToClient, new string[]
			{
				"_PasswordCheck_True"
			});
		}
		else if (this.passwordTries >= 3)
		{
			BanPlayer.BanConnection(base.GetComponent<NetworkIdentity>().connectionToClient, 1);
		}
		else
		{
			this.CallTargetReturnValue(base.GetComponent<NetworkIdentity>().connectionToClient, new string[]
			{
				"_PasswordCheck_False"
			});
			this.passwordTries++;
		}
	}

	private void ProcessQuery(NetworkConnection target, string q)
	{
		if (q == "GetPlayers")
		{
			List<QueryProcessor.PlayerInfo> list = new List<QueryProcessor.PlayerInfo>();
			foreach (NetworkConnection networkConnection in NetworkServer.connections)
			{
				string sesTime = "#UNCONNECTED#";
				GameObject gameObject = GameConsole.Console.FindConnectedRoot(networkConnection);
				if (target != null)
				{
					try
					{
						float num = gameObject.GetComponent<QueryProcessor>().session_time;
						int num2 = 0;
						while (num >= 60f)
						{
							num -= 60f;
							num2++;
						}
						sesTime = string.Concat(new object[]
						{
							num2,
							"m ",
							Mathf.Floor(num),
							"s"
						});
					}
					catch
					{
					}
				}
				try
				{
					list.Add(new QueryProcessor.PlayerInfo(networkConnection.address, gameObject, sesTime));
				}
				catch
				{
				}
			}
			this.CallTargetReturnPlayerInfo(target, list.ToArray());
		}
		else if (q.StartsWith("BAN"))
		{
			while (q.Contains(" "))
			{
				q = q.Replace(" ", string.Empty);
			}
			string text = q.Remove(0, 4);
			text = text.Remove(text.IndexOf(";)"));
			string text2 = q.Remove(0, q.IndexOf('{') + 1);
			text2 = text2.Remove(text2.IndexOf('}'));
			int duration = int.Parse(text2);
			this.CallTargetReturnValue(target, new string[]
			{
				"BAN SUCCESS"
			});
			foreach (NetworkConnection networkConnection2 in NetworkServer.connections)
			{
				foreach (string a in text.Split(new char[]
				{
					';'
				}))
				{
					if (a == networkConnection2.address)
					{
						BanPlayer.BanConnection(networkConnection2, duration);
					}
				}
			}
		}
		else if (q.StartsWith("FORCECLASS"))
		{
			while (q.Contains(" "))
			{
				q = q.Replace(" ", string.Empty);
			}
			string text3 = q.Remove(0, 11);
			text3 = text3.Remove(text3.IndexOf(";)"));
			string text4 = q.Remove(0, q.IndexOf('{') + 1);
			text4 = text4.Remove(text4.IndexOf('}'));
			int classid = int.Parse(text4);
			this.CallTargetReturnValue(target, new string[]
			{
				"FC SUCCESS"
			});
			foreach (NetworkConnection networkConnection3 in NetworkServer.connections)
			{
				foreach (string a2 in text3.Split(new char[]
				{
					';'
				}))
				{
					if (a2 == networkConnection3.address)
					{
						GameObject gameObject2 = GameConsole.Console.FindConnectedRoot(networkConnection3);
						if (gameObject2 != null)
						{
							base.GetComponent<CharacterClassManager>().SetPlayersClass(classid, gameObject2);
						}
					}
				}
			}
		}
	}

	[TargetRpc(channel = 7)]
	private void TargetReturnValue(NetworkConnection target, string[] answer)
	{
		foreach (RemoteAdminBehaviour remoteAdminBehaviour in UnityEngine.Object.FindObjectsOfType<RemoteAdminBehaviour>())
		{
			remoteAdminBehaviour.Reply(answer);
		}
	}

	[TargetRpc(channel = 7)]
	private void TargetReturnPlayerInfo(NetworkConnection target, QueryProcessor.PlayerInfo[] answer)
	{
		foreach (RemoteAdminBehaviour remoteAdminBehaviour in UnityEngine.Object.FindObjectsOfType<RemoteAdminBehaviour>())
		{
			remoteAdminBehaviour.Reply(answer);
		}
	}

	private void UNetVersion()
	{
	}

	protected static void InvokeCmdCmdSendQuery(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSendQuery called on client.");
			return;
		}
		((QueryProcessor)obj).CmdSendQuery(reader.ReadString(), reader.ReadString());
	}

	protected static void InvokeCmdCmdCheckPassword(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdCheckPassword called on client.");
			return;
		}
		((QueryProcessor)obj).CmdCheckPassword(reader.ReadString());
	}

	public void CallCmdSendQuery(string query, string admin_password)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdSendQuery called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdSendQuery(query, admin_password);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)QueryProcessor.kCmdCmdSendQuery);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(query);
		networkWriter.Write(admin_password);
		base.SendCommandInternal(networkWriter, 15, "CmdSendQuery");
	}

	public void CallCmdCheckPassword(string admin_password)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdCheckPassword called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdCheckPassword(admin_password);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)QueryProcessor.kCmdCmdCheckPassword);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(admin_password);
		base.SendCommandInternal(networkWriter, 15, "CmdCheckPassword");
	}

	protected static void InvokeRpcTargetReturnValue(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC TargetReturnValue called on server.");
			return;
		}
		((QueryProcessor)obj).TargetReturnValue(ClientScene.readyConnection, GeneratedNetworkCode._ReadArrayString_None(reader));
	}

	protected static void InvokeRpcTargetReturnPlayerInfo(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC TargetReturnPlayerInfo called on server.");
			return;
		}
		((QueryProcessor)obj).TargetReturnPlayerInfo(ClientScene.readyConnection, GeneratedNetworkCode._ReadArrayPlayerInfo_None(reader));
	}

	public void CallTargetReturnValue(NetworkConnection target, string[] answer)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("TargetRPC Function TargetReturnValue called on client.");
			return;
		}
		if (target is ULocalConnectionToServer)
		{
			Debug.LogError("TargetRPC Function TargetReturnValue called on connection to server");
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)2));
		networkWriter.WritePackedUInt32((uint)QueryProcessor.kTargetRpcTargetReturnValue);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		GeneratedNetworkCode._WriteArrayString_None(networkWriter, answer);
		this.SendTargetRPCInternal(target, networkWriter, 7, "TargetReturnValue");
	}

	public void CallTargetReturnPlayerInfo(NetworkConnection target, QueryProcessor.PlayerInfo[] answer)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("TargetRPC Function TargetReturnPlayerInfo called on client.");
			return;
		}
		if (target is ULocalConnectionToServer)
		{
			Debug.LogError("TargetRPC Function TargetReturnPlayerInfo called on connection to server");
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)2));
		networkWriter.WritePackedUInt32((uint)QueryProcessor.kTargetRpcTargetReturnPlayerInfo);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		GeneratedNetworkCode._WriteArrayPlayerInfo_None(networkWriter, answer);
		this.SendTargetRPCInternal(target, networkWriter, 7, "TargetReturnPlayerInfo");
	}

	static QueryProcessor()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(QueryProcessor), QueryProcessor.kCmdCmdSendQuery, new NetworkBehaviour.CmdDelegate(QueryProcessor.InvokeCmdCmdSendQuery));
		QueryProcessor.kCmdCmdCheckPassword = -439781415;
		NetworkBehaviour.RegisterCommandDelegate(typeof(QueryProcessor), QueryProcessor.kCmdCmdCheckPassword, new NetworkBehaviour.CmdDelegate(QueryProcessor.InvokeCmdCmdCheckPassword));
		QueryProcessor.kTargetRpcTargetReturnValue = -34185862;
		NetworkBehaviour.RegisterRpcDelegate(typeof(QueryProcessor), QueryProcessor.kTargetRpcTargetReturnValue, new NetworkBehaviour.CmdDelegate(QueryProcessor.InvokeRpcTargetReturnValue));
		QueryProcessor.kTargetRpcTargetReturnPlayerInfo = -684167994;
		NetworkBehaviour.RegisterRpcDelegate(typeof(QueryProcessor), QueryProcessor.kTargetRpcTargetReturnPlayerInfo, new NetworkBehaviour.CmdDelegate(QueryProcessor.InvokeRpcTargetReturnPlayerInfo));
		NetworkCRC.RegisterBehaviour("QueryProcessor", 0);
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		bool result;
		return result;
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
	}

	[HideInInspector]
	public string get_ip;

	[HideInInspector]
	public float session_time;

	private int passwordTries;

	private static int kCmdCmdSendQuery = -1744616138;

	private static int kCmdCmdCheckPassword;

	private static int kTargetRpcTargetReturnValue;

	private static int kTargetRpcTargetReturnPlayerInfo;

	[Serializable]
	public struct PlayerInfo
	{
		public PlayerInfo(string addr, GameObject inst, string sesTime)
		{
			this.address = addr;
			this.instance = inst;
			this.time = sesTime;
		}

		public string address;

		public GameObject instance;

		public string time;
	}
}
