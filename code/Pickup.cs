using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Pickup : NetworkBehaviour
{
	public int Networkid
	{
		get
		{
			return this.id;
		}
		set
		{
			uint dirtyBit = 1u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SetID(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<int>(value, ref this.id, dirtyBit);
		}
	}

	public string NetworkmyName
	{
		get
		{
			return this.myName;
		}
		set
		{
			uint dirtyBit = 2u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SetName(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<string>(value, ref this.myName, dirtyBit);
		}
	}

	public float Networkdurability
	{
		get
		{
			return this.durability;
		}
		set
		{
			uint dirtyBit = 4u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SetDurability(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<float>(value, ref this.durability, dirtyBit);
		}
	}

	public Vector3 Networkpos
	{
		get
		{
			return this.pos;
		}
		set
		{
			uint dirtyBit = 8u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SetPosition(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<Vector3>(value, ref this.pos, dirtyBit);
		}
	}

	public Vector3 Networkrotation
	{
		get
		{
			return this.rotation;
		}
		set
		{
			uint dirtyBit = 16u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SetRotation(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<Vector3>(value, ref this.rotation, dirtyBit);
		}
	}

	public Pickup()
	{
	}

	public void SetRotation(Vector3 rot)
	{
		this.Networkrotation = rot;
		base.transform.rotation = Quaternion.Euler(rot);
		this.RefreshPrefab();
	}

	public void KeepRotation(int seconds)
	{
		base.StartCoroutine(this.IKeepRotation(seconds));
	}

	public void KeepEverything()
	{
		base.StartCoroutine(this.IKeepEverything());
	}

	private IEnumerator IKeepRotation(int seconds)
	{
		for (int i = 0; i < seconds; i++)
		{
			base.transform.rotation = Quaternion.Euler(this.rotation);
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	private IEnumerator IKeepEverything()
	{
		for (int i = 0; i < 10; i++)
		{
			base.transform.position = this.pos;
			base.transform.rotation = Quaternion.Euler(this.rotation);
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	public void SetPosition(Vector3 p)
	{
		this.Networkpos = p;
		base.GetComponent<Rigidbody>().velocity = Vector3.zero;
		base.transform.position = this.pos;
		this.RefreshPrefab();
	}

	private void Start()
	{
		base.StartCoroutine(this.InstantiateScript());
		if (this.startItem)
		{
			base.StartCoroutine(this.IKeepEverything());
		}
	}

	private IEnumerator InstantiateScript()
	{
		while (!this.SetAvItems())
		{
			yield return new WaitForEndOfFrame();
		}
		base.InvokeRepeating("RefreshPrefab", 1f, 6f);
		yield break;
	}

	private bool SetAvItems()
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("Player");
		if (gameObject != null)
		{
			this.avItems = gameObject.GetComponent<Inventory>().availableItems;
			return true;
		}
		return false;
	}

	private void RefreshPrefab()
	{
		this.SetAvItems();
		if (this.myModel != null)
		{
			UnityEngine.Object.Destroy(this.myModel);
		}
		this.myModel = UnityEngine.Object.Instantiate<GameObject>(this.avItems[this.id].prefab, base.transform);
		this.myModel.transform.localPosition = Vector3.zero;
		if (base.transform.position.y < -10000f)
		{
			base.transform.position = new Vector3(base.transform.position.x, this.pos.y, base.transform.position.z);
		}
		this.searchTime = this.avItems[this.id].pickingtime;
	}

	public void SetID(int itemid)
	{
		this.Networkid = itemid;
		this.RefreshPrefab();
	}

	public void SetName(string n)
	{
		this.NetworkmyName = n;
		base.name = n;
	}

	public void SetDurability(float dur)
	{
		this.Networkdurability = dur;
	}

	public void PickupItem()
	{
		NetworkServer.Destroy(base.gameObject);
	}

	private void UNetVersion()
	{
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			writer.WritePackedUInt32((uint)this.id);
			writer.Write(this.myName);
			writer.Write(this.durability);
			writer.Write(this.pos);
			writer.Write(this.rotation);
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
			writer.WritePackedUInt32((uint)this.id);
		}
		if ((base.syncVarDirtyBits & 2u) != 0u)
		{
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
				flag = true;
			}
			writer.Write(this.myName);
		}
		if ((base.syncVarDirtyBits & 4u) != 0u)
		{
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
				flag = true;
			}
			writer.Write(this.durability);
		}
		if ((base.syncVarDirtyBits & 8u) != 0u)
		{
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
				flag = true;
			}
			writer.Write(this.pos);
		}
		if ((base.syncVarDirtyBits & 16u) != 0u)
		{
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
				flag = true;
			}
			writer.Write(this.rotation);
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
			this.id = (int)reader.ReadPackedUInt32();
			this.myName = reader.ReadString();
			this.durability = reader.ReadSingle();
			this.pos = reader.ReadVector3();
			this.rotation = reader.ReadVector3();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			this.SetID((int)reader.ReadPackedUInt32());
		}
		if ((num & 2) != 0)
		{
			this.SetName(reader.ReadString());
		}
		if ((num & 4) != 0)
		{
			this.SetDurability(reader.ReadSingle());
		}
		if ((num & 8) != 0)
		{
			this.SetPosition(reader.ReadVector3());
		}
		if ((num & 16) != 0)
		{
			this.SetRotation(reader.ReadVector3());
		}
	}

	private Item[] avItems;

	[SyncVar(hook = "SetID")]
	public int id;

	[SyncVar(hook = "SetName")]
	public string myName;

	[SyncVar(hook = "SetDurability")]
	public float durability;

	public float searchTime = 1f;

	[SyncVar(hook = "SetPosition")]
	public Vector3 pos;

	[SyncVar(hook = "SetRotation")]
	public Vector3 rotation;

	[HideInInspector]
	public bool iCanSeeThatAsHost;

	private GameObject myModel;

	public bool startItem = true;
}
