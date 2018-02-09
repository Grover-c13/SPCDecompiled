using System;
using GameConsole;
using TMPro;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

public class RoundSummary : NetworkBehaviour
{
	private void Awake()
	{
		Radio.roundEnded = false;
	}

	private void Start()
	{
		this.pm = PlayerManager.singleton;
		this.ccm = base.GetComponent<CharacterClassManager>();
		base.InvokeRepeating("CheckForEnding", 12f, 3f);
	}

	private void RoundRestart()
	{
		bool flag = false;
		foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Player"))
		{
			PlayerStats component = gameObject.GetComponent<PlayerStats>();
			if (component.isLocalPlayer && component.isServer)
			{
				flag = true;
				GameConsole.Console.singleton.AddLog("The round is about to restart! Please wait..", new Color32(0, byte.MaxValue, 0, byte.MaxValue), false);
				component.Roundrestart();
			}
		}
		if (!flag)
		{
			GameConsole.Console.singleton.AddLog("You're not owner of this server!", new Color32(byte.MaxValue, 180, 0, byte.MaxValue), false);
		}
	}

	public void CheckForEnding()
	{
		if (base.isLocalPlayer && base.isServer && !this.roundHasEnded)
		{
			if (!this.ccm.roundStarted)
			{
				return;
			}
			this._ClassDs = 0;
			this._ChaosInsurgency = 0;
			this._MobileForces = 0;
			this._Spectators = 0;
			this._Scientists = 0;
			this._SCPs = 0;
			this._SCPsNozombies = 0;
			GameObject[] players = this.pm.players;
			foreach (GameObject gameObject in players)
			{
				CharacterClassManager component = gameObject.GetComponent<CharacterClassManager>();
				if (component.curClass >= 0)
				{
					Team team = component.klasy[component.curClass].team;
					if (team == Team.CDP)
					{
						this._ClassDs++;
					}
					else if (team == Team.CHI)
					{
						this._ChaosInsurgency++;
					}
					else if (team == Team.MTF)
					{
						this._MobileForces++;
					}
					else if (team == Team.RIP)
					{
						this._Spectators++;
					}
					else if (team == Team.RSC)
					{
						this._Scientists++;
					}
					else if (team == Team.SCP)
					{
						this._SCPs++;
						if (component.curClass != 10)
						{
							this._SCPsNozombies++;
						}
					}
				}
			}
			int num = 0;
			if (this._ClassDs > 0)
			{
				num++;
			}
			if (this._MobileForces > 0 || this._Scientists > 0)
			{
				num++;
			}
			if (this._SCPs > 0)
			{
				num++;
			}
			if (this._ChaosInsurgency > 0 && (this._MobileForces > 0 || this._Scientists > 0))
			{
				num = 3;
			}
			if (num <= 1 && players.Length >= 2)
			{
				this.roundHasEnded = true;
			}
			if (this.debugMode)
			{
				this.roundHasEnded = false;
			}
			if (this.roundHasEnded)
			{
				this.summary.classD_escaped += this._ClassDs;
				this.summary.scientists_escaped += this._Scientists;
				this.summary.scp_alive = this._SCPs;
				this.summary.scp_nozombies = this._SCPsNozombies;
				int @int = ConfigFile.GetInt("auto_round_restart_time", 10);
				this.CallCmdSetSummary(this.summary, @int);
				base.Invoke("RoundRestart", (float)@int);
			}
		}
	}

	private void Update()
	{
		if (RoundSummary.host == null)
		{
			GameObject gameObject = GameObject.Find("Host");
			if (gameObject != null)
			{
				RoundSummary.host = gameObject.GetComponent<RoundSummary>();
			}
		}
	}

	[Command(channel = 15)]
	private void CmdSetSummary(RoundSummary.Summary sum, int posttime)
	{
		this.CallRpcSetSummary(sum, posttime);
	}

	[ClientRpc(channel = 15)]
	public void RpcSetSummary(RoundSummary.Summary sum, int posttime)
	{
		Radio.roundEnded = true;
		string text = string.Empty;
		if (PlayerPrefs.GetString("langver", "en") == "pl")
		{
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"<color=#ff0000>",
				sum.classD_escaped,
				"/",
				sum.classD_start,
				"</color> Personelu Klasy D uciekło z placówki\n"
			});
			text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"<color=#ff0000>",
				sum.scientists_escaped,
				"/",
				sum.scientists_start,
				"</color> Naukowców ocalało\n"
			});
			text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"<color=#ff0000>",
				sum.scp_frags,
				"</color> Zabitych przez SCP\n"
			});
			text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"<color=#ff0000>",
				sum.scp_start - sum.scp_nozombies,
				"/",
				sum.scp_start,
				"</color> Unieszkodliwionych podmiotów SCP\n"
			});
			text = text + "Głowica Alfa: <color=#ff0000>" + ((!sum.warheadDetonated) ? "Nie została użyta" : "Zdetonowana") + "</color>\n\n";
			text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"Następna runda rozpocznie się w ciągu ",
				posttime,
				" sekund."
			});
		}
		else
		{
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"<color=#ff0000>",
				sum.classD_escaped,
				"/",
				sum.classD_start,
				"</color> Class-D Personnel escaped\n"
			});
			text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"<color=#ff0000>",
				sum.scientists_escaped,
				"/",
				sum.scientists_start,
				"</color> Scientists survived\n"
			});
			text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"<color=#ff0000>",
				sum.scp_frags,
				"</color> Killed by SCP\n"
			});
			text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"<color=#ff0000>",
				sum.scp_start - sum.scp_alive,
				"/",
				sum.scp_start,
				"</color> Terminated SCP subjects\n"
			});
			text = text + "Alpha Warhead: <color=#ff0000>" + ((!sum.warheadDetonated) ? "Unused" : "Detonated") + "</color>\n\n";
			text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"The next round will start within ",
				posttime,
				" seconds."
			});
		}
		GameObject gameObject = UserMainInterface.singleton.summary;
		gameObject.SetActive(true);
		TextMeshProUGUI component = GameObject.FindGameObjectWithTag("Summary").GetComponent<TextMeshProUGUI>();
		component.text = text;
	}

	private void UNetVersion()
	{
	}

	protected static void InvokeCmdCmdSetSummary(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSetSummary called on client.");
			return;
		}
		((RoundSummary)obj).CmdSetSummary(GeneratedNetworkCode._ReadSummary_RoundSummary(reader), (int)reader.ReadPackedUInt32());
	}

	public void CallCmdSetSummary(RoundSummary.Summary sum, int posttime)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdSetSummary called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdSetSummary(sum, posttime);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)RoundSummary.kCmdCmdSetSummary);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		GeneratedNetworkCode._WriteSummary_RoundSummary(networkWriter, sum);
		networkWriter.WritePackedUInt32((uint)posttime);
		base.SendCommandInternal(networkWriter, 15, "CmdSetSummary");
	}

	protected static void InvokeRpcRpcSetSummary(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcSetSummary called on server.");
			return;
		}
		((RoundSummary)obj).RpcSetSummary(GeneratedNetworkCode._ReadSummary_RoundSummary(reader), (int)reader.ReadPackedUInt32());
	}

	public void CallRpcSetSummary(RoundSummary.Summary sum, int posttime)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("RPC Function RpcSetSummary called on client.");
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)2));
		networkWriter.WritePackedUInt32((uint)RoundSummary.kRpcRpcSetSummary);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		GeneratedNetworkCode._WriteSummary_RoundSummary(networkWriter, sum);
		networkWriter.WritePackedUInt32((uint)posttime);
		this.SendRPCInternal(networkWriter, 15, "RpcSetSummary");
	}

	static RoundSummary()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(RoundSummary), RoundSummary.kCmdCmdSetSummary, new NetworkBehaviour.CmdDelegate(RoundSummary.InvokeCmdCmdSetSummary));
		RoundSummary.kRpcRpcSetSummary = -1626633486;
		NetworkBehaviour.RegisterRpcDelegate(typeof(RoundSummary), RoundSummary.kRpcRpcSetSummary, new NetworkBehaviour.CmdDelegate(RoundSummary.InvokeRpcRpcSetSummary));
		NetworkCRC.RegisterBehaviour("RoundSummary", 0);
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		bool result;
		return result;
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
	}

	public bool debugMode;

	private bool roundHasEnded;

	private PlayerManager pm;

	private CharacterClassManager ccm;

	public static RoundSummary host;

	public RoundSummary.Summary summary;

	private int _ClassDs;

	private int _ChaosInsurgency;

	private int _MobileForces;

	private int _Spectators;

	private int _Scientists;

	private int _SCPs;

	private int _SCPsNozombies;

	private static int kCmdCmdSetSummary = 509590172;

	private static int kRpcRpcSetSummary;

	[Serializable]
	public class Summary
	{
		public int classD_escaped;

		public int classD_start;

		public int scientists_escaped;

		public int scientists_start;

		public int scp_frags;

		public int scp_start;

		public int scp_alive;

		public int scp_nozombies;

		public bool warheadDetonated;
	}
}
