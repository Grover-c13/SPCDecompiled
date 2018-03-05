using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
}
