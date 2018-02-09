using System;
using UnityEngine;
using UnityEngine.Networking;

public class PocketDimensionTeleport : MonoBehaviour
{
	public void SetType(PocketDimensionTeleport.PDTeleportType t)
	{
		this.type = t;
	}

	private void OnTriggerEnter(Collider other)
	{
		NetworkIdentity component = other.GetComponent<NetworkIdentity>();
		if (component != null && component.isLocalPlayer)
		{
			if ((this.type == PocketDimensionTeleport.PDTeleportType.Killer || UnityEngine.Object.FindObjectOfType<BlastDoor>().isClosed) && !Input.GetKey(KeyCode.P))
			{
				component.GetComponent<CharacterClassManager>().CallCmdSuicide(new PlayerStats.HitInfo(999999f, "WORLD", "POCKET"));
			}
			else if (this.type == PocketDimensionTeleport.PDTeleportType.Exit)
			{
				GameObject[] array = GameObject.FindGameObjectsWithTag("PD_EXIT");
				component.transform.position = array[UnityEngine.Random.Range(0, array.Length)].transform.position;
			}
		}
	}

	private PocketDimensionTeleport.PDTeleportType type;

	public enum PDTeleportType
	{
		Killer,
		Exit
	}
}
