using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Dissonance.Integrations.UNet_HLAPI;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponManager : NetworkBehaviour
{
	public WeaponManager()
	{
	}

	private void SyncFF(bool ff)
	{
		this.NetworksyncFF = ff;
	}

	private void Start()
	{
		this.plyID = base.GetComponent<HlapiPlayer>();
		this.inv = base.GetComponent<Inventory>();
		this.fpc = base.GetComponent<FirstPersonController>();
		this.ammoBox = base.GetComponent<AmmoBox>();
		this.ccm = base.GetComponent<CharacterClassManager>();
		this.invDis = UnityEngine.Object.FindObjectOfType<InventoryDisplay>();
		if (base.isLocalPlayer)
		{
			if (base.isServer)
			{
				this.CallCmdSyncFF(ConfigFile.GetString("friendly_fire", "true").ToLower() == "true");
			}
			WeaponManager.holes.Clear();
		}
	}

	[Command(channel = 7)]
	private void CmdSyncFF(bool b)
	{
		this.SyncFF(b);
	}

	private void CalculateCurWeapon()
	{
		this.curWeapon = -1;
		if (this.inv.items.Count > 0 && this.inv.curItem > 0)
		{
			for (int i = 0; i < this.weapons.Length; i++)
			{
				if (this.weapons[i].itemID == this.inv.curItem)
				{
					this.curWeapon = i;
					this.curItem = this.inv.GetItemIndex();
				}
			}
		}
	}

	private void Shoot()
	{
		if (this.curWeapon != -1 && this.curItem != -1 && this.inv.items[this.curItem].durability > 0f)
		{
			this.CallCmdDoAnimation("Shoot");
			this.weapons[this.curWeapon].cooldown = 1f / this.weapons[this.curWeapon].fireRate;
			this.weapons[this.curWeapon].model.SetTrigger("Shoot");
			this.weapons[this.curWeapon].model.GetComponent<AudioSource>().PlayOneShot(this.weapons[this.curWeapon].shootAudio);
			foreach (ParticleSystem particleSystem in this.weapons[this.curWeapon].shotGFX)
			{
				particleSystem.Play();
			}
			this.SetCurRecoil(UnityEngine.Random.Range(1f, 1.5f) * this.weapons[this.curWeapon].recoilScale * this.classRecoil, this.weapons[this.curWeapon].recoilScale * this.classRecoil * (float)UnityEngine.Random.Range(-1, 1));
			Ray ray = new Ray(this.plyCam.transform.position, this.plyCam.transform.forward);
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit, 10000f, this.mask))
			{
				float num = this.weapons[this.curWeapon].damageOverDistance.Evaluate(raycastHit.distance);
				HitboxIdentity component = raycastHit.collider.GetComponent<HitboxIdentity>();
				if (component == null)
				{
					this.CallCmdShoot(string.Empty, null, raycastHit.point, raycastHit.normal, raycastHit.transform.name);
				}
				else
				{
					CharacterClassManager componentInParent = component.GetComponentInParent<CharacterClassManager>();
					this.CallCmdShoot(component.id, componentInParent.gameObject, raycastHit.point, raycastHit.normal, raycastHit.transform.name);
					if (this.GetShootPermission(componentInParent))
					{
						Hitmarker.Hit(this.weapons[this.curWeapon].hitmarkerResizer);
					}
				}
			}
		}
		else
		{
			if (this.weapons[this.curWeapon].ammoAudio != null && this.weapons[this.curWeapon].cooldown <= 0f)
			{
				this.weapons[this.curWeapon].cooldown = 1f / this.weapons[this.curWeapon].fireRate;
				this.weapons[this.curWeapon].model.GetComponent<AudioSource>().PlayOneShot(this.weapons[this.curWeapon].ammoAudio);
			}
			if (TutorialManager.status && this.ammoBox.GetAmmo(this.weapons[this.curWeapon].ammoType) == 0)
			{
				NoammoTrigger[] array = UnityEngine.Object.FindObjectsOfType<NoammoTrigger>();
				NoammoTrigger noammoTrigger = null;
				foreach (NoammoTrigger noammoTrigger2 in array)
				{
					if (noammoTrigger == null || noammoTrigger2.prioirty < noammoTrigger.prioirty)
					{
						noammoTrigger = noammoTrigger2;
					}
				}
				if (noammoTrigger != null)
				{
					noammoTrigger.Trigger(this.curWeapon);
				}
			}
		}
	}

	[Command(channel = 11)]
	private void CmdShoot(string hitboxid, GameObject target, Vector3 hit_point, Vector3 hit_normal, string itemname)
	{
		if (this.curWeapon < 0)
		{
			return;
		}
		if (this.servercd == 0f)
		{
			this.CallRpcSyncAudio(this.weapons[this.curWeapon].shootAudio_aac, false);
			this.CallRpcSyncAnim("Shoot");
			Scp096PlayerScript instance = Scp096PlayerScript.instance;
			this.servercd = 1f / this.weapons[this.curWeapon].fireRate * 0.9f;
			if (!string.IsNullOrEmpty(hitboxid))
			{
				if (this.GetShootPermission(target.GetComponent<CharacterClassManager>()))
				{
					float num = this.weapons[this.curWeapon].damageOverDistance.Evaluate(Vector3.Distance((!TutorialManager.status) ? base.GetComponent<PlyMovementSync>().position : base.transform.position, target.transform.position));
					if (hitboxid == "LEG")
					{
						num *= this.weapons[this.curWeapon].legDamageMultipiler;
					}
					if (hitboxid == "BODY")
					{
						num *= this.weapons[this.curWeapon].bodyDamageMultipiler;
					}
					if (hitboxid == "HEAD")
					{
						num *= this.weapons[this.curWeapon].headDamageMultipiler;
					}
					if (hitboxid == "SCP106")
					{
						num *= 0.1f;
					}
					base.GetComponent<PlayerStats>().HurtPlayer(new PlayerStats.HitInfo(num, "Gunman", "Weapon:" + this.curWeapon), target);
				}
			}
			else
			{
				this.CallRpcMakeHole(this.curWeapon, hit_point, Quaternion.FromToRotation(Vector3.up, hit_normal), itemname);
			}
			this.inv.items.ModifyDuration(this.curItem, this.inv.items[this.curItem].durability - 1f);
		}
		else
		{
			MonoBehaviour.print("Shot was too fast!");
		}
	}

	private IEnumerator Reload()
	{
		if (this.inv.items[this.curItem].durability < (float)this.weapons[this.curWeapon].magSize && this.ammoBox.GetAmmo(this.weapons[this.curWeapon].ammoType) > 0)
		{
			if (TutorialManager.status)
			{
				UnityEngine.Object.FindObjectOfType<TutorialManager>().Reload();
			}
			this.CallCmdDoAnimation("Reload");
			int startWeapon = this.curWeapon;
			this.weapons[this.curWeapon].cooldown = this.weapons[this.curWeapon].reloadingTime + 0.3f;
			this.weapons[this.curWeapon].model.SetBool("Reloading", true);
			this.weapons[this.curWeapon].model.GetComponent<AudioSource>().PlayOneShot(this.weapons[this.curWeapon].reloadAudio);
			this.CallCmdDoAudio(this.weapons[this.curWeapon].reloadAudio_aac, true);
			yield return new WaitForSeconds(this.weapons[this.curWeapon].reloadingTime);
			this.weapons[startWeapon].model.SetBool("Reloading", false);
			if (startWeapon == this.curWeapon)
			{
				this.CallCmdReload(this.weapons[this.curWeapon].ammoType);
			}
		}
		yield break;
	}

	[Command(channel = 2)]
	private void CmdReload(int type)
	{
		string text = string.Empty;
		string[] array = this.ammoBox.amount.Split(new char[]
		{
			':'
		});
		int num = this.ammoBox.GetAmmo(type);
		while (this.inv.items[this.curItem].durability < (float)this.weapons[this.curWeapon].magSize && num > 0)
		{
			num--;
			this.inv.items.ModifyDuration(this.curItem, this.inv.items[this.curItem].durability + 1f);
		}
		array[type] = num.ToString();
		for (int i = 0; i < 3; i++)
		{
			text += array[i];
			if (i != 2)
			{
				text += ":";
			}
		}
		this.ammoBox.Networkamount = text;
	}

	private void FixedUpdate()
	{
		this.CalculateCurWeapon();
		if (base.isLocalPlayer)
		{
			if (this.invDis.rootObject.activeSelf || CursorManager.consoleOpen || CursorManager.pauseOpen || CursorManager.scp106 || !Application.isFocused)
			{
				this.inventoryCooldown = 0.4f;
			}
			if (this.inventoryCooldown > 0f)
			{
				this.inventoryCooldown -= 0.02f;
			}
			this.CalculateZoomStatus();
			this.CalculateMouseSensitivity();
			this.FixUpdate();
			if (this.curWeapon >= 0 && this.allowShoot)
			{
				if (this.prevWeapon != this.curWeapon)
				{
					this.prevWeapon = this.curWeapon;
				}
				bool @bool = this.weapons[this.curWeapon].model.GetBool("Zoomed");
				if (this.weapons[this.curWeapon].cooldown < 0f)
				{
					this.weapons[this.curWeapon].model.SetBool("Zoomed", Input.GetButton("Zoom"));
				}
				this.plyCam.fieldOfView = ((!@bool) ? this.normalFov : (this.normalFov + this.weapons[this.curWeapon].zoomFOV));
				if (this.weapons[this.curWeapon].cooldown < 0f)
				{
					if (Input.GetButton("Reload") && !@bool)
					{
						base.StartCoroutine(this.Reload());
					}
					if (@bool != Input.GetButton("Zoom"))
					{
						this.weapons[this.curWeapon].cooldown = 0.3f;
					}
				}
				else
				{
					this.weapons[this.curWeapon].cooldown -= Time.deltaTime;
				}
			}
			else
			{
				this.prevWeapon = this.curWeapon;
				this.plyCam.fieldOfView = this.normalFov;
			}
			this.allowShoot = !this.fpc.lockMovement;
		}
	}

	private void Update()
	{
		if (base.name == "Host")
		{
			WeaponManager.friendlyFire = this.syncFF;
		}
		if (base.isLocalPlayer && this.curWeapon >= 0 && ((!this.weapons[this.curWeapon].useFullauto) ? Input.GetButtonDown("Fire1") : Input.GetButton("Fire1")) && this.allowShoot && this.weapons[this.curWeapon].cooldown < 0f && this.inventoryCooldown <= 0f)
		{
			this.Shoot();
		}
		this.servercd = Mathf.Clamp01(this.servercd - Time.deltaTime);
	}

	public bool GetShootPermission(Team target)
	{
		if (this.ccm.curClass == 2 || this.ccm.klasy[this.ccm.curClass].team == Team.SCP)
		{
			return false;
		}
		if (WeaponManager.friendlyFire)
		{
			return true;
		}
		Team team = this.ccm.klasy[this.ccm.curClass].team;
		return ((team != Team.MTF && team != Team.RSC) || (target != Team.MTF && target != Team.RSC)) && ((team != Team.CDP && team != Team.CHI) || (target != Team.CDP && target != Team.CHI));
	}

	public bool GetShootPermission(CharacterClassManager c)
	{
		return this.GetShootPermission(c.klasy[c.curClass].team);
	}

	private void CalculateZoomStatus()
	{
		if (this.curWeapon >= 0)
		{
			this.timeToZoom += Time.deltaTime * (float)((!this.weapons[this.curWeapon].model.GetBool("Zoomed")) ? -1 : 1);
		}
		else
		{
			this.timeToZoom = 0f;
		}
		this.timeToZoom = Mathf.Clamp(this.timeToZoom, 0f, 0.3f);
		if (this.timeToZoom == 0.3f && this.curWeapon >= 0)
		{
			this.unfocusedWeaponCamera.SetActive(false);
			this.weapons[this.curWeapon].weaponCamera.SetActive(true);
		}
		else if (!this.unfocusedWeaponCamera.activeSelf)
		{
			this.DisableAllWeaponCameras();
		}
	}

	private void CalculateMouseSensitivity()
	{
		float sensitivityMultiplier = 1f;
		float num = 1f;
		if (this.curWeapon >= 0 && this.weapons[this.curWeapon].model.GetBool("Zoomed"))
		{
			sensitivityMultiplier = this.weapons[this.curWeapon].zoomMouseSensitivityMultiplier;
			num = this.weapons[this.curWeapon].zoomSlowdown;
		}
		this.fpc.m_MouseLook.sensitivityMultiplier = sensitivityMultiplier;
		if (Input.GetButton("Sneak") && num > this.sneakSpeed && this.ccm.curClass >= 0 && this.ccm.klasy[this.ccm.curClass].team != Team.SCP)
		{
			num = this.sneakSpeed;
		}
		this.fpc.zoomSlowdown = num;
	}

	public void DisableAllWeaponCameras()
	{
		foreach (WeaponManager.Weapon weapon in this.weapons)
		{
			weapon.weaponCamera.SetActive(false);
		}
		this.unfocusedWeaponCamera.SetActive(true);
	}

	public void SetRecoil(float f)
	{
		this.classRecoil = f;
	}

	[ClientRpc(channel = 10)]
	private void RpcMakeHole(int weapon, Vector3 point, Quaternion quat, string nam)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.weapons[weapon].holes[UnityEngine.Random.Range(0, this.weapons[weapon].holes.Length)], point, quat);
		gameObject.GetComponentInChildren<MeshRenderer>().gameObject.transform.localRotation = Quaternion.Euler(Vector3.up * (float)UnityEngine.Random.Range(0, 360));
		if (nam.ToLower().Contains("door") || nam.ToLower().Contains("speaker") || base.GetComponentInParent<Door>() != null)
		{
			UnityEngine.Object.Destroy(gameObject, 1.4f);
		}
		else
		{
			WeaponManager.holes.Add(gameObject);
			if (WeaponManager.holes.Count > 80)
			{
				GameObject obj = WeaponManager.holes[0];
				WeaponManager.holes.RemoveAt(0);
				UnityEngine.Object.Destroy(obj);
			}
		}
	}

	[Command(channel = 1)]
	private void CmdDoAnimation(string triggername)
	{
		this.CallRpcSyncAnim(triggername);
	}

	[ClientRpc(channel = 1)]
	private void RpcSyncAnim(string triggername)
	{
		if (!base.isLocalPlayer)
		{
			base.GetComponent<AnimationController>().DoAnimation(triggername);
		}
	}

	[Command(channel = 10)]
	private void CmdDoAudio(string triggername, bool isReloading)
	{
		this.CallRpcSyncAudio(triggername, isReloading);
	}

	[ClientRpc(channel = 10)]
	private void RpcSyncAudio(string triggername, bool isReloading)
	{
		if (!base.isLocalPlayer)
		{
			base.GetComponent<AnimationController>().PlaySound(triggername, !isReloading);
		}
	}

	public void SetCurRecoil(float x, float y)
	{
		float num = (!this.weapons[this.curWeapon].model.GetBool("Zoomed")) ? 1f : this.weapons[this.curWeapon].zoomRecoilCompression;
		x *= num * this.overallRecoilFactor;
		y *= num * this.overallRecoilFactor;
		this.rX = x;
		this.rY = y;
	}

	public void FixUpdate()
	{
		this.rX -= Time.deltaTime * 5f;
		this.rY -= Time.deltaTime * 5f;
		if (this.rX < 0f)
		{
			this.rX = 0f;
		}
		if (this.rY < 0f)
		{
			this.rY = 0f;
		}
		if (this.rY > 0f || this.rX > 0f)
		{
			base.GetComponent<FirstPersonController>().m_MouseLook.LookRotation(base.transform, this.plyCam.transform, 0f, this.rY, this.rX, true);
		}
	}

	static WeaponManager()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(WeaponManager), WeaponManager.kCmdCmdSyncFF, new NetworkBehaviour.CmdDelegate(WeaponManager.InvokeCmdCmdSyncFF));
		WeaponManager.kCmdCmdShoot = -1101833074;
		NetworkBehaviour.RegisterCommandDelegate(typeof(WeaponManager), WeaponManager.kCmdCmdShoot, new NetworkBehaviour.CmdDelegate(WeaponManager.InvokeCmdCmdShoot));
		WeaponManager.kCmdCmdReload = 171423498;
		NetworkBehaviour.RegisterCommandDelegate(typeof(WeaponManager), WeaponManager.kCmdCmdReload, new NetworkBehaviour.CmdDelegate(WeaponManager.InvokeCmdCmdReload));
		WeaponManager.kCmdCmdDoAnimation = -349526936;
		NetworkBehaviour.RegisterCommandDelegate(typeof(WeaponManager), WeaponManager.kCmdCmdDoAnimation, new NetworkBehaviour.CmdDelegate(WeaponManager.InvokeCmdCmdDoAnimation));
		WeaponManager.kCmdCmdDoAudio = 1725773498;
		NetworkBehaviour.RegisterCommandDelegate(typeof(WeaponManager), WeaponManager.kCmdCmdDoAudio, new NetworkBehaviour.CmdDelegate(WeaponManager.InvokeCmdCmdDoAudio));
		WeaponManager.kRpcRpcMakeHole = 2060079829;
		NetworkBehaviour.RegisterRpcDelegate(typeof(WeaponManager), WeaponManager.kRpcRpcMakeHole, new NetworkBehaviour.CmdDelegate(WeaponManager.InvokeRpcRpcMakeHole));
		WeaponManager.kRpcRpcSyncAnim = -458877357;
		NetworkBehaviour.RegisterRpcDelegate(typeof(WeaponManager), WeaponManager.kRpcRpcSyncAnim, new NetworkBehaviour.CmdDelegate(WeaponManager.InvokeRpcRpcSyncAnim));
		WeaponManager.kRpcRpcSyncAudio = -1340092460;
		NetworkBehaviour.RegisterRpcDelegate(typeof(WeaponManager), WeaponManager.kRpcRpcSyncAudio, new NetworkBehaviour.CmdDelegate(WeaponManager.InvokeRpcRpcSyncAudio));
		NetworkCRC.RegisterBehaviour("WeaponManager", 0);
	}

	private void UNetVersion()
	{
	}

	public bool NetworksyncFF
	{
		get
		{
			return this.syncFF;
		}
		set
		{
			uint dirtyBit = 1u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SyncFF(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<bool>(value, ref this.syncFF, dirtyBit);
		}
	}

	protected static void InvokeCmdCmdSyncFF(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogError("Command CmdSyncFF called on client.");
			return;
		}
		((WeaponManager)obj).CmdSyncFF(reader.ReadBoolean());
	}

	protected static void InvokeCmdCmdShoot(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogError("Command CmdShoot called on client.");
			return;
		}
		((WeaponManager)obj).CmdShoot(reader.ReadString(), reader.ReadGameObject(), reader.ReadVector3(), reader.ReadVector3(), reader.ReadString());
	}

	protected static void InvokeCmdCmdReload(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogError("Command CmdReload called on client.");
			return;
		}
		((WeaponManager)obj).CmdReload((int)reader.ReadPackedUInt32());
	}

	protected static void InvokeCmdCmdDoAnimation(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogError("Command CmdDoAnimation called on client.");
			return;
		}
		((WeaponManager)obj).CmdDoAnimation(reader.ReadString());
	}

	protected static void InvokeCmdCmdDoAudio(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogError("Command CmdDoAudio called on client.");
			return;
		}
		((WeaponManager)obj).CmdDoAudio(reader.ReadString(), reader.ReadBoolean());
	}

	public void CallCmdSyncFF(bool b)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("Command function CmdSyncFF called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdSyncFF(b);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)WeaponManager.kCmdCmdSyncFF);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(b);
		base.SendCommandInternal(networkWriter, 7, "CmdSyncFF");
	}

	public void CallCmdShoot(string hitboxid, GameObject target, Vector3 hit_point, Vector3 hit_normal, string itemname)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("Command function CmdShoot called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdShoot(hitboxid, target, hit_point, hit_normal, itemname);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)WeaponManager.kCmdCmdShoot);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(hitboxid);
		networkWriter.Write(target);
		networkWriter.Write(hit_point);
		networkWriter.Write(hit_normal);
		networkWriter.Write(itemname);
		base.SendCommandInternal(networkWriter, 11, "CmdShoot");
	}

	public void CallCmdReload(int type)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("Command function CmdReload called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdReload(type);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)WeaponManager.kCmdCmdReload);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.WritePackedUInt32((uint)type);
		base.SendCommandInternal(networkWriter, 2, "CmdReload");
	}

	public void CallCmdDoAnimation(string triggername)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("Command function CmdDoAnimation called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdDoAnimation(triggername);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)WeaponManager.kCmdCmdDoAnimation);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(triggername);
		base.SendCommandInternal(networkWriter, 1, "CmdDoAnimation");
	}

	public void CallCmdDoAudio(string triggername, bool isReloading)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("Command function CmdDoAudio called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdDoAudio(triggername, isReloading);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)WeaponManager.kCmdCmdDoAudio);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(triggername);
		networkWriter.Write(isReloading);
		base.SendCommandInternal(networkWriter, 10, "CmdDoAudio");
	}

	protected static void InvokeRpcRpcMakeHole(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("RPC RpcMakeHole called on server.");
			return;
		}
		((WeaponManager)obj).RpcMakeHole((int)reader.ReadPackedUInt32(), reader.ReadVector3(), reader.ReadQuaternion(), reader.ReadString());
	}

	protected static void InvokeRpcRpcSyncAnim(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("RPC RpcSyncAnim called on server.");
			return;
		}
		((WeaponManager)obj).RpcSyncAnim(reader.ReadString());
	}

	protected static void InvokeRpcRpcSyncAudio(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("RPC RpcSyncAudio called on server.");
			return;
		}
		((WeaponManager)obj).RpcSyncAudio(reader.ReadString(), reader.ReadBoolean());
	}

	public void CallRpcMakeHole(int weapon, Vector3 point, Quaternion quat, string nam)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogError("RPC Function RpcMakeHole called on client.");
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)2));
		networkWriter.WritePackedUInt32((uint)WeaponManager.kRpcRpcMakeHole);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.WritePackedUInt32((uint)weapon);
		networkWriter.Write(point);
		networkWriter.Write(quat);
		networkWriter.Write(nam);
		this.SendRPCInternal(networkWriter, 10, "RpcMakeHole");
	}

	public void CallRpcSyncAnim(string triggername)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogError("RPC Function RpcSyncAnim called on client.");
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)2));
		networkWriter.WritePackedUInt32((uint)WeaponManager.kRpcRpcSyncAnim);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(triggername);
		this.SendRPCInternal(networkWriter, 1, "RpcSyncAnim");
	}

	public void CallRpcSyncAudio(string triggername, bool isReloading)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogError("RPC Function RpcSyncAudio called on client.");
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)2));
		networkWriter.WritePackedUInt32((uint)WeaponManager.kRpcRpcSyncAudio);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(triggername);
		networkWriter.Write(isReloading);
		this.SendRPCInternal(networkWriter, 10, "RpcSyncAudio");
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			writer.Write(this.syncFF);
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
			writer.Write(this.syncFF);
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
			this.syncFF = reader.ReadBoolean();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			this.SyncFF(reader.ReadBoolean());
		}
	}

	public Camera plyCam;

	public GameObject unfocusedWeaponCamera;

	public GameObject recoilCam;

	public float normalFov;

	public LayerMask mask;

	public string[] holeTags;

	public float sneakSpeed;

	public static bool friendlyFire;

	[SyncVar(hook = "SyncFF")]
	private bool syncFF;

	private Inventory inv;

	private FirstPersonController fpc;

	private AmmoBox ammoBox;

	private InventoryDisplay invDis;

	private CharacterClassManager ccm;

	public int curWeapon;

	private int curItem;

	private float timeToZoom;

	private float classRecoil = 1f;

	public WeaponManager.Weapon[] weapons;

	public static List<GameObject> holes = new List<GameObject>();

	public float overallRecoilFactor = 1.5f;

	private HlapiPlayer plyID;

	public float inventoryCooldown;

	private bool allowShoot;

	private float servercd;

	private int prevWeapon;

	private float rX;

	private float rY;

	private static int kCmdCmdSyncFF = 218570252;

	private static int kCmdCmdShoot;

	private static int kCmdCmdReload;

	private static int kRpcRpcMakeHole;

	private static int kCmdCmdDoAnimation;

	private static int kRpcRpcSyncAnim;

	private static int kCmdCmdDoAudio;

	private static int kRpcRpcSyncAudio;

	[Serializable]
	public class Weapon
	{
		public Weapon()
		{
		}

		public int itemID;

		public Animator model;

		public int magSize = 15;

		public AnimationCurve damageOverDistance;

		public float bodyDamageMultipiler = 1f;

		public float headDamageMultipiler = 2f;

		public float legDamageMultipiler = 0.5f;

		public bool useFullauto;

		public float fireRate = 3f;

		public AudioClip shootAudio;

		public AudioClip reloadAudio;

		public AudioClip ammoAudio;

		public string shootAudio_aac;

		public string reloadAudio_aac;

		public float cooldown;

		public float reloadingTime = 5f;

		public ParticleSystem[] shotGFX;

		public float zoomFOV;

		public float recoilScale;

		public float zoomRecoilCompression;

		public float zoomMouseSensitivityMultiplier;

		public float zoomSlowdown;

		public GameObject[] holes;

		public GameObject weaponCamera;

		public int ammoType;

		public float hitmarkerResizer = 1f;

		[HideInInspector]
		public int arrayID;
	}

	[CompilerGenerated]
	private sealed class <Reload>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <Reload>c__Iterator0()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this.$PC;
			this.$PC = -1;
			switch (num)
			{
			case 0u:
				if (this.$this.inv.items[this.$this.curItem].durability < (float)this.$this.weapons[this.$this.curWeapon].magSize && this.$this.ammoBox.GetAmmo(this.$this.weapons[this.$this.curWeapon].ammoType) > 0)
				{
					if (TutorialManager.status)
					{
						UnityEngine.Object.FindObjectOfType<TutorialManager>().Reload();
					}
					this.$this.CallCmdDoAnimation("Reload");
					this.<startWeapon>__1 = this.$this.curWeapon;
					this.$this.weapons[this.$this.curWeapon].cooldown = this.$this.weapons[this.$this.curWeapon].reloadingTime + 0.3f;
					this.$this.weapons[this.$this.curWeapon].model.SetBool("Reloading", true);
					this.$this.weapons[this.$this.curWeapon].model.GetComponent<AudioSource>().PlayOneShot(this.$this.weapons[this.$this.curWeapon].reloadAudio);
					this.$this.CallCmdDoAudio(this.$this.weapons[this.$this.curWeapon].reloadAudio_aac, true);
					this.$current = new WaitForSeconds(this.$this.weapons[this.$this.curWeapon].reloadingTime);
					if (!this.$disposing)
					{
						this.$PC = 1;
					}
					return true;
				}
				break;
			case 1u:
				this.$this.weapons[this.<startWeapon>__1].model.SetBool("Reloading", false);
				if (this.<startWeapon>__1 == this.$this.curWeapon)
				{
					this.$this.CallCmdReload(this.$this.weapons[this.$this.curWeapon].ammoType);
				}
				break;
			default:
				return false;
			}
			this.$PC = -1;
			return false;
		}

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.$current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.$current;
			}
		}

		[DebuggerHidden]
		public void Dispose()
		{
			this.$disposing = true;
			this.$PC = -1;
		}

		[DebuggerHidden]
		public void Reset()
		{
			throw new NotSupportedException();
		}

		internal int <startWeapon>__1;

		internal WeaponManager $this;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}
}
