using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

public class GrenadeManager : NetworkBehaviour
{
	public GrenadeManager()
	{
	}

	private void Start()
	{
		this.inv = base.GetComponent<Inventory>();
		this.ps = base.GetComponent<PlayerStats>();
	}

	private void Update()
	{
		if (base.isLocalPlayer)
		{
			this.inventoryCooldown -= Time.deltaTime;
			if (Cursor.lockState != CursorLockMode.Locked)
			{
				this.inventoryCooldown = 0.2f;
			}
			if (this.inventoryCooldown <= 0f && Input.GetButtonDown("Fire1"))
			{
				for (int i = 0; i < this.grenades.Length; i++)
				{
					try
					{
						if (this.inv.curItem == this.grenades[i].inventoryID)
						{
							this.inv.items.Remove(this.inv.localInventoryItem);
							if (!base.GetComponent<MicroHID_GFX>().onFire)
							{
								base.StartCoroutine(this.Throw(i));
							}
						}
					}
					catch
					{
						MonoBehaviour.print("Zatrzymano: " + i);
					}
				}
			}
		}
	}

	[Command(channel = 2)]
	private void CmdThrowGrenade(int gId)
	{
		CharacterClassManager component = base.GetComponent<CharacterClassManager>();
		if (component.klasy[component.curClass].team != Team.SCP)
		{
			Vector3 spawnPos = this.plyCam.position + this.plyCam.forward * 0.2f + this.plyCam.right * 0.2f;
			Quaternion rotation = this.plyCam.rotation;
			this.plyCam.localRotation = Quaternion.Euler(Vector3.right * base.GetComponent<PlyMovementSync>().rotX);
			GrenadeManager.GrenadeSpawnInfo g = new GrenadeManager.GrenadeSpawnInfo(gId, spawnPos, this.plyCam.forward * this.throwSpeed, this.grenades[gId].timeToExplode, component.curClass);
			this.plyCam.rotation = rotation;
			this.cooldown = 1f;
			this.CallRpcThrowGrenade(g);
		}
	}

	[ClientRpc(channel = 2)]
	private void RpcThrowGrenade(GrenadeManager.GrenadeSpawnInfo g)
	{
		Rigidbody component = UnityEngine.Object.Instantiate<GameObject>(this.grenades[g.grenadeID].instance, g.spawnPosition, Quaternion.Euler(Vector3.up * 45f)).GetComponent<Rigidbody>();
		component.velocity = g.velocity;
		component.angularVelocity = Vector3.one * 45f;
		component.name = string.Concat(new object[]
		{
			"GRENADE#",
			g.grenadeID,
			"#",
			g.spawnPosition,
			"#",
			g.velocity,
			":"
		});
		UnityEngine.Object.Destroy(component.gameObject, g.timeToExplode + 2f);
		base.StartCoroutine(this.CountDown(component.name, g.timeToExplode, g.throwerclass));
	}

	private IEnumerator CountDown(string grenadeID, float time, int classId)
	{
		yield return new WaitForSeconds(time);
		GameObject.Find(grenadeID).GetComponent<GrenadeInstance>().Explode(classId);
		yield break;
	}

	private IEnumerator Throw(int i)
	{
		this.inv.localInventoryItem.firstpersonModel.GetComponent<Animator>().SetTrigger("Throw");
		base.GetComponent<MicroHID_GFX>().onFire = true;
		yield return new WaitForSeconds(this.grenades[i].throwAnimationTime);
		this.CallCmdThrowGrenade(i);
		this.inv.NetworkcurItem = -1;
		base.GetComponent<MicroHID_GFX>().onFire = false;
		yield break;
	}

	private void UNetVersion()
	{
	}

	protected static void InvokeCmdCmdThrowGrenade(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogError("Command CmdThrowGrenade called on client.");
			return;
		}
		((GrenadeManager)obj).CmdThrowGrenade((int)reader.ReadPackedUInt32());
	}

	public void CallCmdThrowGrenade(int gId)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("Command function CmdThrowGrenade called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdThrowGrenade(gId);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)GrenadeManager.kCmdCmdThrowGrenade);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.WritePackedUInt32((uint)gId);
		base.SendCommandInternal(networkWriter, 2, "CmdThrowGrenade");
	}

	protected static void InvokeRpcRpcThrowGrenade(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("RPC RpcThrowGrenade called on server.");
			return;
		}
		((GrenadeManager)obj).RpcThrowGrenade(GeneratedNetworkCode._ReadGrenadeSpawnInfo_GrenadeManager(reader));
	}

	public void CallRpcThrowGrenade(GrenadeManager.GrenadeSpawnInfo g)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogError("RPC Function RpcThrowGrenade called on client.");
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)2));
		networkWriter.WritePackedUInt32((uint)GrenadeManager.kRpcRpcThrowGrenade);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		GeneratedNetworkCode._WriteGrenadeSpawnInfo_GrenadeManager(networkWriter, g);
		this.SendRPCInternal(networkWriter, 2, "RpcThrowGrenade");
	}

	static GrenadeManager()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(GrenadeManager), GrenadeManager.kCmdCmdThrowGrenade, new NetworkBehaviour.CmdDelegate(GrenadeManager.InvokeCmdCmdThrowGrenade));
		GrenadeManager.kRpcRpcThrowGrenade = 807436509;
		NetworkBehaviour.RegisterRpcDelegate(typeof(GrenadeManager), GrenadeManager.kRpcRpcThrowGrenade, new NetworkBehaviour.CmdDelegate(GrenadeManager.InvokeRpcRpcThrowGrenade));
		NetworkCRC.RegisterBehaviour("GrenadeManager", 0);
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		bool result;
		return result;
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
	}

	public Transform plyCam;

	public float throwSpeed;

	private float inventoryCooldown;

	private Inventory inv;

	private PlayerStats ps;

	public GrenadeManager.Grenade[] grenades;

	private float cooldown;

	private static int kCmdCmdThrowGrenade = 724004359;

	private static int kRpcRpcThrowGrenade;

	[Serializable]
	public struct Grenade
	{
		public int inventoryID;

		public GameObject instance;

		public float timeToExplode;

		public float throwAnimationTime;
	}

	[Serializable]
	public struct GrenadeSpawnInfo
	{
		public GrenadeSpawnInfo(int id, Vector3 spawnPos, Vector3 vel, float t, int thrclass)
		{
			this.grenadeID = id;
			this.spawnPosition = spawnPos;
			this.velocity = vel;
			this.timeToExplode = t;
			this.throwerclass = thrclass;
		}

		public int grenadeID;

		public Vector3 spawnPosition;

		public Vector3 velocity;

		public float timeToExplode;

		public int throwerclass;
	}

	[CompilerGenerated]
	private sealed class <CountDown>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <CountDown>c__Iterator0()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this.$PC;
			this.$PC = -1;
			switch (num)
			{
			case 0u:
				this.$current = new WaitForSeconds(this.time);
				if (!this.$disposing)
				{
					this.$PC = 1;
				}
				return true;
			case 1u:
				GameObject.Find(this.grenadeID).GetComponent<GrenadeInstance>().Explode(this.classId);
				this.$PC = -1;
				break;
			}
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

		internal float time;

		internal string grenadeID;

		internal int classId;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}

	[CompilerGenerated]
	private sealed class <Throw>c__Iterator1 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <Throw>c__Iterator1()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this.$PC;
			this.$PC = -1;
			switch (num)
			{
			case 0u:
				this.$this.inv.localInventoryItem.firstpersonModel.GetComponent<Animator>().SetTrigger("Throw");
				this.$this.GetComponent<MicroHID_GFX>().onFire = true;
				this.$current = new WaitForSeconds(this.$this.grenades[this.i].throwAnimationTime);
				if (!this.$disposing)
				{
					this.$PC = 1;
				}
				return true;
			case 1u:
				this.$this.CallCmdThrowGrenade(this.i);
				this.$this.inv.NetworkcurItem = -1;
				this.$this.GetComponent<MicroHID_GFX>().onFire = false;
				this.$PC = -1;
				break;
			}
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

		internal int i;

		internal GrenadeManager $this;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}
}
