using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AmbientSoundPlayer : NetworkBehaviour
{
	public AmbientSoundPlayer()
	{
	}

	private void Start()
	{
		if (base.isLocalPlayer && base.isServer)
		{
			for (int i = 0; i < this.clips.Length; i++)
			{
				this.clips[i].index = i;
			}
			base.Invoke("GenerateRandom", 10f);
		}
	}

	private void PlaySound(int clipID)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.audioPrefab);
		Vector2 vector = new Vector2((float)UnityEngine.Random.Range(-1, 1), (float)UnityEngine.Random.Range(-1, 1));
		Vector3 vector2 = new Vector3(vector.x, 0f, vector.y);
		Vector3 a = vector2.normalized * 200f;
		gameObject.transform.position = a + base.transform.position;
		gameObject.GetComponent<AudioSource>().clip = this.clips[clipID].clip;
		gameObject.GetComponent<AudioSource>().spatialBlend = (float)((!this.clips[clipID].is3D) ? 0 : 1);
		gameObject.GetComponent<AudioSource>().Play();
		UnityEngine.Object.Destroy(gameObject, 10f);
	}

	private void GenerateRandom()
	{
		List<AmbientSoundPlayer.AmbientClip> list = new List<AmbientSoundPlayer.AmbientClip>();
		foreach (AmbientSoundPlayer.AmbientClip ambientClip in this.clips)
		{
			if (!ambientClip.played)
			{
				list.Add(ambientClip);
			}
		}
		int index = UnityEngine.Random.Range(0, list.Count);
		int index2 = list[index].index;
		if (!this.clips[index2].repeatable)
		{
			this.clips[index2].played = true;
		}
		this.CallCmdPlaySound(index2);
		base.Invoke("GenerateRandom", (float)UnityEngine.Random.Range(this.minTime, this.maxTime));
	}

	[Command(channel = 1)]
	private void CmdPlaySound(int id)
	{
		if (base.isServer)
		{
			this.CallRpcPlaySound(id);
		}
	}

	[ClientRpc(channel = 1)]
	private void RpcPlaySound(int id)
	{
		this.PlaySound(id);
	}

	private void UNetVersion()
	{
	}

	protected static void InvokeCmdCmdPlaySound(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPlaySound called on client.");
			return;
		}
		((AmbientSoundPlayer)obj).CmdPlaySound((int)reader.ReadPackedUInt32());
	}

	public void CallCmdPlaySound(int id)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdPlaySound called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdPlaySound(id);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)AmbientSoundPlayer.kCmdCmdPlaySound);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.WritePackedUInt32((uint)id);
		base.SendCommandInternal(networkWriter, 1, "CmdPlaySound");
	}

	protected static void InvokeRpcRpcPlaySound(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcPlaySound called on server.");
			return;
		}
		((AmbientSoundPlayer)obj).RpcPlaySound((int)reader.ReadPackedUInt32());
	}

	public void CallRpcPlaySound(int id)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("RPC Function RpcPlaySound called on client.");
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)2));
		networkWriter.WritePackedUInt32((uint)AmbientSoundPlayer.kRpcRpcPlaySound);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.WritePackedUInt32((uint)id);
		this.SendRPCInternal(networkWriter, 1, "RpcPlaySound");
	}

	static AmbientSoundPlayer()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(AmbientSoundPlayer), AmbientSoundPlayer.kCmdCmdPlaySound, new NetworkBehaviour.CmdDelegate(AmbientSoundPlayer.InvokeCmdCmdPlaySound));
		AmbientSoundPlayer.kRpcRpcPlaySound = -1539903091;
		NetworkBehaviour.RegisterRpcDelegate(typeof(AmbientSoundPlayer), AmbientSoundPlayer.kRpcRpcPlaySound, new NetworkBehaviour.CmdDelegate(AmbientSoundPlayer.InvokeRpcRpcPlaySound));
		NetworkCRC.RegisterBehaviour("AmbientSoundPlayer", 0);
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		bool result;
		return result;
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
	}

	public GameObject audioPrefab;

	public int minTime = 30;

	public int maxTime = 60;

	public AmbientSoundPlayer.AmbientClip[] clips;

	private static int kCmdCmdPlaySound = 53028003;

	private static int kRpcRpcPlaySound;

	[Serializable]
	public class AmbientClip
	{
		public AmbientClip()
		{
		}

		public AudioClip clip;

		public bool repeatable = true;

		public bool is3D = true;

		public bool played;

		public int index;
	}
}
