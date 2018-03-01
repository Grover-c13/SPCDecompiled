using System;
using Dissonance.Integrations.UNet_HLAPI;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.ImageEffects;

public class Scp173PlayerScript : NetworkBehaviour
{
	public Scp173PlayerScript()
	{
	}

	private void Start()
	{
		this.ps = base.GetComponent<PlayerStats>();
		this.public_ccm = base.GetComponent<CharacterClassManager>();
		if (!base.isLocalPlayer)
		{
			return;
		}
		this.blinkCtrl = base.GetComponentInChildren<VignetteAndChromaticAberration>();
		this.fpc = base.GetComponent<FirstPersonController>();
		this.pms = base.GetComponent<PlyMovementSync>();
	}

	public void Init(int classID, Class c)
	{
		this.sameClass = (c.team == Team.SCP);
		if (base.isLocalPlayer)
		{
			this.fpc.lookingAtMe = false;
		}
		if (Scp173PlayerScript.scpInstance != null)
		{
			if (Scp173PlayerScript.scpInstance.GetComponent<CharacterClassManager>().curClass != 0)
			{
				Scp173PlayerScript.scpInstance = null;
			}
		}
		else if (this.public_ccm.curClass == 0)
		{
			Scp173PlayerScript.scpInstance = base.gameObject;
		}
		base.GetComponent<PlyMovementSync>().iAm173 = (classID == 0);
		this.iAm173 = (classID == 0);
		if (base.isLocalPlayer)
		{
			Scp173PlayerScript.localplayerIs173 = this.iAm173;
		}
		this.hitbox.SetActive(!base.isLocalPlayer && Scp173PlayerScript.localplayerIs173);
	}

	private void FixedUpdate()
	{
		this.DoBlinkingSequence();
		if (base.isLocalPlayer && this.iAm173)
		{
			this.allowMove = true;
			foreach (GameObject gameObject in PlayerManager.singleton.players)
			{
				Scp173PlayerScript component = gameObject.GetComponent<Scp173PlayerScript>();
				if (!component.sameClass && component.LookFor173())
				{
					this.cooldown = 10;
					this.allowMove = false;
				}
			}
			this.CheckForInput();
			this.fpc.lookingAtMe = (!this.allowMove && this.cooldown > 0);
			if (this.cooldown >= 0)
			{
				this.cooldown--;
			}
			float num = this.boost_speed.Evaluate(this.ps.GetHealthPercent());
			this.fpc.m_WalkSpeed = num;
			this.fpc.m_RunSpeed = num;
		}
	}

	public bool LookFor173()
	{
		RaycastHit raycastHit;
		return Scp173PlayerScript.scpInstance && this.public_ccm.curClass != 2 && (Vector3.Dot(this.cam.transform.forward, (this.cam.transform.position - Scp173PlayerScript.scpInstance.transform.position).normalized) < -0.65f && Physics.Raycast(this.cam.transform.position, (Scp173PlayerScript.scpInstance.transform.position - this.cam.transform.position).normalized, out raycastHit, this.range, this.layerMask) && raycastHit.transform.name == Scp173PlayerScript.scpInstance.name);
	}

	private void DoBlinkingSequence()
	{
		if (base.isServer && base.isLocalPlayer)
		{
			this.remainingTime -= Time.fixedDeltaTime;
			if (this.remainingTime < 0f)
			{
				this.remainingTime = UnityEngine.Random.Range(this.minBlinkTime, this.maxBlinkTime);
				foreach (Scp173PlayerScript scp173PlayerScript in UnityEngine.Object.FindObjectsOfType<Scp173PlayerScript>())
				{
					scp173PlayerScript.CallCmdBlinkTime();
				}
			}
		}
	}

	public void Boost()
	{
		if (base.isLocalPlayer)
		{
			this.pms.SetRotation(base.transform.rotation.eulerAngles.y);
			if (this.fpc.lookingAtMe)
			{
				bool flag = false;
				RaycastHit raycastHit;
				if (Physics.Raycast(this.cam.transform.position, this.cam.transform.forward, out raycastHit, 100f, this.teleportMask) && raycastHit.transform.GetComponent<CharacterClassManager>() != null && Input.GetAxisRaw("Vertical") > 0f && Input.GetAxisRaw("Horizontal") == 0f)
				{
					flag = true;
				}
				float num = this.boost_teleportDistance.Evaluate(this.ps.GetHealthPercent());
				Vector3 position = base.transform.position;
				if (flag)
				{
					Vector3 b = raycastHit.transform.position - base.transform.position;
					b = b.normalized * Mathf.Clamp(b.magnitude - 1f, 0f, num);
					base.transform.position += b;
				}
				else
				{
					for (int i = 0; i < 1000; i++)
					{
						if (Vector3.Distance(position, base.transform.position) >= num)
						{
							break;
						}
						this.Forward();
					}
				}
			}
			if (Input.GetButton("Fire1"))
			{
				this.Shoot();
			}
		}
	}

	private void Forward()
	{
		this.fpc.blinkAddition = 0.8f;
		this.fpc.MotorPlayer();
		this.fpc.blinkAddition = 0f;
	}

	public void Blink()
	{
		if (base.isLocalPlayer)
		{
			bool flag = this.LookFor173();
			if (flag)
			{
				this.blinkCtrl.intensity = 1f;
				this.weaponCameras.SetActive(false);
			}
			base.Invoke("UnBlink", (!flag) ? this.blinkDuration_notsee : this.blinkDuration_see);
		}
	}

	private void UnBlink()
	{
		this.blinkCtrl.intensity = 0.036f;
		this.weaponCameras.SetActive(true);
	}

	private void CheckForInput()
	{
		if (Input.GetButtonDown("Fire1") && this.allowMove)
		{
			this.Shoot();
		}
	}

	private void Shoot()
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(this.cam.transform.position, this.cam.transform.forward, out raycastHit, 1.5f, this.hurtLayer))
		{
			CharacterClassManager component = raycastHit.transform.GetComponent<CharacterClassManager>();
			if (component != null && component.klasy[component.curClass].team != Team.SCP)
			{
				this.CallCmdDoAudio();
				this.HurtPlayer(raycastHit.transform.gameObject, base.GetComponent<HlapiPlayer>().PlayerId);
			}
		}
	}

	private void HurtPlayer(GameObject go, string plyID)
	{
		Hitmarker.Hit(1f);
		this.CallCmdHurtPlayer(go);
	}

	[Command(channel = 2)]
	private void CmdHurtPlayer(GameObject target)
	{
		if (base.GetComponent<CharacterClassManager>().curClass == 0 && Vector3.Distance(base.GetComponent<PlyMovementSync>().position, target.transform.position) < 3f + this.boost_teleportDistance.Evaluate(this.ps.GetHealthPercent()))
		{
			target.GetComponent<PlayerStats>().HurtPlayer(new PlayerStats.HitInfo(999999f, "SCP173", "SCP:173"), target);
		}
	}

	[Command(channel = 0)]
	private void CmdBlinkTime()
	{
		this.CallRpcBlinkTime();
	}

	[ClientRpc(channel = 0)]
	private void RpcBlinkTime()
	{
		if (this.iAm173)
		{
			this.Boost();
		}
		if (!this.sameClass)
		{
			this.Blink();
		}
	}

	[Command(channel = 1)]
	private void CmdDoAudio()
	{
		this.CallRpcSyncAudio();
	}

	[ClientRpc(channel = 1)]
	private void RpcSyncAudio()
	{
		base.GetComponent<AnimationController>().gunSource.PlayOneShot(this.necksnaps[UnityEngine.Random.Range(0, this.necksnaps.Length)]);
	}

	private void UNetVersion()
	{
	}

	protected static void InvokeCmdCmdHurtPlayer(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdHurtPlayer called on client.");
			return;
		}
		((Scp173PlayerScript)obj).CmdHurtPlayer(reader.ReadGameObject());
	}

	protected static void InvokeCmdCmdBlinkTime(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdBlinkTime called on client.");
			return;
		}
		((Scp173PlayerScript)obj).CmdBlinkTime();
	}

	protected static void InvokeCmdCmdDoAudio(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdDoAudio called on client.");
			return;
		}
		((Scp173PlayerScript)obj).CmdDoAudio();
	}

	public void CallCmdHurtPlayer(GameObject target)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdHurtPlayer called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdHurtPlayer(target);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)Scp173PlayerScript.kCmdCmdHurtPlayer);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(target);
		base.SendCommandInternal(networkWriter, 2, "CmdHurtPlayer");
	}

	public void CallCmdBlinkTime()
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdBlinkTime called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdBlinkTime();
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)Scp173PlayerScript.kCmdCmdBlinkTime);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		base.SendCommandInternal(networkWriter, 0, "CmdBlinkTime");
	}

	public void CallCmdDoAudio()
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdDoAudio called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdDoAudio();
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)Scp173PlayerScript.kCmdCmdDoAudio);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		base.SendCommandInternal(networkWriter, 1, "CmdDoAudio");
	}

	protected static void InvokeRpcRpcBlinkTime(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcBlinkTime called on server.");
			return;
		}
		((Scp173PlayerScript)obj).RpcBlinkTime();
	}

	protected static void InvokeRpcRpcSyncAudio(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcSyncAudio called on server.");
			return;
		}
		((Scp173PlayerScript)obj).RpcSyncAudio();
	}

	public void CallRpcBlinkTime()
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("RPC Function RpcBlinkTime called on client.");
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)2));
		networkWriter.WritePackedUInt32((uint)Scp173PlayerScript.kRpcRpcBlinkTime);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		this.SendRPCInternal(networkWriter, 0, "RpcBlinkTime");
	}

	public void CallRpcSyncAudio()
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("RPC Function RpcSyncAudio called on client.");
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)2));
		networkWriter.WritePackedUInt32((uint)Scp173PlayerScript.kRpcRpcSyncAudio);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		this.SendRPCInternal(networkWriter, 1, "RpcSyncAudio");
	}

	static Scp173PlayerScript()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(Scp173PlayerScript), Scp173PlayerScript.kCmdCmdHurtPlayer, new NetworkBehaviour.CmdDelegate(Scp173PlayerScript.InvokeCmdCmdHurtPlayer));
		Scp173PlayerScript.kCmdCmdBlinkTime = 514139536;
		NetworkBehaviour.RegisterCommandDelegate(typeof(Scp173PlayerScript), Scp173PlayerScript.kCmdCmdBlinkTime, new NetworkBehaviour.CmdDelegate(Scp173PlayerScript.InvokeCmdCmdBlinkTime));
		Scp173PlayerScript.kCmdCmdDoAudio = 1060446482;
		NetworkBehaviour.RegisterCommandDelegate(typeof(Scp173PlayerScript), Scp173PlayerScript.kCmdCmdDoAudio, new NetworkBehaviour.CmdDelegate(Scp173PlayerScript.InvokeCmdCmdDoAudio));
		Scp173PlayerScript.kRpcRpcBlinkTime = -1078791558;
		NetworkBehaviour.RegisterRpcDelegate(typeof(Scp173PlayerScript), Scp173PlayerScript.kRpcRpcBlinkTime, new NetworkBehaviour.CmdDelegate(Scp173PlayerScript.InvokeRpcRpcBlinkTime));
		Scp173PlayerScript.kRpcRpcSyncAudio = -769227732;
		NetworkBehaviour.RegisterRpcDelegate(typeof(Scp173PlayerScript), Scp173PlayerScript.kRpcRpcSyncAudio, new NetworkBehaviour.CmdDelegate(Scp173PlayerScript.InvokeRpcRpcSyncAudio));
		NetworkCRC.RegisterBehaviour("Scp173PlayerScript", 0);
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		bool result;
		return result;
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
	}

	[Header("Player Properties")]
	public static GameObject scpInstance;

	public bool iAm173;

	public bool sameClass;

	[Header("Raycasting")]
	public GameObject cam;

	public float range;

	public LayerMask layerMask;

	public LayerMask teleportMask;

	public LayerMask hurtLayer;

	[Header("Blinking")]
	public float minBlinkTime;

	public float maxBlinkTime;

	public float blinkDuration_notsee;

	public float blinkDuration_see;

	private float remainingTime;

	private VignetteAndChromaticAberration blinkCtrl;

	private FirstPersonController fpc;

	private PlyMovementSync pms;

	private CharacterClassManager public_ccm;

	private PlayerStats ps;

	public GameObject weaponCameras;

	public GameObject hitbox;

	public AudioClip[] necksnaps;

	[Header("Boosts")]
	public AnimationCurve boost_teleportDistance;

	public AnimationCurve boost_speed;

	private bool allowMove = true;

	private static bool localplayerIs173;

	private int cooldown;

	private static int kCmdCmdHurtPlayer = 1571551081;

	private static int kCmdCmdBlinkTime;

	private static int kRpcRpcBlinkTime;

	private static int kCmdCmdDoAudio;

	private static int kRpcRpcSyncAudio;
}
