using System;
using System.Runtime.CompilerServices;
using Dissonance.Networking;
using UnityEngine.Networking;

namespace Dissonance.Integrations.UNet_HLAPI
{
	public class HlapiClient : BaseClient<HlapiServer, HlapiClient, HlapiConn>
	{
		public HlapiClient([NotNull] HlapiCommsNetwork network) : base(network)
		{
			if (network == null)
			{
				throw new ArgumentNullException("network");
			}
			this._network = network;
			this._sendWriter = new NetworkWriter(new byte[1024]);
		}

		public override void Connect()
		{
			if (!this._network.Mode.IsServerEnabled())
			{
				NetworkManager.singleton.client.RegisterHandler(this._network.TypeCode, new NetworkMessageDelegate(this.OnMessageReceivedHandler));
			}
			base.Connected();
		}

		public override void Disconnect()
		{
			if (!this._network.Mode.IsServerEnabled() && NetworkManager.singleton.client != null)
			{
				NetworkClient client = NetworkManager.singleton.client;
				short typeCode = this._network.TypeCode;
				if (HlapiClient.<>f__mg$cache0 == null)
				{
					HlapiClient.<>f__mg$cache0 = new NetworkMessageDelegate(HlapiCommsNetwork.NullMessageReceivedHandler);
				}
				client.RegisterHandler(typeCode, HlapiClient.<>f__mg$cache0);
			}
			base.Disconnect();
		}

		private void OnMessageReceivedHandler([CanBeNull] NetworkMessage netMsg)
		{
			if (netMsg == null)
			{
				return;
			}
			base.NetworkReceivedPacket(this._network.CopyToArraySegment(netMsg.reader, new ArraySegment<byte>(this._receiveBuffer)));
		}

		protected override void ReadMessages()
		{
		}

		protected override void SendReliable(ArraySegment<byte> packet)
		{
			if (!this.Send(packet, this._network.ReliableSequencedChannel))
			{
				base.FatalError("Failed to send reliable packet (unknown HLAPI error)");
			}
		}

		protected override void SendUnreliable(ArraySegment<byte> packet)
		{
			this.Send(packet, this._network.UnreliableChannel);
		}

		private bool Send(ArraySegment<byte> packet, byte channel)
		{
			if (this._network.PreprocessPacketToServer(packet))
			{
				return true;
			}
			int numBytes = this._network.CopyPacketToNetworkWriter(packet, this._sendWriter);
			return NetworkManager.singleton.client.connection.SendBytes(this._sendWriter.AsArray(), numBytes, (int)channel);
		}

		private readonly HlapiCommsNetwork _network;

		private readonly NetworkWriter _sendWriter;

		private readonly byte[] _receiveBuffer = new byte[1024];

		[CompilerGenerated]
		private static NetworkMessageDelegate <>f__mg$cache0;
	}
}
