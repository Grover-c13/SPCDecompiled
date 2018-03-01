using System;
using System.Collections.Generic;
using GameConsole;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour
{
	public RoomManager()
	{
	}

	private void Start()
	{
		if (this.useSimulator != -1)
		{
			this.GenerateMap(this.useSimulator);
		}
	}

	public void GenerateMap(int seed)
	{
		GameConsole.Console console = UnityEngine.Object.FindObjectOfType<GameConsole.Console>();
		if (!TutorialManager.status)
		{
			base.GetComponent<PocketDimensionGenerator>().GenerateMap(seed);
			for (int i = 0; i < this.positions.Count; i++)
			{
				this.positions[i].point.name = "POINT" + i;
				if (this.positions[i].point.GetComponent<Point>() == null)
				{
					Debug.LogError("RoomManager: Missing 'Point' script at current position.");
					return;
				}
			}
			UnityEngine.Random.InitState(seed);
			console.AddLog("[MG REPLY]: Successfully recieved map seed!", new Color32(0, byte.MaxValue, 0, byte.MaxValue), true);
			List<RoomManager.RoomPosition> list = this.positions;
			console.AddLog("[MG TASK]: Setting rooms positions...", new Color32(0, byte.MaxValue, 0, byte.MaxValue), false);
			foreach (RoomManager.Room room in this.rooms)
			{
				console.AddLog("\t\t[MG INFO]: " + room.label + " is about to set!", new Color32(120, 120, 120, byte.MaxValue), true);
				List<int> list2 = new List<int>();
				for (int j = 0; j < list.Count; j++)
				{
					if (this.positions[j].type.Equals(room.type))
					{
						bool flag = true;
						foreach (Point point in room.roomPrefab.GetComponentsInChildren<Point>())
						{
							if (this.positions[j].point.name == point.gameObject.name)
							{
								flag = false;
							}
						}
						if (flag)
						{
							list2.Add(j);
						}
					}
				}
				List<int> list3 = list2;
				for (int l = 0; l < list3.Count; l++)
				{
					foreach (Point point2 in room.roomPrefab.GetComponentsInChildren<Point>())
					{
						if (this.positions[list3[l]].point.name == point2.gameObject.name)
						{
							list2.Remove(list3[l]);
						}
					}
				}
				int index = list2[UnityEngine.Random.Range(0, list2.Count)];
				RoomManager.RoomPosition roomPosition = this.positions[index];
				GameObject roomPrefab = room.roomPrefab;
				RawImage icon = room.icon;
				room.readonlyPoint = roomPosition.point;
				roomPrefab.transform.SetParent(roomPosition.point);
				roomPrefab.transform.localPosition = room.roomOffset.position;
				roomPrefab.transform.localRotation = Quaternion.Euler(room.roomOffset.rotation);
				roomPrefab.transform.localScale = room.roomOffset.scale;
				if (icon != null)
				{
					icon.rectTransform.SetParent(roomPosition.ui_point);
					icon.transform.localPosition = room.iconoffset.position;
					icon.rectTransform.localRotation = Quaternion.Euler(room.iconoffset.rotation);
					icon.transform.localScale = room.iconoffset.scale;
				}
				roomPrefab.SetActive(true);
				this.positions.RemoveAt(index);
			}
		}
		console.AddLog("--Map successfully generated--", new Color32(0, byte.MaxValue, 0, byte.MaxValue), false);
		this.isGenerated = true;
	}

	public bool isGenerated;

	public int useSimulator = -1;

	public List<RoomManager.Room> rooms = new List<RoomManager.Room>();

	public List<RoomManager.RoomPosition> positions = new List<RoomManager.RoomPosition>();

	[Serializable]
	public class Room
	{
		public Room()
		{
		}

		public string label;

		public Offset roomOffset;

		public GameObject roomPrefab;

		public string type;

		public Transform readonlyPoint;

		public RawImage icon;

		public Offset iconoffset;
	}

	[Serializable]
	public struct RoomPosition
	{
		public string type;

		public Transform point;

		public RectTransform ui_point;
	}
}
