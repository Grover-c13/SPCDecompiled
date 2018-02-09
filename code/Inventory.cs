using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Inventory : NetworkBehaviour
{
	private void SyncVerItems(SyncListInt i)
	{
		this.verifiedItems = i;
	}

	private void Awake()
	{
		for (int i = 0; i < this.availableItems.Length; i++)
		{
			this.availableItems[i].id = i;
		}
		this.verifiedItems.InitializeBehaviour(this, Inventory.kListverifiedItems);
	}

	private void Log(string msg)
	{
	}

	public void SetCurItem(int ci)
	{
		if (base.GetComponent<MicroHID_GFX>().onFire)
		{
			return;
		}
		this.NetworkcurItem = ci;
	}

	private void Start()
	{
		if (base.isLocalPlayer && base.isServer)
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag("Pickup");
			foreach (GameObject gameObject in array)
			{
				gameObject.GetComponent<Pickup>().iCanSeeThatAsHost = true;
			}
		}
		this.ccm = base.GetComponent<CharacterClassManager>();
		this.crosshair = GameObject.Find("CrosshairImage").GetComponent<RawImage>();
		this.ac = base.GetComponent<AnimationController>();
		if (base.isLocalPlayer)
		{
			UnityEngine.Object.FindObjectOfType<InventoryDisplay>().localplayer = base.gameObject;
		}
	}

	private void RefreshModels()
	{
		for (int i = 0; i < this.availableItems.Length; i++)
		{
			try
			{
				this.availableItems[i].firstpersonModel.SetActive(base.isLocalPlayer & i == this.curItem);
			}
			catch
			{
			}
		}
	}

	public void DropItem(int id)
	{
		if (base.isLocalPlayer)
		{
			if (this.items[id].id == this.curItem)
			{
				this.NetworkcurItem = -1;
			}
			this.CallCmdSetPickup(this.items[id].id, this.items[id].durability, base.transform.position, this.kamera.transform.rotation, base.transform.rotation);
			this.items.RemoveAt(id);
		}
	}

	public void DropAll()
	{
		for (int i = 0; i < 20; i++)
		{
			if (this.items.Count > 0)
			{
				this.DropItem(0);
			}
		}
		AmmoBox component = base.GetComponent<AmmoBox>();
		for (int j = 0; j < component.types.Length; j++)
		{
			if (component.types[j].quantity > 0)
			{
				this.CallCmdSetPickup(component.types[j].inventoryID, (float)component.types[j].quantity, base.transform.position, this.kamera.transform.rotation, base.transform.rotation);
				component.types[j].quantity = 0;
			}
		}
	}

	public void AddItem(int id, float dur = -4.65664672E+11f)
	{
		if (base.isLocalPlayer)
		{
			if (TutorialManager.status)
			{
				PickupTrigger[] array = UnityEngine.Object.FindObjectsOfType<PickupTrigger>();
				PickupTrigger pickupTrigger = null;
				foreach (PickupTrigger pickupTrigger2 in array)
				{
					if ((pickupTrigger2.filter == -1 || pickupTrigger2.filter == id) && (pickupTrigger == null || pickupTrigger2.prioirty < pickupTrigger.prioirty))
					{
						pickupTrigger = pickupTrigger2;
					}
				}
				try
				{
					if (pickupTrigger != null)
					{
						pickupTrigger.Trigger(id);
					}
				}
				catch
				{
					MonoBehaviour.print("Error");
				}
			}
			Item item = new Item(this.availableItems[id]);
			if (base.GetComponent<Inventory>().items.Count < 8 || item.noEquipable)
			{
				if (dur != -4.65664672E+11f)
				{
					item.durability = dur;
				}
				this.items.Add(item);
			}
			else
			{
				base.GetComponent<Searching>().ShowErrorMessage();
			}
		}
	}

	private void Update()
	{
		if (TutorialManager.status && !base.isLocalPlayer)
		{
			this.ac.SyncItem(this.curItem);
		}
		if (base.isLocalPlayer)
		{
			this.ac.SyncItem(this.curItem);
			int num = Mathf.Clamp(this.curItem, 0, this.availableItems.Length - 1);
			if (this.ccm.curClass >= 0 && this.ccm.klasy[this.ccm.curClass].forcedCrosshair != -1)
			{
				num = this.ccm.klasy[this.ccm.curClass].forcedCrosshair;
			}
			this.crosshair.texture = this.availableItems[num].crosshair;
			this.crosshair.color = this.availableItems[num].crosshairColor;
		}
		if (this.prevIt != this.curItem)
		{
			this.RefreshModels();
			this.prevIt = this.curItem;
		}
	}

	[Command(channel = 2)]
	public void CmdSetPickup(int dropedItemID, float dur, Vector3 pos, Quaternion camRot, Quaternion myRot)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.pickupPrefab);
		NetworkServer.Spawn(gameObject);
		gameObject.GetComponent<Pickup>().SetDurability(dur);
		gameObject.GetComponent<Pickup>().SetID(dropedItemID);
		gameObject.GetComponent<Pickup>().SetPosition(((this.ccm.curClass != 2) ? pos : this.ccm.deathPosition) + Vector3.up * 0.9f);
		gameObject.GetComponent<Pickup>().SetRotation(new Vector3(camRot.eulerAngles.x, myRot.eulerAngles.y, 0f));
		gameObject.GetComponent<Pickup>().SetName(string.Concat(new object[]
		{
			"PICKUP#",
			dropedItemID,
			":",
			UnityEngine.Random.Range(0f, 1E+10f).ToString("0000000000")
		}));
	}

	private void UNetVersion()
	{
	}

	public int NetworkcurItem
	{
		get
		{
			return this.curItem;
		}
		set
		{
			uint dirtyBit = 2u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SetCurItem(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<int>(value, ref this.curItem, dirtyBit);
		}
	}

	protected static void InvokeSyncListverifiedItems(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("SyncList verifiedItems called on server.");
			return;
		}
		((Inventory)obj).verifiedItems.HandleMsg(reader);
	}

	protected static void InvokeCmdCmdSetPickup(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSetPickup called on client.");
			return;
		}
		((Inventory)obj).CmdSetPickup((int)reader.ReadPackedUInt32(), reader.ReadSingle(), reader.ReadVector3(), reader.ReadQuaternion(), reader.ReadQuaternion());
	}

	public void CallCmdSetPickup(int dropedItemID, float dur, Vector3 pos, Quaternion camRot, Quaternion myRot)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdSetPickup called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdSetPickup(dropedItemID, dur, pos, camRot, myRot);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)Inventory.kCmdCmdSetPickup);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.WritePackedUInt32((uint)dropedItemID);
		networkWriter.Write(dur);
		networkWriter.Write(pos);
		networkWriter.Write(camRot);
		networkWriter.Write(myRot);
		base.SendCommandInternal(networkWriter, 2, "CmdSetPickup");
	}

	static Inventory()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(Inventory), Inventory.kCmdCmdSetPickup, new NetworkBehaviour.CmdDelegate(Inventory.InvokeCmdCmdSetPickup));
		Inventory.kListverifiedItems = -1745481958;
		NetworkBehaviour.RegisterSyncListDelegate(typeof(Inventory), Inventory.kListverifiedItems, new NetworkBehaviour.CmdDelegate(Inventory.InvokeSyncListverifiedItems));
		NetworkCRC.RegisterBehaviour("Inventory", 0);
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			SyncListInt.WriteInstance(writer, this.verifiedItems);
			writer.WritePackedUInt32((uint)this.curItem);
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
			SyncListInt.WriteInstance(writer, this.verifiedItems);
		}
		if ((base.syncVarDirtyBits & 2u) != 0u)
		{
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
				flag = true;
			}
			writer.WritePackedUInt32((uint)this.curItem);
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
			SyncListInt.ReadReference(reader, this.verifiedItems);
			this.curItem = (int)reader.ReadPackedUInt32();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			SyncListInt.ReadReference(reader, this.verifiedItems);
		}
		if ((num & 2) != 0)
		{
			this.SetCurItem((int)reader.ReadPackedUInt32());
		}
	}

	public Item[] availableItems;

	public List<Item> items = new List<Item>();

	[SyncVar(hook = "SyncVerItems")]
	public SyncListInt verifiedItems = new SyncListInt();

	private AnimationController ac;

	[SyncVar(hook = "SetCurItem")]
	public int curItem;

	public GameObject kamera;

	public Item localInventoryItem;

	public GameObject pickupPrefab;

	private RawImage crosshair;

	private CharacterClassManager ccm;

	private int prevIt = -10;

	private static int kListverifiedItems;

	private static int kCmdCmdSetPickup = 1938936418;
}
