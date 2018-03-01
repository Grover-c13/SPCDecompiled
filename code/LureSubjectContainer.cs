using System;
using UnityEngine;
using UnityEngine.Networking;

public class LureSubjectContainer : NetworkBehaviour
{
	public LureSubjectContainer()
	{
	}

	public void SetState(bool b)
	{
		this.NetworkallowContain = b;
		if (b)
		{
			this.hatch.GetComponent<AudioSource>().Play();
		}
	}

	private void Start()
	{
		base.transform.localPosition = this.position;
		base.transform.localRotation = Quaternion.Euler(this.rotation);
	}

	private void Update()
	{
		this.CheckForLure();
		this.hatch.localPosition = Vector3.Slerp(this.hatch.localPosition, (!this.allowContain) ? this.openPosition : this.closedPos, Time.deltaTime * 3f);
	}

	private void CheckForLure()
	{
		if (this.ccm == null)
		{
			this.localplayer = PlayerManager.localPlayer;
			if (this.localplayer != null)
			{
				this.ccm = this.localplayer.GetComponent<CharacterClassManager>();
			}
			return;
		}
		if (this.ccm.curClass >= 0)
		{
			Team team = this.ccm.klasy[this.ccm.curClass].team;
			base.GetComponent<BoxCollider>().enabled = (team == Team.SCP);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.position, this.range);
	}

	private void UNetVersion()
	{
	}

	public bool NetworkallowContain
	{
		get
		{
			return this.allowContain;
		}
		set
		{
			uint dirtyBit = 1u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SetState(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<bool>(value, ref this.allowContain, dirtyBit);
		}
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			writer.Write(this.allowContain);
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
			writer.Write(this.allowContain);
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
			this.allowContain = reader.ReadBoolean();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			this.SetState(reader.ReadBoolean());
		}
	}

	private Vector3 position = new Vector3(-1471f, 160.5f, -3426.9f);

	private Vector3 rotation = new Vector3(0f, 180f, 0f);

	public float range;

	[SyncVar(hook = "SetState")]
	public bool allowContain;

	private CharacterClassManager ccm;

	[Space(10f)]
	public Transform hatch;

	public Vector3 closedPos;

	public Vector3 openPosition;

	private GameObject localplayer;
}
