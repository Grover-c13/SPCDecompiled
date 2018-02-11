using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerInteract : NetworkBehaviour
{
	private void Update()
	{
		RaycastHit raycastHit;
		if (base.isLocalPlayer && Input.GetButtonDown("Interact") && base.GetComponent<CharacterClassManager>().curClass != 2 && Physics.Raycast(this.playerCamera.transform.position, this.playerCamera.transform.forward, out raycastHit, this.raycastMaxDistance, this.mask))
		{
			if (raycastHit.transform.GetComponentInParent<Door>() != null)
			{
				this.CallCmdOpenDoor(raycastHit.transform.GetComponentInParent<Door>().gameObject);
			}
			else if (raycastHit.transform.CompareTag("914_knob"))
			{
				this.CallCmdChange914_State(raycastHit.transform.name);
			}
			else if (raycastHit.transform.CompareTag("914_use"))
			{
				base.GetComponent<Scp914_Controller>().Refine(raycastHit.transform.name);
			}
			else if (raycastHit.transform.CompareTag("Lever"))
			{
				this.CallCmdChangeLeverState(GameObject.Find(raycastHit.transform.name + "_Controller"));
			}
			else if (raycastHit.transform.CompareTag("AW_Button"))
			{
				if (this.inv.curItem != 0)
				{
					foreach (string a in this.inv.availableItems[this.inv.curItem].permissions)
					{
						if (a == "CONT_LVL_3")
						{
							this.CallCmdSwitchAWButton(raycastHit.transform.name);
							return;
						}
					}
				}
				GameObject.Find("Keycard Denied Text").GetComponent<Text>().enabled = true;
				base.Invoke("DisableDeniedText", 1f);
				this.CallCmdSwitchAWButton("DENIED");
			}
			else if (raycastHit.transform.CompareTag("AW_Detonation"))
			{
				if (GameObject.Find("Lever_Alpha_Controller").GetComponent<LeverButton>().GetState())
				{
					this.CallCmdDetonateWarhead();
				}
				else
				{
					GameObject.Find("Alpha Denied Text").GetComponent<Text>().enabled = true;
					base.Invoke("DisableAlphaText", 1f);
				}
			}
			else if (raycastHit.transform.CompareTag("ElevatorButton"))
			{
				foreach (Lift lift in UnityEngine.Object.FindObjectsOfType<Lift>())
				{
					foreach (Lift.Elevator elevator in lift.elevators)
					{
						if (this.ChckDis(elevator.door.transform.position))
						{
							this.CallCmdUseElevator(lift.transform.gameObject);
						}
					}
				}
			}
			else if (raycastHit.transform.CompareTag("294"))
			{
				Inventory component = base.GetComponent<Inventory>();
				if (component.curItem == 17)
				{
					for (int l = 0; l < component.items.Count; l++)
					{
						if (component.items[l].id == 17)
						{
							component.items.RemoveAt(l);
							this.CallCmdUse294(raycastHit.transform.name);
							component.NetworkcurItem = -1;
							break;
						}
					}
				}
				else
				{
					HintManager.singleton.AddHint(3);
				}
			}
			else if (raycastHit.transform.CompareTag("FemurBreaker"))
			{
				this.CallCmdConatin106();
			}
		}
	}

	[Command(channel = 4)]
	private void CmdChange914_State(string label)
	{
		GameObject gameObject = GameObject.Find(label);
		if (this.ChckDis(gameObject.transform.position))
		{
			gameObject.GetComponentInParent<Scp914>().IncrementState();
		}
	}

	[Command(channel = 4)]
	private void CmdUseElevator(GameObject elevator)
	{
		foreach (Lift.Elevator elevator2 in elevator.GetComponent<Lift>().elevators)
		{
			if (this.ChckDis(elevator2.door.transform.position))
			{
				elevator.GetComponent<Lift>().UseLift();
			}
		}
	}

	[Command(channel = 4)]
	private void CmdSwitchAWButton(string label)
	{
		GameObject gameObject = GameObject.Find("DetonationController");
		if (this.ChckDis(gameObject.transform.position))
		{
			foreach (string a in this.inv.availableItems[base.GetComponent<AnimationController>().item].permissions)
			{
				if (a == "CONT_LVL_3")
				{
					gameObject.GetComponentInParent<AlphaWarheadButtonUnlocker>().ChangeButtonStage(label);
					return;
				}
			}
		}
	}

	[Command(channel = 4)]
	private void CmdDetonateWarhead()
	{
		if (this.ChckDis(GameObject.Find("DetonationController").transform.position))
		{
			GameObject.Find("Host").GetComponent<AlphaWarheadDetonationController>().StartDetonation();
		}
	}

	[Command(channel = 14)]
	private void CmdOpenDoor(GameObject doorID)
	{
		bool flag = false;
		Door component = doorID.GetComponent<Door>();
		if (component.buttons.Count == 0)
		{
			flag = this.ChckDis(doorID.transform.position);
		}
		if (!flag)
		{
			foreach (GameObject gameObject in component.buttons)
			{
				if (flag = this.ChckDis(gameObject.transform.position))
				{
					break;
				}
			}
		}
		if (flag)
		{
			if (component.permissionLevel.ToUpper() == "CHCKPOINT_ACC" && base.GetComponent<CharacterClassManager>().klasy[base.GetComponent<CharacterClassManager>().curClass].team == Team.SCP)
			{
				component.ChangeState();
				return;
			}
			try
			{
				if (string.IsNullOrEmpty(component.permissionLevel))
				{
					component.ChangeState();
				}
				else
				{
					foreach (string a in this.inv.availableItems[base.GetComponent<AnimationController>().item].permissions)
					{
						if (a == component.permissionLevel)
						{
							component.ChangeState();
							return;
						}
					}
					this.CallRpcDenied(doorID);
				}
			}
			catch
			{
				this.CallRpcDenied(doorID);
			}
		}
	}

	[ClientRpc(channel = 14)]
	private void RpcDenied(GameObject door)
	{
		base.StartCoroutine(door.GetComponent<Door>().Denied());
	}

	private bool ChckDis(Vector3 pos)
	{
		return TutorialManager.status || Vector3.Distance(base.GetComponent<PlyMovementSync>().position, pos) < this.raycastMaxDistance * 1.5f;
	}

	[Command(channel = 4)]
	private void CmdChangeLeverState(GameObject lever)
	{
		LeverButton component = lever.GetComponent<LeverButton>();
		if (this.ChckDis(component.lever.transform.position))
		{
			component.Switch();
		}
	}

	[Command(channel = 4)]
	private void CmdConatin106()
	{
		if (this.ChckDis(GameObject.FindGameObjectWithTag("FemurBreaker").transform.position) && !UnityEngine.Object.FindObjectOfType<OneOhSixContainer>().used && UnityEngine.Object.FindObjectOfType<LureSubjectContainer>().allowContain)
		{
			this.CallRpcContain106();
			UnityEngine.Object.FindObjectOfType<OneOhSixContainer>().SetState(true);
			base.StartCoroutine(this.Kill106());
		}
	}

	private IEnumerator Kill106()
	{
		yield return new WaitForSeconds(20f);
		foreach (GameObject gameObject in PlayerManager.singleton.players)
		{
			CharacterClassManager component = gameObject.GetComponent<CharacterClassManager>();
			if (component.curClass == 3)
			{
				component.SetPlayersClass(2, gameObject);
				gameObject.GetComponent<Scp106PlayerScript>().CallRpcAnnounceContaining();
			}
		}
		yield break;
	}

	[ClientRpc(channel = 4)]
	private void RpcContain106()
	{
		bool flag = false;
		foreach (GameObject gameObject in PlayerManager.singleton.players)
		{
			if (gameObject.GetComponent<CharacterClassManager>().curClass == 3)
			{
				gameObject.GetComponent<Scp106PlayerScript>().Contain(true);
				flag = true;
			}
		}
		if (!flag && base.isLocalPlayer)
		{
			base.GetComponent<Scp106PlayerScript>().Contain(false);
		}
	}

	[Command(channel = 4)]
	private void CmdUse294(string label)
	{
		GameObject.Find(label).GetComponent<Scp294>().Buy();
	}

	private void Start()
	{
		this.inv = base.GetComponent<Inventory>();
	}

	private void DisableDeniedText()
	{
		GameObject.Find("Keycard Denied Text").GetComponent<Text>().enabled = false;
		HintManager.singleton.AddHint(1);
	}

	private void DisableAlphaText()
	{
		GameObject.Find("Alpha Denied Text").GetComponent<Text>().enabled = false;
		HintManager.singleton.AddHint(2);
	}

	private void DisableLockText()
	{
		GameObject.Find("Lock Denied Text").GetComponent<Text>().enabled = false;
	}

	private void UNetVersion()
	{
	}

	protected static void InvokeCmdCmdChange914_State(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdChange914_State called on client.");
			return;
		}
		((PlayerInteract)obj).CmdChange914_State(reader.ReadString());
	}

	protected static void InvokeCmdCmdUseElevator(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdUseElevator called on client.");
			return;
		}
		((PlayerInteract)obj).CmdUseElevator(reader.ReadGameObject());
	}

	protected static void InvokeCmdCmdSwitchAWButton(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSwitchAWButton called on client.");
			return;
		}
		((PlayerInteract)obj).CmdSwitchAWButton(reader.ReadString());
	}

	protected static void InvokeCmdCmdDetonateWarhead(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdDetonateWarhead called on client.");
			return;
		}
		((PlayerInteract)obj).CmdDetonateWarhead();
	}

	protected static void InvokeCmdCmdOpenDoor(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdOpenDoor called on client.");
			return;
		}
		((PlayerInteract)obj).CmdOpenDoor(reader.ReadGameObject());
	}

	protected static void InvokeCmdCmdChangeLeverState(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdChangeLeverState called on client.");
			return;
		}
		((PlayerInteract)obj).CmdChangeLeverState(reader.ReadGameObject());
	}

	protected static void InvokeCmdCmdConatin106(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdConatin106 called on client.");
			return;
		}
		((PlayerInteract)obj).CmdConatin106();
	}

	protected static void InvokeCmdCmdUse294(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdUse294 called on client.");
			return;
		}
		((PlayerInteract)obj).CmdUse294(reader.ReadString());
	}

	public void CallCmdChange914_State(string label)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdChange914_State called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdChange914_State(label);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)PlayerInteract.kCmdCmdChange914_State);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(label);
		base.SendCommandInternal(networkWriter, 4, "CmdChange914_State");
	}

	public void CallCmdUseElevator(GameObject elevator)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdUseElevator called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdUseElevator(elevator);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)PlayerInteract.kCmdCmdUseElevator);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(elevator);
		base.SendCommandInternal(networkWriter, 4, "CmdUseElevator");
	}

	public void CallCmdSwitchAWButton(string label)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdSwitchAWButton called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdSwitchAWButton(label);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)PlayerInteract.kCmdCmdSwitchAWButton);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(label);
		base.SendCommandInternal(networkWriter, 4, "CmdSwitchAWButton");
	}

	public void CallCmdDetonateWarhead()
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdDetonateWarhead called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdDetonateWarhead();
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)PlayerInteract.kCmdCmdDetonateWarhead);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		base.SendCommandInternal(networkWriter, 4, "CmdDetonateWarhead");
	}

	public void CallCmdOpenDoor(GameObject doorID)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdOpenDoor called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdOpenDoor(doorID);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)PlayerInteract.kCmdCmdOpenDoor);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(doorID);
		base.SendCommandInternal(networkWriter, 14, "CmdOpenDoor");
	}

	public void CallCmdChangeLeverState(GameObject lever)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdChangeLeverState called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdChangeLeverState(lever);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)PlayerInteract.kCmdCmdChangeLeverState);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(lever);
		base.SendCommandInternal(networkWriter, 4, "CmdChangeLeverState");
	}

	public void CallCmdConatin106()
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdConatin106 called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdConatin106();
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)PlayerInteract.kCmdCmdConatin106);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		base.SendCommandInternal(networkWriter, 4, "CmdConatin106");
	}

	public void CallCmdUse294(string label)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdUse294 called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdUse294(label);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)PlayerInteract.kCmdCmdUse294);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(label);
		base.SendCommandInternal(networkWriter, 4, "CmdUse294");
	}

	protected static void InvokeRpcRpcDenied(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcDenied called on server.");
			return;
		}
		((PlayerInteract)obj).RpcDenied(reader.ReadGameObject());
	}

	protected static void InvokeRpcRpcContain106(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcContain106 called on server.");
			return;
		}
		((PlayerInteract)obj).RpcContain106();
	}

	public void CallRpcDenied(GameObject door)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("RPC Function RpcDenied called on client.");
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)2));
		networkWriter.WritePackedUInt32((uint)PlayerInteract.kRpcRpcDenied);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(door);
		this.SendRPCInternal(networkWriter, 14, "RpcDenied");
	}

	public void CallRpcContain106()
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("RPC Function RpcContain106 called on client.");
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)2));
		networkWriter.WritePackedUInt32((uint)PlayerInteract.kRpcRpcContain106);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		this.SendRPCInternal(networkWriter, 4, "RpcContain106");
	}

	static PlayerInteract()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(PlayerInteract), PlayerInteract.kCmdCmdChange914_State, new NetworkBehaviour.CmdDelegate(PlayerInteract.InvokeCmdCmdChange914_State));
		PlayerInteract.kCmdCmdUseElevator = 339400830;
		NetworkBehaviour.RegisterCommandDelegate(typeof(PlayerInteract), PlayerInteract.kCmdCmdUseElevator, new NetworkBehaviour.CmdDelegate(PlayerInteract.InvokeCmdCmdUseElevator));
		PlayerInteract.kCmdCmdSwitchAWButton = -710673229;
		NetworkBehaviour.RegisterCommandDelegate(typeof(PlayerInteract), PlayerInteract.kCmdCmdSwitchAWButton, new NetworkBehaviour.CmdDelegate(PlayerInteract.InvokeCmdCmdSwitchAWButton));
		PlayerInteract.kCmdCmdDetonateWarhead = -151679759;
		NetworkBehaviour.RegisterCommandDelegate(typeof(PlayerInteract), PlayerInteract.kCmdCmdDetonateWarhead, new NetworkBehaviour.CmdDelegate(PlayerInteract.InvokeCmdCmdDetonateWarhead));
		PlayerInteract.kCmdCmdOpenDoor = 1645579471;
		NetworkBehaviour.RegisterCommandDelegate(typeof(PlayerInteract), PlayerInteract.kCmdCmdOpenDoor, new NetworkBehaviour.CmdDelegate(PlayerInteract.InvokeCmdCmdOpenDoor));
		PlayerInteract.kCmdCmdChangeLeverState = -1161998418;
		NetworkBehaviour.RegisterCommandDelegate(typeof(PlayerInteract), PlayerInteract.kCmdCmdChangeLeverState, new NetworkBehaviour.CmdDelegate(PlayerInteract.InvokeCmdCmdChangeLeverState));
		PlayerInteract.kCmdCmdConatin106 = 1945901204;
		NetworkBehaviour.RegisterCommandDelegate(typeof(PlayerInteract), PlayerInteract.kCmdCmdConatin106, new NetworkBehaviour.CmdDelegate(PlayerInteract.InvokeCmdCmdConatin106));
		PlayerInteract.kCmdCmdUse294 = -1419329187;
		NetworkBehaviour.RegisterCommandDelegate(typeof(PlayerInteract), PlayerInteract.kCmdCmdUse294, new NetworkBehaviour.CmdDelegate(PlayerInteract.InvokeCmdCmdUse294));
		PlayerInteract.kRpcRpcDenied = -1136563096;
		NetworkBehaviour.RegisterRpcDelegate(typeof(PlayerInteract), PlayerInteract.kRpcRpcDenied, new NetworkBehaviour.CmdDelegate(PlayerInteract.InvokeRpcRpcDenied));
		PlayerInteract.kRpcRpcContain106 = -1051575568;
		NetworkBehaviour.RegisterRpcDelegate(typeof(PlayerInteract), PlayerInteract.kRpcRpcContain106, new NetworkBehaviour.CmdDelegate(PlayerInteract.InvokeRpcRpcContain106));
		NetworkCRC.RegisterBehaviour("PlayerInteract", 0);
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		bool result;
		return result;
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
	}

	public GameObject playerCamera;

	public LayerMask mask;

	public float raycastMaxDistance;

	private Inventory inv;

	private static int kCmdCmdChange914_State = -1072213689;

	private static int kCmdCmdUseElevator;

	private static int kCmdCmdSwitchAWButton;

	private static int kCmdCmdDetonateWarhead;

	private static int kCmdCmdOpenDoor;

	private static int kRpcRpcDenied;

	private static int kCmdCmdChangeLeverState;

	private static int kCmdCmdConatin106;

	private static int kRpcRpcContain106;

	private static int kCmdCmdUse294;
}
