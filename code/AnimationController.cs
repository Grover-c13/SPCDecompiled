using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AnimationController : NetworkBehaviour
{
	public void SyncItem(int i)
	{
		this.Networkitem = i;
	}

	private void Start()
	{
		this.fpc = base.GetComponent<FirstPersonController>();
		this.inv = base.GetComponent<Inventory>();
		if (!base.isLocalPlayer)
		{
			AnimationController.controllers.Add(this);
			base.Invoke("RefreshItems", 6f);
		}
	}

	private void OnDestroy()
	{
		if (!base.isLocalPlayer)
		{
			AnimationController.controllers.Remove(this);
		}
	}

	private void LateUpdate()
	{
		if (!base.isLocalPlayer && this.handAnimator != null)
		{
			this.handAnimator.SetBool("Cuffed", this.cuffed);
		}
		this.cuffed = false;
	}

	public void PlaySound(int id, bool isGun)
	{
		if (!base.isLocalPlayer)
		{
			if (isGun)
			{
				this.gunSource.PlayOneShot(this.clips[id].audio);
			}
			else
			{
				this.footstepSource.PlayOneShot(this.clips[id].audio);
			}
		}
	}

	public void PlaySound(string label, bool isGun)
	{
		if (!base.isLocalPlayer)
		{
			int num = 0;
			for (int i = 0; i < this.clips.Length; i++)
			{
				if (this.clips[i].clipName == label)
				{
					num = i;
				}
			}
			if (isGun)
			{
				this.gunSource.PlayOneShot(this.clips[num].audio);
			}
			else
			{
				this.footstepSource.PlayOneShot(this.clips[num].audio);
			}
		}
	}

	public void DoAnimation(string trigger)
	{
		if (!base.isLocalPlayer)
		{
			this.handAnimator.SetTrigger(trigger);
		}
	}

	private void FixedUpdate()
	{
		if (!base.isLocalPlayer)
		{
			if (this.prevItem != this.item)
			{
				this.prevItem = this.item;
				this.RefreshItems();
			}
			this.RecieveData();
		}
		else
		{
			this.TransmitData(this.fpc.animationID, this.item, this.fpc.plySpeed);
		}
	}

	private void RefreshItems()
	{
		HandPart[] componentsInChildren = base.GetComponentsInChildren<HandPart>();
		foreach (HandPart handPart in componentsInChildren)
		{
			handPart.Invoke("UpdateItem", 0.3f);
		}
	}

	public void SetState(int i)
	{
		this.NetworkcurAnim = i;
	}

	public void RecieveData()
	{
		if (this.animator != null)
		{
			this.CalculateAnimation();
			if (this.handAnimator == null)
			{
				Animator[] componentsInChildren = this.animator.GetComponentsInChildren<Animator>();
				if (componentsInChildren.Length > 1)
				{
					this.handAnimator = componentsInChildren[1];
				}
			}
			else
			{
				this.handAnimator.SetInteger("CurItem", this.item);
				this.handAnimator.SetInteger("Running", (this.speed.x == 0f) ? 0 : ((this.curAnim != 1) ? 1 : 2));
			}
		}
	}

	private void CalculateAnimation()
	{
		this.animator.SetBool("Stafe", this.curAnim != 2 & (Mathf.Abs(this.speed.y) > 0f & (this.speed.x == 0f | (this.speed.x > 0f & this.curAnim == 0))));
		this.animator.SetBool("Jump", this.curAnim == 2);
		float value = 0f;
		float value2 = 0f;
		if (this.curAnim != 2)
		{
			if (this.speed.x != 0f)
			{
				value = (float)((this.curAnim != 1) ? 1 : 2);
				if (this.speed.x < 0f)
				{
					value = -1f;
				}
			}
			if (this.speed.y != 0f)
			{
				value2 = (float)((this.speed.y <= 0f) ? -1 : 1);
			}
		}
		this.animator.SetFloat("Speed", value, 0.1f, Time.deltaTime);
		this.animator.SetFloat("Direction", value2, 0.1f, Time.deltaTime);
	}

	[ClientCallback]
	private void TransmitData(int state, int it, Vector2 v2)
	{
		if (!NetworkClient.active)
		{
			return;
		}
		this.CallCmdSyncData(state, it, v2);
	}

	[Command(channel = 3)]
	private void CmdSyncData(int state, int it, Vector2 v2)
	{
		this.NetworkcurAnim = state;
		this.Networkitem = it;
		this.Networkspeed = v2;
		Color red = Color.red;
	}

	static AnimationController()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(AnimationController), AnimationController.kCmdCmdSyncData, new NetworkBehaviour.CmdDelegate(AnimationController.InvokeCmdCmdSyncData));
		NetworkCRC.RegisterBehaviour("AnimationController", 0);
	}

	private void UNetVersion()
	{
	}

	public int NetworkcurAnim
	{
		get
		{
			return this.curAnim;
		}
		set
		{
			base.SetSyncVar<int>(value, ref this.curAnim, 1u);
		}
	}

	public Vector2 Networkspeed
	{
		get
		{
			return this.speed;
		}
		set
		{
			base.SetSyncVar<Vector2>(value, ref this.speed, 2u);
		}
	}

	public int Networkitem
	{
		get
		{
			return this.item;
		}
		set
		{
			base.SetSyncVar<int>(value, ref this.item, 4u);
		}
	}

	protected static void InvokeCmdCmdSyncData(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSyncData called on client.");
			return;
		}
		((AnimationController)obj).CmdSyncData((int)reader.ReadPackedUInt32(), (int)reader.ReadPackedUInt32(), reader.ReadVector2());
	}

	public void CallCmdSyncData(int state, int it, Vector2 v2)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdSyncData called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdSyncData(state, it, v2);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)AnimationController.kCmdCmdSyncData);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.WritePackedUInt32((uint)state);
		networkWriter.WritePackedUInt32((uint)it);
		networkWriter.Write(v2);
		base.SendCommandInternal(networkWriter, 3, "CmdSyncData");
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			writer.WritePackedUInt32((uint)this.curAnim);
			writer.Write(this.speed);
			writer.WritePackedUInt32((uint)this.item);
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
			writer.WritePackedUInt32((uint)this.curAnim);
		}
		if ((base.syncVarDirtyBits & 2u) != 0u)
		{
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
				flag = true;
			}
			writer.Write(this.speed);
		}
		if ((base.syncVarDirtyBits & 4u) != 0u)
		{
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
				flag = true;
			}
			writer.WritePackedUInt32((uint)this.item);
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
			this.curAnim = (int)reader.ReadPackedUInt32();
			this.speed = reader.ReadVector2();
			this.item = (int)reader.ReadPackedUInt32();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			this.curAnim = (int)reader.ReadPackedUInt32();
		}
		if ((num & 2) != 0)
		{
			this.speed = reader.ReadVector2();
		}
		if ((num & 4) != 0)
		{
			this.item = (int)reader.ReadPackedUInt32();
		}
	}

	public AnimationController.AnimAudioClip[] clips;

	public AudioSource footstepSource;

	public AudioSource gunSource;

	public Animator animator;

	public Animator handAnimator;

	[SyncVar]
	public int curAnim;

	[SyncVar]
	public Vector2 speed;

	[SyncVar]
	public int item;

	public bool cuffed;

	private FirstPersonController fpc;

	private Inventory inv;

	public static List<AnimationController> controllers = new List<AnimationController>();

	private int prevItem;

	private static int kCmdCmdSyncData = 1138927717;

	[Serializable]
	public class AnimAudioClip
	{
		public string clipName;

		public AudioClip audio;
	}
}
