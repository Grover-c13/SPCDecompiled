using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	private void Awake()
	{
		PlayerManager.singleton = this;
	}

	public void AddPlayer(GameObject player)
	{
		List<GameObject> list = new List<GameObject>();
		foreach (GameObject item in this.players)
		{
			list.Add(item);
		}
		if (!list.Contains(player))
		{
			list.Add(player);
		}
		this.players = list.ToArray();
	}

	public void RemovePlayer(GameObject player)
	{
		List<GameObject> list = new List<GameObject>();
		foreach (GameObject item in this.players)
		{
			list.Add(item);
		}
		if (list.Contains(player))
		{
			list.Remove(player);
		}
		this.players = list.ToArray();
	}

	public GameObject[] players;

	public static PlayerManager singleton;

	public static int playerID;

	public static GameObject localPlayer;
}
