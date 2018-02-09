using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using GameConsole;
using UnityEngine;

public class ServerConsole : MonoBehaviour
{
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
}
