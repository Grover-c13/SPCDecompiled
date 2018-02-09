using System;
using UnityEngine;
using UnityEngine.Networking;

public class AlphaWarheadButtonUnlocker : NetworkBehaviour
{
	private void SetLeft(bool b)
	{
		this.NetworklockL = b;
		this.leftButton.GetComponentInChildren<Light>().enabled = b;
	}

	private void SetRight(bool b)
	{
		this.NetworklockR = b;
		this.rightButton.GetComponentInChildren<Light>().enabled = b;
	}

	private void Start()
	{
		this.leftButton.name = "AW_KEYCARD_LEFT";
		this.rightButton.name = "AW_KEYCARD_RIGHT";
	}

	private void Update()
	{
		if (this.cooldown > 0f)
		{
			this.cooldown -= Time.deltaTime;
		}
		this.glass.transform.localRotation = Quaternion.LerpUnclamped(this.glass.transform.localRotation, Quaternion.Euler((!(this.lockL & this.lockR)) ? this.closedRot : this.openedRot), Time.deltaTime * 2.5f);
	}

	public void ChangeButtonStage(string bid)
	{
		if (this.cooldown > 0f)
		{
			return;
		}
		this.cooldown = 0.7f;
		if (bid == "DENIED")
		{
			base.GetComponent<AudioSource>().Play();
		}
		else if (bid == "AW_KEYCARD_LEFT")
		{
			this.SetLeft(!this.lockL);
			this.leftButton.GetComponent<AudioSource>().Play();
		}
		else
		{
			this.SetRight(!this.lockR);
			this.rightButton.GetComponent<AudioSource>().Play();
		}
	}

	private void UNetVersion()
	{
	}

	public bool NetworklockL
	{
		get
		{
			return this.lockL;
		}
		set
		{
			uint dirtyBit = 1u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SetLeft(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<bool>(value, ref this.lockL, dirtyBit);
		}
	}

	public bool NetworklockR
	{
		get
		{
			return this.lockR;
		}
		set
		{
			uint dirtyBit = 2u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SetRight(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<bool>(value, ref this.lockR, dirtyBit);
		}
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			writer.Write(this.lockL);
			writer.Write(this.lockR);
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
			writer.Write(this.lockL);
		}
		if ((base.syncVarDirtyBits & 2u) != 0u)
		{
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
				flag = true;
			}
			writer.Write(this.lockR);
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
			this.lockL = reader.ReadBoolean();
			this.lockR = reader.ReadBoolean();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			this.SetLeft(reader.ReadBoolean());
		}
		if ((num & 2) != 0)
		{
			this.SetRight(reader.ReadBoolean());
		}
	}

	public GameObject leftButton;

	public GameObject rightButton;

	public Vector3 closedRot;

	public Vector3 openedRot;

	public GameObject glass;

	public float cooldown;

	[SyncVar(hook = "SetLeft")]
	private bool lockL;

	[SyncVar(hook = "SetRight")]
	private bool lockR;
}
