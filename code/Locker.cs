using System;
using UnityEngine;
using UnityEngine.Networking;

public class Locker : NetworkBehaviour
{
	public Locker()
	{
	}

	public int GetItem()
	{
		return (!this.isTaken) ? this.ids[UnityEngine.Random.Range(0, this.ids.Length)] : -1;
	}

	public void SetTaken(bool b)
	{
		this.NetworkisTaken = b;
	}

	public void SetupPos()
	{
		this.localPos = base.transform.localPosition;
	}

	public void Update()
	{
		base.transform.localPosition = this.localPos;
	}

	private void UNetVersion()
	{
	}

	public bool NetworkisTaken
	{
		get
		{
			return this.isTaken;
		}
		set
		{
			base.SetSyncVar<bool>(value, ref this.isTaken, 1u);
		}
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			writer.Write(this.isTaken);
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
			writer.Write(this.isTaken);
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
			this.isTaken = reader.ReadBoolean();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			this.isTaken = reader.ReadBoolean();
		}
	}

	public Vector3 localPos;

	public float searchTime;

	public int[] ids;

	[SyncVar]
	public bool isTaken;
}
