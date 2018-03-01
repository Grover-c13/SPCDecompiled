using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Dissonance.Integrations.UNet_HLAPI;
using UnityEngine;
using UnityEngine.Networking;

public class MicroHID_GFX : NetworkBehaviour
{
	public MicroHID_GFX()
	{
	}

	private void Start()
	{
		this.pmng = PlayerManager.singleton;
		this.invdis = UnityEngine.Object.FindObjectOfType<InventoryDisplay>();
		this.plyid = base.GetComponent<HlapiPlayer>();
	}

	private void Update()
	{
		if (base.isLocalPlayer && Input.GetButtonDown("Fire1") && base.GetComponent<Inventory>().curItem == 16 && !this.onFire && base.GetComponent<WeaponManager>().inventoryCooldown <= 0f && base.GetComponent<Inventory>().localInventoryItem.durability > 0f)
		{
			this.onFire = true;
			base.GetComponent<Inventory>().localInventoryItem.durability = 0f;
			this.CallCmdDoAnimation("Shoot");
			base.StartCoroutine(this.PlayAnimation());
		}
	}

	private IEnumerator PlayAnimation()
	{
		this.damageGiven = 0f;
		this.anim.SetTrigger("Shoot");
		this.shotSource.Play();
		foreach (Light light in this.progress)
		{
			light.intensity = 0f;
		}
		this.GlowLight(0);
		yield return new WaitForSeconds(2.2f);
		this.GlowLight(1);
		yield return new WaitForSeconds(2.2f);
		this.GlowLight(2);
		yield return new WaitForSeconds(2.2f);
		this.GlowLight(3);
		this.GlowLight(5);
		yield return new WaitForSeconds(2.2f);
		this.GlowLight(4);
		yield return new WaitForSeconds(0.6f);
		this.teslaFX.Play();
		for (int i = 0; i < 20; i++)
		{
			GameObject[] players = this.pmng.players;
			foreach (GameObject gameObject in players)
			{
				RaycastHit raycastHit;
				if (Vector3.Dot(this.cam.transform.forward, (this.cam.transform.position - gameObject.transform.position).normalized) < -0.92f && Physics.Raycast(this.cam.transform.position, (gameObject.transform.position - this.cam.transform.position).normalized, out raycastHit, this.range) && raycastHit.transform.name == gameObject.name)
				{
					Hitmarker.Hit(2.3f);
					this.CallCmdHurtPlayersInRange(gameObject);
				}
			}
			yield return new WaitForSeconds(0.25f);
		}
		this.onFire = false;
		foreach (Light light2 in this.progress)
		{
			light2.intensity = 0f;
		}
		yield break;
	}

	[Command(channel = 11)]
	private void CmdHurtPlayersInRange(GameObject ply)
	{
		if (base.GetComponent<AnimationController>().item == 16 && Vector3.Distance(base.GetComponent<PlyMovementSync>().position, ply.transform.position) < this.range && base.GetComponent<WeaponManager>().GetShootPermission(ply.GetComponent<CharacterClassManager>()))
		{
			base.GetComponent<PlayerStats>().HurtPlayer(new PlayerStats.HitInfo((float)UnityEngine.Random.Range(200, 300), string.Empty, "TESLA"), ply);
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
			base.GetComponent<AnimationController>().PlaySound("HID_Shoot", true);
			base.GetComponent<AnimationController>().DoAnimation(triggername);
		}
	}

	private void GlowLight(int id)
	{
		base.StartCoroutine(this.SetLightState((id != 5) ? ((id != 4) ? 3f : 6f) : 50f, this.progress[id], (id != 5) ? 2f : 50f));
	}

	private IEnumerator SetLightState(float targetIntensity, Light light, float speed)
	{
		while (light.intensity < targetIntensity)
		{
			light.intensity += Time.deltaTime * speed;
			yield return new WaitForEndOfFrame();
		}
		yield break;
	}

	private void UNetVersion()
	{
	}

	protected static void InvokeCmdCmdHurtPlayersInRange(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogError("Command CmdHurtPlayersInRange called on client.");
			return;
		}
		((MicroHID_GFX)obj).CmdHurtPlayersInRange(reader.ReadGameObject());
	}

	protected static void InvokeCmdCmdDoAnimation(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogError("Command CmdDoAnimation called on client.");
			return;
		}
		((MicroHID_GFX)obj).CmdDoAnimation(reader.ReadString());
	}

	public void CallCmdHurtPlayersInRange(GameObject ply)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("Command function CmdHurtPlayersInRange called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdHurtPlayersInRange(ply);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)MicroHID_GFX.kCmdCmdHurtPlayersInRange);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(ply);
		base.SendCommandInternal(networkWriter, 11, "CmdHurtPlayersInRange");
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
		networkWriter.WritePackedUInt32((uint)MicroHID_GFX.kCmdCmdDoAnimation);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(triggername);
		base.SendCommandInternal(networkWriter, 1, "CmdDoAnimation");
	}

	protected static void InvokeRpcRpcSyncAnim(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("RPC RpcSyncAnim called on server.");
			return;
		}
		((MicroHID_GFX)obj).RpcSyncAnim(reader.ReadString());
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
		networkWriter.WritePackedUInt32((uint)MicroHID_GFX.kRpcRpcSyncAnim);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(triggername);
		this.SendRPCInternal(networkWriter, 1, "RpcSyncAnim");
	}

	static MicroHID_GFX()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(MicroHID_GFX), MicroHID_GFX.kCmdCmdHurtPlayersInRange, new NetworkBehaviour.CmdDelegate(MicroHID_GFX.InvokeCmdCmdHurtPlayersInRange));
		MicroHID_GFX.kCmdCmdDoAnimation = 1828045792;
		NetworkBehaviour.RegisterCommandDelegate(typeof(MicroHID_GFX), MicroHID_GFX.kCmdCmdDoAnimation, new NetworkBehaviour.CmdDelegate(MicroHID_GFX.InvokeCmdCmdDoAnimation));
		MicroHID_GFX.kRpcRpcSyncAnim = -572266021;
		NetworkBehaviour.RegisterRpcDelegate(typeof(MicroHID_GFX), MicroHID_GFX.kRpcRpcSyncAnim, new NetworkBehaviour.CmdDelegate(MicroHID_GFX.InvokeRpcRpcSyncAnim));
		NetworkCRC.RegisterBehaviour("MicroHID_GFX", 0);
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		bool result;
		return result;
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
	}

	public Light[] progress;

	public ParticleSystem teslaFX;

	public Animator anim;

	public AudioSource shotSource;

	public bool onFire;

	public float range;

	public GameObject cam;

	private PlayerManager pmng;

	private HlapiPlayer plyid;

	private InventoryDisplay invdis;

	private float damageGiven;

	private static int kCmdCmdHurtPlayersInRange = 1650017390;

	private static int kCmdCmdDoAnimation;

	private static int kRpcRpcSyncAnim;

	[CompilerGenerated]
	private sealed class <PlayAnimation>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <PlayAnimation>c__Iterator0()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this.$PC;
			this.$PC = -1;
			switch (num)
			{
			case 0u:
				this.$this.damageGiven = 0f;
				this.$this.anim.SetTrigger("Shoot");
				this.$this.shotSource.Play();
				this.$locvar0 = this.$this.progress;
				this.$locvar1 = 0;
				while (this.$locvar1 < this.$locvar0.Length)
				{
					Light light = this.$locvar0[this.$locvar1];
					light.intensity = 0f;
					this.$locvar1++;
				}
				this.$this.GlowLight(0);
				this.$current = new WaitForSeconds(2.2f);
				if (!this.$disposing)
				{
					this.$PC = 1;
				}
				return true;
			case 1u:
				this.$this.GlowLight(1);
				this.$current = new WaitForSeconds(2.2f);
				if (!this.$disposing)
				{
					this.$PC = 2;
				}
				return true;
			case 2u:
				this.$this.GlowLight(2);
				this.$current = new WaitForSeconds(2.2f);
				if (!this.$disposing)
				{
					this.$PC = 3;
				}
				return true;
			case 3u:
				this.$this.GlowLight(3);
				this.$this.GlowLight(5);
				this.$current = new WaitForSeconds(2.2f);
				if (!this.$disposing)
				{
					this.$PC = 4;
				}
				return true;
			case 4u:
				this.$this.GlowLight(4);
				this.$current = new WaitForSeconds(0.6f);
				if (!this.$disposing)
				{
					this.$PC = 5;
				}
				return true;
			case 5u:
				this.$this.teslaFX.Play();
				this.<i>__1 = 0;
				break;
			case 6u:
				this.<i>__1++;
				break;
			default:
				return false;
			}
			if (this.<i>__1 < 20)
			{
				this.<players>__2 = this.$this.pmng.players;
				this.$locvar2 = this.<players>__2;
				this.$locvar3 = 0;
				while (this.$locvar3 < this.$locvar2.Length)
				{
					GameObject gameObject = this.$locvar2[this.$locvar3];
					RaycastHit raycastHit;
					if (Vector3.Dot(this.$this.cam.transform.forward, (this.$this.cam.transform.position - gameObject.transform.position).normalized) < -0.92f && Physics.Raycast(this.$this.cam.transform.position, (gameObject.transform.position - this.$this.cam.transform.position).normalized, out raycastHit, this.$this.range) && raycastHit.transform.name == gameObject.name)
					{
						Hitmarker.Hit(2.3f);
						this.$this.CallCmdHurtPlayersInRange(gameObject);
					}
					this.$locvar3++;
				}
				this.$current = new WaitForSeconds(0.25f);
				if (!this.$disposing)
				{
					this.$PC = 6;
				}
				return true;
			}
			this.$this.onFire = false;
			this.$locvar4 = this.$this.progress;
			this.$locvar5 = 0;
			while (this.$locvar5 < this.$locvar4.Length)
			{
				Light light2 = this.$locvar4[this.$locvar5];
				light2.intensity = 0f;
				this.$locvar5++;
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

		internal Light[] $locvar0;

		internal int $locvar1;

		internal int <i>__1;

		internal GameObject[] <players>__2;

		internal GameObject[] $locvar2;

		internal int $locvar3;

		internal Light[] $locvar4;

		internal int $locvar5;

		internal MicroHID_GFX $this;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}

	[CompilerGenerated]
	private sealed class <SetLightState>c__Iterator1 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <SetLightState>c__Iterator1()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this.$PC;
			this.$PC = -1;
			switch (num)
			{
			case 0u:
				break;
			case 1u:
				break;
			default:
				return false;
			}
			if (this.light.intensity < this.targetIntensity)
			{
				this.light.intensity += Time.deltaTime * this.speed;
				this.$current = new WaitForEndOfFrame();
				if (!this.$disposing)
				{
					this.$PC = 1;
				}
				return true;
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

		internal Light light;

		internal float targetIntensity;

		internal float speed;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}
}
