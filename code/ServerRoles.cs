using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

public class ServerRoles : NetworkBehaviour
{
	public ServerRoles()
	{
	}

	[Command(channel = 2)]
	public void CmdRequestRole(string code)
	{
		if (!this.requested)
		{
			base.StartCoroutine(this.RequestRoleFromServer(code));
			this.requested = true;
		}
	}

	private IEnumerator RequestRoleFromServer(string code)
	{
		WWWForm form = new WWWForm();
		form.AddField("passcode", code);
		if (!ServerStatic.isDedicated || code.Split(new char[]
		{
			':'
		})[0] == ServerConsole.ip)
		{
			WWW www = new WWW("https://hubertmoszka.pl/roleverificator.php", form);
			yield return www;
			int role = 0;
			if (string.IsNullOrEmpty(www.error) && int.TryParse(www.text, out role))
			{
				this.SetRole(role);
				base.GetComponent<QueryProcessor>().CallCmdCheckPassword("override");
			}
		}
		yield break;
	}

	public bool GetAccessToRemoteAdmin()
	{
		ServerRoles.Role role = this.roles[this.myRole];
		return role.remoteAdminAccess != ServerRoles.RemoteAdminAccess.Never && (role.remoteAdminAccess == ServerRoles.RemoteAdminAccess.Allways || (role.remoteAdminAccess == ServerRoles.RemoteAdminAccess.ConfigOnly && ConfigFile.GetString("allow_scpsl_staff_to_use_remoteadmin", "true") == "true"));
	}

	public string GetColoredRoleString()
	{
		if (this.myRole == 0)
		{
			return string.Empty;
		}
		ServerRoles.Role role = this.roles[this.myRole];
		return string.Concat(new string[]
		{
			"<color=#",
			role.colorHex,
			">",
			role.name,
			"</color>"
		});
	}

	public Gradient[] GetGradient()
	{
		return new Gradient[]
		{
			this.roles[this.myRole].speakingColor_in,
			this.roles[this.myRole].speakingColor_out
		};
	}

	public void SetRole(int i)
	{
		this.NetworkmyRole = i;
	}

	private void UNetVersion()
	{
	}

	public int NetworkmyRole
	{
		get
		{
			return this.myRole;
		}
		set
		{
			uint dirtyBit = 1u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SetRole(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<int>(value, ref this.myRole, dirtyBit);
		}
	}

	protected static void InvokeCmdCmdRequestRole(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogError("Command CmdRequestRole called on client.");
			return;
		}
		((ServerRoles)obj).CmdRequestRole(reader.ReadString());
	}

	public void CallCmdRequestRole(string code)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("Command function CmdRequestRole called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdRequestRole(code);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)ServerRoles.kCmdCmdRequestRole);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(code);
		base.SendCommandInternal(networkWriter, 2, "CmdRequestRole");
	}

	static ServerRoles()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(ServerRoles), ServerRoles.kCmdCmdRequestRole, new NetworkBehaviour.CmdDelegate(ServerRoles.InvokeCmdCmdRequestRole));
		NetworkCRC.RegisterBehaviour("ServerRoles", 0);
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			writer.WritePackedUInt32((uint)this.myRole);
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
			writer.WritePackedUInt32((uint)this.myRole);
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
			this.myRole = (int)reader.ReadPackedUInt32();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			this.SetRole((int)reader.ReadPackedUInt32());
		}
	}

	private bool requested;

	public ServerRoles.Role[] roles;

	[SyncVar(hook = "SetRole")]
	public int myRole;

	private static int kCmdCmdRequestRole = -1616353557;

	[Serializable]
	public class Role
	{
		public Role()
		{
		}

		public string name;

		public Gradient speakingColor_in;

		public Gradient speakingColor_out;

		public string colorHex;

		public ServerRoles.RemoteAdminAccess remoteAdminAccess;
	}

	public enum RemoteAdminAccess
	{
		Allways,
		ConfigOnly,
		Never
	}

	[CompilerGenerated]
	private sealed class <RequestRoleFromServer>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <RequestRoleFromServer>c__Iterator0()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this.$PC;
			this.$PC = -1;
			switch (num)
			{
			case 0u:
				this.<form>__0 = new WWWForm();
				this.<form>__0.AddField("passcode", this.code);
				if (!ServerStatic.isDedicated || this.code.Split(new char[]
				{
					':'
				})[0] == ServerConsole.ip)
				{
					this.<www>__1 = new WWW("https://hubertmoszka.pl/roleverificator.php", this.<form>__0);
					this.$current = this.<www>__1;
					if (!this.$disposing)
					{
						this.$PC = 1;
					}
					return true;
				}
				break;
			case 1u:
				this.<role>__1 = 0;
				if (string.IsNullOrEmpty(this.<www>__1.error) && int.TryParse(this.<www>__1.text, out this.<role>__1))
				{
					this.$this.SetRole(this.<role>__1);
					this.$this.GetComponent<QueryProcessor>().CallCmdCheckPassword("override");
				}
				break;
			default:
				return false;
			}
			this.$PC = -1;
			return false;
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

		internal WWWForm <form>__0;

		internal string code;

		internal WWW <www>__1;

		internal int <role>__1;

		internal ServerRoles $this;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}
}
