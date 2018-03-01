using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using GameConsole;
using UnityEngine;

public class ServerConsole : MonoBehaviour
{
	public ServerConsole()
	{
	}

	private static IEnumerator CheckLog()
	{
		for (;;)
		{
			string[] tasks = Directory.GetFiles("SCPSL_Data/Dedicated/" + ServerConsole.session, "cs*.mapi", SearchOption.TopDirectoryOnly);
			foreach (string task in tasks)
			{
				string t = task.Remove(0, task.IndexOf("cs"));
				string toLog = string.Empty;
				string exception = string.Empty;
				try
				{
					exception = "Error while reading the file: " + t;
					StreamReader streamReader = new StreamReader("SCPSL_Data/Dedicated/" + ServerConsole.session + "/" + t);
					string text = streamReader.ReadToEnd();
					if (text.Contains("terminator"))
					{
						text = text.Remove(text.LastIndexOf("terminator"));
					}
					toLog = ServerConsole.EnterCommand(text);
					try
					{
						exception = "Error while closing the file: " + t + " :: " + text;
					}
					catch
					{
					}
					streamReader.Close();
					try
					{
						exception = "Error while deleting the file: " + t + " :: " + text;
					}
					catch
					{
					}
					File.Delete("SCPSL_Data/Dedicated/" + ServerConsole.session + "/" + t);
				}
				catch
				{
				}
				if (!string.IsNullOrEmpty(toLog))
				{
					ServerConsole.AddLog(toLog);
				}
				yield return new WaitForSeconds(0.07f);
			}
			yield return new WaitForSeconds(1f);
			if (ServerConsole.consoleID == null || ServerConsole.consoleID.HasExited)
			{
				ServerConsole.TerminateProcess();
			}
		}
		yield break;
	}

	public static void AddLog(string q)
	{
		if (ServerStatic.isDedicated)
		{
			StreamWriter streamWriter = new StreamWriter(string.Concat(new object[]
			{
				"SCPSL_Data/Dedicated/",
				ServerConsole.session,
				"/sl",
				ServerConsole.logID,
				".mapi"
			}));
			ServerConsole.logID++;
			streamWriter.WriteLine(q);
			streamWriter.Close();
			MonoBehaviour.print(q);
		}
	}

	private static string EnterCommand(string cmds)
	{
		string result = "Command accepted.";
		string[] array = cmds.ToUpper().Split(new char[]
		{
			' '
		});
		if (array.Length > 0)
		{
			string a = array[0];
			if (a == "FORCESTART")
			{
				bool flag = false;
				GameObject gameObject = GameObject.Find("Host");
				if (gameObject != null)
				{
					CharacterClassManager component = gameObject.GetComponent<CharacterClassManager>();
					if (component != null && component.isLocalPlayer && component.isServer && !component.roundStarted)
					{
						component.ForceRoundStart();
						flag = true;
					}
				}
				result = ((!flag) ? "Failed to force start.LOGTYPE14" : "Forced round start.");
			}
			else if (a == "CONFIG")
			{
				if (File.Exists(ConfigFile.path))
				{
					Application.OpenURL(ConfigFile.path);
				}
				else
				{
					result = "Config file not found!";
				}
			}
			else
			{
				result = GameConsole.Console.singleton.TypeCommand(cmds);
			}
		}
		return result;
	}

	private void Start()
	{
		if (!ServerStatic.isDedicated)
		{
			return;
		}
		ServerConsole.logID = 0;
		base.StartCoroutine(ServerConsole.CheckLog());
	}

	public void RunServer()
	{
		base.StartCoroutine(this.RefreshSession());
	}

	private IEnumerator RefreshSession()
	{
		for (;;)
		{
			WWWForm form = new WWWForm();
			form.AddField("update", 0);
			form.AddField("ip", ServerConsole.ip);
			form.AddField("passcode", ServerConsole.password);
			int plys = 0;
			try
			{
				plys = GameObject.FindGameObjectsWithTag("Player").Length - 1;
			}
			catch
			{
			}
			form.AddField("players", plys);
			form.AddField("port", base.GetComponent<CustomNetworkManager>().networkPort);
			float timeBefore = Time.realtimeSinceStartup;
			WWW www = new WWW("https://hubertmoszka.pl/authenticator.php", form);
			yield return www;
			if (!string.IsNullOrEmpty(www.error) || !www.text.Contains("YES"))
			{
				ServerConsole.AddLog("Could not update the session - " + www.error + www.text + "LOGTYPE-8");
			}
			yield return new WaitForSeconds(5f - (Time.realtimeSinceStartup - timeBefore));
		}
		yield break;
	}

	private static void TerminateProcess()
	{
		ServerStatic.isDedicated = false;
		Application.Quit();
	}

	public static int logID;

	public static Process consoleID;

	public static string session;

	public static string password;

	public static string ip;

	[CompilerGenerated]
	private sealed class <CheckLog>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <CheckLog>c__Iterator0()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this.$PC;
			this.$PC = -1;
			switch (num)
			{
			case 0u:
				break;
			case 1u:
				this.$locvar1++;
				goto IL_20A;
			case 2u:
				if (ServerConsole.consoleID == null || ServerConsole.consoleID.HasExited)
				{
					ServerConsole.TerminateProcess();
				}
				break;
			default:
				return false;
			}
			this.<tasks>__1 = Directory.GetFiles("SCPSL_Data/Dedicated/" + ServerConsole.session, "cs*.mapi", SearchOption.TopDirectoryOnly);
			this.$locvar0 = this.<tasks>__1;
			this.$locvar1 = 0;
			IL_20A:
			if (this.$locvar1 >= this.$locvar0.Length)
			{
				this.$current = new WaitForSeconds(1f);
				if (!this.$disposing)
				{
					this.$PC = 2;
				}
			}
			else
			{
				this.<task>__2 = this.$locvar0[this.$locvar1];
				this.<t>__3 = this.<task>__2.Remove(0, this.<task>__2.IndexOf("cs"));
				this.<toLog>__3 = string.Empty;
				this.<exception>__3 = string.Empty;
				try
				{
					this.<exception>__3 = "Error while reading the file: " + this.<t>__3;
					StreamReader streamReader = new StreamReader("SCPSL_Data/Dedicated/" + ServerConsole.session + "/" + this.<t>__3);
					string text = streamReader.ReadToEnd();
					this.<exception>__3 = "Error while dedecting 'terminator end-of-message' signal.";
					if (text.Contains("terminator"))
					{
						text = text.Remove(text.LastIndexOf("terminator"));
					}
					this.<exception>__3 = "Error while sending message.";
					this.<toLog>__3 = ServerConsole.EnterCommand(text);
					try
					{
						this.<exception>__3 = "Error while closing the file: " + this.<t>__3 + " :: " + text;
					}
					catch
					{
						this.<exception>__3 = "Error while closing the file.";
					}
					streamReader.Close();
					try
					{
						this.<exception>__3 = "Error while deleting the file: " + this.<t>__3 + " :: " + text;
					}
					catch
					{
						this.<exception>__3 = "Error while deleting the file.";
					}
					File.Delete("SCPSL_Data/Dedicated/" + ServerConsole.session + "/" + this.<t>__3);
				}
				catch
				{
				}
				if (!string.IsNullOrEmpty(this.<toLog>__3))
				{
					ServerConsole.AddLog(this.<toLog>__3);
				}
				this.$current = new WaitForSeconds(0.07f);
				if (!this.$disposing)
				{
					this.$PC = 1;
				}
			}
			return true;
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

		internal string[] <tasks>__1;

		internal string[] $locvar0;

		internal int $locvar1;

		internal string <task>__2;

		internal string <t>__3;

		internal string <toLog>__3;

		internal string <exception>__3;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}

	[CompilerGenerated]
	private sealed class <RefreshSession>c__Iterator1 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <RefreshSession>c__Iterator1()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this.$PC;
			this.$PC = -1;
			switch (num)
			{
			case 0u:
				break;
			case 1u:
				if (!string.IsNullOrEmpty(this.<www>__1.error) || !this.<www>__1.text.Contains("YES"))
				{
					ServerConsole.AddLog("Could not update the session - " + this.<www>__1.error + this.<www>__1.text + "LOGTYPE-8");
				}
				this.$current = new WaitForSeconds(5f - (Time.realtimeSinceStartup - this.<timeBefore>__1));
				if (!this.$disposing)
				{
					this.$PC = 2;
				}
				return true;
			case 2u:
				break;
			default:
				return false;
			}
			this.<form>__1 = new WWWForm();
			this.<form>__1.AddField("update", 0);
			this.<form>__1.AddField("ip", ServerConsole.ip);
			this.<form>__1.AddField("passcode", ServerConsole.password);
			this.<plys>__1 = 0;
			try
			{
				this.<plys>__1 = GameObject.FindGameObjectsWithTag("Player").Length - 1;
			}
			catch
			{
			}
			this.<form>__1.AddField("players", this.<plys>__1);
			this.<form>__1.AddField("port", this.$this.GetComponent<CustomNetworkManager>().networkPort);
			this.<timeBefore>__1 = Time.realtimeSinceStartup;
			this.<www>__1 = new WWW("https://hubertmoszka.pl/authenticator.php", this.<form>__1);
			this.$current = this.<www>__1;
			if (!this.$disposing)
			{
				this.$PC = 1;
			}
			return true;
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

		internal WWWForm <form>__1;

		internal int <plys>__1;

		internal float <timeBefore>__1;

		internal WWW <www>__1;

		internal ServerConsole $this;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}
}
