using System;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Handcuffs : NetworkBehaviour
{
	private void Start()
	{
		this.uncuffProgress = GameObject.Find("UncuffProgress").GetComponent<Image>();
		this.inv = base.GetComponent<Inventory>();
		this.plyCam = base.GetComponent<Scp049PlayerScript>().plyCam.transform;
		this.ccm = base.GetComponent<CharacterClassManager>();
	}

	private void Update()
	{
		if (base.isLocalPlayer)
		{
			this.CheckForInput();
			this.UpdateText();
		}
		if (this.cuffTarget != null)
		{
			this.cuffTarget.GetComponent<AnimationController>().cuffed = true;
		}
	}

	private void CheckForInput()
	{
		if (this.cuffTarget != null)
		{
			bool flag = false;
			foreach (Item item in this.inv.items)
			{
				if (item.id == 27)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				this.CallCmdTarget(null);
			}
		}
		if (base.GetComponent<WeaponManager>().inventoryCooldown <= 0f)
		{
			if (this.inv.curItem == 27)
			{
				if (Input.GetButtonDown("Fire1") && this.cuffTarget == null)
				{
					this.CuffPlayer();
				}
				else if (Input.GetButtonDown("Zoom") && this.cuffTarget != null)
				{
					this.CallCmdTarget(null);
				}
			}
			if (this.ccm.curClass >= 0 && this.ccm.klasy[this.ccm.curClass].team != Team.SCP && Input.GetButton("Interact"))
			{
				RaycastHit raycastHit;
				if (Physics.Raycast(this.plyCam.position, this.plyCam.forward, out raycastHit, this.maxDistance))
				{
					Handcuffs componentInParent = raycastHit.collider.GetComponentInParent<Handcuffs>();
					if (componentInParent != null && componentInParent.GetComponent<AnimationController>().handAnimator.GetBool("Cuffed"))
					{
						this.progress += Time.deltaTime;
						if (this.progress >= 5f)
						{
							this.progress = 0f;
							foreach (GameObject gameObject in PlayerManager.singleton.players)
							{
								if (gameObject.GetComponent<Handcuffs>().cuffTarget == componentInParent.gameObject)
								{
									this.CallCmdResetTarget(gameObject);
								}
							}
						}
					}
					else
					{
						this.progress = 0f;
					}
				}
				else
				{
					this.progress = 0f;
				}
			}
			else
			{
				this.progress = 0f;
			}
			if (this.ccm.curClass != 3)
			{
				this.uncuffProgress.fillAmount = Mathf.Clamp01(this.progress / 5f);
			}
		}
	}

	private void CuffPlayer()
	{
		Ray ray = new Ray(this.plyCam.position, this.plyCam.forward);
		RaycastHit raycastHit;
		if (Physics.Raycast(ray, out raycastHit, this.maxDistance, this.mask))
		{
			CharacterClassManager componentInParent = raycastHit.collider.GetComponentInParent<CharacterClassManager>();
			if (componentInParent != null)
			{
				Class @class = this.ccm.klasy[componentInParent.curClass];
				if (@class.team != Team.SCP && (@class.team == Team.CDP || @class.team == Team.CHI) != (this.ccm.klasy[this.ccm.curClass].team == Team.CDP || this.ccm.klasy[this.ccm.curClass].team == Team.CHI) && componentInParent.GetComponent<AnimationController>().curAnim == 0 && componentInParent.GetComponent<AnimationController>().speed == Vector2.zero)
				{
					this.CallCmdTarget(componentInParent.gameObject);
				}
			}
		}
	}

	[Command(channel = 2)]
	public void CmdTarget(GameObject t)
	{
		this.SetTarget(t);
		this.CallRpcDropItems(t);
	}

	[Command(channel = 2)]
	public void CmdResetTarget(GameObject t)
	{
		t.GetComponent<Handcuffs>().SetTarget(null);
	}

	[ClientRpc(channel = 2)]
	private void RpcDropItems(GameObject ply)
	{
		foreach (GameObject gameObject in PlayerManager.singleton.players)
		{
			if (ply == gameObject && gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
			{
				ply.GetComponent<Inventory>().DropAll();
				break;
			}
		}
	}

	private void SetTarget(GameObject t)
	{
		this.NetworkcuffTarget = t;
	}

	private void UpdateText()
	{
		if (this.cuffTarget != null)
		{
			float num = Vector3.Distance(base.transform.position, this.cuffTarget.transform.position);
			if (num > 200f)
			{
				num = 200f;
				this.lostCooldown += Time.deltaTime;
				if (this.lostCooldown > 1f)
				{
					this.CallCmdTarget(null);
				}
			}
			else
			{
				this.lostCooldown = 0f;
			}
			this.distanceText.text = (num * 1.5f).ToString("0 m");
		}
		else
		{
			this.distanceText.text = "NONE";
		}
	}

	private void UNetVersion()
	{
	}

	public GameObject NetworkcuffTarget
	{
		get
		{
			return this.cuffTarget;
		}
		set
		{
			uint dirtyBit = 1u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SetTarget(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVarGameObject(value, ref this.cuffTarget, dirtyBit, ref this.___cuffTargetNetId);
		}
	}

	protected static void InvokeCmdCmdTarget(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdTarget called on client.");
			return;
		}
		((Handcuffs)obj).CmdTarget(reader.ReadGameObject());
	}

	protected static void InvokeCmdCmdResetTarget(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdResetTarget called on client.");
			return;
		}
		((Handcuffs)obj).CmdResetTarget(reader.ReadGameObject());
	}

	public void CallCmdTarget(GameObject t)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdTarget called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdTarget(t);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)Handcuffs.kCmdCmdTarget);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(t);
		base.SendCommandInternal(networkWriter, 2, "CmdTarget");
	}

	public void CallCmdResetTarget(GameObject t)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdResetTarget called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdResetTarget(t);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)Handcuffs.kCmdCmdResetTarget);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(t);
		base.SendCommandInternal(networkWriter, 2, "CmdResetTarget");
	}

	protected static void InvokeRpcRpcDropItems(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcDropItems called on server.");
			return;
		}
		((Handcuffs)obj).RpcDropItems(reader.ReadGameObject());
	}

	public void CallRpcDropItems(GameObject ply)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("RPC Function RpcDropItems called on client.");
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)2));
		networkWriter.WritePackedUInt32((uint)Handcuffs.kRpcRpcDropItems);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(ply);
		this.SendRPCInternal(networkWriter, 2, "RpcDropItems");
	}

	static Handcuffs()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(Handcuffs), Handcuffs.kCmdCmdTarget, new NetworkBehaviour.CmdDelegate(Handcuffs.InvokeCmdCmdTarget));
		Handcuffs.kCmdCmdResetTarget = -1476369842;
		NetworkBehaviour.RegisterCommandDelegate(typeof(Handcuffs), Handcuffs.kCmdCmdResetTarget, new NetworkBehaviour.CmdDelegate(Handcuffs.InvokeCmdCmdResetTarget));
		Handcuffs.kRpcRpcDropItems = -710722295;
		NetworkBehaviour.RegisterRpcDelegate(typeof(Handcuffs), Handcuffs.kRpcRpcDropItems, new NetworkBehaviour.CmdDelegate(Handcuffs.InvokeRpcRpcDropItems));
		NetworkCRC.RegisterBehaviour("Handcuffs", 0);
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			writer.Write(this.cuffTarget);
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
			writer.Write(this.cuffTarget);
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
			this.___cuffTargetNetId = reader.ReadNetworkId();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			this.SetTarget(reader.ReadGameObject());
		}
	}

	public override void PreStartClient()
	{
		if (!this.___cuffTargetNetId.IsEmpty())
		{
			this.NetworkcuffTarget = ClientScene.FindLocalObject(this.___cuffTargetNetId);
		}
	}

	public TextMeshProUGUI distanceText;

	private Transform plyCam;

	private CharacterClassManager ccm;

	private Inventory inv;

	public LayerMask mask;

	public float maxDistance;

	private Image uncuffProgress;

	[SyncVar(hook = "SetTarget")]
	public GameObject cuffTarget;

	private float progress;

	private float lostCooldown;

	private NetworkInstanceId ___cuffTargetNetId;

	private static int kCmdCmdTarget = 624996931;

	private static int kCmdCmdResetTarget;

	private static int kRpcRpcDropItems;
}
