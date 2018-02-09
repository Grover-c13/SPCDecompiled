using System;
using TMPro;
using UnityEngine;

public class AmmoBox : MonoBehaviour
{
	private void Start()
	{
		this.inv = base.GetComponent<Inventory>();
		this.ccm = base.GetComponent<CharacterClassManager>();
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
			if (Input.GetButtonDown("Fire1") && this.types[this.chosenID].quantity >= this.amountToDrop && this.amountToDrop != 0)
			{
				this.types[this.chosenID].quantity -= this.amountToDrop;
				this.inv.CallCmdSetPickup(this.types[this.chosenID].inventoryID, (float)this.amountToDrop, base.transform.position, this.inv.kamera.transform.rotation, base.transform.rotation);
			}
			if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
			{
				this.amountToDrop--;
			}
			if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
			{
				this.amountToDrop++;
			}
			int quantity = this.types[this.chosenID].quantity;
			if (quantity >= 15)
			{
				this.amountToDrop = Mathf.Clamp(this.amountToDrop, 15, quantity);
			}
			else
			{
				this.amountToDrop = 0;
			}
		}
	}

	public void SetAmmoAmount()
	{
		for (int i = 0; i < 3; i++)
		{
			this.types[i].quantity = this.ccm.klasy[this.ccm.curClass].ammoTypes[i];
		}
	}

	private void UpdateText()
	{
		this.text.text = string.Empty;
		string text;
		for (int i = 0; i < this.types.Length; i++)
		{
			TextMeshProUGUI textMeshProUGUI = this.text;
			text = textMeshProUGUI.text;
			textMeshProUGUI.text = string.Concat(new string[]
			{
				text,
				this.types[i].label,
				" ",
				(this.types[i].quantity <= 999) ? this.ColorValue(this.types[i].quantity) : "999+",
				(i != this.chosenID) ? "\n" : " <\n"
			});
		}
		TextMeshProUGUI textMeshProUGUI2 = this.text;
		text = textMeshProUGUI2.text;
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

	private Inventory inv;

	private CharacterClassManager ccm;

	public TextMeshProUGUI text;

	public AmmoBox.AmmoType[] types;

	public int greenValue;

	public int amountToDrop = 5;

	public int chosenID;

	[Serializable]
	public class AmmoType
	{
		public string label;

		public int inventoryID;

		public int quantity;
	}
}
