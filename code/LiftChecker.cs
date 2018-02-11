using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class LiftChecker : NetworkBehaviour
{
	private IEnumerator ServerCoroutine()
	{
		for (;;)
		{
			foreach (GameObject player in PlayerManager.singleton.players)
			{
				foreach (Lift lift in this.lifts)
				{
					if (lift.status != Lift.Status.Moving)
					{
						foreach (Lift.Elevator item in lift.elevators)
						{
							if (!item.door.GetBool("isOpen") && lift.operative)
							{
								bool found = true;
								if (Mathf.Abs(item.target.position.x - player.transform.position.x) > lift.maxDistance)
								{
									found = false;
								}
								yield return new WaitForEndOfFrame();
								if (found && Mathf.Abs(item.target.position.y - player.transform.position.y) > lift.maxDistance)
								{
									found = false;
								}
								yield return new WaitForEndOfFrame();
								if (found && Mathf.Abs(item.target.position.z - player.transform.position.z) > lift.maxDistance)
								{
									found = false;
								}
								yield return new WaitForEndOfFrame();
								if (found)
								{
									Transform transform = null;
									GameObject gameObject = item.target.gameObject;
									foreach (Lift.Elevator elevator in lift.elevators)
									{
										if (elevator.target.gameObject != gameObject)
										{
											transform = elevator.target;
										}
									}
									Transform transform2 = player.transform;
									transform2.parent = gameObject.transform;
									Vector3 localPosition = transform2.transform.localPosition;
									transform2.parent = transform.transform;
									transform2.localPosition = localPosition;
									transform2.parent = null;
									this.MovePlayer(player, transform2.position);
								}
							}
						}
						yield return new WaitForEndOfFrame();
					}
					yield return new WaitForEndOfFrame();
				}
				yield return new WaitForEndOfFrame();
			}
			yield return new WaitForEndOfFrame();
		}
		yield break;
	}

	private void Start()
	{
		this.glitch = UnityEngine.Object.FindObjectOfType<ExplosionCameraShake>();
		this.lifts = UnityEngine.Object.FindObjectsOfType<Lift>();
		if (base.isLocalPlayer)
		{
			if (base.isServer)
			{
				base.StartCoroutine(this.ServerCoroutine());
			}
			base.StartCoroutine(this.AmInElevator());
		}
	}

	private IEnumerator AmInElevator()
	{
		for (;;)
		{
			foreach (Lift item in this.lifts)
			{
				if (item.statusID == 2)
				{
					GameObject my = null;
					if (item.InRange(base.transform.position, out my))
					{
						yield return new WaitForSeconds(1.7f);
						int movingtime = 0;
						while (item.InRange(base.transform.position, out my) && movingtime < 20)
						{
							this.glitch.Shake(UnityEngine.Random.Range(0f, 0.08f));
							yield return new WaitForSeconds(0.05f);
							if (item.statusID != 2)
							{
								movingtime++;
							}
						}
					}
				}
				yield return new WaitForEndOfFrame();
			}
			yield return new WaitForEndOfFrame();
		}
		yield break;
	}

	[ServerCallback]
	private void MovePlayer(GameObject player, Vector3 pos)
	{
		if (!NetworkServer.active)
		{
			return;
		}
		this.CallRpcMovePlayer(player, pos);
	}

	[ClientRpc]
	private void RpcMovePlayer(GameObject player, Vector3 pos)
	{
		player.transform.position = pos;
	}

	private void UNetVersion()
	{
	}

	protected static void InvokeRpcRpcMovePlayer(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcMovePlayer called on server.");
			return;
		}
		((LiftChecker)obj).RpcMovePlayer(reader.ReadGameObject(), reader.ReadVector3());
	}

	public void CallRpcMovePlayer(GameObject player, Vector3 pos)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("RPC Function RpcMovePlayer called on client.");
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)2));
		networkWriter.WritePackedUInt32((uint)LiftChecker.kRpcRpcMovePlayer);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(player);
		networkWriter.Write(pos);
		this.SendRPCInternal(networkWriter, 0, "RpcMovePlayer");
	}

	static LiftChecker()
	{
		NetworkBehaviour.RegisterRpcDelegate(typeof(LiftChecker), LiftChecker.kRpcRpcMovePlayer, new NetworkBehaviour.CmdDelegate(LiftChecker.InvokeRpcRpcMovePlayer));
		NetworkCRC.RegisterBehaviour("LiftChecker", 0);
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		bool result;
		return result;
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
	}

	private Lift[] lifts;

	private ExplosionCameraShake glitch;

	private static int kRpcRpcMovePlayer = -1644545198;
}
