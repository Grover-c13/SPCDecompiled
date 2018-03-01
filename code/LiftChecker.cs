using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

public class LiftChecker : NetworkBehaviour
{
	public LiftChecker()
	{
	}

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
			UnityEngine.Debug.LogError("RPC RpcMovePlayer called on server.");
			return;
		}
		((LiftChecker)obj).RpcMovePlayer(reader.ReadGameObject(), reader.ReadVector3());
	}

	public void CallRpcMovePlayer(GameObject player, Vector3 pos)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogError("RPC Function RpcMovePlayer called on client.");
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

	[CompilerGenerated]
	private sealed class <ServerCoroutine>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <ServerCoroutine>c__Iterator0()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this.$PC;
			this.$PC = -1;
			switch (num)
			{
			case 0u:
				break;
			case 1u:
				if (this.<found>__4 && Mathf.Abs(this.<item>__3.target.position.y - this.<player>__1.transform.position.y) > this.<lift>__2.maxDistance)
				{
					this.<found>__4 = false;
				}
				this.$current = new WaitForEndOfFrame();
				if (!this.$disposing)
				{
					this.$PC = 2;
				}
				return true;
			case 2u:
				if (this.<found>__4 && Mathf.Abs(this.<item>__3.target.position.z - this.<player>__1.transform.position.z) > this.<lift>__2.maxDistance)
				{
					this.<found>__4 = false;
				}
				this.$current = new WaitForEndOfFrame();
				if (!this.$disposing)
				{
					this.$PC = 3;
				}
				return true;
			case 3u:
				if (this.<found>__4)
				{
					Transform transform = null;
					GameObject gameObject = this.<item>__3.target.gameObject;
					foreach (Lift.Elevator elevator in this.<lift>__2.elevators)
					{
						if (elevator.target.gameObject != gameObject)
						{
							transform = elevator.target;
						}
					}
					Transform transform2 = this.<player>__1.transform;
					transform2.parent = gameObject.transform;
					Vector3 localPosition = transform2.transform.localPosition;
					transform2.parent = transform.transform;
					transform2.localPosition = localPosition;
					transform2.parent = null;
					this.$this.MovePlayer(this.<player>__1, transform2.position);
					goto IL_347;
				}
				goto IL_347;
			case 4u:
				IL_387:
				this.$current = new WaitForEndOfFrame();
				if (!this.$disposing)
				{
					this.$PC = 5;
				}
				return true;
			case 5u:
				this.$locvar3++;
				goto IL_3B4;
			case 6u:
				this.$locvar1++;
				goto IL_3F4;
			case 7u:
				break;
			default:
				return false;
			}
			this.$locvar0 = PlayerManager.singleton.players;
			this.$locvar1 = 0;
			goto IL_3F4;
			IL_347:
			this.$locvar5++;
			IL_355:
			if (this.$locvar5 >= this.$locvar4.Length)
			{
				this.$current = new WaitForEndOfFrame();
				if (!this.$disposing)
				{
					this.$PC = 4;
				}
				return true;
			}
			this.<item>__3 = this.$locvar4[this.$locvar5];
			if (!this.<item>__3.door.GetBool("isOpen") && this.<lift>__2.operative)
			{
				this.<found>__4 = true;
				if (Mathf.Abs(this.<item>__3.target.position.x - this.<player>__1.transform.position.x) > this.<lift>__2.maxDistance)
				{
					this.<found>__4 = false;
				}
				this.$current = new WaitForEndOfFrame();
				if (!this.$disposing)
				{
					this.$PC = 1;
				}
				return true;
			}
			goto IL_347;
			IL_3B4:
			if (this.$locvar3 >= this.$locvar2.Length)
			{
				this.$current = new WaitForEndOfFrame();
				if (!this.$disposing)
				{
					this.$PC = 6;
				}
				return true;
			}
			this.<lift>__2 = this.$locvar2[this.$locvar3];
			if (this.<lift>__2.status != Lift.Status.Moving)
			{
				this.$locvar4 = this.<lift>__2.elevators;
				this.$locvar5 = 0;
				goto IL_355;
			}
			goto IL_387;
			IL_3F4:
			if (this.$locvar1 < this.$locvar0.Length)
			{
				this.<player>__1 = this.$locvar0[this.$locvar1];
				this.$locvar2 = this.$this.lifts;
				this.$locvar3 = 0;
				goto IL_3B4;
			}
			this.$current = new WaitForEndOfFrame();
			if (!this.$disposing)
			{
				this.$PC = 7;
			}
			return true;
		}

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.$current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.$current;
			}
		}

		[DebuggerHidden]
		public void Dispose()
		{
			this.$disposing = true;
			this.$PC = -1;
		}

		[DebuggerHidden]
		public void Reset()
		{
			throw new NotSupportedException();
		}

		internal GameObject[] $locvar0;

		internal int $locvar1;

		internal GameObject <player>__1;

		internal Lift[] $locvar2;

		internal int $locvar3;

		internal Lift <lift>__2;

		internal Lift.Elevator[] $locvar4;

		internal int $locvar5;

		internal Lift.Elevator <item>__3;

		internal bool <found>__4;

		internal LiftChecker $this;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}

	[CompilerGenerated]
	private sealed class <AmInElevator>c__Iterator1 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <AmInElevator>c__Iterator1()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this.$PC;
			this.$PC = -1;
			switch (num)
			{
			case 0u:
				break;
			case 1u:
				this.<movingtime>__3 = 0;
				goto IL_12D;
			case 2u:
				if (this.<item>__1.statusID != 2)
				{
					this.<movingtime>__3++;
					goto IL_12D;
				}
				goto IL_12D;
			case 3u:
				this.$locvar1++;
				goto IL_18D;
			case 4u:
				break;
			default:
				return false;
			}
			this.$locvar0 = this.$this.lifts;
			this.$locvar1 = 0;
			goto IL_18D;
			IL_12D:
			if (this.<item>__1.InRange(this.$this.transform.position, out this.<my>__2) && this.<movingtime>__3 < 20)
			{
				this.$this.glitch.Shake(UnityEngine.Random.Range(0f, 0.08f));
				this.$current = new WaitForSeconds(0.05f);
				if (!this.$disposing)
				{
					this.$PC = 2;
				}
				return true;
			}
			IL_160:
			this.$current = new WaitForEndOfFrame();
			if (!this.$disposing)
			{
				this.$PC = 3;
			}
			return true;
			IL_18D:
			if (this.$locvar1 >= this.$locvar0.Length)
			{
				this.$current = new WaitForEndOfFrame();
				if (!this.$disposing)
				{
					this.$PC = 4;
				}
			}
			else
			{
				this.<item>__1 = this.$locvar0[this.$locvar1];
				if (this.<item>__1.statusID != 2)
				{
					goto IL_160;
				}
				this.<my>__2 = null;
				if (!this.<item>__1.InRange(this.$this.transform.position, out this.<my>__2))
				{
					goto IL_160;
				}
				this.$current = new WaitForSeconds(1.7f);
				if (!this.$disposing)
				{
					this.$PC = 1;
				}
			}
			return true;
		}

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.$current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.$current;
			}
		}

		[DebuggerHidden]
		public void Dispose()
		{
			this.$disposing = true;
			this.$PC = -1;
		}

		[DebuggerHidden]
		public void Reset()
		{
			throw new NotSupportedException();
		}

		internal Lift[] $locvar0;

		internal int $locvar1;

		internal Lift <item>__1;

		internal GameObject <my>__2;

		internal int <movingtime>__3;

		internal LiftChecker $this;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}
}
