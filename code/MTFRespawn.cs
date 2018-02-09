using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MTFRespawn : NetworkBehaviour
{
	private void Start()
	{
		this.minMtfTimeToRespawn = ConfigFile.GetInt("minimum_MTF_time_to_spawn", 200);
		this.maxMtfTimeToRespawn = ConfigFile.GetInt("maximum_MTF_time_to_spawn", 400);
		this.CI_Percent = (float)ConfigFile.GetInt("ci_respawn_percent", 35);
	}

	private void Update()
	{
		if (base.name != "Host" || !base.isLocalPlayer)
		{
			return;
		}
		if (this.mtf_a == null)
		{
			this.mtf_a = UnityEngine.Object.FindObjectOfType<ChopperAutostart>();
		}
		this.timeToNextRespawn -= Time.deltaTime;
		if (this.timeToNextRespawn < ((!this.nextWaveIsCI) ? 18f : 13.5f) && !this.loaded)
		{
			this.loaded = true;
			GameObject[] players = PlayerManager.singleton.players;
			foreach (GameObject gameObject in players)
			{
				if (gameObject.GetComponent<CharacterClassManager>().curClass == 2)
				{
					this.chopperStarted = true;
					if (this.nextWaveIsCI)
					{
						this.SummonVan();
					}
					else
					{
						this.SummonChopper(true);
					}
					break;
				}
			}
		}
		if (this.timeToNextRespawn < 0f)
		{
			this.loaded = false;
			if (base.GetComponent<CharacterClassManager>().roundStarted)
			{
				this.SummonChopper(false);
			}
			if (this.chopperStarted)
			{
				this.RespawnDeadPlayers();
			}
			this.nextWaveIsCI = ((float)UnityEngine.Random.Range(0, 100) <= this.CI_Percent);
			this.timeToNextRespawn = (float)UnityEngine.Random.Range(this.minMtfTimeToRespawn, this.maxMtfTimeToRespawn) * ((!this.nextWaveIsCI) ? 1f : (1f / this.CI_Time_Multiplier));
			this.chopperStarted = false;
		}
	}

	private void RespawnDeadPlayers()
	{
		int num = 0;
		List<GameObject> list = new List<GameObject>();
		foreach (GameObject gameObject in PlayerManager.singleton.players)
		{
			if (gameObject.GetComponent<CharacterClassManager>().curClass == 2)
			{
				list.Add(gameObject);
			}
		}
		while (list.Count > this.maxRespawnAmount)
		{
			list.RemoveAt(UnityEngine.Random.Range(0, list.Count));
		}
		for (int j = 0; j < list.Count; j++)
		{
			if (list[j] != null)
			{
				num++;
				if (this.nextWaveIsCI)
				{
					base.GetComponent<CharacterClassManager>().SetPlayersClass(8, list[j]);
				}
				else
				{
					this.playersToNTF.Add(list[j]);
				}
			}
		}
		if (num > 0)
		{
			if (this.nextWaveIsCI)
			{
				base.Invoke("CmdDelayCIAnnounc", 1f);
			}
			else
			{
				this.PlayAnnonc();
			}
		}
		this.SummonNTF();
	}

	public void SummonNTF()
	{
		if (this.playersToNTF.Count > 0)
		{
			this.SetUnit(this.playersToNTF.ToArray());
			for (int i = 0; i < this.playersToNTF.Count; i++)
			{
				if (i == 0)
				{
					base.GetComponent<CharacterClassManager>().SetPlayersClass(12, this.playersToNTF[i]);
				}
				else if (i <= 3)
				{
					base.GetComponent<CharacterClassManager>().SetPlayersClass(11, this.playersToNTF[i]);
				}
				else
				{
					base.GetComponent<CharacterClassManager>().SetPlayersClass(13, this.playersToNTF[i]);
				}
			}
			this.playersToNTF.Clear();
		}
	}

	[ServerCallback]
	private void SetUnit(GameObject[] ply)
	{
		if (!NetworkServer.active)
		{
			return;
		}
		int unit = base.GetComponent<NineTailedFoxUnits>().NewName();
		foreach (GameObject gameObject in ply)
		{
			gameObject.GetComponent<CharacterClassManager>().SetUnit(unit);
		}
	}

	[ServerCallback]
	private void SummonChopper(bool state)
	{
		if (!NetworkServer.active)
		{
			return;
		}
		this.mtf_a.SetState(state);
	}

	[ServerCallback]
	private void SummonVan()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		this.CallRpcVan();
	}

	[ClientRpc(channel = 2)]
	private void RpcVan()
	{
		GameObject.Find("CIVanArrive").GetComponent<Animator>().SetTrigger("Arrive");
	}

	private void CmdDelayCIAnnounc()
	{
		this.PlayAnnoncCI();
	}

	[ServerCallback]
	private void PlayAnnonc()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		this.CallRpcAnnounc();
	}

	[ClientRpc(channel = 2)]
	private void RpcAnnounc()
	{
		GameObject.Find("MTF_Announc").GetComponent<AudioSource>().Play();
	}

	[ServerCallback]
	private void PlayAnnoncCI()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		this.CallRpcAnnouncCI();
	}

	[ClientRpc(channel = 2)]
	private void RpcAnnouncCI()
	{
		foreach (GameObject gameObject in PlayerManager.singleton.players)
		{
			CharacterClassManager component = gameObject.GetComponent<CharacterClassManager>();
			if (component.isLocalPlayer)
			{
				Team team = component.klasy[component.curClass].team;
				if (team == Team.CDP || team == Team.CHI)
				{
					UnityEngine.Object.Instantiate<GameObject>(this.ciTheme);
				}
			}
		}
	}

	private void UNetVersion()
	{
	}

	protected static void InvokeRpcRpcVan(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcVan called on server.");
			return;
		}
		((MTFRespawn)obj).RpcVan();
	}

	protected static void InvokeRpcRpcAnnounc(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcAnnounc called on server.");
			return;
		}
		((MTFRespawn)obj).RpcAnnounc();
	}

	protected static void InvokeRpcRpcAnnouncCI(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcAnnouncCI called on server.");
			return;
		}
		((MTFRespawn)obj).RpcAnnouncCI();
	}

	public void CallRpcVan()
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("RPC Function RpcVan called on client.");
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)2));
		networkWriter.WritePackedUInt32((uint)MTFRespawn.kRpcRpcVan);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		this.SendRPCInternal(networkWriter, 2, "RpcVan");
	}

	public void CallRpcAnnounc()
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("RPC Function RpcAnnounc called on client.");
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)2));
		networkWriter.WritePackedUInt32((uint)MTFRespawn.kRpcRpcAnnounc);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		this.SendRPCInternal(networkWriter, 2, "RpcAnnounc");
	}

	public void CallRpcAnnouncCI()
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("RPC Function RpcAnnouncCI called on client.");
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)2));
		networkWriter.WritePackedUInt32((uint)MTFRespawn.kRpcRpcAnnouncCI);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		this.SendRPCInternal(networkWriter, 2, "RpcAnnouncCI");
	}

	static MTFRespawn()
	{
		NetworkBehaviour.RegisterRpcDelegate(typeof(MTFRespawn), MTFRespawn.kRpcRpcVan, new NetworkBehaviour.CmdDelegate(MTFRespawn.InvokeRpcRpcVan));
		MTFRespawn.kRpcRpcAnnounc = -125867587;
		NetworkBehaviour.RegisterRpcDelegate(typeof(MTFRespawn), MTFRespawn.kRpcRpcAnnounc, new NetworkBehaviour.CmdDelegate(MTFRespawn.InvokeRpcRpcAnnounc));
		MTFRespawn.kRpcRpcAnnouncCI = -699664669;
		NetworkBehaviour.RegisterRpcDelegate(typeof(MTFRespawn), MTFRespawn.kRpcRpcAnnouncCI, new NetworkBehaviour.CmdDelegate(MTFRespawn.InvokeRpcRpcAnnouncCI));
		NetworkCRC.RegisterBehaviour("MTFRespawn", 0);
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		bool result;
		return result;
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
	}

	public GameObject ciTheme;

	private ChopperAutostart mtf_a;

	[Range(30f, 1000f)]
	public int minMtfTimeToRespawn = 200;

	[Range(40f, 1200f)]
	public int maxMtfTimeToRespawn = 400;

	public float CI_Time_Multiplier = 2f;

	public float CI_Percent = 20f;

	[Range(2f, 15f)]
	[Space(10f)]
	public int maxRespawnAmount = 15;

	public float timeToNextRespawn;

	public bool nextWaveIsCI;

	public List<GameObject> playersToNTF = new List<GameObject>();

	private bool loaded;

	private bool chopperStarted;

	private static int kRpcRpcVan = -871850524;

	private static int kRpcRpcAnnounc;

	private static int kRpcRpcAnnouncCI;
}
