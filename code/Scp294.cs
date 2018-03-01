using System;
using UnityEngine;
using UnityEngine.Networking;

public class Scp294 : NetworkBehaviour
{
	public Scp294()
	{
	}

	public void Buy()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Pickup");
		GameObject gameObject = null;
		foreach (GameObject gameObject2 in array)
		{
		}
		if (gameObject != null)
		{
			this.CallCmdSetPickup(gameObject.name, 18);
		}
	}

	[Command(channel = 2)]
	public void CmdSetPickup(string objname, int dropedItemID)
	{
		GameObject gameObject = GameObject.Find(objname);
		gameObject.GetComponent<Pickup>().SetDurability(0f);
		gameObject.GetComponent<Pickup>().SetID(dropedItemID);
		gameObject.GetComponent<Pickup>().SetPosition(this.position.position);
		gameObject.GetComponent<Pickup>().SetRotation(this.position.rotation.eulerAngles);
	}

	private void UNetVersion()
	{
	}

	protected static void InvokeCmdCmdSetPickup(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSetPickup called on client.");
			return;
		}
		((Scp294)obj).CmdSetPickup(reader.ReadString(), (int)reader.ReadPackedUInt32());
	}

	public void CallCmdSetPickup(string objname, int dropedItemID)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdSetPickup called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdSetPickup(objname, dropedItemID);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)Scp294.kCmdCmdSetPickup);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(objname);
		networkWriter.WritePackedUInt32((uint)dropedItemID);
		base.SendCommandInternal(networkWriter, 2, "CmdSetPickup");
	}

	static Scp294()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(Scp294), Scp294.kCmdCmdSetPickup, new NetworkBehaviour.CmdDelegate(Scp294.InvokeCmdCmdSetPickup));
		NetworkCRC.RegisterBehaviour("Scp294", 0);
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		bool result;
		return result;
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
	}

	public Transform position;

	private static int kCmdCmdSetPickup = -844185359;
}
