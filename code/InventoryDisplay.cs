using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDisplay : MonoBehaviour
{
	public InventoryDisplay()
	{
	}

	public void SetDescriptionByID(int id)
	{
		if (id == -1)
		{
			this.hoveredID = -1;
			this.description.text = string.Empty;
		}
		else if (this.items.Count > id)
		{
			string text = TranslationReader.Get("Inventory", this.items[id].id).Replace("\\n", Environment.NewLine);
			string label = this.items[id].label;
			this.description.text = text;
			this.hoveredID = id;
		}
		else
		{
			this.hoveredID = -1;
			this.description.text = string.Empty;
		}
	}

	private void Update()
	{
		if (this.localplayer == null)
		{
			return;
		}
		if (!this.rootObject.activeSelf)
		{
			this.hoveredID = -1;
		}
		this.items = this.localplayer.GetComponent<Inventory>().items;
		if (Input.GetButtonDown("Cancel") || this.isSCP)
		{
			this.rootObject.SetActive(false);
			this.hoveredID = -1;
			this.localplayer.GetComponent<FirstPersonController>().m_MouseLook.isOpenEq = this.rootObject.activeSelf;
			CursorManager.eqOpen = this.rootObject.activeSelf;
		}
		if (!this.isSCP && Input.GetButtonDown("Inventory") && !this.localplayer.GetComponent<MicroHID_GFX>().onFire && !CursorManager.pauseOpen)
		{
			this.hoveredID = -1;
			this.rootObject.SetActive(!this.rootObject.activeSelf);
			this.localplayer.GetComponent<FirstPersonController>().m_MouseLook.isOpenEq = this.rootObject.activeSelf;
			CursorManager.eqOpen = this.rootObject.activeSelf;
		}
		if (Input.GetKeyDown(KeyCode.Mouse1) && this.hoveredID >= 0 && this.rootObject.activeSelf)
		{
			this.localplayer.GetComponent<Inventory>().DropItem(this.hoveredID);
		}
		if (Input.GetKeyDown(KeyCode.Mouse0) && this.rootObject.activeSelf)
		{
			this.localplayer.GetComponent<Inventory>().localInventoryItem = ((this.hoveredID < 0) ? null : this.localplayer.GetComponent<Inventory>().items[this.hoveredID]);
			this.localplayer.GetComponent<Inventory>().NetworkcurItem = ((this.hoveredID < 0) ? this.hoveredID : this.items[this.hoveredID].id);
			this.localplayer.GetComponent<FirstPersonController>().m_MouseLook.isOpenEq = false;
			CursorManager.eqOpen = false;
			this.rootObject.SetActive(false);
		}
		foreach (Image image in this.itemslots)
		{
			image.GetComponentInChildren<RawImage>().texture = this.blackTexture;
		}
		for (int j = this.itemslots.Length - 1; j >= 0; j--)
		{
			if (j < this.items.Count)
			{
				this.itemslots[j].GetComponentInChildren<RawImage>().texture = this.items[j].icon;
			}
		}
	}

	[HideInInspector]
	public GameObject localplayer;

	public GameObject rootObject;

	public Texture2D blackTexture;

	public TextMeshProUGUI description;

	public Image[] itemslots;

	private List<Item> items = new List<Item>();

	public int hoveredID;

	public bool isSCP;
}
