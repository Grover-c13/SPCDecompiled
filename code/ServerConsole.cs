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
		if (PlayerPrefs.GetString("server_accepted", "false") != "true")
		{
			StreamWriter sw = new StreamWriter(string.Concat(new object[]
			{
				"SCPSL_Data/Dedicated/",
				ServerConsole.session,
				"/sl",
				ServerConsole.logID,
				".mapi"
			}));
			ServerConsole.logID++;
			sw.WriteLine("THE PROJECT OWNERS  (HUBERT MOSZKA & WESLEY VAN VELSEN) RESERVE THE RIGHT TO OVERRIDE ACCESS TO THE REMOTE ADMIN PANEL LOGTYPE-12");
			sw.Close();
			sw = new StreamWriter(string.Concat(new object[]
			{
				"SCPSL_Data/Dedicated/",
				ServerConsole.session,
				"/sl",
				ServerConsole.logID,
				".mapi"
			}));
			ServerConsole.logID++;
			sw.WriteLine("TYPE 'Yes, I accept' if you agree LOGTYPE-12");
			sw.Close();
			ServerConsole.accepted = false;
			while (!ServerConsole.accepted)
			{
				string[] tasks = Directory.GetFiles("SCPSL_Data/Dedicated/" + ServerConsole.session, "cs*.mapi", SearchOption.TopDirectoryOnly);
				foreach (string task in tasks)
				{
					string t = task.Remove(0, task.IndexOf("cs"));
					StreamReader sr = new StreamReader("SCPSL_Data/Dedicated/" + ServerConsole.session + "/" + t);
					string content = sr.ReadToEnd();
					sr.Close();
					File.Delete("SCPSL_Data/Dedicated/" + ServerConsole.session + "/" + t);
					if (content.ToUpper().Contains("YES, I ACCEPT"))
					{
						PlayerPrefs.SetString("server_accepted", "true");
						sw = new StreamWriter(string.Concat(new object[]
						{
							"SCPSL_Data/Dedicated/",
							ServerConsole.session,
							"/sl",
							ServerConsole.logID,
							".mapi"
						}));
						ServerConsole.logID++;
						sw.WriteLine("Cool! Let me just restart the session... LOGTYPE-10");
						sw.Close();
						yield return new WaitForSeconds(5f);
						ServerConsole.TerminateProcess();
					}
					else
					{
						ServerConsole.TerminateProcess();
					}
				}
				yield return new WaitForSeconds(0.07f);
			}
			yield return new WaitForSeconds(1f);
			if (ServerConsole.consoleID == null || ServerConsole.consoleID.HasExited)
			{
				ServerConsole.TerminateProcess();
			}
		}
		yield return new WaitForSeconds(10f);
		for (;;)
		{
			string[] tasks2 = Directory.GetFiles("SCPSL_Data/Dedicated/" + ServerConsole.session, "cs*.mapi", SearchOption.TopDirectoryOnly);
			foreach (string task2 in tasks2)
			{
				string t2 = task2.Remove(0, task2.IndexOf("cs"));
				string toLog = string.Empty;
				string exception = string.Empty;
				try
				{
					exception = "Error while reading the file: " + t2;
					StreamReader streamReader = new StreamReader("SCPSL_Data/Dedicated/" + ServerConsole.session + "/" + t2);
					string text = streamReader.ReadToEnd();
					if (text.Contains("terminator"))
					{
						text = text.Remove(text.LastIndexOf("terminator"));
					}
					toLog = ServerConsole.EnterCommand(text);
					try
					{
						exception = "Error while closing the file: " + t2 + " :: " + text;
					}
					catch
					{
					}
					streamReader.Close();
					try
					{
						exception = "Error while deleting the file: " + t2 + " :: " + text;
					}
					catch
					{
					}
					File.Delete("SCPSL_Data/Dedicated/" + ServerConsole.session + "/" + t2);
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
			ServerConsole.prompterQueue.Add(q);
		}
	}

	public IEnumerator Prompt()
	{
		for (;;)
		{
			while (!ServerConsole.accepted)
			{
				yield return new WaitForEndOfFrame();
			}
			if (ServerConsole.prompterQueue.Count > 0)
			{
				string text = ServerConsole.prompterQueue[0];
				ServerConsole.prompterQueue.RemoveAt(0);
				StreamWriter streamWriter = new StreamWriter(string.Concat(new object[]
				{
					"SCPSL_Data/Dedicated/",
					ServerConsole.session,
					"/sl",
					ServerConsole.logID,
					".mapi"
				}));
				ServerConsole.logID++;
				streamWriter.WriteLine(text);
				streamWriter.Close();
				MonoBehaviour.print(text);
			}
			yield return new WaitForEndOfFrame();
		}
		yield break;
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

	private void Awake()
	{
		ServerConsole.singleton = this;
	}

	private void Start()
	{
		if (!ServerStatic.isDedicated)
		{
			return;
		}
		ServerConsole.logID = 0;
		ServerConsole.accepted = true;
		base.StartCoroutine(this.Prompt());
		base.StartCoroutine(ServerConsole.CheckLog());
	}

	public void RunServer()
	{
		base.StartCoroutine(this.RefreshSession());
	}

	private IEnumerator RefreshSession()
	{
		CustomNetworkManager cnm = base.GetComponent<CustomNetworkManager>();
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
			form.AddField("players", plys + "/" + cnm.maxConnections);
			form.AddField("port", cnm.networkPort);
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

	static ServerConsole()
	{
		// Note: this type is marked as 'beforefieldinit'.
	}

	public static int logID;

	public static Process consoleID;

	public static string session;

	public static string password;

	public static string ip;

	private static bool accepted = true;

	private static List<string> prompterQueue = new List<string>();

	public static ServerConsole singleton;

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
				if (PlayerPrefs.GetString("server_accepted", "false") != "true")
				{
					this.<sw>__1 = new StreamWriter(string.Concat(new object[]
					{
						"SCPSL_Data/Dedicated/",
						ServerConsole.session,
						"/sl",
						ServerConsole.logID,
						".mapi"
					}));
					ServerConsole.logID++;
					this.<sw>__1.WriteLine("THE PROJECT OWNERS  (HUBERT MOSZKA & WESLEY VAN VELSEN) RESERVE THE RIGHT TO OVERRIDE ACCESS TO THE REMOTE ADMIN PANEL LOGTYPE-12");
					this.<sw>__1.Close();
					this.<sw>__1 = new StreamWriter(string.Concat(new object[]
					{
						"SCPSL_Data/Dedicated/",
						ServerConsole.session,
						"/sl",
						ServerConsole.logID,
						".mapi"
					}));
					ServerConsole.logID++;
					this.<sw>__1.WriteLine("TYPE 'Yes, I accept' if you agree LOGTYPE-12");
					this.<sw>__1.Close();
					ServerConsole.accepted = false;
					goto IL_30A;
				}
				goto IL_356;
			case 1u:
				ServerConsole.TerminateProcess();
				break;
			case 2u:
				goto IL_30A;
			case 3u:
				if (ServerConsole.consoleID == null || ServerConsole.consoleID.HasExited)
				{
					ServerConsole.TerminateProcess();
					goto IL_356;
				}
				goto IL_356;
			case 4u:
				goto IL_37A;
			case 5u:
				this.$locvar3++;
				goto IL_55F;
			case 6u:
				if (ServerConsole.consoleID == null || ServerConsole.consoleID.HasExited)
				{
					ServerConsole.TerminateProcess();
				}
				goto IL_37A;
			default:
				return false;
			}
			IL_2C5:
			this.$locvar1++;
			IL_2D3:
			if (this.$locvar1 >= this.$locvar0.Length)
			{
				this.$current = new WaitForSeconds(0.07f);
				if (!this.$disposing)
				{
					this.$PC = 2;
				}
				return true;
			}
			this.<task>__3 = this.$locvar0[this.$locvar1];
			this.<t>__4 = this.<task>__3.Remove(0, this.<task>__3.IndexOf("cs"));
			this.<sr>__4 = new StreamReader("SCPSL_Data/Dedicated/" + ServerConsole.session + "/" + this.<t>__4);
			this.<content>__4 = this.<sr>__4.ReadToEnd();
			this.<sr>__4.Close();
			File.Delete("SCPSL_Data/Dedicated/" + ServerConsole.session + "/" + this.<t>__4);
			if (this.<content>__4.ToUpper().Contains("YES, I ACCEPT"))
			{
				PlayerPrefs.SetString("server_accepted", "true");
				this.<sw>__1 = new StreamWriter(string.Concat(new object[]
				{
					"SCPSL_Data/Dedicated/",
					ServerConsole.session,
					"/sl",
					ServerConsole.logID,
					".mapi"
				}));
				ServerConsole.logID++;
				this.<sw>__1.WriteLine("Cool! Let me just restart the session... LOGTYPE-10");
				this.<sw>__1.Close();
				this.$current = new WaitForSeconds(5f);
				if (!this.$disposing)
				{
					this.$PC = 1;
				}
				return true;
			}
			ServerConsole.TerminateProcess();
			goto IL_2C5;
			IL_30A:
			if (ServerConsole.accepted)
			{
				this.$current = new WaitForSeconds(1f);
				if (!this.$disposing)
				{
					this.$PC = 3;
				}
				return true;
			}
			this.<tasks>__2 = Directory.GetFiles("SCPSL_Data/Dedicated/" + ServerConsole.session, "cs*.mapi", SearchOption.TopDirectoryOnly);
			this.$locvar0 = this.<tasks>__2;
			this.$locvar1 = 0;
			goto IL_2D3;
			IL_356:
			this.$current = new WaitForSeconds(10f);
			if (!this.$disposing)
			{
				this.$PC = 4;
			}
			return true;
			IL_37A:
			this.<tasks>__5 = Directory.GetFiles("SCPSL_Data/Dedicated/" + ServerConsole.session, "cs*.mapi", SearchOption.TopDirectoryOnly);
			this.$locvar2 = this.<tasks>__5;
			this.$locvar3 = 0;
			IL_55F:
			if (this.$locvar3 >= this.$locvar2.Length)
			{
				this.$current = new WaitForSeconds(1f);
				if (!this.$disposing)
				{
					this.$PC = 6;
				}
			}
			else
			{
				this.<task>__6 = this.$locvar2[this.$locvar3];
				this.<t>__7 = this.<task>__6.Remove(0, this.<task>__6.IndexOf("cs"));
				this.<toLog>__7 = string.Empty;
				this.<exception>__7 = string.Empty;
				try
				{
					this.<exception>__7 = "Error while reading the file: " + this.<t>__7;
					StreamReader streamReader = new StreamReader("SCPSL_Data/Dedicated/" + ServerConsole.session + "/" + this.<t>__7);
					string text = streamReader.ReadToEnd();
					this.<exception>__7 = "Error while dedecting 'terminator end-of-message' signal.";
					if (text.Contains("terminator"))
					{
						text = text.Remove(text.LastIndexOf("terminator"));
					}
					this.<exception>__7 = "Error while sending message.";
					this.<toLog>__7 = ServerConsole.EnterCommand(text);
					try
					{
						this.<exception>__7 = "Error while closing the file: " + this.<t>__7 + " :: " + text;
					}
					catch
					{
						this.<exception>__7 = "Error while closing the file.";
					}
					streamReader.Close();
					try
					{
						this.<exception>__7 = "Error while deleting the file: " + this.<t>__7 + " :: " + text;
					}
					catch
					{
						this.<exception>__7 = "Error while deleting the file.";
					}
					File.Delete("SCPSL_Data/Dedicated/" + ServerConsole.session + "/" + this.<t>__7);
				}
				catch
				{
				}
				if (!string.IsNullOrEmpty(this.<toLog>__7))
				{
					ServerConsole.AddLog(this.<toLog>__7);
				}
				this.$current = new WaitForSeconds(0.07f);
				if (!this.$disposing)
				{
					this.$PC = 5;
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

		internal StreamWriter <sw>__1;

		internal string[] <tasks>__2;

		internal string[] $locvar0;

		internal int $locvar1;

		internal string <task>__3;

		internal string <t>__4;

		internal StreamReader <sr>__4;

		internal string <content>__4;

		internal string[] <tasks>__5;

		internal string[] $locvar2;

		internal int $locvar3;

		internal string <task>__6;

		internal string <t>__7;

		internal string <toLog>__7;

		internal string <exception>__7;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}

	[CompilerGenerated]
	private sealed class <Prompt>c__Iterator1 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <Prompt>c__Iterator1()
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
				break;
			case 2u:
				break;
			default:
				return false;
			}
			if (ServerConsole.accepted)
			{
				if (ServerConsole.prompterQueue.Count > 0)
				{
					string text = ServerConsole.prompterQueue[0];
					ServerConsole.prompterQueue.RemoveAt(0);
					StreamWriter streamWriter = new StreamWriter(string.Concat(new object[]
					{
						"SCPSL_Data/Dedicated/",
						ServerConsole.session,
						"/sl",
						ServerConsole.logID,
						".mapi"
					}));
					ServerConsole.logID++;
					streamWriter.WriteLine(text);
					streamWriter.Close();
					MonoBehaviour.print(text);
				}
				this.$current = new WaitForEndOfFrame();
				if (!this.$disposing)
				{
					this.$PC = 2;
				}
			}
			else
			{
				this.$current = new WaitForEndOfFrame();
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

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}

	[CompilerGenerated]
	private sealed class <RefreshSession>c__Iterator2 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <RefreshSession>c__Iterator2()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this.$PC;
			this.$PC = -1;
			switch (num)
			{
			case 0u:
				this.<cnm>__0 = this.$this.GetComponent<CustomNetworkManager>();
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
			this.<form>__1.AddField("players", this.<plys>__1 + "/" + this.<cnm>__0.maxConnections);
			this.<form>__1.AddField("port", this.<cnm>__0.networkPort);
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

		internal CustomNetworkManager <cnm>__0;

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
