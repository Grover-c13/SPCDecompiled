using System;
using UnityEngine;
using UnityEngine.Networking;

public class AlphaWarheadNukesitePanel : NetworkBehaviour
{
	public bool Networkenabled
	{
		get
		{
			return this.enabled;
		}
		set
		{
			uint dirtyBit = 1u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SetEnabled(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<bool>(value, ref this.enabled, dirtyBit);
		}
	}

	public AlphaWarheadNukesitePanel()
	{
	}

	private void FixedUpdate()
	{
		this.UpdateLeverStatus();
	}

	public bool AllowChangeLevelState()
	{
		return this.leverStatus == 0f || this.leverStatus == 1f;
	}

	private void UpdateLeverStatus()
	{
		if (AlphaWarheadController.host == null)
		{
			return;
		}
		Color color = new Color(0.2f, 0.3f, 0.5f);
		this.led_detonationinprogress.SetColor("_EmissionColor", (!AlphaWarheadController.host.detonationInProgress) ? Color.black : color);
		this.led_outsidedoor.SetColor("_EmissionColor", (!this.outsideDoor.isOpen) ? Color.black : color);
		this.led_blastdoors.SetColor("_EmissionColor", (!this.blastDoor.isClosed) ? Color.black : color);
		this.led_cancel.SetColor("_EmissionColor", (AlphaWarheadController.host.timeToDetonation <= 10f || !AlphaWarheadController.host.detonationInProgress) ? Color.black : Color.red);
		this.leverStatus += ((!this.enabled) ? -0.04f : 0.04f);
		this.leverStatus = Mathf.Clamp01(this.leverStatus);
		for (int i = 0; i < 2; i++)
		{
			this.onOffMaterial[i].SetColor("_EmissionColor", (i != Mathf.RoundToInt(this.leverStatus)) ? Color.black : new Color(1.2f, 1.2f, 1.2f, 1f));
		}
		this.lever.localRotation = Quaternion.Euler(new Vector3(Mathf.Lerp(10f, -170f, this.leverStatus), -90f, 90f));
	}

	private void Awake()
	{
		AlphaWarheadOutsitePanel.nukeside = this;
	}

	public void SetEnabled(bool b)
	{
		this.Networkenabled = b;
	}

	private void UNetVersion()
	{
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			writer.Write(this.enabled);
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
			writer.Write(this.enabled);
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
			this.enabled = reader.ReadBoolean();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			this.SetEnabled(reader.ReadBoolean());
		}
	}

	public Transform lever;

	[SyncVar(hook = "SetEnabled")]
	public new bool enabled;

	private float leverStatus;

	public BlastDoor blastDoor;

	public Door outsideDoor;

	public Material[] onOffMaterial;

	public Material led_blastdoors;

	public Material led_outsidedoor;

	public Material led_detonationinprogress;

	public Material led_cancel;
}
