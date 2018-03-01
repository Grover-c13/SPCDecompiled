using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Searching : NetworkBehaviour
{
	public Searching()
	{
	}

	private void Start()
	{
		this.fpc = base.GetComponent<FirstPersonController>();
		this.cam = base.GetComponent<Scp049PlayerScript>().plyCam.transform;
		this.ccm = base.GetComponent<CharacterClassManager>();
		this.inv = base.GetComponent<Inventory>();
		this.overloaderror = UserMainInterface.singleton.overloadMsg;
		this.progress = UserMainInterface.singleton.searchProgress;
		this.progressGO = UserMainInterface.singleton.searchOBJ;
		this.ammobox = base.GetComponent<AmmoBox>();
	}

	public void Init(bool isNotHuman)
	{
		this.isHuman = !isNotHuman;
	}

	private void Update()
	{
		if (base.isLocalPlayer)
		{
			this.Raycast();
			this.ContinuePickup();
			this.ErrorMessage();
		}
	}

	public void ShowErrorMessage()
	{
		this.errorMsgDur = 2f;
	}

	private void ErrorMessage()
	{
		if (this.errorMsgDur > 0f)
		{
			this.errorMsgDur -= Time.deltaTime;
		}
		this.overloaderror.SetActive(this.errorMsgDur > 0f);
	}

	private void ContinuePickup()
	{
		if (this.pickup != null)
		{
			if (!Input.GetButton("Interact"))
			{
				this.pickup = null;
				this.fpc.isSearching = false;
				this.progressGO.SetActive(false);
				return;
			}
			this.timeToPickUp -= Time.deltaTime;
			this.progressGO.SetActive(true);
			this.progress.value = this.progress.maxValue - this.timeToPickUp;
			if (this.timeToPickUp <= 0f)
			{
				this.progressGO.SetActive(false);
				this.CallCmdPickupItem(this.pickup, base.gameObject);
				this.fpc.isSearching = false;
				this.pickup = null;
			}
		}
		else
		{
			this.fpc.isSearching = false;
			this.progressGO.SetActive(false);
		}
	}

	private void Raycast()
	{
		RaycastHit raycastHit;
		if (Input.GetButtonDown("Interact") && this.AllowPickup() && Physics.Raycast(new Ray(this.cam.position, this.cam.forward), out raycastHit, this.rayDistance, base.GetComponent<PlayerInteract>().mask))
		{
			Pickup componentInParent = raycastHit.transform.GetComponentInParent<Pickup>();
			Locker componentInParent2 = raycastHit.transform.GetComponentInParent<Locker>();
			if (componentInParent != null)
			{
				if (this.inv.items.Count < 8 || this.inv.availableItems[componentInParent.id].noEquipable)
				{
					this.timeToPickUp = componentInParent.searchTime;
					this.progress.maxValue = componentInParent.searchTime;
					this.fpc.isSearching = true;
					this.pickup = componentInParent.gameObject;
				}
				else
				{
					this.ShowErrorMessage();
				}
			}
			if (componentInParent2 != null)
			{
				if (this.inv.items.Count < 8)
				{
					this.timeToPickUp = componentInParent2.searchTime;
					this.progress.maxValue = componentInParent2.searchTime;
					this.fpc.isSearching = true;
					this.pickup = componentInParent2.gameObject;
				}
				else
				{
					this.ShowErrorMessage();
				}
			}
		}
	}

	private bool AllowPickup()
	{
		if (!this.isHuman)
		{
			return false;
		}
		GameObject[] players = PlayerManager.singleton.players;
		foreach (GameObject gameObject in players)
		{
			if (gameObject.GetComponent<Handcuffs>().cuffTarget == base.gameObject)
			{
				return false;
			}
		}
		return true;
	}

	[Command(channel = 2)]
	private void CmdPickupItem(GameObject t, GameObject taker)
	{
		int id = 0;
		Pickup component = t.GetComponent<Pickup>();
		if (component != null)
		{
			id = component.id;
			component.PickupItem();
		}
		Locker component2 = t.GetComponent<Locker>();
		if (component2 != null)
		{
			id = component2.GetItem();
			component2.SetTaken(true);
		}
		this.CallRpcPickupItem(taker, id, (!(t.GetComponent<Pickup>() == null)) ? component.durability : -1f);
	}

	[ClientRpc(channel = 2)]
	private void RpcPickupItem(GameObject who, int id, float dur)
	{
		if (who == null)
		{
			return;
		}
		if (who.GetComponent<Locker>() != null && who.GetComponent<Locker>().isTaken)
		{
			return;
		}
		who.GetComponent<Searching>().AddItem(id, dur);
	}

	public void AddItem(int id, float dur)
	{
		if (base.isLocalPlayer)
		{
			if (!this.inv.availableItems[id].noEquipable)
			{
				this.inv.AddItem(id, (dur != -1f) ? dur : this.inv.availableItems[id].durability);
			}
			else
			{
				foreach (AmmoBox.AmmoType ammoType in this.ammobox.types)
				{
					if (ammoType.inventoryID == id)
					{
						ammoType.quantity += (int)dur;
					}
				}
			}
		}
	}

	private void UNetVersion()
	{
	}

	protected static void InvokeCmdCmdPickupItem(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPickupItem called on client.");
			return;
		}
		((Searching)obj).CmdPickupItem(reader.ReadGameObject(), reader.ReadGameObject());
	}

	public void CallCmdPickupItem(GameObject t, GameObject taker)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdPickupItem called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdPickupItem(t, taker);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)Searching.kCmdCmdPickupItem);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(t);
		networkWriter.Write(taker);
		base.SendCommandInternal(networkWriter, 2, "CmdPickupItem");
	}

	protected static void InvokeRpcRpcPickupItem(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcPickupItem called on server.");
			return;
		}
		((Searching)obj).RpcPickupItem(reader.ReadGameObject(), (int)reader.ReadPackedUInt32(), reader.ReadSingle());
	}

	public void CallRpcPickupItem(GameObject who, int id, float dur)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("RPC Function RpcPickupItem called on client.");
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)2));
		networkWriter.WritePackedUInt32((uint)Searching.kRpcRpcPickupItem);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(who);
		networkWriter.WritePackedUInt32((uint)id);
		networkWriter.Write(dur);
		this.SendRPCInternal(networkWriter, 2, "RpcPickupItem");
	}

	static Searching()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(Searching), Searching.kCmdCmdPickupItem, new NetworkBehaviour.CmdDelegate(Searching.InvokeCmdCmdPickupItem));
		Searching.kRpcRpcPickupItem = -114936833;
		NetworkBehaviour.RegisterRpcDelegate(typeof(Searching), Searching.kRpcRpcPickupItem, new NetworkBehaviour.CmdDelegate(Searching.InvokeRpcRpcPickupItem));
		NetworkCRC.RegisterBehaviour("Searching", 0);
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		bool result;
		return result;
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
	}

	private CharacterClassManager ccm;

	private Inventory inv;

	private bool isHuman;

	private GameObject pickup;

	private Transform cam;

	private FirstPersonController fpc;

	private AmmoBox ammobox;

	private float timeToPickUp;

	private float errorMsgDur;

	private GameObject overloaderror;

	private Slider progress;

	private GameObject progressGO;

	public float rayDistance;

	private static int kCmdCmdPickupItem = 2021286825;

	private static int kRpcRpcPickupItem;
}
