using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RemoteAdmin
{
	public class ItemDownloader : MonoBehaviour
	{
		public ItemDownloader()
		{
		}

		private void Start()
		{
			List<Button> list = new List<Button>();
			list.Add(this.element.GetComponent<Button>());
			Item[] availableItems = UnityEngine.Object.FindObjectOfType<Inventory>().availableItems;
			for (int i = 1; i < availableItems.Length; i++)
			{
				if (!availableItems[i].noEquipable)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.element, this.target);
					gameObject.name = "Item_" + i;
					gameObject.transform.localScale = Vector3.one;
					gameObject.GetComponentInChildren<RawImage>().texture = availableItems[i].icon;
					list.Add(gameObject.GetComponent<Button>());
				}
			}
			base.GetComponentInParent<GiveItem>().SetButtons(list.ToArray());
		}

		public Transform target;

		public GameObject element;
	}
}
