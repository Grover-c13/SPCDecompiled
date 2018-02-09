using System;
using UnityEngine;

public class RandomItemSpawner : MonoBehaviour
{
	public void RefreshIndexes()
	{
		for (int i = 0; i < this.posIds.Length; i++)
		{
			this.posIds[i].index = i;
		}
	}

	public RandomItemSpawner.PickupPositionRelation[] pickups;

	public RandomItemSpawner.PositionPosIdRelation[] posIds;

	[Serializable]
	public class PickupPositionRelation
	{
		public Pickup pickup;

		public int itemID;

		public string posID;

		public int duration = -1;
	}

	[Serializable]
	public class PositionPosIdRelation
	{
		public string posID;

		public Transform position;

		public int index;
	}
}
