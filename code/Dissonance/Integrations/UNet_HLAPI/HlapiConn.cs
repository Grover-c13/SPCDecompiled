using System;
using UnityEngine.Networking;

namespace Dissonance.Integrations.UNet_HLAPI
{
	public struct HlapiConn : IEquatable<HlapiConn>
	{
		public HlapiConn(NetworkConnection connection)
		{
			this = default(HlapiConn);
			this.Connection = connection;
		}

		public override int GetHashCode()
		{
			return this.Connection.GetHashCode();
		}

		public override string ToString()
		{
			return this.Connection.ToString();
		}

		public override bool Equals(object obj)
		{
			return !object.ReferenceEquals(null, obj) && obj is HlapiConn && this.Equals((HlapiConn)obj);
		}

		public bool Equals(HlapiConn other)
		{
			if (this.Connection == null)
			{
				return other.Connection == null;
			}
			return this.Connection.Equals(other.Connection);
		}

		public readonly NetworkConnection Connection;
	}
}
