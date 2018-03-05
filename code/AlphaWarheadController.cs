using System;
using UnityEngine;
using UnityEngine.Networking;

public class AlphaWarheadController : NetworkBehaviour
{
	public float NetworktimeToDetonation
	{
		get
		{
			return this.timeToDetonation;
		}
		set
		{
			uint dirtyBit = 1u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SetTime(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<float>(value, ref this.timeToDetonation, dirtyBit);
		}
	}

	public bool NetworkdetonationInProgress
	{
		get
		{
			return this.detonationInProgress;
		}
		set
		{
			uint dirtyBit = 2u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SetStatus(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<bool>(value, ref this.detonationInProgress, dirtyBit);
		}
	}

	public AlphaWarheadController()
	{
	}

	private void SetTime(float f)
	{
		this.NetworktimeToDetonation = f;
	}

	private void SetStatus(bool b)
	{
		this.NetworkdetonationInProgress = b;
	}

	public void StartDetonation()
	{
		this.doorsOpen = false;
		if (this.timeToDetonation == (float)AlphaWarheadController.realDetonationTime && !this.detonationInProgress)
		{
			this.SetStatus(true);
		}
	}

	public void CancelDetonation()
	{
		if (this.detonationInProgress && this.timeToDetonation > 10f)
		{
			this.SetStatus(false);
			this.SetTime((float)(AlphaWarheadController.realDetonationTime + this.cooldown));
		}
	}

	private void Detonate()
	{
		this.CallRpcShake();
		GameObject[] array = GameObject.FindGameObjectsWithTag("LiftTarget");
		foreach (GameObject gameObject in PlayerManager.singleton.players)
		{
			foreach (GameObject gameObject2 in array)
			{
				gameObject.GetComponent<PlayerStats>().Explode(Vector3.Distance(gameObject2.transform.position, gameObject.transform.position) < 3.5f);
			}
		}
	}

	[ClientRpc]
	private void RpcShake()
	{
		ExplosionCameraShake.singleton.Shake(1f);
	}

	private void FixedUpdate()
	{
		if (base.name == "Host")
		{
			AlphaWarheadController.host = this;
		}
		if (AlphaWarheadController.host == null)
		{
			return;
		}
		if (base.isLocalPlayer)
		{
			this.UpdateSourceState();
			if (base.isServer)
			{
				this.ServerCountdown();
			}
		}
	}

	private void UpdateSourceState()
	{
		if (TutorialManager.status)
		{
			return;
		}
		if (AlphaWarheadController.host.detonationInProgress)
		{
			AlphaWarheadController.alarmSource.volume = 1f;
			if (AlphaWarheadController.host.timeToDetonation != 0f)
			{
				if (!AlphaWarheadController.alarmSource.isPlaying)
				{
					AlphaWarheadController.alarmSource.Play();
				}
				float num = (float)AlphaWarheadController.realDetonationTime - AlphaWarheadController.host.timeToDetonation;
				if (Mathf.Abs(AlphaWarheadController.alarmSource.time - num) > 0.5f)
				{
					AlphaWarheadController.alarmSource.time = Mathf.Clamp(num, 0f, (float)AlphaWarheadController.realDetonationTime);
				}
			}
			if (AlphaWarheadController.host.timeToDetonation < 5f && AlphaWarheadController.host.timeToDetonation != 0f)
			{
				this.shake += Time.fixedDeltaTime / 20f;
				this.shake = Mathf.Clamp(this.shake, 0f, 0.5f);
				if (Vector3.Distance(base.transform.position, AlphaWarheadOutsitePanel.nukeside.transform.position) < 100f)
				{
					ExplosionCameraShake.singleton.Shake(this.shake);
				}
			}
		}
		else
		{
			AlphaWarheadController.alarmSource.volume = 0f;
		}
	}

	[ServerCallback]
	private void ServerCountdown()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		float num = this.timeToDetonation;
		if (this.timeToDetonation != 0f)
		{
			if (this.detonationInProgress)
			{
				num -= Time.fixedDeltaTime;
				if (num < 2f && !this.doorsClosed)
				{
					this.doorsClosed = true;
					foreach (BlastDoor blastDoor in this.blastDoors)
					{
						blastDoor.SetClosed(true);
					}
				}
				if (num < (float)(AlphaWarheadController.realDetonationTime - 13) && !this.doorsOpen)
				{
					this.doorsOpen = true;
					foreach (Door door in UnityEngine.Object.FindObjectsOfType<Door>())
					{
						door.OpenWarhead();
					}
				}
				if (num <= 0f)
				{
					this.Detonate();
					RoundSummary.host.summary.warheadDetonated = true;
				}
				num = Mathf.Clamp(num, 0f, (float)AlphaWarheadController.realDetonationTime);
			}
			else
			{
				if (num > (float)AlphaWarheadController.realDetonationTime)
				{
					num -= Time.fixedDeltaTime;
				}
				num = Mathf.Clamp(num, (float)AlphaWarheadController.realDetonationTime, (float)(this.cooldown + AlphaWarheadController.realDetonationTime));
			}
		}
		if (num != this.timeToDetonation)
		{
			this.SetTime(num);
		}
	}

	private void Start()
	{
		if (base.isLocalPlayer && !TutorialManager.status)
		{
			AlphaWarheadController.alarmSource = GameObject.Find("GameManager").GetComponent<AudioSource>();
			this.blastDoors = UnityEngine.Object.FindObjectsOfType<BlastDoor>();
		}
	}

	static AlphaWarheadController()
	{
		NetworkBehaviour.RegisterRpcDelegate(typeof(AlphaWarheadController), AlphaWarheadController.kRpcRpcShake, new NetworkBehaviour.CmdDelegate(AlphaWarheadController.InvokeRpcRpcShake));
		NetworkCRC.RegisterBehaviour("AlphaWarheadController", 0);
	}

	private void UNetVersion()
	{
	}

	protected static void InvokeRpcRpcShake(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcShake called on server.");
			return;
		}
		((AlphaWarheadController)obj).RpcShake();
	}

	public void CallRpcShake()
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("RPC Function RpcShake called on client.");
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)2));
		networkWriter.WritePackedUInt32((uint)AlphaWarheadController.kRpcRpcShake);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		this.SendRPCInternal(networkWriter, 0, "RpcShake");
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			writer.Write(this.timeToDetonation);
			writer.Write(this.detonationInProgress);
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
			writer.Write(this.timeToDetonation);
		}
		if ((base.syncVarDirtyBits & 2u) != 0u)
		{
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
				flag = true;
			}
			writer.Write(this.detonationInProgress);
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
			this.timeToDetonation = reader.ReadSingle();
			this.detonationInProgress = reader.ReadBoolean();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			this.SetTime(reader.ReadSingle());
		}
		if ((num & 2) != 0)
		{
			this.SetStatus(reader.ReadBoolean());
		}
	}

	[SyncVar(hook = "SetTime")]
	public float timeToDetonation = (float)AlphaWarheadController.realDetonationTime;

	[SyncVar(hook = "SetStatus")]
	public bool detonationInProgress;

	public int cooldown = 30;

	public static AudioSource alarmSource;

	public static AlphaWarheadController host;

	public static int realDetonationTime = 104;

	private bool doorsClosed;

	private bool doorsOpen;

	private BlastDoor[] blastDoors;

	private float shake;

	private static int kRpcRpcShake = -737840022;
}
