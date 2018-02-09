using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class LeverButton : NetworkBehaviour
{
	private void SetSwitch(bool b)
	{
		this.NetworkdefaultState = b;
		this.SetupLights();
	}

	public void Switch()
	{
		if (this.cooldown <= 0f)
		{
			this.cooldown = 0.6f;
			this.lever.GetComponent<AudioSource>().Play();
			this.SetSwitch(!this.defaultState);
		}
	}

	private void SetupLights()
	{
		if (this.onLights.Length == 0 || this.offLights.Length == 0)
		{
			return;
		}
		for (int i = 0; i < this.onLights.Length; i++)
		{
			base.StartCoroutine(this.SetupLights(this.onLights[i], (!this.GetState()) ? 0f : this.intensity));
		}
		for (int j = 0; j < this.offLights.Length; j++)
		{
			base.StartCoroutine(this.SetupLights(this.offLights[j], this.GetState() ? 0f : this.intensity));
		}
	}

	private IEnumerator SetupLights(Light l, float targetIntens)
	{
		bool increase = l.intensity < targetIntens;
		if (increase)
		{
			while (l.intensity < targetIntens)
			{
				l.intensity += Time.deltaTime * 5f;
				yield return new WaitForEndOfFrame();
			}
		}
		else
		{
			while (l.intensity > 0f)
			{
				l.intensity -= Time.deltaTime * 5f;
				yield return new WaitForEndOfFrame();
			}
		}
		yield break;
	}

	public bool GetState()
	{
		return (this.orientation != LeverButton.LeverOrientation.OnIsDown) ? (!this.defaultState) : this.defaultState;
	}

	private void Update()
	{
		if (this.cooldown > 0f)
		{
			this.cooldown -= Time.deltaTime;
		}
		this.targetQuaternion = Quaternion.Euler((!this.defaultState) ? new Vector3(92f, 0f, 0f) : new Vector3(268f, 0f, 0f));
		this.lever.transform.localRotation = Quaternion.LerpUnclamped(this.lever.transform.localRotation, this.targetQuaternion, Time.deltaTime * 5f);
	}

	private void UNetVersion()
	{
	}

	public bool NetworkdefaultState
	{
		get
		{
			return this.defaultState;
		}
		set
		{
			uint dirtyBit = 1u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SetSwitch(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<bool>(value, ref this.defaultState, dirtyBit);
		}
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			writer.Write(this.defaultState);
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
			writer.Write(this.defaultState);
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
			this.defaultState = reader.ReadBoolean();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			this.SetSwitch(reader.ReadBoolean());
		}
	}

	[SyncVar(hook = "SetSwitch")]
	public bool defaultState;

	public Transform lever;

	private Quaternion targetQuaternion;

	[SerializeField]
	private LeverButton.LeverOrientation orientation;

	private float cooldown;

	public Light[] onLights;

	public Light[] offLights;

	public float intensity = 1.4f;

	public enum LeverOrientation
	{
		OnIsDown,
		OnIsUp
	}
}
