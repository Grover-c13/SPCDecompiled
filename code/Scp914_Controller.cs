using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Scp914_Controller : NetworkBehaviour
{
	public void Refine(string label)
	{
		this.CallCmdRefine914(label);
		base.StartCoroutine(this.SetRandomResults());
	}

	private void Start()
	{
		this.avItems = UnityEngine.Object.FindObjectOfType<Inventory>().availableItems;
	}

	private IEnumerator SetRandomResults()
	{
		int state = UnityEngine.Object.FindObjectOfType<Scp914>().state;
		UnityEngine.Object.FindObjectOfType<Scp914Grabber>().GetComponent<BoxCollider>().isTrigger = true;
		yield return new WaitForSeconds(11f);
		Collider[] colliders = UnityEngine.Object.FindObjectOfType<Scp914Grabber>().observes.ToArray();
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders.Length == 0)
			{
				break;
			}
			if (colliders[i] != null && colliders[i].GetComponentInParent<Pickup>() != null)
			{
				int num = 0;
				if (state == 0)
				{
					int[] output = this.outputs[colliders[i].gameObject.GetComponentInParent<Pickup>().id].output0;
					num = output[UnityEngine.Random.Range(0, output.Length)];
				}
				if (state == 1)
				{
					int[] output2 = this.outputs[colliders[i].gameObject.GetComponentInParent<Pickup>().id].output1;
					num = output2[UnityEngine.Random.Range(0, output2.Length)];
				}
				if (state == 2)
				{
					int[] output3 = this.outputs[colliders[i].gameObject.GetComponentInParent<Pickup>().id].output2;
					num = output3[UnityEngine.Random.Range(0, output3.Length)];
				}
				if (state == 3)
				{
					int[] output4 = this.outputs[colliders[i].gameObject.GetComponentInParent<Pickup>().id].output3;
					num = output4[UnityEngine.Random.Range(0, output4.Length)];
				}
				if (state == 4)
				{
					int[] output5 = this.outputs[colliders[i].gameObject.GetComponentInParent<Pickup>().id].output4;
					num = output5[UnityEngine.Random.Range(0, output5.Length)];
				}
				if (num < 0)
				{
					this.CallCmdDestroyItem(colliders[i].name);
				}
				else
				{
					Vector3 vector = UnityEngine.Object.FindObjectOfType<Scp914>().outputPlace.transform.position;
					vector += new Vector3(UnityEngine.Random.Range(-0.7f, 0.7f), 0f, UnityEngine.Random.Range(-0.7f, 0.7f));
					this.CallCmdSetupPickup(colliders[i].name, num, vector);
				}
			}
			if (!(colliders[i] != null) || colliders[i].tag == "Player")
			{
			}
			yield return new WaitForEndOfFrame();
			UnityEngine.Object.FindObjectOfType<Scp914Grabber>().observes.Clear();
			UnityEngine.Object.FindObjectOfType<Scp914Grabber>().GetComponent<BoxCollider>().isTrigger = false;
		}
		yield break;
	}

	[Command(channel = 2)]
	private void CmdSetupPickup(string label, int result, Vector3 pos)
	{
		GameObject gameObject = GameObject.Find(label);
		gameObject.GetComponent<Pickup>().SetDurability(this.avItems[result].durability);
		gameObject.GetComponentInParent<Pickup>().SetID(result);
		gameObject.GetComponentInParent<Pickup>().SetPosition(pos);
	}

	[Command(channel = 2)]
	private void CmdDestroyItem(string label)
	{
		if (TutorialManager.status)
		{
			UnityEngine.Object.FindObjectOfType<TutorialManager>().Invoke("Tutorial3_KeycardBurnt", 3f);
		}
		GameObject gameObject = GameObject.Find(label);
		gameObject.GetComponentInParent<Pickup>().PickupItem();
	}

	[Command(channel = 2)]
	private void CmdRefine914(string label)
	{
		GameObject gameObject = GameObject.Find(label);
		gameObject.GetComponentInParent<Scp914>().Refine();
	}

	private void UNetVersion()
	{
	}

	protected static void InvokeCmdCmdSetupPickup(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSetupPickup called on client.");
			return;
		}
		((Scp914_Controller)obj).CmdSetupPickup(reader.ReadString(), (int)reader.ReadPackedUInt32(), reader.ReadVector3());
	}

	protected static void InvokeCmdCmdDestroyItem(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdDestroyItem called on client.");
			return;
		}
		((Scp914_Controller)obj).CmdDestroyItem(reader.ReadString());
	}

	protected static void InvokeCmdCmdRefine914(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdRefine914 called on client.");
			return;
		}
		((Scp914_Controller)obj).CmdRefine914(reader.ReadString());
	}

	public void CallCmdSetupPickup(string label, int result, Vector3 pos)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdSetupPickup called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdSetupPickup(label, result, pos);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)Scp914_Controller.kCmdCmdSetupPickup);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(label);
		networkWriter.WritePackedUInt32((uint)result);
		networkWriter.Write(pos);
		base.SendCommandInternal(networkWriter, 2, "CmdSetupPickup");
	}

	public void CallCmdDestroyItem(string label)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdDestroyItem called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdDestroyItem(label);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)Scp914_Controller.kCmdCmdDestroyItem);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(label);
		base.SendCommandInternal(networkWriter, 2, "CmdDestroyItem");
	}

	public void CallCmdRefine914(string label)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdRefine914 called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdRefine914(label);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)Scp914_Controller.kCmdCmdRefine914);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(label);
		base.SendCommandInternal(networkWriter, 2, "CmdRefine914");
	}

	static Scp914_Controller()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(Scp914_Controller), Scp914_Controller.kCmdCmdSetupPickup, new NetworkBehaviour.CmdDelegate(Scp914_Controller.InvokeCmdCmdSetupPickup));
		Scp914_Controller.kCmdCmdDestroyItem = 1055253166;
		NetworkBehaviour.RegisterCommandDelegate(typeof(Scp914_Controller), Scp914_Controller.kCmdCmdDestroyItem, new NetworkBehaviour.CmdDelegate(Scp914_Controller.InvokeCmdCmdDestroyItem));
		Scp914_Controller.kCmdCmdRefine914 = 1834582864;
		NetworkBehaviour.RegisterCommandDelegate(typeof(Scp914_Controller), Scp914_Controller.kCmdCmdRefine914, new NetworkBehaviour.CmdDelegate(Scp914_Controller.InvokeCmdCmdRefine914));
		NetworkCRC.RegisterBehaviour("Scp914_Controller", 0);
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		bool result;
		return result;
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
	}

	public Scp914_Controller.SCP914Output[] outputs;

	private Item[] avItems;

	private static int kCmdCmdSetupPickup = 1424588762;

	private static int kCmdCmdDestroyItem;

	private static int kCmdCmdRefine914;

	[Serializable]
	public class SCP914Output
	{
		public int[] output0;

		public int[] output1;

		public int[] output2;

		public int[] output3;

		public int[] output4;
	}
}
