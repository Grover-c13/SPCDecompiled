using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Lift : NetworkBehaviour
{
	private void SetStatus(int i)
	{
		this.NetworkstatusID = i;
		this.status = (Lift.Status)i;
	}

	private void Awake()
	{
		foreach (Lift.Elevator elevator in this.elevators)
		{
			elevator.target.tag = "LiftTarget";
		}
	}

	private void FixedUpdate()
	{
		for (int i = 0; i < this.elevators.Length; i++)
		{
			bool value = this.statusID == i && this.status != Lift.Status.Moving;
			this.elevators[i].door.SetBool("isOpen", value);
		}
	}

	public void UseLift()
	{
		if (this.operative)
		{
			base.StartCoroutine(this.LiftAnimation());
			this.operative = false;
		}
	}

	private IEnumerator LiftAnimation()
	{
		Lift.Status previousStatus = this.status;
		this.SetStatus(2);
		yield return new WaitForSeconds(0.7f);
		this.CallRpcPlayMusic();
		yield return new WaitForSeconds(2f);
		this.CallRpcMovePlayers();
		yield return new WaitForSeconds(this.movingSpeed - 2f);
		this.SetStatus((previousStatus != Lift.Status.Down) ? 1 : 0);
		yield return new WaitForSeconds(2f);
		this.operative = true;
		yield break;
	}

	[ClientRpc(channel = 4)]
	private void RpcPlayMusic()
	{
		foreach (Lift.Elevator elevator in this.elevators)
		{
			try
			{
				elevator.musicSpeaker.Play();
			}
			catch
			{
			}
		}
	}

	[ClientRpc(channel = 4)]
	private void RpcMovePlayers()
	{
		Transform transform = PlayerManager.localPlayer.transform;
		GameObject gameObject = null;
		if (this.InRange(transform.position, out gameObject))
		{
			Transform transform2 = null;
			foreach (Lift.Elevator elevator in this.elevators)
			{
				if (elevator.target.gameObject != gameObject)
				{
					transform2 = elevator.target;
				}
			}
			transform.parent = gameObject.transform;
			Vector3 localPosition = transform.transform.localPosition;
			transform.parent = transform2.transform;
			transform.localPosition = localPosition;
			transform.parent = null;
			transform.GetComponent<FirstPersonController>().m_MouseLook.SetRotation(transform2.transform.rotation.eulerAngles.y - gameObject.transform.rotation.eulerAngles.y);
			UnityEngine.Object.FindObjectOfType<ExplosionCameraShake>().Shake(0.15f);
			transform.parent = null;
		}
	}

	public bool InRange(Vector3 pos, out GameObject which)
	{
		foreach (Lift.Elevator elevator in this.elevators)
		{
			bool flag = true;
			if (Mathf.Abs(elevator.target.position.x - pos.x) > this.maxDistance)
			{
				flag = false;
			}
			if (Mathf.Abs(elevator.target.position.y - pos.y) > this.maxDistance)
			{
				flag = false;
			}
			if (Mathf.Abs(elevator.target.position.z - pos.z) > this.maxDistance)
			{
				flag = false;
			}
			if (flag)
			{
				which = elevator.target.gameObject;
				return true;
			}
		}
		which = null;
		return false;
	}

	private void UNetVersion()
	{
	}

	public int NetworkstatusID
	{
		get
		{
			return this.statusID;
		}
		set
		{
			uint dirtyBit = 1u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SetStatus(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<int>(value, ref this.statusID, dirtyBit);
		}
	}

	protected static void InvokeRpcRpcPlayMusic(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcPlayMusic called on server.");
			return;
		}
		((Lift)obj).RpcPlayMusic();
	}

	protected static void InvokeRpcRpcMovePlayers(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcMovePlayers called on server.");
			return;
		}
		((Lift)obj).RpcMovePlayers();
	}

	public void CallRpcPlayMusic()
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("RPC Function RpcPlayMusic called on client.");
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)2));
		networkWriter.WritePackedUInt32((uint)Lift.kRpcRpcPlayMusic);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		this.SendRPCInternal(networkWriter, 4, "RpcPlayMusic");
	}

	public void CallRpcMovePlayers()
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("RPC Function RpcMovePlayers called on client.");
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)2));
		networkWriter.WritePackedUInt32((uint)Lift.kRpcRpcMovePlayers);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		this.SendRPCInternal(networkWriter, 4, "RpcMovePlayers");
	}

	static Lift()
	{
		NetworkBehaviour.RegisterRpcDelegate(typeof(Lift), Lift.kRpcRpcPlayMusic, new NetworkBehaviour.CmdDelegate(Lift.InvokeRpcRpcPlayMusic));
		Lift.kRpcRpcMovePlayers = -1736442720;
		NetworkBehaviour.RegisterRpcDelegate(typeof(Lift), Lift.kRpcRpcMovePlayers, new NetworkBehaviour.CmdDelegate(Lift.InvokeRpcRpcMovePlayers));
		NetworkCRC.RegisterBehaviour("Lift", 0);
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			writer.WritePackedUInt32((uint)this.statusID);
			return true;
		}
		bool flag = false;
		if ((base.syncVarDirtyBits & 1u) != 0u)
		{
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
				flag = true;
			}
			writer.WritePackedUInt32((uint)this.statusID);
		}
		if (!flag)
		{
			writer.WritePackedUInt32(base.syncVarDirtyBits);
		}
		return flag;
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
		if (initialState)
		{
			this.statusID = (int)reader.ReadPackedUInt32();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			this.SetStatus((int)reader.ReadPackedUInt32());
		}
	}

	[SyncVar(hook = "SetStatus")]
	public int statusID;

	public Lift.Elevator[] elevators;

	public Lift.Status status;

	public float movingSpeed;

	public bool operative = true;

	public float maxDistance;

	private static int kRpcRpcPlayMusic = 374858512;

	private static int kRpcRpcMovePlayers;

	[Serializable]
	public struct Elevator
	{
		public Transform target;

		public Animator door;

		public AudioSource musicSpeaker;
	}

	public enum Status
	{
		Up,
		Down,
		Moving
	}
}
