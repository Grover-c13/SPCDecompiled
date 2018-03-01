using System;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class AmmoBox : NetworkBehaviour
{
	public AmmoBox()
	{
	}

	public void SetAmount(string am)
	{
		this.Networkamount = am;
	}

	private void Start()
	{
		this.inv = base.GetComponent<Inventory>();
		this.ccm = base.GetComponent<CharacterClassManager>();
	}

	public void SetAmmoAmount()
	{
		int[] ammoTypes = this.ccm.klasy[this.ccm.curClass].ammoTypes;
		this.Networkamount = string.Concat(new object[]
		{
			ammoTypes[0],
			":",
			ammoTypes[1],
			":",
			ammoTypes[2]
		});
	}

	public int GetAmmo(int type)
	{
		int result = 0;
		if (this.amount.Contains(":") && !int.TryParse(this.amount.Split(new char[]
		{
			':'
		})[Mathf.Clamp(type, 0, 2)], out result))
		{
			MonoBehaviour.print("Parse failed");
		}
		return result;
	}

	[Command(channel = 2)]
	private void CmdDrop(int a, int id)
	{
		if (id >= 0 && id < 3 && a >= 15 && a <= this.GetAmmo(id))
		{
			string[] array = this.amount.Split(new char[]
			{
				':'
			});
			array[id] = (this.GetAmmo(id) - a).ToString();
			this.inv.SetPickup(this.types[id].inventoryID, (float)a, base.transform.position, this.inv.kamera.transform.rotation, base.transform.rotation);
			this.Networkamount = string.Concat(new string[]
			{
				array[0],
				":",
				array[1],
				":",
				array[2]
			});
		}
	}

	private void Update()
	{
		this.UpdateText();
		if (this.inv.curItem == 19 && base.GetComponent<WeaponManager>().inventoryCooldown <= 0f)
		{
			if (Input.GetButtonDown("Zoom"))
			{
				this.chosenID++;
				if (this.chosenID >= this.types.Length)
				{
					this.chosenID = 0;
				}
			}
			if (Input.GetButtonDown("Fire1") && this.GetAmmo(this.chosenID) >= this.amountToDrop && this.amountToDrop != 0)
			{
				this.CallCmdDrop(this.amountToDrop, this.chosenID);
			}
			if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
			{
				this.amountToDrop--;
			}
			if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
			{
				this.amountToDrop++;
			}
			int ammo = this.GetAmmo(this.chosenID);
			if (ammo >= 15)
			{
				this.amountToDrop = Mathf.Clamp(this.amountToDrop, 15, ammo);
			}
			else
			{
				this.amountToDrop = 0;
			}
		}
	}

	private void UpdateText()
	{
		this.text.text = string.Empty;
		for (int i = 0; i < this.types.Length; i++)
		{
			TextMeshProUGUI textMeshProUGUI = this.text;
			textMeshProUGUI.text = textMeshProUGUI.text + this.types[i].label + " " + ((this.GetAmmo(i) <= 999) ? (this.ColorValue(this.GetAmmo(i)) + ((i != this.chosenID) ? "\n" : " <\n")) : "999+");
		}
		TextMeshProUGUI textMeshProUGUI2 = this.text;
		string text = textMeshProUGUI2.text;
		textMeshProUGUI2.text = string.Concat(new object[]
		{
			text,
			"[",
			this.amountToDrop,
			"]"
		});
	}

	private string ColorValue(int ammo)
	{
		string text = "#ff0000";
		if (ammo > 0)
		{
			text = "#ffff00";
		}
		if (ammo > this.greenValue)
		{
			text = "#00ff00";
		}
		return string.Concat(new string[]
		{
			"<color=",
			text,
			">",
			ammo.ToString("000"),
			"</color>"
		});
	}

	private void UNetVersion()
	{
	}

	public string Networkamount
	{
		get
		{
			return this.amount;
		}
		set
		{
			uint dirtyBit = 1u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SetAmount(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<string>(value, ref this.amount, dirtyBit);
		}
	}

	protected static void InvokeCmdCmdDrop(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdDrop called on client.");
			return;
		}
		((AmmoBox)obj).CmdDrop((int)reader.ReadPackedUInt32(), (int)reader.ReadPackedUInt32());
	}

	public void CallCmdDrop(int a, int id)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdDrop called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdDrop(a, id);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)AmmoBox.kCmdCmdDrop);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.WritePackedUInt32((uint)a);
		networkWriter.WritePackedUInt32((uint)id);
		base.SendCommandInternal(networkWriter, 2, "CmdDrop");
	}

	static AmmoBox()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(AmmoBox), AmmoBox.kCmdCmdDrop, new NetworkBehaviour.CmdDelegate(AmmoBox.InvokeCmdCmdDrop));
		NetworkCRC.RegisterBehaviour("AmmoBox", 0);
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			writer.Write(this.amount);
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
			writer.Write(this.amount);
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
			this.amount = reader.ReadString();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			this.SetAmount(reader.ReadString());
		}
	}

	private Inventory inv;

	private CharacterClassManager ccm;

	public TextMeshProUGUI text;

	public AmmoBox.AmmoType[] types;

	[SyncVar(hook = "SetAmount")]
	public string amount;

	public int greenValue;

	public int amountToDrop = 5;

	public int chosenID;

	private static int kCmdCmdDrop = -1122225972;

	[Serializable]
	public class AmmoType
	{
		public AmmoType()
		{
		}

		public string label;

		public int inventoryID;
	}
}
