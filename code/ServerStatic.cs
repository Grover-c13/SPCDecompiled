using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerStatic : MonoBehaviour
{
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

	private void Start()
	{
		if (ServerStatic.isDedicated && SceneManager.GetActiveScene().buildIndex == 0)
		{
			base.GetComponent<CustomNetworkManager>().CreateMatch();
		}
	}

	public static bool isDedicated;

	public bool simulate;

	private bool processStarted;
}
