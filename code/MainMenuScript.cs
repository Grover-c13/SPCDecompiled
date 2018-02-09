using System;
using Steamworks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
	private void Update()
	{
		if (SceneManager.GetActiveScene().buildIndex == 0)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}

	public void SetLang(string lang)
	{
		string @string = PlayerPrefs.GetString("langver");
		PlayerPrefs.SetString("langver", lang);
		if (lang != @string)
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
		}
		else
		{
			this.ChangeMenu(0);
		}
	}

	public void SetIP(string ip)
	{
		NetworkServer.Reset();
		this.mng.networkPort = 7777;
		try
		{
			string s = ip.Remove(0, ip.LastIndexOf(":") + 1);
			this.mng.networkPort = int.Parse(s);
		}
		catch
		{
		}
		this.mng.networkAddress = ((!ip.Contains(":")) ? ip : ip.Remove(ip.IndexOf(":")));
	}

	public void ChangeMenu(int id)
	{
		this.curMenu = id;
		for (int i = 0; i < this.submenus.Length; i++)
		{
			this.submenus[i].SetActive(i == id);
		}
	}

	public void QuitGame()
	{
		Application.Quit();
	}

	private void Start()
	{
		this.mng = UnityEngine.Object.FindObjectOfType<CustomNetworkManager>();
		CursorManager.UnsetAll();
		if (SteamManager.Initialized)
		{
			SteamUserStats.SetAchievement("TEST_1");
		}
		this.ChangeMenu(0);
	}

	public void StartServer()
	{
		this.mng.onlineScene = "Facility";
		this.mng.maxConnections = 20;
		this.mng.createpop.SetActive(true);
	}

	public void StartTutorial(string scene)
	{
		this.mng.onlineScene = scene;
		this.mng.maxConnections = 1;
		this.mng.ShowLog(15);
		this.mng.StartHost();
	}

	public void Connect()
	{
		NetworkServer.Reset();
		this.mng.ShowLog(13);
		this.mng.StartClient();
	}

	public GameObject[] submenus;

	private CustomNetworkManager mng;

	public int curMenu;
}
