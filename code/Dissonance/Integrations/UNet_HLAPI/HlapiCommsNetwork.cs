using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Dissonance.Datastructures;
using Dissonance.Extensions;
using Dissonance.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace Dissonance.Integrations.UNet_HLAPI
{
	public class HlapiCommsNetwork : BaseCommsNetwork<HlapiServer, HlapiClient, HlapiConn, Unit, Unit>
	{
		public HlapiCommsNetwork()
		{
		}

		protected override HlapiServer CreateServer(Unit details)
		{
			HlapiServer hlapiServer = new HlapiServer(this);
			HlapiServer._instance = hlapiServer;
			return hlapiServer;
		}

		protected override HlapiClient CreateClient(Unit details)
		{
			return new HlapiClient(this);
		}

		protected override void Update()
		{
			if (base.IsInitialized)
			{
				bool flag = NetworkManager.singleton.isNetworkActive && (NetworkServer.active || NetworkClient.active) && (!NetworkClient.active || (NetworkManager.singleton.client != null && NetworkManager.singleton.client.connection != null));
				if (flag)
				{
					bool active = NetworkServer.active;
					bool active2 = NetworkClient.active;
					if (base.Mode.IsServerEnabled() != active || base.Mode.IsClientEnabled() != active2)
					{
						if (active && active2)
						{
							base.RunAsHost(Unit.None, Unit.None);
						}
						else if (active)
						{
							base.RunAsDedicatedServer(Unit.None);
						}
						else if (active2)
						{
							base.RunAsClient(Unit.None);
						}
					}
				}
				else if (base.Mode != NetworkMode.None)
				{
					base.Stop();
				}
				for (int i = 0; i < this._loopbackQueue.Count; i++)
				{
					if (base.Client != null)
					{
						base.Client.NetworkReceivedPacket(this._loopbackQueue[i]);
					}
					this._loopbackBuffers.Put(this._loopbackQueue[i].Array);
				}
				this._loopbackQueue.Clear();
			}
			base.Update();
		}

		protected override void Initialize()
		{
			if ((int)this.UnreliableChannel >= NetworkManager.singleton.channels.Count)
			{
				throw this.Log.CreateUserErrorException("configured 'unreliable' channel is out of range", "set the wrong channel number in the HLAPI Comms Network component", "https://dissonance.readthedocs.io/en/latest/Basics/Quick-Start-UNet-HLAPI/", "B19B4916-8709-490B-8152-A646CCAD788E");
			}
			QosType qosType = NetworkManager.singleton.channels[(int)this.UnreliableChannel];
			if (qosType != QosType.Unreliable)
			{
				throw this.Log.CreateUserErrorException(string.Format("configured 'unreliable' channel has QoS type '{0}'", qosType), "not creating the channel with the correct QoS type", "https://dissonance.readthedocs.io/en/latest/Basics/Quick-Start-UNet-HLAPI/", "24ee53b1-7517-4672-8a4a-64a3e3c87ef6");
			}
			if ((int)this.ReliableSequencedChannel >= NetworkManager.singleton.channels.Count)
			{
				throw this.Log.CreateUserErrorException("configured 'reliable' channel is out of range", "set the wrong channel number in the HLAPI Comms Network component", "https://dissonance.readthedocs.io/en/latest/Basics/Quick-Start-UNet-HLAPI/", "5F5F2875-ECC8-433D-B0CB-97C151B8094D");
			}
			QosType qosType2 = NetworkManager.singleton.channels[(int)this.ReliableSequencedChannel];
			if (qosType2 != QosType.ReliableSequenced)
			{
				throw this.Log.CreateUserErrorException(string.Format("configured 'reliable sequenced' channel has QoS type '{0}'", qosType2), "not creating the channel with the correct QoS type", "https://dissonance.readthedocs.io/en/latest/Basics/Quick-Start-UNet-HLAPI/", "035773ec-aef3-477a-8eeb-c234d416171c");
			}
			short typeCode = this.TypeCode;
			if (HlapiCommsNetwork.<>f__mg$cache0 == null)
			{
				HlapiCommsNetwork.<>f__mg$cache0 = new NetworkMessageDelegate(HlapiCommsNetwork.NullMessageReceivedHandler);
			}
			NetworkServer.RegisterHandler(typeCode, HlapiCommsNetwork.<>f__mg$cache0);
			base.Initialize();
		}

		internal bool PreprocessPacketToClient(ArraySegment<byte> packet, HlapiConn destination)
		{
			if (base.Server == null)
			{
				throw this.Log.CreatePossibleBugException("server packet preprocessing running, but this peer is not a server", "8f9dc0a0-1b48-4a7f-9bb6-f767b2542ab1");
			}
			if (base.Client == null)
			{
				return false;
			}
			if (NetworkManager.singleton.client.connection != destination.Connection)
			{
				return false;
			}
			if (base.Client != null)
			{
				this._loopbackQueue.Add(packet.CopyTo(this._loopbackBuffers.Get(), 0));
			}
			return true;
		}

		internal bool PreprocessPacketToServer(ArraySegment<byte> packet)
		{
			if (base.Client == null)
			{
				throw this.Log.CreatePossibleBugException("client packet processing running, but this peer is not a client", "dd75dce4-e85c-4bb3-96ec-3a3636cc4fbe");
			}
			if (base.Server == null)
			{
				return false;
			}
			base.Server.NetworkReceivedPacket(new HlapiConn(NetworkManager.singleton.client.connection), packet);
			return true;
		}

		internal static void NullMessageReceivedHandler([NotNull] NetworkMessage netmsg)
		{
			if (netmsg == null)
			{
				throw new ArgumentNullException("netmsg");
			}
			if (Logs.GetLogLevel(LogCategory.Network) <= LogLevel.Trace)
			{
				Debug.Log("Discarding Dissonance network message");
			}
			int num = (int)netmsg.reader.ReadPackedUInt32();
			for (int i = 0; i < num; i++)
			{
				netmsg.reader.ReadByte();
			}
		}

		internal ArraySegment<byte> CopyToArraySegment([NotNull] NetworkReader msg, ArraySegment<byte> segment)
		{
			if (msg == null)
			{
				throw new ArgumentNullException("msg");
			}
			byte[] array = segment.Array;
			if (array == null)
			{
				throw new ArgumentNullException("segment");
			}
			int num = (int)msg.ReadPackedUInt32();
			if (num > segment.Count)
			{
				throw this.Log.CreatePossibleBugException("receive buffer is too small", "A7387195-BF3D-4796-A362-6C64BB546445");
			}
			for (int i = 0; i < num; i++)
			{
				array[segment.Offset + i] = msg.ReadByte();
			}
			return new ArraySegment<byte>(array, segment.Offset, num);
		}

		internal int CopyPacketToNetworkWriter(ArraySegment<byte> packet, [NotNull] NetworkWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			byte[] array = packet.Array;
			if (array == null)
			{
				throw new ArgumentNullException("packet");
			}
			writer.SeekZero();
			writer.StartMessage(this.TypeCode);
			writer.WritePackedUInt32((uint)packet.Count);
			for (int i = 0; i < packet.Count; i++)
			{
				writer.Write(array[packet.Offset + i]);
			}
			writer.FinishMessage();
			return (int)writer.Position;
		}

		[CompilerGenerated]
		private static byte[] <_loopbackBuffers>m__0()
		{
			return new byte[1024];
		}

		public byte UnreliableChannel = 1;

		public byte ReliableSequencedChannel;

		public short TypeCode = 18385;

		private readonly ConcurrentPool<byte[]> _loopbackBuffers = new ConcurrentPool<byte[]>(8, () => new byte[1024]);

		private readonly List<ArraySegment<byte>> _loopbackQueue = new List<ArraySegment<byte>>();

		[CompilerGenerated]
		private static NetworkMessageDelegate <>f__mg$cache0;

		[CompilerGenerated]
		private static Func<byte[]> <>f__am$cache0;
	}
}
