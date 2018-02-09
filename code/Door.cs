using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Door : NetworkBehaviour
{
	public void SetLocalPos()
	{
		this.localPos = base.transform.localPosition;
		this.localRot = base.transform.localRotation;
	}

	private IEnumerator UpdatePosition()
	{
		foreach (Animator animator in this.parts)
		{
			animator.SetBool("isOpen", this.isOpen);
		}
		if (this.sound_checkpointWarning != null && this.isOpen)
		{
			this.deniedInProgress = true;
			this.moving.moving = true;
			this.SetActiveStatus(2);
			float t = 0f;
			while (t < 5f)
			{
				t += 0.1f;
				yield return new WaitForSeconds(0.1f);
				if (this.curCooldown < 0f)
				{
					this.SetActiveStatus(1);
				}
			}
			this.soundsource.PlayOneShot(this.sound_checkpointWarning);
			this.SetActiveStatus(4);
			yield return new WaitForSeconds(2f);
			this.SetActiveStatus(0);
			this.moving.moving = false;
			this.deniedInProgress = false;
			this.SetState(false);
			this.soundsource.PlayOneShot(this.sound_close[UnityEngine.Random.Range(0, this.sound_close.Length)]);
		}
		yield break;
	}

	public void SetState(bool open)
	{
		this.NetworkisOpen = open;
		this.ForceCooldown(this.cooldown);
	}

	private IEnumerator Start()
	{
		this.SetActiveStatus(0);
		float time = 0f;
		while (time < 10f)
		{
			time += Time.deltaTime;
			if (this.buffedStatus != this.isOpen)
			{
				this.buffedStatus = this.isOpen;
				this.ForceCooldown(this.cooldown);
				break;
			}
			yield return new WaitForEndOfFrame();
		}
		foreach (Renderer renderer in base.GetComponentsInChildren(typeof(Renderer)))
		{
			if (renderer.tag == "DoorButton")
			{
				this.buttons.Add(renderer.gameObject);
			}
		}
		yield break;
	}

	public void UpdatePos()
	{
		if (this.localPos != Vector3.zero)
		{
			base.transform.localPosition = this.localPos;
			base.transform.localRotation = this.localRot;
		}
	}

	public void SetZero()
	{
		this.localPos = Vector3.zero;
	}

	public void ChangeState()
	{
		if (this.curCooldown < 0f && !this.moving.moving && !this.deniedInProgress)
		{
			this.moving.moving = true;
			this.SetState(!this.isOpen);
			this.CallRpcDoSound();
		}
	}

	[ClientRpc(channel = 14)]
	private void RpcDoSound()
	{
		this.soundsource.PlayOneShot((!this.isOpen) ? this.sound_close[UnityEngine.Random.Range(0, this.sound_close.Length)] : this.sound_open[UnityEngine.Random.Range(0, this.sound_open.Length)]);
	}

	private void SetActiveStatus(int s)
	{
		if (this.status != s)
		{
			this.status = s;
			foreach (GameObject gameObject in this.buttons)
			{
				try
				{
					gameObject.GetComponent<MeshRenderer>().material = this.panelStates[s].mat;
				}
				catch
				{
				}
				try
				{
					gameObject.GetComponentInChildren<Text>().text = this.panelStates[s].info;
				}
				catch
				{
				}
				try
				{
					gameObject.GetComponentInChildren<Image>().color = ((!(this.panelStates[s].texture == null)) ? Color.white : Color.clear);
					gameObject.GetComponentInChildren<Image>().sprite = this.panelStates[s].texture;
				}
				catch
				{
				}
			}
		}
	}

	private void LateUpdate()
	{
		if (this.curCooldown >= 0f)
		{
			this.curCooldown -= Time.deltaTime;
		}
		if (!this.deniedInProgress)
		{
			if (this.moving.moving && this.status != 3)
			{
				if (this.sound_checkpointWarning == null)
				{
					this.SetActiveStatus(2);
				}
			}
			else
			{
				this.SetActiveStatus((!this.isOpen) ? 0 : 1);
			}
		}
	}

	public IEnumerator Denied()
	{
		if (this.curCooldown < 0f && !this.moving.moving && !this.deniedInProgress)
		{
			this.deniedInProgress = true;
			this.soundsource.PlayOneShot(this.sound_denied);
			this.SetActiveStatus(3);
			yield return new WaitForSeconds(1f);
			this.deniedInProgress = false;
		}
		yield break;
	}

	public void ForceCooldown(float cd)
	{
		this.curCooldown = cd;
		base.StartCoroutine(this.UpdatePosition());
	}

	private void UNetVersion()
	{
	}

	public bool NetworkisOpen
	{
		get
		{
			return this.isOpen;
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
			base.SetSyncVar<bool>(value, ref this.isOpen, dirtyBit);
		}
	}

	protected static void InvokeRpcRpcDoSound(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcDoSound called on server.");
			return;
		}
		((Door)obj).RpcDoSound();
	}

	public void CallRpcDoSound()
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("RPC Function RpcDoSound called on client.");
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)2));
		networkWriter.WritePackedUInt32((uint)Door.kRpcRpcDoSound);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		this.SendRPCInternal(networkWriter, 14, "RpcDoSound");
	}

	static Door()
	{
		NetworkBehaviour.RegisterRpcDelegate(typeof(Door), Door.kRpcRpcDoSound, new NetworkBehaviour.CmdDelegate(Door.InvokeRpcRpcDoSound));
		NetworkCRC.RegisterBehaviour("Door", 0);
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			writer.Write(this.isOpen);
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
			writer.Write(this.isOpen);
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
			this.isOpen = reader.ReadBoolean();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			this.SetState(reader.ReadBoolean());
		}
	}

	public string permissionLevel;

	[SyncVar(hook = "SetState")]
	public bool isOpen;

	private bool buffedStatus;

	public bool dontOpenOnWarhead;

	public float curCooldown;

	public float cooldown;

	public Animator[] parts;

	public AudioSource soundsource;

	public AudioClip[] sound_open;

	public AudioClip[] sound_close;

	public AudioClip sound_checkpointWarning;

	public AudioClip sound_denied;

	public MovingStatus moving;

	public Door.PanelState[] panelStates;

	[HideInInspector]
	public List<GameObject> buttons = new List<GameObject>();

	public Vector3 localPos;

	public Quaternion localRot;

	private int status = -1;

	private bool deniedInProgress;

	private static int kRpcRpcDoSound = 630763456;

	[Serializable]
	public class PanelState
	{
		public Sprite texture;

		public Material mat;

		[Multiline]
		public string info;
	}
}
