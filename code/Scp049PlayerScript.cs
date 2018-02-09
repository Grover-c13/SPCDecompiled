using System;
using Dissonance.Integrations.UNet_HLAPI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Scp049PlayerScript : NetworkBehaviour
{
	private void Start()
	{
		this.interfaces = UnityEngine.Object.FindObjectOfType<ScpInterfaces>();
		this.loadingCircle = this.interfaces.Scp049_loading;
		if (base.isLocalPlayer)
		{
			this.fpc = base.GetComponent<FirstPersonController>();
		}
	}

	public void Init(int classID, Class c)
	{
		this.sameClass = (c.team == Team.SCP);
		this.iAm049 = (classID == 5);
		if (base.isLocalPlayer)
		{
			this.interfaces.Scp049_eq.SetActive(this.iAm049);
		}
	}

	private void Update()
	{
		this.DeductInfection();
		this.UpdateInput();
	}

	private void DeductInfection()
	{
		if (this.currentInfection > 0f)
		{
			this.currentInfection -= Time.deltaTime;
		}
		if (this.currentInfection < 0f)
		{
			this.currentInfection = 0f;
		}
	}

	private void UpdateInput()
	{
		if (base.isLocalPlayer)
		{
			if (Input.GetButtonDown("Fire1"))
			{
				this.Attack();
			}
			if (Input.GetButtonDown("Interact"))
			{
				this.Surgery();
			}
			this.Recalling();
		}
	}

	private void Attack()
	{
		RaycastHit raycastHit;
		if (this.iAm049 && Physics.Raycast(this.plyCam.transform.position, this.plyCam.transform.forward, out raycastHit, this.distance))
		{
			Scp049PlayerScript component = raycastHit.transform.GetComponent<Scp049PlayerScript>();
			if (component != null && !component.sameClass)
			{
				this.InfectPlayer(component.gameObject, base.GetComponent<HlapiPlayer>().PlayerId);
			}
		}
	}

	private void Surgery()
	{
		RaycastHit raycastHit;
		if (this.iAm049 && Physics.Raycast(this.plyCam.transform.position, this.plyCam.transform.forward, out raycastHit, this.recallDistance))
		{
			Ragdoll componentInParent = raycastHit.transform.GetComponentInParent<Ragdoll>();
			if (componentInParent != null && componentInParent.allowRecall)
			{
				GameObject[] players = PlayerManager.singleton.players;
				foreach (GameObject gameObject in players)
				{
					if (gameObject.GetComponent<HlapiPlayer>().PlayerId == componentInParent.owner.ownerHLAPI_id && gameObject.GetComponent<Scp049PlayerScript>().currentInfection > 0f && componentInParent.allowRecall)
					{
						this.recallingObject = gameObject;
						this.recallingRagdoll = componentInParent;
					}
				}
			}
		}
	}

	[Command(channel = 2)]
	private void CmdDestroyPlayer(GameObject recallingRagdoll)
	{
		if (recallingRagdoll.CompareTag("Ragdoll"))
		{
			NetworkServer.Destroy(recallingRagdoll);
		}
	}

	private void Recalling()
	{
		if (this.iAm049 && Input.GetButton("Interact") && this.recallingObject != null)
		{
			this.fpc.lookingAtMe = true;
			this.recallProgress += Time.deltaTime / this.boost_recallTime.Evaluate(base.GetComponent<PlayerStats>().GetHealthPercent());
			if (this.recallProgress >= 1f)
			{
				this.CallCmdRecallPlayer(this.recallingObject, base.gameObject);
				this.CallCmdDestroyPlayer(this.recallingRagdoll.gameObject);
				this.recallProgress = 0f;
				this.recallingObject = null;
			}
		}
		else
		{
			this.recallingObject = null;
			this.recallProgress = 0f;
			if (this.iAm049)
			{
				this.fpc.lookingAtMe = false;
			}
		}
		this.loadingCircle.fillAmount = this.recallProgress;
	}

	private void InfectPlayer(GameObject target, string id)
	{
		this.CallCmdInfectPlayer(target, id);
		Hitmarker.Hit(1f);
	}

	[Command(channel = 2)]
	private void CmdInfectPlayer(GameObject target, string id)
	{
		if (base.GetComponent<CharacterClassManager>().curClass == 5 && Vector3.Distance(target.transform.position, base.GetComponent<PlyMovementSync>().position) < this.distance * 1.3f)
		{
			base.GetComponent<PlayerStats>().HurtPlayer(new PlayerStats.HitInfo(999999f, id, "SCP:049"), target);
			this.CallRpcInfectPlayer(target, this.boost_infectTime.Evaluate(base.GetComponent<PlayerStats>().GetHealthPercent()));
		}
	}

	[ClientRpc(channel = 2)]
	private void RpcInfectPlayer(GameObject target, float infTime)
	{
		target.GetComponent<Scp049PlayerScript>().currentInfection = infTime;
	}

	[Command(channel = 2)]
	private void CmdRecallPlayer(GameObject target, GameObject sender)
	{
		CharacterClassManager component = target.GetComponent<CharacterClassManager>();
		if (component.curClass == 2 && sender.GetComponent<CharacterClassManager>().curClass == 5)
		{
			component.SetClassID(10);
			target.GetComponent<PlayerStats>().Networkhealth = component.klasy[10].maxHP;
		}
	}

	private void UNetVersion()
	{
	}

	protected static void InvokeCmdCmdDestroyPlayer(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdDestroyPlayer called on client.");
			return;
		}
		((Scp049PlayerScript)obj).CmdDestroyPlayer(reader.ReadGameObject());
	}

	protected static void InvokeCmdCmdInfectPlayer(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdInfectPlayer called on client.");
			return;
		}
		((Scp049PlayerScript)obj).CmdInfectPlayer(reader.ReadGameObject(), reader.ReadString());
	}

	protected static void InvokeCmdCmdRecallPlayer(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdRecallPlayer called on client.");
			return;
		}
		((Scp049PlayerScript)obj).CmdRecallPlayer(reader.ReadGameObject(), reader.ReadGameObject());
	}

	public void CallCmdDestroyPlayer(GameObject recallingRagdoll)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdDestroyPlayer called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdDestroyPlayer(recallingRagdoll);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)Scp049PlayerScript.kCmdCmdDestroyPlayer);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(recallingRagdoll);
		base.SendCommandInternal(networkWriter, 2, "CmdDestroyPlayer");
	}

	public void CallCmdInfectPlayer(GameObject target, string id)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdInfectPlayer called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdInfectPlayer(target, id);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)Scp049PlayerScript.kCmdCmdInfectPlayer);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(target);
		networkWriter.Write(id);
		base.SendCommandInternal(networkWriter, 2, "CmdInfectPlayer");
	}

	public void CallCmdRecallPlayer(GameObject target, GameObject sender)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdRecallPlayer called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdRecallPlayer(target, sender);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)Scp049PlayerScript.kCmdCmdRecallPlayer);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(target);
		networkWriter.Write(sender);
		base.SendCommandInternal(networkWriter, 2, "CmdRecallPlayer");
	}

	protected static void InvokeRpcRpcInfectPlayer(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcInfectPlayer called on server.");
			return;
		}
		((Scp049PlayerScript)obj).RpcInfectPlayer(reader.ReadGameObject(), reader.ReadSingle());
	}

	public void CallRpcInfectPlayer(GameObject target, float infTime)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("RPC Function RpcInfectPlayer called on client.");
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)2));
		networkWriter.WritePackedUInt32((uint)Scp049PlayerScript.kRpcRpcInfectPlayer);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(target);
		networkWriter.Write(infTime);
		this.SendRPCInternal(networkWriter, 2, "RpcInfectPlayer");
	}

	static Scp049PlayerScript()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(Scp049PlayerScript), Scp049PlayerScript.kCmdCmdDestroyPlayer, new NetworkBehaviour.CmdDelegate(Scp049PlayerScript.InvokeCmdCmdDestroyPlayer));
		Scp049PlayerScript.kCmdCmdInfectPlayer = -2004090729;
		NetworkBehaviour.RegisterCommandDelegate(typeof(Scp049PlayerScript), Scp049PlayerScript.kCmdCmdInfectPlayer, new NetworkBehaviour.CmdDelegate(Scp049PlayerScript.InvokeCmdCmdInfectPlayer));
		Scp049PlayerScript.kCmdCmdRecallPlayer = 1670066835;
		NetworkBehaviour.RegisterCommandDelegate(typeof(Scp049PlayerScript), Scp049PlayerScript.kCmdCmdRecallPlayer, new NetworkBehaviour.CmdDelegate(Scp049PlayerScript.InvokeCmdCmdRecallPlayer));
		Scp049PlayerScript.kRpcRpcInfectPlayer = -1920658579;
		NetworkBehaviour.RegisterRpcDelegate(typeof(Scp049PlayerScript), Scp049PlayerScript.kRpcRpcInfectPlayer, new NetworkBehaviour.CmdDelegate(Scp049PlayerScript.InvokeRpcRpcInfectPlayer));
		NetworkCRC.RegisterBehaviour("Scp049PlayerScript", 0);
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		bool result;
		return result;
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
	}

	[Header("Player Properties")]
	public Camera plyCam;

	public bool iAm049;

	public bool sameClass;

	public GameObject scpInstance;

	[Header("Infection")]
	public float currentInfection;

	[Header("Attack & Recall")]
	public float distance = 2.4f;

	public float recallDistance = 3.5f;

	public float recallProgress;

	private GameObject recallingObject;

	private Ragdoll recallingRagdoll;

	private ScpInterfaces interfaces;

	private Image loadingCircle;

	private FirstPersonController fpc;

	[Header("Boosts")]
	public AnimationCurve boost_recallTime;

	public AnimationCurve boost_infectTime;

	private static int kCmdCmdDestroyPlayer = 1834329690;

	private static int kCmdCmdInfectPlayer;

	private static int kRpcRpcInfectPlayer;

	private static int kCmdCmdRecallPlayer;
}
