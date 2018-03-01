using System;
using UnityEngine;
using UnityEngine.Networking;

public class NetManagerValueSetter : MonoBehaviour
{
	public NetManagerValueSetter()
	{
	}

	private void Start()
	{
		this.singleton = NetworkManager.singleton.GetComponent<CustomNetworkManager>();
	}

	public void ChangeIP(string ip)
	{
		this.singleton.networkAddress = ip;
	}

	public void ChangePort(int port)
	{
		this.singleton.networkPort = port;
	}

	public void JoinGame()
	{
		this.singleton.StartClient();
	}

	public void HostGame()
	{
		this.singleton.StartHost();
	}

	public void Disconnect()
	{
		this.singleton.StopHost();
	}

	private CustomNetworkManager singleton;
}
