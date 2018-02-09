using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Dissonance.Networking;
using Dissonance.Networking.Server;
using UnityEngine.Networking;

namespace Dissonance.Integrations.UNet_HLAPI
{
	public class HlapiServer : BaseServer<HlapiServer, HlapiClient, HlapiConn>
	{
		public HlapiServer([NotNull] HlapiCommsNetwork network)
		{
			if (network == null)
			{
				throw new ArgumentNullException("network");
			}
			this._network = network;
		}

		public override void Connect()
		{
			NetworkServer.RegisterHandler(this._network.TypeCode, new NetworkMessageDelegate(this.OnMessageReceivedHandler));
			base.Connect();
		}

		private void OnMessageReceivedHandler([NotNull] NetworkMessage netmsg)
		{
			base.NetworkReceivedPacket(new HlapiConn(netmsg.conn), this._network.CopyToArraySegment(netmsg.reader, new ArraySegment<byte>(this._receiveBuffer)));
		}

		protected override void AddClient([NotNull] ClientInfo<HlapiConn> client)
		{
			base.AddClient(client);
			if (client.PlayerName != this._network.PlayerName)
			{
				this._addedConnections.Add(client.Connection.Connection);
			}
		}

		public override void Disconnect()
		{
			base.Disconnect();
			short typeCode = this._network.TypeCode;
			if (HlapiServer.<>f__mg$cache0 == null)
			{
				HlapiServer.<>f__mg$cache0 = new NetworkMessageDelegate(HlapiCommsNetwork.NullMessageReceivedHandler);
			}
			NetworkServer.RegisterHandler(typeCode, HlapiServer.<>f__mg$cache0);
		}

		protected override void ReadMessages()
		{
		}

		public static void OnServerDisconnect(NetworkConnection connection)
		{
			if (HlapiServer._instance != null)
			{
				HlapiServer._instance.OnServerDisconnect(new HlapiConn(connection));
			}
		}

		private void OnServerDisconnect(HlapiConn conn)
		{
			int num = this._addedConnections.IndexOf(conn.Connection);
			if (num >= 0)
			{
				this._addedConnections.RemoveAt(num);
				base.ClientDisconnected(conn);
			}
		}

		public override ServerState Update()
		{
			for (int i = this._addedConnections.Count - 1; i >= 0; i--)
			{
				NetworkConnection networkConnection = this._addedConnections[i];
				if (!networkConnection.isConnected || networkConnection.lastError == NetworkError.Timeout || !NetworkServer.connections.Contains(this._addedConnections[i]))
				{
					base.ClientDisconnected(new HlapiConn(this._addedConnections[i]));
					this._addedConnections.RemoveAt(i);
				}
			}
			return base.Update();
		}

		protected override void SendReliable(HlapiConn connection, ArraySegment<byte> packet)
		{
			if (!this.Send(packet, connection, this._network.ReliableSequencedChannel))
			{
				base.FatalError("Failed to send reliable packet (unknown HLAPI error)");
			}
		}

		protected override void SendUnreliable(HlapiConn connection, ArraySegment<byte> packet)
		{
			this.Send(packet, connection, this._network.UnreliableChannel);
		}

		private bool Send(ArraySegment<byte> packet, HlapiConn connection, byte channel)
		{
			if (this._network.PreprocessPacketToClient(packet, connection))
			{
				return true;
			}
			if (!connection.Connection.isConnected || connection.Connection.lastError == NetworkError.Timeout)
			{
				return true;
			}
			int numBytes = this._network.CopyPacketToNetworkWriter(packet, this._sendWriter);
			if (connection.Connection == null)
			{
				this.Log.Error("Cannot send to a null destination");
				return false;
			}
			return connection.Connection.SendBytes(this._sendWriter.AsArray(), numBytes, (int)channel);
		}

		[NotNull]
		private readonly HlapiCommsNetwork _network;

		private readonly NetworkWriter _sendWriter = new NetworkWriter(new byte[1024]);

		private readonly byte[] _receiveBuffer = new byte[1024];

		private readonly List<NetworkConnection> _addedConnections = new List<NetworkConnection>();

		public static HlapiServer _instance;

		[CompilerGenerated]
		private static NetworkMessageDelegate <>f__mg$cache0;
	}
}
