using System;
using Kino;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Scp079PlayerScript : NetworkBehaviour
{
	private void Start()
	{
		if (TutorialManager.status)
		{
			return;
		}
		this.interfaces = UnityEngine.Object.FindObjectOfType<ScpInterfaces>();
		this.loadingCircle = this.interfaces.Scp049_loading;
		this.lookRotation = GameObject.Find("LookRotation_079").transform;
		if (base.isLocalPlayer)
		{
			this.fpc = base.GetComponent<FirstPersonController>();
			this.cam = UnityEngine.Object.FindObjectOfType<SpectatorCamera>().cam079;
			this.glowCam = UnityEngine.Object.FindObjectOfType<SpectatorCamera>().cam079_glow;
			this.glitchEffect = this.glowCam.GetComponent<AnalogGlitch>();
			this.gui = this.interfaces.Scp079_eq.GetComponent<Interface079>();
			this.gui.localplayer = base.gameObject;
		}
	}

	public void Init(int classID, Class c)
	{
		this.sameClass = (c.team == Team.SCP);
		this.iAm079 = (classID == 7);
		if (base.isLocalPlayer)
		{
			this.gui.gameObject.SetActive(this.iAm079);
			this.interfaces.Scp079_eq.SetActive(this.iAm079);
			this.cam.gameObject.SetActive(this.iAm079);
			if (!this.iAm079 && this.lockedDoor != null)
			{
				this.CallCmdLockDoor(null);
			}
			if (this.iAm079)
			{
				this.cam.transform.SetParent(null);
				this.RefreshCamerasLODs();
			}
		}
	}

	private void Update()
	{
		if (TutorialManager.status)
		{
			return;
		}
		this.CheckForInput();
		this.UpdateInteraction();
		this.MoveCamera();
		this.FovRelation();
		this.DeductLockDown();
	}

	private void DeductLockDown()
	{
		if (this.remainingLockdown > 0f)
		{
			this.remainingLockdown -= Time.deltaTime;
			if (this.remainingLockdown <= 0f)
			{
				this.remainingLockdown = -1f;
			}
		}
		if (this.remainingLockdown == -1f)
		{
			this.remainingLockdown = 0f;
			this.CallCmdLockDoor(null);
		}
	}

	private void FovRelation()
	{
		if (base.isLocalPlayer && this.iAm079)
		{
			this.gui.transform.localScale = Vector3.one * this.fovRelation.Evaluate(this.cam.fieldOfView);
			this.gui.fovSlider.rectTransform.sizeDelta = new Vector2(this.sliderRelation.Evaluate(this.cam.fieldOfView), 13.25f);
		}
	}

	private void MoveCamera()
	{
		if (base.isLocalPlayer && this.iAm079)
		{
			this.targetFov -= Input.GetAxis("Mouse ScrollWheel") * 35f;
			this.targetFov = Mathf.Clamp(this.targetFov, 25f, 85f);
			this.cam.fieldOfView = Mathf.Lerp(this.cam.fieldOfView, this.targetFov, Time.deltaTime * 3.2f);
			this.glowCam.fieldOfView = this.cam.fieldOfView;
			if (this.hackingTime == 0f)
			{
				this.offsetY += Input.GetAxis("Horizontal") * Time.deltaTime * this.cam.fieldOfView * ((!Input.GetButton("Run")) ? 1.6f : 3.3f);
				this.offsetX -= Input.GetAxis("Vertical") * Time.deltaTime * this.cam.fieldOfView * ((!Input.GetButton("Run")) ? 1.6f : 3.3f);
				this.offsetX = Mathf.Clamp(this.offsetX, -20f, 70f);
			}
			else if (!this.isHacked)
			{
				MeshRenderer meshRenderer = this.hit.transform.GetComponent<MeshRenderer>();
				if (meshRenderer == null)
				{
					meshRenderer = this.hit.transform.GetComponentInParent<MeshRenderer>();
				}
				if (meshRenderer == null)
				{
					meshRenderer = this.hit.transform.GetComponentInChildren<MeshRenderer>();
				}
				this.lookRotation.LookAt(meshRenderer.bounds.center);
				this.offsetX = this.lookRotation.rotation.eulerAngles.x;
				this.offsetY = this.lookRotation.rotation.eulerAngles.y;
				if (this.offsetX > 180f)
				{
					this.offsetX -= 360f;
				}
			}
			this.cam.transform.localRotation = Quaternion.Lerp(this.cam.transform.localRotation, Quaternion.Euler(new Vector3(this.offsetX, this.offsetY, 0f)), Time.deltaTime * 9.5f);
		}
	}

	private void CheckForInput()
	{
		if (this.iAm079 && base.isLocalPlayer)
		{
			if (Input.GetButtonDown("Fire1"))
			{
				this.Interact();
			}
			if (Input.GetButtonDown("Zoom"))
			{
				this.StopInteracting();
			}
		}
	}

	private void Interact()
	{
		if (!this.isHacked && this.interactableType == "unclassified")
		{
			if (Physics.Raycast(this.cam.ScreenPointToRay(Input.mousePosition), out this.hit, 50f, this.cameraMask))
			{
				this.interactable = this.hit.transform.GetComponentInParent<CCTV_Camera>().gameObject;
				this.interactableType = this.RecognizeInteractable();
			}
			else if (Physics.Raycast(this.cam.ScreenPointToRay(Input.mousePosition), out this.hit))
			{
				this.interactable = this.hit.transform.gameObject;
				this.interactableType = this.RecognizeInteractable();
			}
		}
		if (this.interactableType == "stop")
		{
			this.StopInteracting();
		}
	}

	public void StopInteracting()
	{
		this.isHacked = false;
		this.gui.ResetProgress();
		this.interactable = null;
		this.hackingTime = 0f;
		this.interactableType = "unclassified";
	}

	private void UpdateInteraction()
	{
		if (base.isLocalPlayer)
		{
			CursorManager.is079 = ((this.iAm079 && this.hackingTime == 0f) || this.isHacked);
			this.glitchEffect.colorDrift = ((!CursorManager.is079) ? 0.15f : 0f);
		}
		if (base.isLocalPlayer && this.iAm079)
		{
			this.gui.console.SetActive(this.isHacked);
			this.gui.progress_obj.SetActive(this.hackingTime != 0f && !this.isHacked);
			if (this.hackingTime != 0f)
			{
				this.gui.AddProgress(Time.deltaTime / this.hackingTime);
			}
			else
			{
				this.gui.ResetProgress();
			}
			if (this.gui.Action())
			{
				if (!this.isHacked)
				{
					this.isHacked = true;
					if (this.interactableType == "door" || this.interactableType == "sdoor")
					{
						GameObject gameObject = (!(this.interactableType == "door")) ? GameObject.Find(this.interactable.name.Remove(this.interactable.name.IndexOf("-"))) : this.interactable;
						this.gui.SetConsoleScreen(0);
					}
					else if (this.interactableType == "cctv")
					{
						CCTV_Camera cctv_Camera = this.hit.transform.GetComponent<CCTV_Camera>();
						if (cctv_Camera == null)
						{
							cctv_Camera = this.hit.transform.GetComponentInParent<CCTV_Camera>();
						}
						this.cam.transform.position = cctv_Camera.cameraTarget.position;
						this.curCamera = cctv_Camera.gameObject;
						this.liftID = cctv_Camera.liftID;
						this.gui.liftButton.SetActive(this.liftID != string.Empty && !this.liftID.Contains("tesla"));
						this.gui.teslaButton.SetActive(this.liftID.Contains("tesla"));
						this.isHacked = false;
						this.StopInteracting();
						this.RefreshCamerasLODs();
					}
					else if (this.interactableType == "lift")
					{
						CCTV_Camera[] array = UnityEngine.Object.FindObjectsOfType<CCTV_Camera>();
						foreach (CCTV_Camera cctv_Camera2 in array)
						{
							if (cctv_Camera2.liftID.Contains(this.liftID.Remove(4)) && cctv_Camera2.liftID != this.liftID)
							{
								this.cam.transform.position = cctv_Camera2.cameraTarget.position;
								this.liftID = cctv_Camera2.liftID;
								this.gui.liftButton.SetActive(this.liftID != string.Empty);
								this.isHacked = false;
								this.StopInteracting();
								this.RefreshCamerasLODs();
								break;
							}
						}
					}
				}
				else
				{
					if (Input.GetKeyDown(KeyCode.Alpha1))
					{
						this.Console_OpenDoor();
						this.StopInteracting();
					}
					if (Input.GetKeyDown(KeyCode.Alpha2))
					{
						this.Console_LockDoor();
						this.StopInteracting();
					}
				}
			}
		}
	}

	private string RecognizeInteractable()
	{
		string result = "unclassified";
		if (this.interactable != null)
		{
			if (this.interactable.GetComponentInParent<Door>() != null)
			{
				result = "door";
				this.hackingTime = 0.3f;
			}
			else if (this.interactable.tag == "CCTV")
			{
				result = "cctv";
				this.hackingTime = 0.3f;
			}
			else if (this.interactable.tag == "LiftTarget")
			{
				result = "lift";
				this.hackingTime = 3.6f;
			}
			if (this.hackingTime == 0f)
			{
				result = "stop";
			}
		}
		return result;
	}

	public void UseElevator()
	{
		if (!this.isHacked && this.interactableType == "unclassified")
		{
			this.interactable = GameObject.FindGameObjectWithTag("LiftTarget");
			this.interactableType = this.RecognizeInteractable();
		}
	}

	public void UseTesla()
	{
		if (!this.isHacked && this.interactableType == "unclassified")
		{
			this.CallCmdTriggerTesla(this.curCamera.GetComponentInParent<TeslaGate>().gameObject);
		}
	}

	[Command(channel = 4)]
	private void CmdTriggerTesla(GameObject tesla)
	{
		this.CallRpcTriggerTelsa(tesla);
	}

	[ClientRpc(channel = 4)]
	private void RpcTriggerTelsa(GameObject tesla)
	{
		tesla.GetComponent<TeslaGate>().Hack();
	}

	private void RefreshCamerasLODs()
	{
		CCTV_Camera[] array = UnityEngine.Object.FindObjectsOfType<CCTV_Camera>();
		foreach (CCTV_Camera cctv_Camera in array)
		{
			cctv_Camera.UpdateLOD();
		}
	}

	private void LateUpdate()
	{
		this.SetListenerPosition();
		this.SetLookRotationPosition();
	}

	private void SetListenerPosition()
	{
		if (base.isLocalPlayer && this.iAm079)
		{
			if (this.spectCam == null)
			{
				this.spectCam = UnityEngine.Object.FindObjectOfType<SpectatorCamera>();
				return;
			}
			this.spectCam.cam.transform.position = this.spectCam.cam079.transform.position;
			this.spectCam.cam.transform.rotation = this.spectCam.cam079.transform.rotation;
		}
	}

	private void SetLookRotationPosition()
	{
		if (base.isLocalPlayer && this.iAm079)
		{
			this.lookRotation.transform.position = this.cam.transform.position;
		}
	}

	[Command(channel = 4)]
	private void CmdOpenDoor(GameObject door)
	{
		if (base.GetComponent<CharacterClassManager>().curClass == 7)
		{
			door.GetComponentInParent<Door>().ChangeState();
		}
	}

	[Command(channel = 4)]
	private void CmdLockDoor(GameObject door)
	{
		this.CallRpcLockDoor(door);
	}

	[ClientRpc(channel = 4)]
	private void RpcLockDoor(GameObject door)
	{
		this.lockedDoor = door;
	}

	public void Console_OpenDoor()
	{
		if (this.interactableType == "door" || this.interactableType == "sdoor")
		{
			GameObject door = (!(this.interactableType == "door")) ? GameObject.Find(this.interactable.name.Remove(this.interactable.name.IndexOf("-"))) : this.interactable;
			this.CallCmdOpenDoor(door);
		}
	}

	public void Console_LockDoor()
	{
		if (this.interactableType == "door" || (this.interactableType == "sdoor" && this.gui.ability > 40f))
		{
			this.gui.ability = 0f;
			GameObject door = (!(this.interactableType == "door")) ? GameObject.Find(this.interactable.name.Remove(this.interactable.name.IndexOf("-"))) : this.interactable;
			this.CallCmdLockDoor(door);
			this.remainingLockdown = UnityEngine.Random.Range(this.minLockTime, this.maxLockTime);
		}
	}

	private void UNetVersion()
	{
	}

	protected static void InvokeCmdCmdTriggerTesla(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdTriggerTesla called on client.");
			return;
		}
		((Scp079PlayerScript)obj).CmdTriggerTesla(reader.ReadGameObject());
	}

	protected static void InvokeCmdCmdOpenDoor(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdOpenDoor called on client.");
			return;
		}
		((Scp079PlayerScript)obj).CmdOpenDoor(reader.ReadGameObject());
	}

	protected static void InvokeCmdCmdLockDoor(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdLockDoor called on client.");
			return;
		}
		((Scp079PlayerScript)obj).CmdLockDoor(reader.ReadGameObject());
	}

	public void CallCmdTriggerTesla(GameObject tesla)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdTriggerTesla called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdTriggerTesla(tesla);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)Scp079PlayerScript.kCmdCmdTriggerTesla);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(tesla);
		base.SendCommandInternal(networkWriter, 4, "CmdTriggerTesla");
	}

	public void CallCmdOpenDoor(GameObject door)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdOpenDoor called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdOpenDoor(door);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)Scp079PlayerScript.kCmdCmdOpenDoor);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(door);
		base.SendCommandInternal(networkWriter, 4, "CmdOpenDoor");
	}

	public void CallCmdLockDoor(GameObject door)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdLockDoor called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdLockDoor(door);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)Scp079PlayerScript.kCmdCmdLockDoor);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(door);
		base.SendCommandInternal(networkWriter, 4, "CmdLockDoor");
	}

	protected static void InvokeRpcRpcTriggerTelsa(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcTriggerTelsa called on server.");
			return;
		}
		((Scp079PlayerScript)obj).RpcTriggerTelsa(reader.ReadGameObject());
	}

	protected static void InvokeRpcRpcLockDoor(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcLockDoor called on server.");
			return;
		}
		((Scp079PlayerScript)obj).RpcLockDoor(reader.ReadGameObject());
	}

	public void CallRpcTriggerTelsa(GameObject tesla)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("RPC Function RpcTriggerTelsa called on client.");
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)2));
		networkWriter.WritePackedUInt32((uint)Scp079PlayerScript.kRpcRpcTriggerTelsa);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(tesla);
		this.SendRPCInternal(networkWriter, 4, "RpcTriggerTelsa");
	}

	public void CallRpcLockDoor(GameObject door)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("RPC Function RpcLockDoor called on client.");
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)2));
		networkWriter.WritePackedUInt32((uint)Scp079PlayerScript.kRpcRpcLockDoor);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(door);
		this.SendRPCInternal(networkWriter, 4, "RpcLockDoor");
	}

	static Scp079PlayerScript()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(Scp079PlayerScript), Scp079PlayerScript.kCmdCmdTriggerTesla, new NetworkBehaviour.CmdDelegate(Scp079PlayerScript.InvokeCmdCmdTriggerTesla));
		Scp079PlayerScript.kCmdCmdOpenDoor = 1928731062;
		NetworkBehaviour.RegisterCommandDelegate(typeof(Scp079PlayerScript), Scp079PlayerScript.kCmdCmdOpenDoor, new NetworkBehaviour.CmdDelegate(Scp079PlayerScript.InvokeCmdCmdOpenDoor));
		Scp079PlayerScript.kCmdCmdLockDoor = 47734807;
		NetworkBehaviour.RegisterCommandDelegate(typeof(Scp079PlayerScript), Scp079PlayerScript.kCmdCmdLockDoor, new NetworkBehaviour.CmdDelegate(Scp079PlayerScript.InvokeCmdCmdLockDoor));
		Scp079PlayerScript.kRpcRpcTriggerTelsa = -1321605819;
		NetworkBehaviour.RegisterRpcDelegate(typeof(Scp079PlayerScript), Scp079PlayerScript.kRpcRpcTriggerTelsa, new NetworkBehaviour.CmdDelegate(Scp079PlayerScript.InvokeRpcRpcTriggerTelsa));
		Scp079PlayerScript.kRpcRpcLockDoor = -3650067;
		NetworkBehaviour.RegisterRpcDelegate(typeof(Scp079PlayerScript), Scp079PlayerScript.kRpcRpcLockDoor, new NetworkBehaviour.CmdDelegate(Scp079PlayerScript.InvokeRpcRpcLockDoor));
		NetworkCRC.RegisterBehaviour("Scp079PlayerScript", 0);
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
	public bool iAm079;

	public bool sameClass;

	public LayerMask cameraMask;

	[Header("FOV relationships")]
	public AnimationCurve fovRelation;

	public AnimationCurve sliderRelation;

	[Header("Lockdown")]
	public float remainingLockdown;

	public GameObject lockedDoor;

	public float minLockTime = 13f;

	public float maxLockTime = 28f;

	private ScpInterfaces interfaces;

	private Camera cam;

	private Camera glowCam;

	private Image loadingCircle;

	private FirstPersonController fpc;

	private Interface079 gui;

	private SpectatorCamera spectCam;

	private Transform lookRotation;

	private AnalogGlitch glitchEffect;

	private GameObject curCamera;

	private GameObject interactable;

	private string interactableType = "unclassified";

	private float hackingTime;

	private RaycastHit hit;

	private bool isHacked;

	private float targetFov = 75f;

	private float offsetX;

	private float offsetY;

	private string liftID;

	private static int kCmdCmdTriggerTesla = -1405031459;

	private static int kRpcRpcTriggerTelsa;

	private static int kCmdCmdOpenDoor;

	private static int kCmdCmdLockDoor;

	private static int kRpcRpcLockDoor;
}
