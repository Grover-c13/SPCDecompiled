﻿using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Dissonance.Integrations.UNet_HLAPI
{
	[RequireComponent(typeof(NetworkIdentity))]
	public class HlapiPlayer : NetworkBehaviour, IDissonancePlayer
	{
		public bool IsTracking { get; private set; }

		public string PlayerId
		{
			get
			{
				return this._playerId;
			}
		}

		public Vector3 Position
		{
			get
			{
				return base.transform.position;
			}
		}

		public Quaternion Rotation
		{
			get
			{
				return base.transform.rotation;
			}
		}

		public NetworkPlayerType Type
		{
			get
			{
				return (!base.isLocalPlayer) ? NetworkPlayerType.Remote : NetworkPlayerType.Local;
			}
		}

		public string Network_playerId
		{
			get
			{
				return this._playerId;
			}
			set
			{
				base.SetSyncVar<string>(value, ref this._playerId, 1u);
			}
		}

		public HlapiPlayer()
		{
		}

		public void OnDestroy()
		{
			if (this._comms != null)
			{
				this._comms.LocalPlayerNameChanged -= this.SetPlayerName;
			}
		}

		public void OnEnable()
		{
			this._comms = UnityEngine.Object.FindObjectOfType<DissonanceComms>();
		}

		public void OnDisable()
		{
			if (this.IsTracking)
			{
				this.StopTracking();
			}
		}

		public override void OnStartAuthority()
		{
			base.OnStartAuthority();
			DissonanceComms dissonanceComms = UnityEngine.Object.FindObjectOfType<DissonanceComms>();
			if (dissonanceComms == null)
			{
				throw HlapiPlayer.Log.CreateUserErrorException("cannot find DissonanceComms component in scene", "not placing a DissonanceComms component on a game object in the scene", "https://dissonance.readthedocs.io/en/latest/Basics/Quick-Start-UNet-HLAPI/", "9A79FDCB-263E-4124-B54D-67EDA39C09A5");
			}
			if (dissonanceComms.LocalPlayerName != null)
			{
				this.SetPlayerName(dissonanceComms.LocalPlayerName);
			}
			dissonanceComms.LocalPlayerNameChanged += this.SetPlayerName;
		}

		private void SetPlayerName(string playerName)
		{
			if (this.IsTracking)
			{
				this.StopTracking();
			}
			this.Network_playerId = playerName;
			this.StartTracking();
			if (base.hasAuthority)
			{
				this.CallCmdSetPlayerName(playerName);
			}
		}

		public override void OnStartClient()
		{
			base.OnStartClient();
			if (!string.IsNullOrEmpty(this.PlayerId))
			{
				this.StartTracking();
			}
		}

		[Command]
		private void CmdSetPlayerName(string playerName)
		{
			this.Network_playerId = playerName;
			this.CallRpcSetPlayerName(playerName);
		}

		[ClientRpc]
		private void RpcSetPlayerName(string playerName)
		{
			if (!base.hasAuthority)
			{
				this.SetPlayerName(playerName);
			}
		}

		private void StartTracking()
		{
			if (this.IsTracking)
			{
				throw HlapiPlayer.Log.CreatePossibleBugException("Attempting to start player tracking, but tracking is already started", "B7D1F25E-72AF-4E93-8CFF-90CEBEAC68CF");
			}
			if (this._comms != null)
			{
				this._comms.TrackPlayerPosition(this);
				this.IsTracking = true;
			}
		}

		private void StopTracking()
		{
			if (!this.IsTracking)
			{
				throw HlapiPlayer.Log.CreatePossibleBugException("Attempting to stop player tracking, but tracking is not started", "EC5C395D-B544-49DC-B33C-7D7533349134");
			}
			if (this._comms != null)
			{
				this._comms.StopTracking(this);
				this.IsTracking = false;
			}
		}

		static HlapiPlayer()
		{
			NetworkBehaviour.RegisterCommandDelegate(typeof(HlapiPlayer), HlapiPlayer.kCmdCmdSetPlayerName, new NetworkBehaviour.CmdDelegate(HlapiPlayer.InvokeCmdCmdSetPlayerName));
			HlapiPlayer.kRpcRpcSetPlayerName = 1332331777;
			NetworkBehaviour.RegisterRpcDelegate(typeof(HlapiPlayer), HlapiPlayer.kRpcRpcSetPlayerName, new NetworkBehaviour.CmdDelegate(HlapiPlayer.InvokeRpcRpcSetPlayerName));
			NetworkCRC.RegisterBehaviour("HlapiPlayer", 0);
		}

		private void UNetVersion()
		{
		}

		protected static void InvokeCmdCmdSetPlayerName(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdSetPlayerName called on client.");
				return;
			}
			((HlapiPlayer)obj).CmdSetPlayerName(reader.ReadString());
		}

		public void CallCmdSetPlayerName(string playerName)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("Command function CmdSetPlayerName called on server.");
				return;
			}
			if (base.isServer)
			{
				this.CmdSetPlayerName(playerName);
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)5));
			networkWriter.WritePackedUInt32((uint)HlapiPlayer.kCmdCmdSetPlayerName);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.Write(playerName);
			base.SendCommandInternal(networkWriter, 0, "CmdSetPlayerName");
		}

		protected static void InvokeRpcRpcSetPlayerName(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcSetPlayerName called on server.");
				return;
			}
			((HlapiPlayer)obj).RpcSetPlayerName(reader.ReadString());
		}

		public void CallRpcSetPlayerName(string playerName)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcSetPlayerName called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)HlapiPlayer.kRpcRpcSetPlayerName);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.Write(playerName);
			this.SendRPCInternal(networkWriter, 0, "RpcSetPlayerName");
		}

		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this._playerId);
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
				writer.Write(this._playerId);
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
				this._playerId = reader.ReadString();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this._playerId = reader.ReadString();
			}
		}

		private static readonly Log Log = Logs.Create(LogCategory.Network, "HLAPI Player Component");

		private DissonanceComms _comms;

		[SyncVar]
		private string _playerId;

		private static int kCmdCmdSetPlayerName = -1254064873;

		private static int kRpcRpcSetPlayerName;
	}
}
