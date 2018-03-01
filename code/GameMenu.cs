using System;
using UnityEngine;
using UnityEngine.Networking;

public class GameMenu : MonoBehaviour
{
	public GameMenu()
	{
	}

	private void Update()
	{
		if (Input.GetButtonDown("Cancel") && !CursorManager.eqOpen)
		{
			this.ToggleMenu();
		}
	}

	public void ToggleMenu()
	{
		foreach (GameObject gameObject in this.minors)
		{
			if (gameObject.activeSelf)
			{
				gameObject.SetActive(false);
			}
		}
		this.background.SetActive(!this.background.activeSelf);
		CursorManager.pauseOpen = this.background.activeSelf;
		GameObject[] players = PlayerManager.singleton.players;
		foreach (GameObject gameObject2 in players)
		{
			if (gameObject2.GetComponent<NetworkIdentity>().isLocalPlayer)
			{
				gameObject2.GetComponent<FirstPersonController>().isPaused = this.background.activeSelf;
			}
		}
		this.main.SetActive(true);
	}

	public void Disconnect()
	{
		GameObject[] players = PlayerManager.singleton.players;
		foreach (GameObject gameObject in players)
		{
			if (gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
			{
				if (gameObject.GetComponent<NetworkIdentity>().isServer)
				{
					UnityEngine.Object.FindObjectOfType<NetworkManager>().StopHost();
				}
				else
				{
					UnityEngine.Object.FindObjectOfType<NetworkManager>().StopClient();
				}
			}
		}
	}

	public GameObject background;

	public GameObject main;

	public GameObject[] minors;
}
