using System;
using Unity;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Inventory : NetworkBehaviour
{
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

	public int NetworkitemUniq
	{
		get
		{
			return this.itemUniq;
		}
		set
		{
			uint dirtyBit = 4u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SetUniq(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<int>(value, ref this.itemUniq, dirtyBit);
		}
	}

	public Inventory()
	{
	}

	public void SetUniq(int i)
	{
		this.NetworkitemUniq = i;
	}

	[Command(channel = 2)]
	public void CmdSetUnic(int i)
	{
		this.NetworkitemUniq = i;
	}

	private void Awake()
	{
		for (int i = 0; i < this.availableItems.Length; i++)
		{
			this.availableItems[i].id = i;
		}
		this.items.InitializeBehaviour(this, Inventory.kListitems);
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
			UnityEngine.Object.FindObjectOfType<InventoryDisplay>().localplayer = this;
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
			this.CallCmdDropItem(id, this.items[id].id);
		}
	}

	public void ServerDropAll()
	{
		foreach (Inventory.SyncItemInfo syncItemInfo in this.items)
		{
			this.SetPickup(syncItemInfo.id, syncItemInfo.durability, base.transform.position, this.kamera.transform.rotation, base.transform.rotation);
		}
		AmmoBox component = base.GetComponent<AmmoBox>();
		for (int i = 0; i < 3; i++)
		{
			if (component.GetAmmo(i) != 0)
			{
				this.SetPickup(component.types[i].inventoryID, (float)component.GetAmmo(i), base.transform.position, this.kamera.transform.rotation, base.transform.rotation);
			}
		}
		this.items.Clear();
		component.Networkamount = "0:0:0";
	}

	public int GetItemIndex()
	{
		int num = 0;
		foreach (Inventory.SyncItemInfo syncItemInfo in this.items)
		{
			if (this.itemUniq == syncItemInfo.uniq)
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	public void AddNewItem(int id, float dur = -4.65664672E+11f)
	{
		Inventory.uniqid++;
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
		if (this.items.Count < 8 || item.noEquipable)
		{
			if (dur != -4.65664672E+11f)
			{
				item.durability = dur;
			}
			this.items.Add(new Inventory.SyncItemInfo
			{
				id = item.id,
				durability = item.durability,
				uniq = Inventory.uniqid
			});
		}
	}

	[Command(channel = 3)]
	private void CmdSyncItem(int i)
	{
		foreach (Inventory.SyncItemInfo syncItemInfo in this.items)
		{
			if (syncItemInfo.id == i)
			{
				this.NetworkcurItem = i;
				return;
			}
		}
		this.NetworkcurItem = -1;
	}

	private void Update()
	{
		if (base.isLocalPlayer)
		{
			this.CallCmdSyncItem(this.curItem);
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
	private void CmdDropItem(int itemInventoryIndex, int itemId)
	{
		if (this.items[itemInventoryIndex].id == itemId)
		{
			this.SetPickup(this.items[itemInventoryIndex].id, this.items[itemInventoryIndex].durability, base.transform.position, this.kamera.transform.rotation, base.transform.rotation);
			this.items.RemoveAt(itemInventoryIndex);
		}
	}

	public void SetPickup(int dropedItemID, float dur, Vector3 pos, Quaternion camRot, Quaternion myRot)
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

	protected static void InvokeSyncListitems(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("SyncList items called on server.");
			return;
		}
		((Inventory)obj).items.HandleMsg(reader);
	}

	protected static void InvokeCmdCmdSetUnic(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSetUnic called on client.");
			return;
		}
		((Inventory)obj).CmdSetUnic((int)reader.ReadPackedUInt32());
	}

	protected static void InvokeCmdCmdSyncItem(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSyncItem called on client.");
			return;
		}
		((Inventory)obj).CmdSyncItem((int)reader.ReadPackedUInt32());
	}

	protected static void InvokeCmdCmdDropItem(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdDropItem called on client.");
			return;
		}
		((Inventory)obj).CmdDropItem((int)reader.ReadPackedUInt32(), (int)reader.ReadPackedUInt32());
	}

	public void CallCmdSetUnic(int i)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdSetUnic called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdSetUnic(i);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)Inventory.kCmdCmdSetUnic);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.WritePackedUInt32((uint)i);
		base.SendCommandInternal(networkWriter, 2, "CmdSetUnic");
	}

	public void CallCmdSyncItem(int i)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdSyncItem called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdSyncItem(i);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)Inventory.kCmdCmdSyncItem);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.WritePackedUInt32((uint)i);
		base.SendCommandInternal(networkWriter, 3, "CmdSyncItem");
	}

	public void CallCmdDropItem(int itemInventoryIndex, int itemId)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdDropItem called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdDropItem(itemInventoryIndex, itemId);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)Inventory.kCmdCmdDropItem);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.WritePackedUInt32((uint)itemInventoryIndex);
		networkWriter.WritePackedUInt32((uint)itemId);
		base.SendCommandInternal(networkWriter, 2, "CmdDropItem");
	}

	static Inventory()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(Inventory), Inventory.kCmdCmdSetUnic, new NetworkBehaviour.CmdDelegate(Inventory.InvokeCmdCmdSetUnic));
		Inventory.kCmdCmdSyncItem = 2140153578;
		NetworkBehaviour.RegisterCommandDelegate(typeof(Inventory), Inventory.kCmdCmdSyncItem, new NetworkBehaviour.CmdDelegate(Inventory.InvokeCmdCmdSyncItem));
		Inventory.kCmdCmdDropItem = -109121218;
		NetworkBehaviour.RegisterCommandDelegate(typeof(Inventory), Inventory.kCmdCmdDropItem, new NetworkBehaviour.CmdDelegate(Inventory.InvokeCmdCmdDropItem));
		Inventory.kListitems = 1683194626;
		NetworkBehaviour.RegisterSyncListDelegate(typeof(Inventory), Inventory.kListitems, new NetworkBehaviour.CmdDelegate(Inventory.InvokeSyncListitems));
		NetworkCRC.RegisterBehaviour("Inventory", 0);
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			GeneratedNetworkCode._WriteStructSyncListItemInfo_Inventory(writer, this.items);
			writer.WritePackedUInt32((uint)this.curItem);
			writer.WritePackedUInt32((uint)this.itemUniq);
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
			GeneratedNetworkCode._WriteStructSyncListItemInfo_Inventory(writer, this.items);
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
		if ((base.syncVarDirtyBits & 4u) != 0u)
		{
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
				flag = true;
			}
			writer.WritePackedUInt32((uint)this.itemUniq);
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
			GeneratedNetworkCode._ReadStructSyncListItemInfo_Inventory(reader, this.items);
			this.curItem = (int)reader.ReadPackedUInt32();
			this.itemUniq = (int)reader.ReadPackedUInt32();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			GeneratedNetworkCode._ReadStructSyncListItemInfo_Inventory(reader, this.items);
		}
		if ((num & 2) != 0)
		{
			this.SetCurItem((int)reader.ReadPackedUInt32());
		}
		if ((num & 4) != 0)
		{
			this.SetUniq((int)reader.ReadPackedUInt32());
		}
	}

	public Inventory.SyncListItemInfo items = new Inventory.SyncListItemInfo();

	public Item[] availableItems;

	private AnimationController ac;

	[SyncVar(hook = "SetCurItem")]
	public int curItem;

	public GameObject kamera;

	[SyncVar(hook = "SetUniq")]
	public int itemUniq;

	public GameObject pickupPrefab;

	private RawImage crosshair;

	private CharacterClassManager ccm;

	private static int uniqid;

	private int prevIt = -10;

	private static int kListitems;

	private static int kCmdCmdSetUnic = 1995465433;

	private static int kCmdCmdSyncItem;

	private static int kCmdCmdDropItem;

	[Serializable]
	public struct SyncItemInfo
	{
		public int id;

		public float durability;

		public int uniq;
	}

	public class SyncListItemInfo : SyncListStruct<Inventory.SyncItemInfo>
	{
		public SyncListItemInfo()
		{
		}

		public void ModifyDuration(int index, float value)
		{
			Inventory.SyncItemInfo value2 = base[index];
			value2.durability = value;
			base[index] = value2;
		}

		public override void SerializeItem(NetworkWriter writer, Inventory.SyncItemInfo item)
		{
			writer.WritePackedUInt32((uint)item.id);
			writer.Write(item.durability);
			writer.WritePackedUInt32((uint)item.uniq);
		}

		public override Inventory.SyncItemInfo DeserializeItem(NetworkReader reader)
		{
			return new Inventory.SyncItemInfo
			{
				id = (int)reader.ReadPackedUInt32(),
				durability = reader.ReadSingle(),
				uniq = (int)reader.ReadPackedUInt32()
			};
		}
	}
}
