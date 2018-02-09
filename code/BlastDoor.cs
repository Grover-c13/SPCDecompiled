using System;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkIdentity))]
public class BlastDoor : NetworkBehaviour
{
	public void SetClosed(bool b)
	{
		this.NetworkisClosed = b;
		if (this.isClosed)
		{
			base.GetComponent<Animator>().SetTrigger("Close");
		}
	}

	private void UNetVersion()
	{
	}

	public bool NetworkisClosed
	{
		get
		{
			return this.isClosed;
		}
		set
		{
			uint dirtyBit = 1u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SetClosed(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<bool>(value, ref this.isClosed, dirtyBit);
		}
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			writer.Write(this.isClosed);
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
			writer.Write(this.isClosed);
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
			this.isClosed = reader.ReadBoolean();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			this.SetClosed(reader.ReadBoolean());
		}
	}

	[SyncVar(hook = "SetClosed")]
	public bool isClosed;
}
