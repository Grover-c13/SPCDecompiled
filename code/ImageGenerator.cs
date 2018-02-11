using System;
using System.Collections.Generic;
using UnityEngine;

public class ImageGenerator : MonoBehaviour
{
	public bool GenerateMap(int seed)
	{
		base.GetComponent<PocketDimensionGenerator>().GenerateMap(seed);
		string empty = string.Empty;
		UnityEngine.Random.InitState(seed);
		this.map = this.maps[UnityEngine.Random.Range(0, this.maps.Length)];
		this.InitEntrance();
		this.copy = this.map.GetPixels();
		this.GeneratorTask_CheckRooms();
		this.GeneratorTask_RemoveNotRequired();
		this.GeneratorTask_SetRooms();
		this.GeneratorTask_Cleanup();
		this.GeneratorTask_RemoveDoubledDoorPoints();
		this.map.SetPixels(this.copy);
		this.map.Apply();
		return true;
	}

	private void InitEntrance()
	{
		if (this.height == -1001)
		{
			Transform transform = GameObject.Find("Root_Checkpoint").transform;
			for (int i = 0; i < this.map.height; i++)
			{
				for (int j = 0; j < this.map.width; j++)
				{
					Color pixel = this.map.GetPixel(j, i);
					if (pixel == Color.white)
					{
						this.offset = -new Vector3((float)j * this.gridSize, 0f, (float)i * this.gridSize) / 3f;
					}
				}
			}
			this.offset += Vector3.up;
		}
	}

	private void GeneratorTask_Cleanup()
	{
		foreach (ImageGenerator.RoomsOfType roomsOfType in this.roomsOfType)
		{
			foreach (ImageGenerator.Room room in roomsOfType.roomsOfType)
			{
				foreach (GameObject gameObject in room.room)
				{
					if (room.type != ImageGenerator.RoomType.Prison)
					{
						gameObject.SetActive(false);
					}
				}
			}
		}
	}

	private void GeneratorTask_RemoveDoubledDoorPoints()
	{
		if (this.doors.Count == 0)
		{
			return;
		}
		List<GameObject> list = new List<GameObject>();
		foreach (GameObject item in GameObject.FindGameObjectsWithTag("DoorPoint" + this.height))
		{
			list.Add(item);
		}
		foreach (GameObject gameObject in list)
		{
			foreach (GameObject gameObject2 in list)
			{
				if (Vector3.Distance(gameObject.transform.position, gameObject2.transform.position) < 2f && gameObject != gameObject2)
				{
					UnityEngine.Object.DestroyImmediate(gameObject2);
					this.GeneratorTask_RemoveDoubledDoorPoints();
					return;
				}
			}
		}
		for (int j = 0; j < this.doors.Count; j++)
		{
			try
			{
				if (j < list.Count)
				{
					this.doors[j].transform.position = list[j].transform.position;
					this.doors[j].transform.rotation = list[j].transform.rotation;
				}
				else
				{
					this.doors[j].SetActive(false);
				}
			}
			catch
			{
				Debug.LogError("Not enough doors!");
			}
		}
	}

	private void GeneratorTask_SetRooms()
	{
		for (int i = 0; i < this.map.height; i++)
		{
			for (int j = 0; j < this.map.width; j++)
			{
				Color pixel = this.map.GetPixel(j, i);
				foreach (ImageGenerator.ColorMap colorMap in this.colorMap)
				{
					if (colorMap.color == pixel)
					{
						this.PlaceRoom(new Vector2((float)j, (float)i) + colorMap.centerOffset, colorMap);
					}
				}
			}
		}
	}

	private void GeneratorTask_RemoveNotRequired()
	{
		foreach (ImageGenerator.ColorMap colorMap in this.colorMap)
		{
			bool flag = false;
			while (!flag)
			{
				int num = 0;
				foreach (ImageGenerator.Room room in this.roomsOfType[(int)colorMap.type].roomsOfType)
				{
					num += room.room.Count;
				}
				if (num <= this.roomsOfType[(int)colorMap.type].amount)
				{
					break;
				}
				flag = true;
				for (int i = 0; i < this.roomsOfType[(int)colorMap.type].roomsOfType.Count; i++)
				{
					if (!this.roomsOfType[(int)colorMap.type].roomsOfType[i].required && this.roomsOfType[(int)colorMap.type].roomsOfType[i].room.Count > 0)
					{
						this.roomsOfType[(int)colorMap.type].roomsOfType[i].room[0].SetActive(false);
						this.roomsOfType[(int)colorMap.type].roomsOfType[i].room.RemoveAt(0);
						flag = false;
						break;
					}
				}
			}
		}
	}

	private void GeneratorTask_CheckRooms()
	{
		for (int i = 0; i < this.map.height; i++)
		{
			for (int j = 0; j < this.map.width; j++)
			{
				Color pixel = this.map.GetPixel(j, i);
				foreach (ImageGenerator.ColorMap colorMap in this.colorMap)
				{
					if (colorMap.color == pixel)
					{
						this.BlankSquare(new Vector2((float)j, (float)i) + colorMap.centerOffset);
						this.roomsOfType[(int)colorMap.type].amount++;
						List<ImageGenerator.Room> list = new List<ImageGenerator.Room>();
						bool flag = false;
						for (int k = 0; k < this.availableRooms.Count; k++)
						{
							if (this.availableRooms[k].type == colorMap.type && this.availableRooms[k].room.Count > 0 && this.availableRooms[k].required)
							{
								flag = true;
							}
						}
						bool flag2 = false;
						do
						{
							flag2 = false;
							for (int l = 0; l < this.availableRooms.Count; l++)
							{
								if (this.availableRooms[l].type == colorMap.type && this.availableRooms[l].room.Count > 0 && (this.availableRooms[l].required || !flag))
								{
									list.Add(new ImageGenerator.Room(this.availableRooms[l]));
									this.availableRooms.RemoveAt(l);
									flag2 = true;
									break;
								}
							}
						}
						while (flag2);
						foreach (ImageGenerator.Room r in list)
						{
							this.roomsOfType[(int)colorMap.type].roomsOfType.Add(new ImageGenerator.Room(r));
						}
					}
				}
			}
		}
		this.map.SetPixels(this.copy);
		this.map.Apply();
	}

	private void PlaceRoom(Vector2 pos, ImageGenerator.ColorMap type)
	{
		string message = string.Empty;
		try
		{
			message = "blanking";
			this.BlankSquare(pos);
			message = "do";
			ImageGenerator.Room room;
			do
			{
				message = "rand";
				int num = UnityEngine.Random.Range(0, this.roomsOfType[(int)type.type].roomsOfType.Count);
				message = string.Concat(new object[]
				{
					"rset ",
					(int)type.type,
					"/",
					this.roomsOfType.Length,
					num
				});
				room = this.roomsOfType[(int)type.type].roomsOfType[num];
				if (room.room.Count == 0)
				{
					message = "remove";
					this.roomsOfType[(int)type.type].roomsOfType.RemoveAt(num);
				}
			}
			while (room.room.Count == 0);
			message = "pos";
			room.room[0].transform.localPosition = new Vector3(pos.x * this.gridSize / 3f, (float)this.height, pos.y * this.gridSize / 3f) + this.offset;
			message = "rot";
			room.room[0].transform.localRotation = Quaternion.Euler(Vector3.up * (type.rotationY + this.y_offset));
			message = "rev";
			room.room.RemoveAt(0);
		}
		catch
		{
			MonoBehaviour.print(message);
		}
	}

	private void BlankSquare(Vector2 centerPoint)
	{
		centerPoint = new Vector2(centerPoint.x - 1f, centerPoint.y - 1f);
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				this.map.SetPixel((int)centerPoint.x + i, (int)centerPoint.y + j, new Color(0.3921f, 0.3921f, 0.3921f, 1f));
			}
		}
		this.map.Apply();
	}

	private void Awake()
	{
		foreach (GameObject gameObject in this.doors)
		{
			gameObject.GetComponent<Door>().SetZero();
		}
	}

	public int height;

	public Texture2D[] maps;

	private Texture2D map;

	private Color[] copy;

	public float gridSize;

	public List<ImageGenerator.ColorMap> colorMap = new List<ImageGenerator.ColorMap>();

	public List<ImageGenerator.Room> availableRooms = new List<ImageGenerator.Room>();

	public List<GameObject> doors = new List<GameObject>();

	private Vector3 offset;

	public float y_offset;

	public ImageGenerator.RoomsOfType[] roomsOfType;

	[Serializable]
	public class ColorMap
	{
		public Color color = Color.white;

		public ImageGenerator.RoomType type;

		public float rotationY;

		public Vector2 centerOffset;
	}

	[Serializable]
	public class RoomsOfType
	{
		public List<ImageGenerator.Room> roomsOfType = new List<ImageGenerator.Room>();

		public int amount;
	}

	[Serializable]
	public class Room
	{
		public Room(ImageGenerator.Room r)
		{
			this.room = r.room;
			this.type = r.type;
			this.required = r.required;
		}

		public List<GameObject> room = new List<GameObject>();

		public ImageGenerator.RoomType type;

		public bool required;
	}

	public enum RoomType
	{
		Straight,
		Curve,
		RoomT,
		Cross,
		Endoff,
		Prison
	}
}
