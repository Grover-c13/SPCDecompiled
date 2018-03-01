using System;
using UnityEngine;

public class PlayerArms : MonoBehaviour
{
	public PlayerArms()
	{
	}

	public PlayerArms.Arm[] arms;

	[Serializable]
	public class Arm
	{
		public Arm()
		{
		}
	}
}
