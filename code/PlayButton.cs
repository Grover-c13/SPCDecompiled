using System;
using GameConsole;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
	public PlayButton()
	{
	}

	public void Click()
	{
		if (!CrashDetector.Show())
		{
			CustomNetworkManager customNetworkManager = UnityEngine.Object.FindObjectOfType<CustomNetworkManager>();
			if (NetworkClient.active)
			{
				customNetworkManager.StopClient();
			}
			NetworkServer.Reset();
			customNetworkManager.ShowLog(13);
			customNetworkManager.networkAddress = this.ip;
			try
			{
				customNetworkManager.networkPort = int.Parse(this.port);
			}
			catch
			{
				GameConsole.Console.singleton.AddLog("Wrong server port, parsing to 7777!", new Color32(182, 182, 182, byte.MaxValue), false);
				customNetworkManager.networkPort = 7777;
			}
			GameConsole.Console.singleton.AddLog(string.Concat(new string[]
			{
				"Connecting to ",
				this.ip,
				":",
				this.port,
				"!"
			}), new Color32(182, 182, 182, byte.MaxValue), false);
			customNetworkManager.StartClient();
		}
	}

	public void ShowInfo()
	{
		ServerInfo.ShowInfo(this.infoType);
	}

	public string ip;

	public string port;

	public string infoType;

	public Text motd;

	public Text players;
}
