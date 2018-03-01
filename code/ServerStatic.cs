using System;
using System.Diagnostics;
using UnityEngine;

public class ServerStatic : MonoBehaviour
{
	public ServerStatic()
	{
	}

	private void Awake()
	{
		this.processStarted = false;
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		foreach (string text in commandLineArgs)
		{
			if (text == "-nographics" && !this.simulate)
			{
				this.simulate = true;
			}
			if (text.Contains("-key"))
			{
				ServerConsole.session = text.Remove(0, 4);
			}
			if (text.Contains("-id"))
			{
				foreach (Process process in Process.GetProcesses())
				{
					if (process.Id.ToString() == text.Remove(0, 3))
					{
						ServerConsole.consoleID = process;
					}
				}
			}
		}
		if (this.simulate)
		{
			ServerStatic.isDedicated = true;
			AudioListener.volume = 0f;
			ServerConsole.AddLog("SCP Secret Laboratory process started. Creating match... LOGTYPE02");
		}
	}

	private void OnLevelWasLoaded(int i)
	{
		if (ServerStatic.isDedicated && i == 1)
		{
			base.GetComponent<CustomNetworkManager>().CreateMatch();
		}
	}

	static ServerStatic()
	{
		// Note: this type is marked as 'beforefieldinit'.
	}

	public static bool isDedicated;

	public bool simulate;

	private bool processStarted;
}
