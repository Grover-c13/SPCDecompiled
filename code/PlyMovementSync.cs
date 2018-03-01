using System;
using AntiFaker;
using UnityEngine;
using UnityEngine.Networking;

public class PlyMovementSync : NetworkBehaviour
{
	public PlyMovementSync()
	{
	}

	private void FixedUpdate()
	{
		if (base.isLocalPlayer && !this.iAm173)
		{
			this.myRotation = base.transform.rotation.eulerAngles.y;
		}
		this.TransmitData();
		this.RecieveData();
	}

	[ClientCallback]
	private void TransmitData()
	{
		if (!NetworkClient.active)
		{
			return;
		}
		if (base.isLocalPlayer)
		{
			if (base.transform.position.y > 2000f && this.ccm.curClass != 2)
			{
				GameObject randomPosition = UnityEngine.Object.FindObjectOfType<SpawnpointManager>().GetRandomPosition(this.ccm.curClass);
				if (randomPosition != null)
				{
					base.transform.position = randomPosition.transform.position;
					base.transform.rotation = randomPosition.transform.rotation;
				}
				else
				{
					base.transform.position = this.ccm.deathPosition;
				}
			}
			this.CallCmdSyncData(this.myRotation, base.transform.position, base.GetComponent<PlayerInteract>().playerCamera.transform.localRotation.eulerAngles.x);
		}
	}

	private void Start()
	{
		this.plyCam = base.GetComponent<Scp049PlayerScript>().plyCam.transform;
		this.speedhack = base.GetComponent<AntiFakeCommands>();
		this.ccm = base.GetComponent<CharacterClassManager>();
		base.InvokeRepeating("LocalPlayerTeam", 1f, 10f);
	}

	private void LocalPlayerTeam()
	{
		GameObject[] players = PlayerManager.singleton.players;
		Team team = Team.RIP;
		foreach (GameObject gameObject in players)
		{
			if (gameObject.GetComponent<CharacterClassManager>().isLocalPlayer && gameObject.GetComponent<CharacterClassManager>().curClass >= 0)
			{
				team = gameObject.GetComponent<CharacterClassManager>().klasy[gameObject.GetComponent<CharacterClassManager>().curClass].team;
			}
		}
		this.localPlayerTeamAccepted = (team == Team.RIP | team == Team.SCP);
	}

	private void RecieveData()
	{
		if (!base.isLocalPlayer)
		{
			if (!this.iAm173 || this.localPlayerTeamAccepted)
			{
				if (Vector3.Distance(base.transform.position, this.position) > 10f)
				{
					base.transform.position = this.position;
				}
				base.transform.position = Vector3.Lerp(base.transform.position, this.position, Time.deltaTime * this.positionLerpSpeed);
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(0f, this.rotation, 0f), Time.deltaTime * this.rotationLerpSpeed);
			}
			else
			{
				base.transform.position = this.position;
				base.transform.rotation = Quaternion.Euler(0f, this.rotation, 0f);
			}
		}
	}

	[Command(channel = 5)]
	private void CmdSyncData(float rot, Vector3 pos, float x)
	{
		this.Networkrotation = rot;
		if (this.speedhack.CheckMovement(pos))
		{
			if (this.ccm.curClass == 2)
			{
				pos = new Vector3(0f, 2048f, 0f);
			}
			this.Networkposition = pos;
		}
		else
		{
			this.CallRpcMoveBack(this.position);
		}
		this.NetworkrotX = x;
		this.plyCam.transform.localRotation = Quaternion.Euler(x, 0f, 0f);
	}

	[ClientRpc]
	private void RpcMoveBack(Vector3 pos)
	{
		base.transform.position = pos;
		this.Networkposition = pos;
	}

	public void SetRotation(float rot)
	{
		this.myRotation = rot;
	}

	private void UNetVersion()
	{
	}

	public float Networkrotation
	{
		get
		{
			return this.rotation;
		}
		set
		{
			base.SetSyncVar<float>(value, ref this.rotation, 1u);
		}
	}

	public Vector3 Networkposition
	{
		get
		{
			return this.position;
		}
		set
		{
			base.SetSyncVar<Vector3>(value, ref this.position, 2u);
		}
	}

	public float NetworkrotX
	{
		get
		{
			return this.rotX;
		}
		set
		{
			base.SetSyncVar<float>(value, ref this.rotX, 4u);
		}
	}

	protected static void InvokeCmdCmdSyncData(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSyncData called on client.");
			return;
		}
		((PlyMovementSync)obj).CmdSyncData(reader.ReadSingle(), reader.ReadVector3(), reader.ReadSingle());
	}

	public void CallCmdSyncData(float rot, Vector3 pos, float x)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdSyncData called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdSyncData(rot, pos, x);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)PlyMovementSync.kCmdCmdSyncData);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(rot);
		networkWriter.Write(pos);
		networkWriter.Write(x);
		base.SendCommandInternal(networkWriter, 5, "CmdSyncData");
	}

	protected static void InvokeRpcRpcMoveBack(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcMoveBack called on server.");
			return;
		}
		((PlyMovementSync)obj).RpcMoveBack(reader.ReadVector3());
	}

	public void CallRpcMoveBack(Vector3 pos)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("RPC Function RpcMoveBack called on client.");
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)2));
		networkWriter.WritePackedUInt32((uint)PlyMovementSync.kRpcRpcMoveBack);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(pos);
		this.SendRPCInternal(networkWriter, 0, "RpcMoveBack");
	}

	static PlyMovementSync()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(PlyMovementSync), PlyMovementSync.kCmdCmdSyncData, new NetworkBehaviour.CmdDelegate(PlyMovementSync.InvokeCmdCmdSyncData));
		PlyMovementSync.kRpcRpcMoveBack = 1135972341;
		NetworkBehaviour.RegisterRpcDelegate(typeof(PlyMovementSync), PlyMovementSync.kRpcRpcMoveBack, new NetworkBehaviour.CmdDelegate(PlyMovementSync.InvokeRpcRpcMoveBack));
		NetworkCRC.RegisterBehaviour("PlyMovementSync", 0);
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			writer.Write(this.rotation);
			writer.Write(this.position);
			writer.Write(this.rotX);
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
			writer.Write(this.rotation);
		}
		if ((base.syncVarDirtyBits & 2u) != 0u)
		{
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
				flag = true;
			}
			writer.Write(this.position);
		}
		if ((base.syncVarDirtyBits & 4u) != 0u)
		{
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
				flag = true;
			}
			writer.Write(this.rotX);
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
			this.rotation = reader.ReadSingle();
			this.position = reader.ReadVector3();
			this.rotX = reader.ReadSingle();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			this.rotation = reader.ReadSingle();
		}
		if ((num & 2) != 0)
		{
			this.position = reader.ReadVector3();
		}
		if ((num & 4) != 0)
		{
			this.rotX = reader.ReadSingle();
		}
	}

	public float positionLerpSpeed = 10f;

	public float rotationLerpSpeed = 15f;

	[SyncVar]
	public float rotation;

	[SyncVar]
	public Vector3 position;

	[SyncVar]
	public float rotX;

	public bool iAmScpAndSomeoneIsLookingAtMe;

	private float myRotation;

	private CharacterClassManager ccm;

	private AntiFakeCommands speedhack;

	private bool localPlayerTeamAccepted;

	public bool iAm173;

	private Vector3 prevPos;

	private Transform plyCam;

	private static int kCmdCmdSyncData = -1186400596;

	private static int kRpcRpcMoveBack;
}
