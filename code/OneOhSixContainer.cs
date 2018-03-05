using System;
using UnityEngine.Networking;

public class OneOhSixContainer : NetworkBehaviour
{
	public bool Networkused
	{
		get
		{
			return this.used;
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
			base.SetSyncVar<bool>(value, ref this.used, dirtyBit);
		}
	}

	public OneOhSixContainer()
	{
	}

	public void SetState(bool b)
	{
		this.Networkused = b;
	}

	private void UNetVersion()
	{
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			writer.Write(this.used);
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
			writer.Write(this.used);
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
			this.used = reader.ReadBoolean();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			this.SetState(reader.ReadBoolean());
		}
	}

	[SyncVar(hook = "SetState")]
	public bool used;
}
