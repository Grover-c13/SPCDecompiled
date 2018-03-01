using System;
using System.Runtime.InteropServices;
using UnityEngine.Networking;

namespace Unity
{
	[StructLayout(LayoutKind.Auto, CharSet = CharSet.Auto)]
	public class GeneratedNetworkCode
	{
		public GeneratedNetworkCode()
		{
		}

		public static void _ReadStructSyncListItemInfo_Inventory(NetworkReader reader, Inventory.SyncListItemInfo instance)
		{
			ushort num = reader.ReadUInt16();
			instance.Clear();
			for (ushort num2 = 0; num2 < num; num2 += 1)
			{
				instance.AddInternal(instance.DeserializeItem(reader));
			}
		}

		public static void _WriteStructSyncListItemInfo_Inventory(NetworkWriter writer, Inventory.SyncListItemInfo value)
		{
			ushort count = value.Count;
			writer.Write(count);
			for (ushort num = 0; num < count; num += 1)
			{
				value.SerializeItem(writer, value.GetItem((int)num));
			}
		}

		public static GrenadeManager.GrenadeSpawnInfo _ReadGrenadeSpawnInfo_GrenadeManager(NetworkReader reader)
		{
			return new GrenadeManager.GrenadeSpawnInfo
			{
				grenadeID = (int)reader.ReadPackedUInt32(),
				spawnPosition = reader.ReadVector3(),
				velocity = reader.ReadVector3(),
				timeToExplode = reader.ReadSingle(),
				throwerclass = (int)reader.ReadPackedUInt32()
			};
		}

		public static void _WriteGrenadeSpawnInfo_GrenadeManager(NetworkWriter writer, GrenadeManager.GrenadeSpawnInfo value)
		{
			writer.WritePackedUInt32((uint)value.grenadeID);
			writer.Write(value.spawnPosition);
			writer.Write(value.velocity);
			writer.Write(value.timeToExplode);
			writer.WritePackedUInt32((uint)value.throwerclass);
		}

		public static PlayerStats.HitInfo _ReadHitInfo_PlayerStats(NetworkReader reader)
		{
			return new PlayerStats.HitInfo
			{
				amount = reader.ReadSingle(),
				tool = reader.ReadString(),
				time = (int)reader.ReadPackedUInt32()
			};
		}

		public static void _WriteHitInfo_PlayerStats(NetworkWriter writer, PlayerStats.HitInfo value)
		{
			writer.Write(value.amount);
			writer.Write(value.tool);
			writer.WritePackedUInt32((uint)value.time);
		}

		public static void _WriteInfo_Ragdoll(NetworkWriter writer, Ragdoll.Info value)
		{
			writer.Write(value.ownerHLAPI_id);
			writer.Write(value.steamClientName);
			GeneratedNetworkCode._WriteHitInfo_PlayerStats(writer, value.deathCause);
			writer.WritePackedUInt32((uint)value.charclass);
		}

		public static Ragdoll.Info _ReadInfo_Ragdoll(NetworkReader reader)
		{
			return new Ragdoll.Info
			{
				ownerHLAPI_id = reader.ReadString(),
				steamClientName = reader.ReadString(),
				deathCause = GeneratedNetworkCode._ReadHitInfo_PlayerStats(reader),
				charclass = (int)reader.ReadPackedUInt32()
			};
		}

		public static RoundSummary.Summary _ReadSummary_RoundSummary(NetworkReader reader)
		{
			return new RoundSummary.Summary
			{
				classD_escaped = (int)reader.ReadPackedUInt32(),
				classD_start = (int)reader.ReadPackedUInt32(),
				scientists_escaped = (int)reader.ReadPackedUInt32(),
				scientists_start = (int)reader.ReadPackedUInt32(),
				scp_frags = (int)reader.ReadPackedUInt32(),
				scp_start = (int)reader.ReadPackedUInt32(),
				scp_alive = (int)reader.ReadPackedUInt32(),
				scp_nozombies = (int)reader.ReadPackedUInt32(),
				warheadDetonated = reader.ReadBoolean()
			};
		}

		public static void _WriteSummary_RoundSummary(NetworkWriter writer, RoundSummary.Summary value)
		{
			writer.WritePackedUInt32((uint)value.classD_escaped);
			writer.WritePackedUInt32((uint)value.classD_start);
			writer.WritePackedUInt32((uint)value.scientists_escaped);
			writer.WritePackedUInt32((uint)value.scientists_start);
			writer.WritePackedUInt32((uint)value.scp_frags);
			writer.WritePackedUInt32((uint)value.scp_start);
			writer.WritePackedUInt32((uint)value.scp_alive);
			writer.WritePackedUInt32((uint)value.scp_nozombies);
			writer.Write(value.warheadDetonated);
		}

		public static string[] _ReadArrayString_None(NetworkReader reader)
		{
			int num = (int)reader.ReadUInt16();
			if (num == 0)
			{
				return new string[0];
			}
			string[] array = new string[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = reader.ReadString();
			}
			return array;
		}

		public static void _WriteArrayString_None(NetworkWriter writer, string[] value)
		{
			if (value == null)
			{
				writer.Write(0);
				return;
			}
			ushort value2 = (ushort)value.Length;
			writer.Write(value2);
			ushort num = 0;
			while ((int)num < value.Length)
			{
				writer.Write(value[(int)num]);
				num += 1;
			}
		}

		public static QueryProcessor.PlayerInfo _ReadPlayerInfo_QueryProcessor(NetworkReader reader)
		{
			return new QueryProcessor.PlayerInfo
			{
				address = reader.ReadString(),
				instance = reader.ReadGameObject(),
				time = reader.ReadString()
			};
		}

		public static QueryProcessor.PlayerInfo[] _ReadArrayPlayerInfo_None(NetworkReader reader)
		{
			int num = (int)reader.ReadUInt16();
			if (num == 0)
			{
				return new QueryProcessor.PlayerInfo[0];
			}
			QueryProcessor.PlayerInfo[] array = new QueryProcessor.PlayerInfo[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = GeneratedNetworkCode._ReadPlayerInfo_QueryProcessor(reader);
			}
			return array;
		}

		public static void _WritePlayerInfo_QueryProcessor(NetworkWriter writer, QueryProcessor.PlayerInfo value)
		{
			writer.Write(value.address);
			writer.Write(value.instance);
			writer.Write(value.time);
		}

		public static void _WriteArrayPlayerInfo_None(NetworkWriter writer, QueryProcessor.PlayerInfo[] value)
		{
			if (value == null)
			{
				writer.Write(0);
				return;
			}
			ushort value2 = (ushort)value.Length;
			writer.Write(value2);
			ushort num = 0;
			while ((int)num < value.Length)
			{
				GeneratedNetworkCode._WritePlayerInfo_QueryProcessor(writer, value[(int)num]);
				num += 1;
			}
		}
	}
}
