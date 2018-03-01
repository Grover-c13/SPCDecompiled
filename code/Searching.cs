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
				this.CallCmdPickupItem(this.pickup);
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
	private void CmdPickupItem(GameObject t)
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
		this.AddItem(id, (!(t.GetComponent<Pickup>() == null)) ? component.durability : -1f);
	}

	public void AddItem(int id, float dur)
	{
		if (id != -1)
		{
			if (!this.inv.availableItems[id].noEquipable)
			{
				this.inv.AddNewItem(id, (dur != -1f) ? dur : this.inv.availableItems[id].durability);
			}
			else
			{
				string[] array = this.ammobox.amount.Split(new char[]
				{
					':'
				});
				for (int i = 0; i < 3; i++)
				{
					if (this.ammobox.types[i].inventoryID == id)
					{
						array[i] = ((float)this.ammobox.GetAmmo(i) + dur).ToString();
					}
				}
				this.ammobox.Networkamount = string.Concat(new string[]
				{
					array[0],
					":",
					array[1],
					":",
					array[2]
				});
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
		((Searching)obj).CmdPickupItem(reader.ReadGameObject());
	}

	public void CallCmdPickupItem(GameObject t)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdPickupItem called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdPickupItem(t);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)Searching.kCmdCmdPickupItem);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(t);
		base.SendCommandInternal(networkWriter, 2, "CmdPickupItem");
	}

	static Searching()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(Searching), Searching.kCmdCmdPickupItem, new NetworkBehaviour.CmdDelegate(Searching.InvokeCmdCmdPickupItem));
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
}
