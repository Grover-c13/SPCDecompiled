using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

public class StaffLogin : MonoBehaviour
{
	public StaffLogin()
	{
	}

	private void Awake()
	{
		StaffLogin.singleton = this;
	}

	public void UpdateServer()
	{
		base.StartCoroutine(this.IUpdateServer());
	}

	private IEnumerator IUpdateServer()
	{
		yield return new WaitForEndOfFrame();
		CustomNetworkManager cnm = base.GetComponent<CustomNetworkManager>();
		WWWForm form = new WWWForm();
		string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/SCP Secret Laboratory/staff_login.txt";
		string nickname = string.Empty;
		string password = string.Empty;
		if (File.Exists(path))
		{
			StreamReader streamReader = new StreamReader(path);
			string text = streamReader.ReadToEnd();
			streamReader.Close();
			nickname = text.Remove(0, text.IndexOf("[") + 1);
			nickname = nickname.Remove(nickname.IndexOf("]"));
			password = text.Remove(0, text.LastIndexOf("[") + 1);
			password = password.Remove(password.IndexOf("]"));
		}
		if (!string.IsNullOrEmpty(nickname) && !string.IsNullOrEmpty(password))
		{
			string code = this.GetRandomString();
			string server_connected = string.Concat(new object[]
			{
				cnm.networkAddress,
				":",
				cnm.networkPort,
				":",
				code
			});
			form.AddField("nickname", nickname);
			form.AddField("password", password);
			form.AddField("server_connected", server_connected);
			WWW www = new WWW("https://hubertmoszka.pl/staffsite.php", form);
			yield return www;
			while (PlayerManager.localPlayer == null)
			{
				yield return new WaitForEndOfFrame();
			}
			PlayerManager.localPlayer.GetComponent<ServerRoles>().CallCmdRequestRole(server_connected);
		}
		yield break;
	}

	public string GetRandomString()
	{
		string text = string.Empty;
		for (int i = 0; i < 16; i++)
		{
			text += "qweSDFrty&*_uiopasdfghjkl#$%^zxcvbnmQWE[{]}RTYUI|~`OPAGHJ()KLZXCV'></BNM;1234@-=567890!+"[UnityEngine.Random.Range(0, 88)];
		}
		return text;
	}

	public static StaffLogin singleton;

	[CompilerGenerated]
	private sealed class <IUpdateServer>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <IUpdateServer>c__Iterator0()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this.$PC;
			this.$PC = -1;
			switch (num)
			{
			case 0u:
				this.$current = new WaitForEndOfFrame();
				if (!this.$disposing)
				{
					this.$PC = 1;
				}
				return true;
			case 1u:
				this.<cnm>__0 = this.$this.GetComponent<CustomNetworkManager>();
				this.<form>__0 = new WWWForm();
				this.<path>__0 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/SCP Secret Laboratory/staff_login.txt";
				this.<nickname>__0 = string.Empty;
				this.<password>__0 = string.Empty;
				if (File.Exists(this.<path>__0))
				{
					StreamReader streamReader = new StreamReader(this.<path>__0);
					string text = streamReader.ReadToEnd();
					streamReader.Close();
					this.<nickname>__0 = text.Remove(0, text.IndexOf("[") + 1);
					this.<nickname>__0 = this.<nickname>__0.Remove(this.<nickname>__0.IndexOf("]"));
					this.<password>__0 = text.Remove(0, text.LastIndexOf("[") + 1);
					this.<password>__0 = this.<password>__0.Remove(this.<password>__0.IndexOf("]"));
				}
				if (!string.IsNullOrEmpty(this.<nickname>__0) && !string.IsNullOrEmpty(this.<password>__0))
				{
					this.<code>__1 = this.$this.GetRandomString();
					this.<server_connected>__1 = string.Concat(new object[]
					{
						this.<cnm>__0.networkAddress,
						":",
						this.<cnm>__0.networkPort,
						":",
						this.<code>__1
					});
					this.<form>__0.AddField("nickname", this.<nickname>__0);
					this.<form>__0.AddField("password", this.<password>__0);
					this.<form>__0.AddField("server_connected", this.<server_connected>__1);
					this.<www>__1 = new WWW("https://hubertmoszka.pl/staffsite.php", this.<form>__0);
					this.$current = this.<www>__1;
					if (!this.$disposing)
					{
						this.$PC = 2;
					}
					return true;
				}
				goto IL_26D;
			case 2u:
				break;
			case 3u:
				break;
			default:
				return false;
			}
			if (PlayerManager.localPlayer == null)
			{
				this.$current = new WaitForEndOfFrame();
				if (!this.$disposing)
				{
					this.$PC = 3;
				}
				return true;
			}
			PlayerManager.localPlayer.GetComponent<ServerRoles>().CallCmdRequestRole(this.<server_connected>__1);
			IL_26D:
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

		internal CustomNetworkManager <cnm>__0;

		internal WWWForm <form>__0;

		internal string <path>__0;

		internal string <nickname>__0;

		internal string <password>__0;

		internal string <code>__1;

		internal string <server_connected>__1;

		internal WWW <www>__1;

		internal StaffLogin $this;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}
}
