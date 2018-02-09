﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Scp914 : NetworkBehaviour
{
	public void SetProcessing(bool p)
	{
		this.NetworkisProcessing = p;
		if (p)
		{
			base.StartCoroutine(this.StartProcessing());
		}
	}

	public void ChangeState(int st)
	{
		this.Networkstate = st;
		this.knob.GetComponent<AudioSource>().Play();
	}

	public void Refine()
	{
		if (!this.isProcessing)
		{
			this.SetProcessing(true);
		}
	}

	private IEnumerator StartProcessing()
	{
		this.source.Play();
		yield return new WaitForSeconds(1f);
		while (this.doors.transform.localPosition.x > 0f)
		{
			this.doors.transform.localPosition -= Vector3.right * Time.deltaTime * 1.8f * ((!base.isServer) ? 1f : 0.5f);
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForSeconds(11.7f);
		while (this.doors.transform.localPosition.x < 1.74f)
		{
			this.doors.transform.localPosition += Vector3.right * Time.deltaTime * 1.5f * ((!base.isServer) ? 1f : 0.5f);
			yield return new WaitForEndOfFrame();
		}
		this.SetProcessing(false);
		yield break;
	}

	public void IncrementState()
	{
		if (this.cooldown > 0f || this.isProcessing)
		{
			return;
		}
		if (this.state + 1 > 4)
		{
			this.Networkstate = 0;
		}
		else
		{
			this.Networkstate = this.state + 1;
		}
		this.cooldown = 0.2f;
	}

	private void Update()
	{
		if (this.cooldown > 0f)
		{
			this.cooldown -= Time.deltaTime;
		}
		this.knob.transform.localRotation = Quaternion.LerpUnclamped(this.knob.transform.localRotation, Quaternion.Euler(0f, 0f, (float)(45 * this.state - 90)), Time.deltaTime * 5f);
		base.transform.localPosition = Vector3.right * 20.62f;
		base.transform.localRotation = Quaternion.Euler(Vector3.down * 90f);
	}

	private void UNetVersion()
	{
	}

	public int Networkstate
	{
		get
		{
			return this.state;
		}
		set
		{
			uint dirtyBit = 1u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.ChangeState(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<int>(value, ref this.state, dirtyBit);
		}
	}

	public bool NetworkisProcessing
	{
		get
		{
			return this.isProcessing;
		}
		set
		{
			uint dirtyBit = 2u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SetProcessing(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<bool>(value, ref this.isProcessing, dirtyBit);
		}
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			writer.WritePackedUInt32((uint)this.state);
			writer.Write(this.isProcessing);
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
			writer.WritePackedUInt32((uint)this.state);
		}
		if ((base.syncVarDirtyBits & 2u) != 0u)
		{
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
				flag = true;
			}
			writer.Write(this.isProcessing);
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
			this.state = (int)reader.ReadPackedUInt32();
			this.isProcessing = reader.ReadBoolean();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			this.ChangeState((int)reader.ReadPackedUInt32());
		}
		if ((num & 2) != 0)
		{
			this.SetProcessing(reader.ReadBoolean());
		}
	}

	public GameObject knob;

	public GameObject outputPlace;

	public GameObject doors;

	public AudioSource source;

	[SyncVar(hook = "ChangeState")]
	public int state;

	private float cooldown;

	[SyncVar(hook = "SetProcessing")]
	public bool isProcessing;
}