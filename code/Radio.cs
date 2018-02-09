using System;
using Dissonance.Integrations.UNet_HLAPI;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;

public class Radio : NetworkBehaviour
{
	private void Start()
	{
		this.ccm = base.GetComponent<CharacterClassManager>();
		this.noiseSource = GameObject.Find("RadioNoiseSound").GetComponent<AudioSource>();
		if (base.isLocalPlayer)
		{
			Radio.localRadio = this;
			this.inv = base.GetComponent<Inventory>();
		}
		this.icon = base.GetComponentInChildren<SpeakerIcon>();
		base.InvokeRepeating("UpdateClass", 0f, 0.3f);
	}

	public void UpdateClass()
	{
		bool isSCP = false;
		bool isAliveHuman = false;
		if (this.ccm.curClass != -1 && this.ccm.curClass != 2)
		{
			if (this.ccm.klasy[this.ccm.curClass].team == Team.SCP)
			{
				isSCP = true;
			}
			else
			{
				isAliveHuman = true;
			}
		}
		this.voiceInfo.isAliveHuman = isAliveHuman;
		this.voiceInfo.isSCP = isSCP;
	}

	private void Update()
	{
		if (this.host == null)
		{
			this.host = GameObject.Find("Host");
		}
		if (base.isLocalPlayer)
		{
			this.noiseSource.volume = Radio.noiseIntensity * this.noiseMultiplier;
			Radio.noiseIntensity = 0f;
			this.GetInput();
			this.UseBattery();
			if (this.inv.localInventoryItem != null && this.inv.localInventoryItem.id == 12)
			{
				this.myRadio = this.inv.localInventoryItem;
			}
			if (!this.inv.items.Contains(this.myRadio))
			{
				this.myRadio = null;
			}
		}
	}

	private void UseBattery()
	{
		if (this.CheckRadio())
		{
			this.myRadio.durability -= 1.67f * (1f / this.presets[this.curPreset].powerTime) * Time.deltaTime * (float)((!this.isTransmitting) ? 1 : 3);
			if (this.myRadio.durability == 0f)
			{
				this.myRadio.durability = -1f;
			}
			if (this.myRadio.durability < 0f)
			{
				this.beepSource.PlayOneShot(this.b_battery);
				this.myRadio.durability = 0f;
			}
		}
		if (this.myRadio != null)
		{
			RadioDisplay.battery = Mathf.Clamp(Mathf.CeilToInt(this.myRadio.durability), 0, 100).ToString();
			RadioDisplay.power = this.presets[this.curPreset].powerText;
			RadioDisplay.label = this.presets[this.curPreset].label;
		}
	}

	private void GetInput()
	{
		if (this.timeToNextTransmition > 0f)
		{
			this.timeToNextTransmition -= Time.deltaTime;
		}
		bool flag = Input.GetButton("VoiceChat") && this.CheckRadio();
		if (flag != this.isTransmitting && this.timeToNextTransmition <= 0f)
		{
			this.NetworkisTransmitting = flag;
			this.timeToNextTransmition = 0.5f;
			this.CallCmdSyncTransmitionStatus(flag, base.transform.position);
		}
		if (this.inv.curItem == 12 && base.GetComponent<WeaponManager>().inventoryCooldown <= 0f)
		{
			if (Input.GetButtonDown("Fire1") && this.curPreset != 0)
			{
				this.NetworkcurPreset = this.curPreset + 1;
				if (this.curPreset >= this.presets.Length)
				{
					this.NetworkcurPreset = 1;
				}
				this.lastPreset = this.curPreset;
				this.CallCmdUpdatePreset(this.curPreset);
			}
			if (Input.GetButtonDown("Zoom"))
			{
				this.lastPreset = Mathf.Clamp(this.lastPreset, 1, this.presets.Length - 1);
				this.NetworkcurPreset = ((this.curPreset != 0) ? 0 : this.lastPreset);
				this.CallCmdUpdatePreset(this.curPreset);
			}
		}
	}

	public void SetRelationship()
	{
		if (base.isLocalPlayer)
		{
			return;
		}
		if (this.mySource == null && !base.isLocalPlayer)
		{
			GameObject gameObject = GameObject.Find("Player " + base.GetComponent<HlapiPlayer>().PlayerId + " voice comms");
			if (gameObject != null)
			{
				this.mySource = gameObject.GetComponent<AudioSource>();
			}
			return;
		}
		this.icon.id = 0;
		bool flag = false;
		bool flag2 = false;
		this.mySource.outputAudioMixerGroup = this.g_voice;
		this.mySource.volume = 0f;
		this.mySource.spatialBlend = 1f;
		if (!Radio.roundStarted || Radio.roundEnded || (this.voiceInfo.IsDead() && Radio.localRadio.voiceInfo.IsDead()))
		{
			this.mySource.volume = 1f;
			this.mySource.spatialBlend = 0f;
			return;
		}
		if (this.voiceInfo.isAliveHuman)
		{
			flag2 = true;
		}
		if (this.voiceInfo.isSCP && Radio.localRadio.voiceInfo.isSCP)
		{
			flag2 = true;
			flag = true;
		}
		if (flag2)
		{
			this.icon.id = 1;
			this.mySource.volume = 1f;
		}
		if (!flag && this.host != null && base.gameObject == this.host.GetComponent<Intercom>().speaker)
		{
			this.icon.id = 2;
			this.mySource.outputAudioMixerGroup = this.g_icom;
			flag = true;
			if (this.icomNoise > Radio.noiseIntensity)
			{
				Radio.noiseIntensity = this.icomNoise;
			}
		}
		else if (this.isTransmitting && Radio.localRadio.CheckRadio() && !flag)
		{
			this.mySource.outputAudioMixerGroup = this.g_radio;
			flag = true;
			int lowerPresetID = this.GetLowerPresetID();
			float time = Vector3.Distance(Radio.localRadio.transform.position, base.transform.position);
			this.mySource.volume = this.presets[lowerPresetID].volume.Evaluate(time);
			float num = this.presets[lowerPresetID].nosie.Evaluate(time);
			if (num > Radio.noiseIntensity && !base.isLocalPlayer)
			{
				Radio.noiseIntensity = num;
			}
		}
		if (this.isTransmitting)
		{
			this.icon.id = 2;
		}
		if (flag)
		{
			this.mySource.spatialBlend = 0f;
		}
	}

	public int GetLowerPresetID()
	{
		return (this.curPreset >= Radio.localRadio.curPreset) ? Radio.localRadio.curPreset : this.curPreset;
	}

	public bool CheckRadio()
	{
		return this.myRadio != null && this.myRadio.durability > 0f && this.voiceInfo.isAliveHuman && this.curPreset > 0;
	}

	[Command(channel = 6)]
	private void CmdSyncTransmitionStatus(bool b, Vector3 myPos)
	{
		this.NetworkisTransmitting = b;
		this.CallRpcPlaySound(b, myPos);
	}

	[ClientRpc]
	private void RpcPlaySound(bool b, Vector3 myPos)
	{
		if (Radio.localRadio.CheckRadio() && this.presets[this.GetLowerPresetID()].beepRange > this.Distance(myPos, Radio.localRadio.transform.position))
		{
			this.beepSource.PlayOneShot((!b) ? this.b_off : this.b_on);
		}
	}

	private float Distance(Vector3 a, Vector3 b)
	{
		return Vector3.Distance(new Vector3(a.x, a.y / 4f, a.z), new Vector3(b.x, b.y / 4f, b.z));
	}

	public bool ShouldBeVisible(GameObject localplayer)
	{
		return !this.isTransmitting || this.presets[this.GetLowerPresetID()].beepRange > this.Distance(base.transform.position, localplayer.transform.position);
	}

	private void SetPreset(int preset)
	{
		this.NetworkcurPreset = preset;
	}

	[Command(channel = 6)]
	public void CmdUpdatePreset(int preset)
	{
		this.SetPreset(preset);
	}

	private void UNetVersion()
	{
	}

	public int NetworkcurPreset
	{
		get
		{
			return this.curPreset;
		}
		set
		{
			uint dirtyBit = 1u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SetPreset(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<int>(value, ref this.curPreset, dirtyBit);
		}
	}

	public bool NetworkisTransmitting
	{
		get
		{
			return this.isTransmitting;
		}
		set
		{
			base.SetSyncVar<bool>(value, ref this.isTransmitting, 2u);
		}
	}

	protected static void InvokeCmdCmdSyncTransmitionStatus(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSyncTransmitionStatus called on client.");
			return;
		}
		((Radio)obj).CmdSyncTransmitionStatus(reader.ReadBoolean(), reader.ReadVector3());
	}

	protected static void InvokeCmdCmdUpdatePreset(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdUpdatePreset called on client.");
			return;
		}
		((Radio)obj).CmdUpdatePreset((int)reader.ReadPackedUInt32());
	}

	public void CallCmdSyncTransmitionStatus(bool b, Vector3 myPos)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdSyncTransmitionStatus called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdSyncTransmitionStatus(b, myPos);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)Radio.kCmdCmdSyncTransmitionStatus);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(b);
		networkWriter.Write(myPos);
		base.SendCommandInternal(networkWriter, 6, "CmdSyncTransmitionStatus");
	}

	public void CallCmdUpdatePreset(int preset)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdUpdatePreset called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdUpdatePreset(preset);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)Radio.kCmdCmdUpdatePreset);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.WritePackedUInt32((uint)preset);
		base.SendCommandInternal(networkWriter, 6, "CmdUpdatePreset");
	}

	protected static void InvokeRpcRpcPlaySound(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcPlaySound called on server.");
			return;
		}
		((Radio)obj).RpcPlaySound(reader.ReadBoolean(), reader.ReadVector3());
	}

	public void CallRpcPlaySound(bool b, Vector3 myPos)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("RPC Function RpcPlaySound called on client.");
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)2));
		networkWriter.WritePackedUInt32((uint)Radio.kRpcRpcPlaySound);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(b);
		networkWriter.Write(myPos);
		this.SendRPCInternal(networkWriter, 0, "RpcPlaySound");
	}

	static Radio()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(Radio), Radio.kCmdCmdSyncTransmitionStatus, new NetworkBehaviour.CmdDelegate(Radio.InvokeCmdCmdSyncTransmitionStatus));
		Radio.kCmdCmdUpdatePreset = -1209260349;
		NetworkBehaviour.RegisterCommandDelegate(typeof(Radio), Radio.kCmdCmdUpdatePreset, new NetworkBehaviour.CmdDelegate(Radio.InvokeCmdCmdUpdatePreset));
		Radio.kRpcRpcPlaySound = 1107833674;
		NetworkBehaviour.RegisterRpcDelegate(typeof(Radio), Radio.kRpcRpcPlaySound, new NetworkBehaviour.CmdDelegate(Radio.InvokeRpcRpcPlaySound));
		NetworkCRC.RegisterBehaviour("Radio", 0);
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			writer.WritePackedUInt32((uint)this.curPreset);
			writer.Write(this.isTransmitting);
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
			writer.WritePackedUInt32((uint)this.curPreset);
		}
		if ((base.syncVarDirtyBits & 2u) != 0u)
		{
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
				flag = true;
			}
			writer.Write(this.isTransmitting);
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
			this.curPreset = (int)reader.ReadPackedUInt32();
			this.isTransmitting = reader.ReadBoolean();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			this.SetPreset((int)reader.ReadPackedUInt32());
		}
		if ((num & 2) != 0)
		{
			this.isTransmitting = reader.ReadBoolean();
		}
	}

	public static Radio localRadio;

	public AudioMixerGroup g_voice;

	public AudioMixerGroup g_radio;

	public AudioMixerGroup g_icom;

	public AudioClip b_on;

	public AudioClip b_off;

	public AudioClip b_battery;

	public AudioSource beepSource;

	[Space]
	public AudioSource mySource;

	[Space]
	public Radio.VoiceInfo voiceInfo;

	public Radio.RadioPreset[] presets;

	[SyncVar(hook = "SetPreset")]
	public int curPreset;

	[SyncVar]
	public bool isTransmitting;

	private Item myRadio;

	private float timeToNextTransmition;

	private AudioSource noiseSource;

	private int lastPreset;

	private SpeakerIcon icon;

	private static float noiseIntensity;

	public static bool roundStarted;

	public static bool roundEnded;

	private GameObject host;

	public float icomNoise;

	private Inventory inv;

	public float noiseMultiplier;

	private CharacterClassManager ccm;

	private static int kCmdCmdSyncTransmitionStatus = 860412084;

	private static int kRpcRpcPlaySound;

	private static int kCmdCmdUpdatePreset;

	[Serializable]
	public class VoiceInfo
	{
		public bool IsDead()
		{
			return !this.isSCP && !this.isAliveHuman;
		}

		public bool isAliveHuman;

		public bool isSCP;
	}

	[Serializable]
	public class RadioPreset
	{
		public string label;

		public string powerText;

		public float powerTime;

		public AnimationCurve nosie;

		public AnimationCurve volume;

		public float beepRange;
	}
}
