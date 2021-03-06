﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Intercom : NetworkBehaviour
{
	public GameObject Networkspeaker
	{
		get
		{
			return this.speaker;
		}
		set
		{
			uint dirtyBit = 1u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SetSpeaker(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVarGameObject(value, ref this.speaker, dirtyBit, ref this.___speakerNetId);
		}
	}

	public Intercom()
	{
	}

	private void SetSpeaker(GameObject go)
	{
		this.Networkspeaker = go;
	}

	private IEnumerator StartTransmitting(GameObject sp)
	{
		this.CallRpcPlaySound(true);
		yield return new WaitForSeconds(2f);
		this.SetSpeaker(sp);
		this.speechRemainingTime = this.speechTime;
		while (this.speechRemainingTime > 0f && this.speaker != null)
		{
			this.speechRemainingTime -= Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		if (this.speaker != null)
		{
			this.SetSpeaker(null);
		}
		this.CallRpcPlaySound(false);
		this.remainingCooldown = this.cooldownAfter;
		while (this.remainingCooldown >= 0f)
		{
			this.remainingCooldown -= Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		this.inUse = false;
		yield break;
	}

	private void Start()
	{
		if (TutorialManager.status)
		{
			return;
		}
		this.txt = GameObject.Find("IntercomMonitor").GetComponent<Text>();
		this.ccm = base.GetComponent<CharacterClassManager>();
		this.area = GameObject.Find("IntercomSpeakingZone").transform;
		this.speechTime = (float)ConfigFile.GetInt("intercom_max_speech_time", 20);
		this.cooldownAfter = (float)ConfigFile.GetInt("intercom_cooldown", 180);
		base.StartCoroutine(this.FindHost());
		base.StartCoroutine(this.CheckForInput());
		if (base.isLocalPlayer && base.isServer)
		{
			base.InvokeRepeating("RefreshText", 5f, 7f);
		}
	}

	private void RefreshText()
	{
		this.CallCmdUpdateText(this.content);
	}

	private IEnumerator FindHost()
	{
		while (Intercom.host == null)
		{
			GameObject h = GameObject.Find("Host");
			if (h != null)
			{
				Intercom.host = h.GetComponent<Intercom>();
			}
			yield return new WaitForFixedUpdate();
		}
		yield break;
	}

	[ClientRpc]
	public void RpcPlaySound(bool start)
	{
		GameObject obj = UnityEngine.Object.Instantiate<GameObject>((!start) ? this.stop_sound : this.start_sound);
		UnityEngine.Object.Destroy(obj, 10f);
	}

	private void Update()
	{
		if (TutorialManager.status)
		{
			return;
		}
		if (base.isLocalPlayer && base.isServer)
		{
			this.UpdateText();
		}
	}

	private void UpdateText()
	{
		if (this.remainingCooldown > 0f)
		{
			this.content = "RESTARTING\n" + Mathf.CeilToInt(this.remainingCooldown);
		}
		else if (this.speaker != null)
		{
			this.content = "TRANSMITTING...\nTIME LEFT - " + Mathf.CeilToInt(this.speechRemainingTime);
		}
		else
		{
			this.content = "READY";
		}
		if (this.content != this.txt.text)
		{
			this.CallCmdUpdateText(this.content);
		}
	}

	[Command(channel = 2)]
	private void CmdUpdateText(string t)
	{
		this.CallRpcUpdateText(t);
	}

	[ClientRpc(channel = 2)]
	private void RpcUpdateText(string t)
	{
		try
		{
			this.txt.text = t;
		}
		catch
		{
		}
	}

	public void RequestTransmission(GameObject spk)
	{
		if (spk == null)
		{
			this.SetSpeaker(null);
		}
		else if (this.remainingCooldown <= 0f && !this.inUse)
		{
			this.inUse = true;
			base.StartCoroutine(this.StartTransmitting(spk));
		}
	}

	private IEnumerator CheckForInput()
	{
		if (!base.isLocalPlayer)
		{
			yield return new WaitForEndOfFrame();
			yield break;
		}
		for (;;)
		{
			if (Intercom.host != null)
			{
				if (this.AllowToSpeak() && Intercom.host.speaker == null)
				{
					this.CallCmdSetTransmit(base.gameObject);
				}
				if (!this.AllowToSpeak() && Intercom.host.speaker == base.gameObject)
				{
					yield return new WaitForSeconds(1f);
					if (!this.AllowToSpeak())
					{
						this.CallCmdSetTransmit(null);
					}
				}
			}
			yield return new WaitForEndOfFrame();
		}
	}

	private bool AllowToSpeak()
	{
		return Vector3.Distance(base.transform.position, this.area.position) < this.triggerDistance && Input.GetButton("VoiceChat") && this.ccm.klasy[this.ccm.curClass].team != Team.SCP;
	}

	[Command(channel = 2)]
	private void CmdSetTransmit(GameObject player)
	{
		GameObject.Find("Host").GetComponent<Intercom>().RequestTransmission(player);
	}

	private void UNetVersion()
	{
	}

	protected static void InvokeCmdCmdUpdateText(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdUpdateText called on client.");
			return;
		}
		((Intercom)obj).CmdUpdateText(reader.ReadString());
	}

	protected static void InvokeCmdCmdSetTransmit(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSetTransmit called on client.");
			return;
		}
		((Intercom)obj).CmdSetTransmit(reader.ReadGameObject());
	}

	public void CallCmdUpdateText(string t)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdUpdateText called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdUpdateText(t);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)Intercom.kCmdCmdUpdateText);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(t);
		base.SendCommandInternal(networkWriter, 2, "CmdUpdateText");
	}

	public void CallCmdSetTransmit(GameObject player)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdSetTransmit called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdSetTransmit(player);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)Intercom.kCmdCmdSetTransmit);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(player);
		base.SendCommandInternal(networkWriter, 2, "CmdSetTransmit");
	}

	protected static void InvokeRpcRpcPlaySound(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcPlaySound called on server.");
			return;
		}
		((Intercom)obj).RpcPlaySound(reader.ReadBoolean());
	}

	protected static void InvokeRpcRpcUpdateText(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcUpdateText called on server.");
			return;
		}
		((Intercom)obj).RpcUpdateText(reader.ReadString());
	}

	public void CallRpcPlaySound(bool start)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("RPC Function RpcPlaySound called on client.");
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)2));
		networkWriter.WritePackedUInt32((uint)Intercom.kRpcRpcPlaySound);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(start);
		this.SendRPCInternal(networkWriter, 0, "RpcPlaySound");
	}

	public void CallRpcUpdateText(string t)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("RPC Function RpcUpdateText called on client.");
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)2));
		networkWriter.WritePackedUInt32((uint)Intercom.kRpcRpcUpdateText);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(t);
		this.SendRPCInternal(networkWriter, 2, "RpcUpdateText");
	}

	static Intercom()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(Intercom), Intercom.kCmdCmdUpdateText, new NetworkBehaviour.CmdDelegate(Intercom.InvokeCmdCmdUpdateText));
		Intercom.kCmdCmdSetTransmit = 1248049261;
		NetworkBehaviour.RegisterCommandDelegate(typeof(Intercom), Intercom.kCmdCmdSetTransmit, new NetworkBehaviour.CmdDelegate(Intercom.InvokeCmdCmdSetTransmit));
		Intercom.kRpcRpcPlaySound = 239129888;
		NetworkBehaviour.RegisterRpcDelegate(typeof(Intercom), Intercom.kRpcRpcPlaySound, new NetworkBehaviour.CmdDelegate(Intercom.InvokeRpcRpcPlaySound));
		Intercom.kRpcRpcUpdateText = 1243388753;
		NetworkBehaviour.RegisterRpcDelegate(typeof(Intercom), Intercom.kRpcRpcUpdateText, new NetworkBehaviour.CmdDelegate(Intercom.InvokeRpcRpcUpdateText));
		NetworkCRC.RegisterBehaviour("Intercom", 0);
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			writer.Write(this.speaker);
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
			writer.Write(this.speaker);
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
			this.___speakerNetId = reader.ReadNetworkId();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			this.SetSpeaker(reader.ReadGameObject());
		}
	}

	public override void PreStartClient()
	{
		if (!this.___speakerNetId.IsEmpty())
		{
			this.Networkspeaker = ClientScene.FindLocalObject(this.___speakerNetId);
		}
	}

	private CharacterClassManager ccm;

	private Transform area;

	public float triggerDistance;

	private float speechTime;

	private float cooldownAfter;

	public float speechRemainingTime;

	public float remainingCooldown;

	public Text txt;

	[SyncVar(hook = "SetSpeaker")]
	public GameObject speaker;

	public static Intercom host;

	public GameObject start_sound;

	public GameObject stop_sound;

	private string content = string.Empty;

	private bool inUse;

	private bool isTransmitting;

	private NetworkInstanceId ___speakerNetId;

	private static int kRpcRpcPlaySound;

	private static int kCmdCmdUpdateText = -915354885;

	private static int kRpcRpcUpdateText;

	private static int kCmdCmdSetTransmit;
}
