using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class LiftIdentity : NetworkBehaviour
{
	private void Start()
	{
		if (TutorialManager.status)
		{
			return;
		}
		if (this.isSecond)
		{
			base.StartCoroutine(this.Animation());
		}
	}

	public void SetUp(bool b)
	{
		if (this.isSecond && !this.isWorking)
		{
			this.isWorking = true;
			this.NetworkisUp = b;
			base.StartCoroutine(this.Animation());
		}
	}

	public void Toggle()
	{
		this.SetUp(!this.isUp);
	}

	private IEnumerator Animation()
	{
		this.up_d.SetOpen(false);
		this.down_d.SetOpen(false);
		yield return new WaitForSeconds(5f);
		ElevatorController[] ecs = UnityEngine.Object.FindObjectsOfType<ElevatorController>();
		foreach (ElevatorController ec in ecs)
		{
			if (ec.isLocalPlayer)
			{
				ec.Teleport(this.identity, false);
				yield return new WaitForSeconds(1f);
				if (!ec.Teleport(this.identity, true))
				{
					ec.Teleport(this.identity, false);
				}
			}
		}
		this.up_d.SetOpen(this.isUp);
		this.down_d.SetOpen(!this.isUp);
		yield return new WaitForSeconds(2f);
		this.isWorking = false;
		yield break;
	}

	public bool InArea(Vector3 player)
	{
		Vector3 vector = player - base.GetComponentInParent<MeshCollider>().transform.position;
		return Mathf.Abs(vector.x) < this.liftArea.x / 2f && Mathf.Abs(vector.z) < this.liftArea.z / 2f && Mathf.Abs(vector.y) < this.liftArea.y / 2f;
	}

	private void UNetVersion()
	{
	}

	public bool NetworkisUp
	{
		get
		{
			return this.isUp;
		}
		set
		{
			uint dirtyBit = 1u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SetUp(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<bool>(value, ref this.isUp, dirtyBit);
		}
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			writer.Write(this.isUp);
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
			writer.Write(this.isUp);
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
			this.isUp = reader.ReadBoolean();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			this.SetUp(reader.ReadBoolean());
		}
	}

	public Vector3 liftArea;

	public string identity;

	[Header("isSecond = top lift")]
	public bool isSecond;

	private bool isWorking;

	public ElevatorDoor up_d;

	public ElevatorDoor down_d;

	[SyncVar(hook = "SetUp")]
	public bool isUp;
}
