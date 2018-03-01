using System;
using UnityEngine;
using UnityEngine.Networking;

public class AlphaWarheadDetonationController : NetworkBehaviour
{
	public AlphaWarheadDetonationController()
	{
	}

	public void StartDetonation()
	{
		if (this.detonationInProgress || !this.lever.GetState())
		{
			return;
		}
		this.detonationInProgress = true;
		this.NetworkdetonationTime = 90f;
		this.doorsOpen = false;
	}

	public void CancelDetonation()
	{
		if (this.detonationInProgress && this.detonationTime > 2f)
		{
			this.detonationInProgress = false;
			this.NetworkdetonationTime = 0f;
		}
	}

	private void FixedUpdate()
	{
		if (base.isLocalPlayer && this.awdc != null && this.lightStatus != (this.awdc.detonationTime != 0f))
		{
			this.lightStatus = (this.awdc.detonationTime != 0f);
			this.SetLights(this.lightStatus);
		}
		if (base.name == "Host")
		{
			if (this.detonationTime > 0f)
			{
				this.NetworkdetonationTime = this.detonationTime - Time.deltaTime;
				if (!this.lever.GetState())
				{
					this.CancelDetonation();
				}
				if (this.detonationTime < 83f && !this.doorsOpen && base.isLocalPlayer)
				{
					this.doorsOpen = true;
					this.OpenDoors();
				}
				if (this.detonationTime < 2f && !this.blastDoors && this.detonationInProgress && base.isLocalPlayer)
				{
					this.blastDoors = true;
					this.CloseBlastDoors();
				}
			}
			else
			{
				if (this.detonationTime < 0f)
				{
					base.GetComponent<RoundSummary>().summary.warheadDetonated = true;
					this.Explode();
				}
				this.NetworkdetonationTime = 0f;
			}
		}
		if (base.isLocalPlayer && base.isServer)
		{
			this.TransmitData(this.detonationTime);
		}
		if (this.awsc == null || this.awdc == null)
		{
			this.awsc = UnityEngine.Object.FindObjectOfType<AWSoundController>();
			if (this.host == null)
			{
				this.host = GameObject.Find("Host");
			}
			if (this.host != null)
			{
				this.awdc = this.host.GetComponent<AlphaWarheadDetonationController>();
			}
		}
		else
		{
			this.awsc.UpdateSound(90f - this.awdc.detonationTime, this.detonated);
		}
	}

	private void Explode()
	{
		this.detonated = true;
		this.ExplodePlayers();
	}

	[ServerCallback]
	private void OpenDoors()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		foreach (Door door in UnityEngine.Object.FindObjectsOfType<Door>())
		{
			if (!door.isOpen && !door.permissionLevel.Contains("CONT") && door.permissionLevel != "UNACCESSIBLE")
			{
				door.OpenWarhead();
			}
		}
	}

	[ServerCallback]
	private void ExplodePlayers()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("LiftTarget");
		foreach (GameObject gameObject in PlayerManager.singleton.players)
		{
			foreach (GameObject gameObject2 in array)
			{
				gameObject.GetComponent<PlayerStats>().Explode(Vector3.Distance(gameObject2.transform.position, gameObject.transform.position) < 3.5f);
			}
		}
	}

	[ServerCallback]
	private void CloseBlastDoors()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		foreach (BlastDoor blastDoor in UnityEngine.Object.FindObjectsOfType<BlastDoor>())
		{
			blastDoor.SetClosed(true);
		}
	}

	[ClientCallback]
	private void TransmitData(float t)
	{
		if (!NetworkClient.active)
		{
			return;
		}
		this.CmdSyncData(t);
	}

	[ServerCallback]
	private void CmdSyncData(float t)
	{
		if (!NetworkServer.active)
		{
			return;
		}
		this.NetworkdetonationTime = t;
	}

	private void Start()
	{
		if (!TutorialManager.status)
		{
			this.lever = GameObject.Find("Lever_Alpha_Controller").GetComponent<LeverButton>();
			this.lights = UnityEngine.Object.FindObjectsOfType<ToggleableLight>();
		}
	}

	private void SetLights(bool b)
	{
		foreach (ToggleableLight toggleableLight in this.lights)
		{
			toggleableLight.SetLights(b);
		}
	}

	private void UNetVersion()
	{
	}

	public float NetworkdetonationTime
	{
		get
		{
			return this.detonationTime;
		}
		set
		{
			base.SetSyncVar<float>(value, ref this.detonationTime, 1u);
		}
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			writer.Write(this.detonationTime);
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
			writer.Write(this.detonationTime);
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
			this.detonationTime = reader.ReadSingle();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			this.detonationTime = reader.ReadSingle();
		}
	}

	[SyncVar]
	public float detonationTime;

	private bool detonationInProgress;

	private bool detonated;

	private bool doorsOpen;

	private bool blastDoors;

	private GameObject host;

	private bool lightStatus;

	private AWSoundController awsc;

	private LeverButton lever;

	private AlphaWarheadDetonationController awdc;

	private ToggleableLight[] lights;
}
