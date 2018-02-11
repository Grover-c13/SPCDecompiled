using System;
using System.Collections.Generic;
using UnityEngine;

public class PocketDimensionGenerator : MonoBehaviour
{
	public void GenerateMap(int seed)
	{
		UnityEngine.Random.InitState(seed);
		foreach (PocketDimensionTeleport item in UnityEngine.Object.FindObjectsOfType<PocketDimensionTeleport>())
		{
			this.pdtps.Add(item);
		}
		foreach (PocketDimensionTeleport pocketDimensionTeleport in this.pdtps)
		{
			pocketDimensionTeleport.SetType(PocketDimensionTeleport.PDTeleportType.Killer);
		}
		for (int j = 0; j < 2; j++)
		{
			this.SetRandomTeleport(PocketDimensionTeleport.PDTeleportType.Exit);
		}
	}

	private void SetRandomTeleport(PocketDimensionTeleport.PDTeleportType type)
	{
		int index = UnityEngine.Random.Range(0, this.pdtps.Count);
		this.pdtps[index].SetType(type);
		this.pdtps.RemoveAt(index);
	}

	private List<PocketDimensionTeleport> pdtps = new List<PocketDimensionTeleport>();
}
