using System;
using UnityEngine;
using UnityEngine.Networking;

public class Medkit : NetworkBehaviour
{
	public Medkit()
	{
	}

	private void Start()
	{
		this.ccm = base.GetComponent<CharacterClassManager>();
		this.inv = base.GetComponent<Inventory>();
		this.ps = base.GetComponent<PlayerStats>();
	}

	private void Update()
	{
		if (this.time >= 0f)
		{
			this.time -= Time.deltaTime;
		}
		if (base.isLocalPlayer)
		{
			this.inventoryCooldown -= Time.deltaTime;
			if (Cursor.lockState != CursorLockMode.Locked)
			{
				this.inventoryCooldown = 0.2f;
			}
			if (this.inventoryCooldown <= 0f && Input.GetButtonDown("Fire1") && this.inv.curItem == 14 && this.time < 0f)
			{
				this.CallCmdUseMedkit();
				this.time = 1f;
				this.inv.SetCurItem(-1);
			}
		}
	}

	[Command(channel = 2)]
	private void CmdUseMedkit()
	{
		foreach (Inventory.SyncItemInfo item in this.inv.items)
		{
			if (item.id == 14)
			{
				Team team = this.ccm.klasy[this.ccm.curClass].team;
				if (team != Team.SCP && team != Team.RIP && this.time < 0f)
				{
					this.ps.Networkhealth = Mathf.Clamp(this.ps.health + UnityEngine.Random.Range(50, 85), 0, this.ccm.klasy[this.ccm.curClass].maxHP);
				}
				this.time = 1f;
				this.inv.items.Remove(item);
			}
		}
	}

	private void UNetVersion()
	{
	}

	protected static void InvokeCmdCmdUseMedkit(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdUseMedkit called on client.");
			return;
		}
		((Medkit)obj).CmdUseMedkit();
	}

	public void CallCmdUseMedkit()
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdUseMedkit called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdUseMedkit();
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)Medkit.kCmdCmdUseMedkit);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		base.SendCommandInternal(networkWriter, 2, "CmdUseMedkit");
	}

	static Medkit()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(Medkit), Medkit.kCmdCmdUseMedkit, new NetworkBehaviour.CmdDelegate(Medkit.InvokeCmdCmdUseMedkit));
		NetworkCRC.RegisterBehaviour("Medkit", 0);
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		bool result;
		return result;
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
	}

	private float inventoryCooldown;

	private Inventory inv;

	private PlayerStats ps;

	private CharacterClassManager ccm;

	private float time;

	private static int kCmdCmdUseMedkit = -2049042393;
}
