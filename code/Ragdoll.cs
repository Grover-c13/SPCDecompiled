using System;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

public class Ragdoll : NetworkBehaviour
{
	public Ragdoll()
	{
	}

	public void SetOwner(Ragdoll.Info s)
	{
		this.Networkowner = s;
	}

	private void Start()
	{
		base.Invoke("Unfr", 0.1f);
	}

	public void SetRecall(bool b)
	{
		this.NetworkallowRecall = b;
	}

	private void Unfr()
	{
		base.GetComponent<Rigidbody>().isKinematic = false;
		foreach (Rigidbody rigidbody in base.GetComponentsInChildren<Rigidbody>())
		{
			rigidbody.isKinematic = false;
		}
		foreach (Collider collider in base.GetComponentsInChildren<Collider>())
		{
			collider.enabled = true;
		}
	}

	private void UNetVersion()
	{
	}

	public Ragdoll.Info Networkowner
	{
		get
		{
			return this.owner;
		}
		set
		{
			uint dirtyBit = 1u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SetOwner(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<Ragdoll.Info>(value, ref this.owner, dirtyBit);
		}
	}

	public bool NetworkallowRecall
	{
		get
		{
			return this.allowRecall;
		}
		set
		{
			uint dirtyBit = 2u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SetRecall(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<bool>(value, ref this.allowRecall, dirtyBit);
		}
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			GeneratedNetworkCode._WriteInfo_Ragdoll(writer, this.owner);
			writer.Write(this.allowRecall);
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
			GeneratedNetworkCode._WriteInfo_Ragdoll(writer, this.owner);
		}
		if ((base.syncVarDirtyBits & 2u) != 0u)
		{
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
				flag = true;
			}
			writer.Write(this.allowRecall);
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
			this.owner = GeneratedNetworkCode._ReadInfo_Ragdoll(reader);
			this.allowRecall = reader.ReadBoolean();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			this.SetOwner(GeneratedNetworkCode._ReadInfo_Ragdoll(reader));
		}
		if ((num & 2) != 0)
		{
			this.SetRecall(reader.ReadBoolean());
		}
	}

	[SyncVar(hook = "SetOwner")]
	public Ragdoll.Info owner;

	[SyncVar(hook = "SetRecall")]
	public bool allowRecall;

	[Serializable]
	public struct Info
	{
		public Info(string owner, string nick, PlayerStats.HitInfo info, int cc)
		{
			this.ownerHLAPI_id = owner;
			this.steamClientName = nick;
			this.charclass = cc;
			this.deathCause = info;
		}

		public string ownerHLAPI_id;

		public string steamClientName;

		public PlayerStats.HitInfo deathCause;

		public int charclass;
	}
}
