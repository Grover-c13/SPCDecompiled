using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerInteract : NetworkBehaviour
{
	public PlayerInteract()
	{
	}

	private void Update()
	{
		RaycastHit raycastHit;
		if (base.isLocalPlayer && Input.GetButtonDown("Interact") && base.GetComponent<CharacterClassManager>().curClass != 2 && Physics.Raycast(this.playerCamera.transform.position, this.playerCamera.transform.forward, out raycastHit, this.raycastMaxDistance, this.mask))
		{
			if (raycastHit.transform.GetComponentInParent<Door>() != null)
			{
				this.CallCmdOpenDoor(raycastHit.transform.GetComponentInParent<Door>().gameObject);
			}
			else if (raycastHit.transform.CompareTag("AW_Button"))
			{
				if (this.inv.curItem != 0)
				{
					foreach (string a in this.inv.availableItems[Mathf.Clamp(this.inv.curItem, 0, this.inv.availableItems.Length - 1)].permissions)
					{
						if (a == "CONT_LVL_3")
						{
							this.CallCmdSwitchAWButton();
							return;
						}
					}
				}
				GameObject.Find("Keycard Denied Text").GetComponent<Text>().enabled = true;
				base.Invoke("DisableDeniedText", 1f);
			}
			else if (raycastHit.transform.CompareTag("AW_Detonation"))
			{
				if (AlphaWarheadOutsitePanel.nukeside.enabled && !AlphaWarheadController.host.detonationInProgress)
				{
					this.CallCmdDetonateWarhead();
				}
				else
				{
					GameObject.Find("Alpha Denied Text").GetComponent<Text>().enabled = true;
					base.Invoke("DisableAlphaText", 1f);
				}
			}
			else if (raycastHit.transform.CompareTag("AW_Panel"))
			{
				this.CallCmdUsePanel(raycastHit.transform.name);
			}
			else if (raycastHit.transform.CompareTag("914_use"))
			{
				this.CallCmdUse914();
			}
			else if (raycastHit.transform.CompareTag("914_knob"))
			{
				this.CallCmdChange914knob();
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
			else if (raycastHit.transform.CompareTag("FemurBreaker"))
			{
				this.CallCmdConatin106();
			}
		}
	}

	[Command(channel = 4)]
	private void CmdUse914()
	{
		if (!Scp914.singleton.working && this.ChckDis(GameObject.FindGameObjectWithTag("914_use").transform.position))
		{
			this.CallRpcUse914();
		}
	}

	[Command(channel = 4)]
	private void CmdChange914knob()
	{
		if (!Scp914.singleton.working && this.ChckDis(GameObject.FindGameObjectWithTag("914_use").transform.position))
		{
			Scp914.singleton.ChangeKnobStatus();
		}
	}

	[ClientRpc(channel = 4)]
	private void RpcUse914()
	{
		Scp914.singleton.StartRefining();
	}

	[Command(channel = 4)]
	private void CmdUsePanel(string n)
	{
		AlphaWarheadNukesitePanel nukeside = AlphaWarheadOutsitePanel.nukeside;
		if (this.ChckDis(nukeside.transform.position))
		{
			if (n.Contains("cancel"))
			{
				AlphaWarheadController.host.CancelDetonation();
			}
			else if (n.Contains("lever") && nukeside.AllowChangeLevelState())
			{
				nukeside.Networkenabled = !nukeside.enabled;
				this.CallRpcLeverSound();
			}
		}
	}

	[ClientRpc(channel = 4)]
	private void RpcLeverSound()
	{
		AlphaWarheadOutsitePanel.nukeside.lever.GetComponent<AudioSource>().Play();
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
	private void CmdSwitchAWButton()
	{
		GameObject gameObject = GameObject.Find("OutsitePanelScript");
		if (this.ChckDis(gameObject.transform.position))
		{
			foreach (string a in this.inv.availableItems[this.inv.curItem].permissions)
			{
				if (a == "CONT_LVL_3")
				{
					gameObject.GetComponentInParent<AlphaWarheadOutsitePanel>().SetKeycardState(true);
					return;
				}
			}
		}
	}

	[Command(channel = 4)]
	private void CmdDetonateWarhead()
	{
		GameObject gameObject = GameObject.Find("OutsitePanelScript");
		if (this.ChckDis(gameObject.transform.position) && AlphaWarheadOutsitePanel.nukeside.enabled && gameObject.GetComponent<AlphaWarheadOutsitePanel>().keycardEntered)
		{
			AlphaWarheadController.host.StartDetonation();
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
			Scp096PlayerScript component2 = base.GetComponent<Scp096PlayerScript>();
			if (component.destroyedPrefab != null && (!component.isOpen || component.curCooldown > 0f) && component2.iAm096 && component2.enraged == Scp096PlayerScript.RageState.Enraged)
			{
				component.DestroyDoor(true);
				return;
			}
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
					foreach (string a in this.inv.availableItems[this.inv.curItem].permissions)
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

	protected static void InvokeCmdCmdUse914(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdUse914 called on client.");
			return;
		}
		((PlayerInteract)obj).CmdUse914();
	}

	protected static void InvokeCmdCmdChange914knob(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdChange914knob called on client.");
			return;
		}
		((PlayerInteract)obj).CmdChange914knob();
	}

	protected static void InvokeCmdCmdUsePanel(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdUsePanel called on client.");
			return;
		}
		((PlayerInteract)obj).CmdUsePanel(reader.ReadString());
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
		((PlayerInteract)obj).CmdSwitchAWButton();
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

	protected static void InvokeCmdCmdConatin106(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdConatin106 called on client.");
			return;
		}
		((PlayerInteract)obj).CmdConatin106();
	}

	public void CallCmdUse914()
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdUse914 called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdUse914();
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)PlayerInteract.kCmdCmdUse914);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		base.SendCommandInternal(networkWriter, 4, "CmdUse914");
	}

	public void CallCmdChange914knob()
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdChange914knob called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdChange914knob();
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)PlayerInteract.kCmdCmdChange914knob);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		base.SendCommandInternal(networkWriter, 4, "CmdChange914knob");
	}

	public void CallCmdUsePanel(string n)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdUsePanel called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdUsePanel(n);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)PlayerInteract.kCmdCmdUsePanel);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(n);
		base.SendCommandInternal(networkWriter, 4, "CmdUsePanel");
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

	public void CallCmdSwitchAWButton()
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdSwitchAWButton called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdSwitchAWButton();
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)PlayerInteract.kCmdCmdSwitchAWButton);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
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

	protected static void InvokeRpcRpcUse914(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcUse914 called on server.");
			return;
		}
		((PlayerInteract)obj).RpcUse914();
	}

	protected static void InvokeRpcRpcLeverSound(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcLeverSound called on server.");
			return;
		}
		((PlayerInteract)obj).RpcLeverSound();
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

	public void CallRpcUse914()
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("RPC Function RpcUse914 called on client.");
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)2));
		networkWriter.WritePackedUInt32((uint)PlayerInteract.kRpcRpcUse914);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		this.SendRPCInternal(networkWriter, 4, "RpcUse914");
	}

	public void CallRpcLeverSound()
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("RPC Function RpcLeverSound called on client.");
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)2));
		networkWriter.WritePackedUInt32((uint)PlayerInteract.kRpcRpcLeverSound);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		this.SendRPCInternal(networkWriter, 4, "RpcLeverSound");
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
		NetworkBehaviour.RegisterCommandDelegate(typeof(PlayerInteract), PlayerInteract.kCmdCmdUse914, new NetworkBehaviour.CmdDelegate(PlayerInteract.InvokeCmdCmdUse914));
		PlayerInteract.kCmdCmdChange914knob = -845424245;
		NetworkBehaviour.RegisterCommandDelegate(typeof(PlayerInteract), PlayerInteract.kCmdCmdChange914knob, new NetworkBehaviour.CmdDelegate(PlayerInteract.InvokeCmdCmdChange914knob));
		PlayerInteract.kCmdCmdUsePanel = 1853207668;
		NetworkBehaviour.RegisterCommandDelegate(typeof(PlayerInteract), PlayerInteract.kCmdCmdUsePanel, new NetworkBehaviour.CmdDelegate(PlayerInteract.InvokeCmdCmdUsePanel));
		PlayerInteract.kCmdCmdUseElevator = 339400830;
		NetworkBehaviour.RegisterCommandDelegate(typeof(PlayerInteract), PlayerInteract.kCmdCmdUseElevator, new NetworkBehaviour.CmdDelegate(PlayerInteract.InvokeCmdCmdUseElevator));
		PlayerInteract.kCmdCmdSwitchAWButton = -710673229;
		NetworkBehaviour.RegisterCommandDelegate(typeof(PlayerInteract), PlayerInteract.kCmdCmdSwitchAWButton, new NetworkBehaviour.CmdDelegate(PlayerInteract.InvokeCmdCmdSwitchAWButton));
		PlayerInteract.kCmdCmdDetonateWarhead = -151679759;
		NetworkBehaviour.RegisterCommandDelegate(typeof(PlayerInteract), PlayerInteract.kCmdCmdDetonateWarhead, new NetworkBehaviour.CmdDelegate(PlayerInteract.InvokeCmdCmdDetonateWarhead));
		PlayerInteract.kCmdCmdOpenDoor = 1645579471;
		NetworkBehaviour.RegisterCommandDelegate(typeof(PlayerInteract), PlayerInteract.kCmdCmdOpenDoor, new NetworkBehaviour.CmdDelegate(PlayerInteract.InvokeCmdCmdOpenDoor));
		PlayerInteract.kCmdCmdConatin106 = 1945901204;
		NetworkBehaviour.RegisterCommandDelegate(typeof(PlayerInteract), PlayerInteract.kCmdCmdConatin106, new NetworkBehaviour.CmdDelegate(PlayerInteract.InvokeCmdCmdConatin106));
		PlayerInteract.kRpcRpcUse914 = -637254142;
		NetworkBehaviour.RegisterRpcDelegate(typeof(PlayerInteract), PlayerInteract.kRpcRpcUse914, new NetworkBehaviour.CmdDelegate(PlayerInteract.InvokeRpcRpcUse914));
		PlayerInteract.kRpcRpcLeverSound = -829118990;
		NetworkBehaviour.RegisterRpcDelegate(typeof(PlayerInteract), PlayerInteract.kRpcRpcLeverSound, new NetworkBehaviour.CmdDelegate(PlayerInteract.InvokeRpcRpcLeverSound));
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

	private static int kCmdCmdUse914 = -1419322708;

	private static int kCmdCmdChange914knob;

	private static int kRpcRpcUse914;

	private static int kCmdCmdUsePanel;

	private static int kRpcRpcLeverSound;

	private static int kCmdCmdUseElevator;

	private static int kCmdCmdSwitchAWButton;

	private static int kCmdCmdDetonateWarhead;

	private static int kCmdCmdOpenDoor;

	private static int kRpcRpcDenied;

	private static int kCmdCmdConatin106;

	private static int kRpcRpcContain106;
}
