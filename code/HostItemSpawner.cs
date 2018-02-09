using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HostItemSpawner : NetworkBehaviour
{
	private void Start()
	{
		this.avItems = UnityEngine.Object.FindObjectOfType<Inventory>().availableItems;
	}

	public void Spawn(int seed)
	{
		UnityEngine.Random.InitState(seed);
		string str = string.Empty;
		try
		{
			this.ris = UnityEngine.Object.FindObjectOfType<RandomItemSpawner>();
			RandomItemSpawner.PickupPositionRelation[] pickups = this.ris.pickups;
			List<RandomItemSpawner.PositionPosIdRelation> list = new List<RandomItemSpawner.PositionPosIdRelation>();
			str = "Rozpoczynianie dodawania pozycji";
			foreach (RandomItemSpawner.PositionPosIdRelation item in this.ris.posIds)
			{
				list.Add(item);
			}
			int num = 0;
			foreach (RandomItemSpawner.PickupPositionRelation pickupPositionRelation in pickups)
			{
				for (int k = 0; k < list.Count; k++)
				{
					list[k].index = k;
				}
				List<RandomItemSpawner.PositionPosIdRelation> list2 = new List<RandomItemSpawner.PositionPosIdRelation>();
				foreach (RandomItemSpawner.PositionPosIdRelation positionPosIdRelation in list)
				{
					if (positionPosIdRelation.posID == pickupPositionRelation.posID)
					{
						list2.Add(positionPosIdRelation);
					}
				}
				str = "Setowanie rzeczy " + num;
				int index = UnityEngine.Random.Range(0, list2.Count);
				RandomItemSpawner.PositionPosIdRelation positionPosIdRelation2 = list2[index];
				int index2 = positionPosIdRelation2.index;
				this.CallCmdSetPos(pickupPositionRelation.pickup.gameObject, positionPosIdRelation2.position.position, pickupPositionRelation.itemID, positionPosIdRelation2.position.rotation.eulerAngles, (pickupPositionRelation.duration != -1) ? ((float)pickupPositionRelation.duration) : this.avItems[pickupPositionRelation.itemID].durability);
				list.RemoveAt(index2);
				num++;
			}
		}
		catch
		{
			Debug.LogError("Nie działa cos, lel" + str);
		}
	}

	[Command(channel = 2)]
	private void CmdSetPos(GameObject obj, Vector3 pos, int item, Vector3 rot, float dur)
	{
		Pickup component = obj.GetComponent<Pickup>();
		component.SetDurability(dur);
		component.SetID(item);
		component.KeepRotation(2);
		if (rot == Vector3.zero)
		{
			component.SetRotation(new Vector3((float)UnityEngine.Random.Range(0, 360), (float)UnityEngine.Random.Range(0, 360), (float)UnityEngine.Random.Range(0, 360)));
		}
		else
		{
			component.SetRotation(rot);
		}
		component.SetPosition(pos);
	}

	private void UNetVersion()
	{
	}

	protected static void InvokeCmdCmdSetPos(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSetPos called on client.");
			return;
		}
		((HostItemSpawner)obj).CmdSetPos(reader.ReadGameObject(), reader.ReadVector3(), (int)reader.ReadPackedUInt32(), reader.ReadVector3(), reader.ReadSingle());
	}

	public void CallCmdSetPos(GameObject obj, Vector3 pos, int item, Vector3 rot, float dur)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdSetPos called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdSetPos(obj, pos, item, rot, dur);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)HostItemSpawner.kCmdCmdSetPos);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(obj);
		networkWriter.Write(pos);
		networkWriter.WritePackedUInt32((uint)item);
		networkWriter.Write(rot);
		networkWriter.Write(dur);
		base.SendCommandInternal(networkWriter, 2, "CmdSetPos");
	}

	static HostItemSpawner()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(HostItemSpawner), HostItemSpawner.kCmdCmdSetPos, new NetworkBehaviour.CmdDelegate(HostItemSpawner.InvokeCmdCmdSetPos));
		NetworkCRC.RegisterBehaviour("HostItemSpawner", 0);
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		bool result;
		return result;
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
	}

	private RandomItemSpawner ris;

	private Item[] avItems;

	private static int kCmdCmdSetPos = 440799519;
}
