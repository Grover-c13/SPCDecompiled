using System;
using UnityEngine;

public class CheckpointKiller : MonoBehaviour
{
	public CheckpointKiller()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		CharacterClassManager component = other.GetComponent<CharacterClassManager>();
		if (component != null && component.isLocalPlayer)
		{
			component.CallCmdSuicide(new PlayerStats.HitInfo(999999f, "WORLD", "WALL"));
		}
	}
}
