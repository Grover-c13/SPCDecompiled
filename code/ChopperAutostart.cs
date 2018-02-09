using System;
using UnityEngine;
using UnityEngine.Networking;

public class ChopperAutostart : NetworkBehaviour
{
	public void SetState(bool b)
	{
		this.NetworkisLanded = b;
		this.RefreshState();
	}

	private void Start()
	{
		this.RefreshState();
	}

	private void RefreshState()
	{
		base.GetComponent<Animator>().SetBool("IsLanded", this.isLanded);
	}

	private void UNetVersion()
	{
	}

	public bool NetworkisLanded
	{
		get
		{
			return this.isLanded;
		}
		set
		{
			uint dirtyBit = 1u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SetState(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<bool>(value, ref this.isLanded, dirtyBit);
		}
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			writer.Write(this.isLanded);
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
			writer.Write(this.isLanded);
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
			this.isLanded = reader.ReadBoolean();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			this.SetState(reader.ReadBoolean());
		}
	}

	[SyncVar(hook = "SetState")]
	public bool isLanded = true;
}
