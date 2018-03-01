using System;
using GameConsole;
using Steamworks;
using UnityEngine;

public class SteamServerManager : MonoBehaviour
{
	public SteamServerManager()
	{
	}

	private void Start()
	{
		this.console = GameConsole.Console.singleton;
		SteamServerManager._instance = this;
	}

	public static SteamServerManager _instance;

	private bool gs_Initialized;

	private Callback<SteamServersConnected_t> Callback_ServerConnected;

	private GameConsole.Console console;
}
