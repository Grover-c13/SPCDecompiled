﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameConsole
{
	public class Console : MonoBehaviour
	{
		public Console()
		{
		}

		public List<Console.Log> GetAllLogs()
		{
			return this.logs;
		}

		public void UpdateValue(string key, string value)
		{
			bool flag = false;
			key = key.ToUpper();
			foreach (Console.Value value2 in this.values)
			{
				if (value2.key == key)
				{
					value2.value = value;
					flag = true;
				}
			}
			if (!flag)
			{
				this.values.Add(new Console.Value(key, value));
			}
		}

		private void Awake()
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			if (Console.singleton == null)
			{
				Console.singleton = this;
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(base.gameObject);
			}
		}

		private void Start()
		{
			this.AddLog("Hi there! Initializing console...", new Color32(0, byte.MaxValue, 0, byte.MaxValue), false);
			this.AddLog("Done! Type 'help' to print the list of available commands.", new Color32(0, byte.MaxValue, 0, byte.MaxValue), false);
			this.RefreshConsoleScreen();
		}

		private void RefreshConsoleScreen()
		{
			bool flag = false;
			if (this.txt.text.Length > 15000)
			{
				this.logs.RemoveAt(0);
				flag = true;
			}
			if (this.txt == null)
			{
				return;
			}
			this.txt.text = string.Empty;
			if (this.logs.Count > 0)
			{
				for (int i = 0; i < this.logs.Count - this.scrollup; i++)
				{
					string text = string.Concat(new string[]
					{
						(!this.logs[i].nospace) ? "\n\n" : "\n",
						"<color=",
						this.ColorToHex(this.logs[i].color),
						">",
						this.logs[i].text,
						"</color>"
					});
					if (text.Contains("@#{["))
					{
						string str = text.Remove(text.IndexOf("@#{["));
						string text2 = text.Remove(0, text.IndexOf("@#{[") + 4);
						text2 = text2.Remove(text2.Length - 12);
						foreach (Console.Value value in this.values)
						{
							if (value.key == text2)
							{
								text = str + value.value + "</color>";
							}
						}
					}
					Text text3 = this.txt;
					text3.text += text;
				}
			}
			if (flag)
			{
				this.RefreshConsoleScreen();
			}
		}

		public void AddLog(string text, Color32 c, bool nospace = false)
		{
			this.response = this.response + text + Environment.NewLine;
			if (!nospace)
			{
				this.response += Environment.NewLine;
			}
			this.scrollup = 0;
			this.logs.Add(new Console.Log(text, c, nospace));
			this.RefreshConsoleScreen();
		}

		private string ColorToHex(Color32 color)
		{
			string str = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
			return "#" + str;
		}

		public static GameObject FindConnectedRoot(NetworkConnection conn)
		{
			try
			{
				foreach (PlayerController playerController in conn.playerControllers)
				{
					if (playerController.gameObject.tag == "Player")
					{
						return playerController.gameObject;
					}
				}
			}
			catch
			{
				return null;
			}
			return null;
		}

		public string TypeCommand(string cmd)
		{
			try
			{
				if (!GameObject.Find("Host").GetComponent<NetworkIdentity>().isLocalPlayer)
				{
					this.AddLog("Console commands are disabled for the clients.", new Color32(byte.MaxValue, 0, 0, byte.MaxValue), false);
					return "not owner";
				}
			}
			catch
			{
				return "not owner";
			}
			this.response = string.Empty;
			string[] array = cmd.ToUpper().Split(new char[]
			{
				' '
			});
			cmd = array[0];
			if (cmd == "HELLO")
			{
				this.AddLog("Hello World!", new Color32(0, byte.MaxValue, 0, byte.MaxValue), false);
			}
			else if (cmd == "LENNY")
			{
				this.AddLog("<size=450>( ͡° ͜ʖ ͡°)</size>\n\n", new Color32(byte.MaxValue, 180, 180, byte.MaxValue), false);
			}
			else if (cmd == "GIVE")
			{
				int num = 0;
				if (array.Length >= 2 && int.TryParse(array[1], out num))
				{
					string a = "offline";
					GameObject[] array2 = GameObject.FindGameObjectsWithTag("Player");
					foreach (GameObject gameObject in array2)
					{
						if (gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
						{
							a = "online";
							Inventory component = gameObject.GetComponent<Inventory>();
							if (component != null)
							{
								if (component.availableItems.Length > num)
								{
									component.AddItem(num, -4.65664672E+11f);
									a = "none";
								}
								else
								{
									this.AddLog("Failed to add ITEM#" + num.ToString("000") + " - item does not exist!", new Color32(byte.MaxValue, 180, 0, byte.MaxValue), false);
								}
							}
						}
					}
					if (a == "offline" || a == "online")
					{
						this.AddLog((!(a == "offline")) ? "Player inventory script couldn't be find!" : "You cannot use that command if you are not playing on any server!", new Color32(byte.MaxValue, 180, 0, byte.MaxValue), false);
					}
					else
					{
						this.AddLog("ITEM#" + num.ToString("000") + " has been added!", new Color32(0, byte.MaxValue, 0, byte.MaxValue), false);
					}
				}
				else
				{
					this.AddLog("Second argument has to be a number!", new Color32(byte.MaxValue, 180, 0, byte.MaxValue), false);
				}
			}
			else if (cmd == "ITEMLIST")
			{
				string a2 = "offline";
				GameObject[] array4 = GameObject.FindGameObjectsWithTag("Player");
				foreach (GameObject gameObject2 in array4)
				{
					int num2 = 1;
					if (array.Length >= 2 && !int.TryParse(array[1], out num2))
					{
						this.AddLog("Please enter correct page number!", new Color32(byte.MaxValue, 180, 0, byte.MaxValue), false);
						return this.response;
					}
					if (gameObject2.GetComponent<NetworkIdentity>().isLocalPlayer)
					{
						a2 = "online";
						Inventory component2 = gameObject2.GetComponent<Inventory>();
						if (component2 != null)
						{
							a2 = "none";
							if (num2 < 1)
							{
								this.AddLog("Page '" + num2 + "' does not exist!", new Color32(byte.MaxValue, 180, 0, byte.MaxValue), false);
								this.RefreshConsoleScreen();
								return this.response;
							}
							Item[] availableItems = component2.availableItems;
							for (int k = 10 * (num2 - 1); k < 10 * num2; k++)
							{
								if (10 * (num2 - 1) > availableItems.Length)
								{
									this.AddLog("Page '" + num2 + "' does not exist!", new Color32(byte.MaxValue, 180, 0, byte.MaxValue), false);
									break;
								}
								if (k >= availableItems.Length)
								{
									break;
								}
								this.AddLog("ITEM#" + k.ToString("000") + " : " + availableItems[k].label, new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue), false);
							}
						}
					}
				}
				if (a2 != "none")
				{
					this.AddLog((!(a2 == "offline")) ? "Player inventory script couldn't be find!" : "You cannot use that command if you are not playing on any server!", new Color32(byte.MaxValue, 180, 0, byte.MaxValue), false);
				}
			}
			else if (cmd == "CLS" || cmd == "CLEAR")
			{
				this.logs.Clear();
				this.RefreshConsoleScreen();
			}
			else if (cmd == "QUIT" || cmd == "EXIT")
			{
				this.logs.Clear();
				this.RefreshConsoleScreen();
				this.AddLog("<size=50>GOODBYE!</size>", new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue), false);
				this.RefreshConsoleScreen();
				base.Invoke("QuitGame", 1f);
			}
			else if (cmd == "HELP")
			{
				if (array.Length > 1)
				{
					string b = array[1];
					foreach (Console.CommandHint commandHint in this.hints)
					{
						if (commandHint.name == b)
						{
							this.AddLog(commandHint.name + " - " + commandHint.fullDesc, new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue), false);
							this.RefreshConsoleScreen();
							return this.response;
						}
					}
					this.AddLog("Help for command '" + array[1] + "' does not exist!", new Color32(byte.MaxValue, 180, 0, byte.MaxValue), false);
					this.RefreshConsoleScreen();
					return this.response;
				}
				this.AddLog("List of available commands:\n", new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue), false);
				foreach (Console.CommandHint commandHint2 in this.hints)
				{
					this.AddLog(commandHint2.name + " - " + commandHint2.shortDesc, new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue), true);
				}
				this.AddLog("Type 'HELP [COMMAND]' to print a full description of the chosen command.", new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue), false);
				this.RefreshConsoleScreen();
			}
			else if (cmd == "REFRESHFIX")
			{
				this.allwaysRefreshing = !this.allwaysRefreshing;
				this.AddLog("Console log refresh mode: " + ((!this.allwaysRefreshing) ? "OPTIMIZED" : "FIXED"), new Color32(0, byte.MaxValue, 0, byte.MaxValue), false);
			}
			else if (cmd == "VALUE")
			{
				if (array.Length < 2)
				{
					this.AddLog("The second argument cannot be <i>null</i>!", new Color32(byte.MaxValue, 180, 0, byte.MaxValue), false);
				}
				else
				{
					bool flag = false;
					string text = array[1];
					foreach (Console.Value value in this.values)
					{
						if (value.key == text)
						{
							flag = true;
							this.AddLog(string.Concat(new string[]
							{
								"The value of ",
								text,
								" is: @#{[",
								text,
								"}]#@"
							}), new Color32(50, 70, 100, byte.MaxValue), false);
						}
					}
					if (!flag)
					{
						this.AddLog("Key " + text + " not found!", new Color32(byte.MaxValue, 180, 0, byte.MaxValue), false);
					}
				}
			}
			else if (cmd == "SEED")
			{
				GameObject gameObject3 = GameObject.Find("Host");
				int num3 = -1;
				if (gameObject3 != null)
				{
					num3 = gameObject3.GetComponent<RandomSeedSync>().seed;
				}
				this.AddLog("Map seed is: <b>" + ((num3 != -1) ? num3.ToString() : "NONE") + "</b>", new Color32(0, byte.MaxValue, 0, byte.MaxValue), false);
			}
			else if (cmd == "SHOWRIDS")
			{
				GameObject[] array8 = GameObject.FindGameObjectsWithTag("RoomID");
				foreach (GameObject gameObject4 in array8)
				{
					gameObject4.GetComponentsInChildren<MeshRenderer>()[0].enabled = !gameObject4.GetComponentsInChildren<MeshRenderer>()[0].enabled;
					gameObject4.GetComponentsInChildren<MeshRenderer>()[1].enabled = !gameObject4.GetComponentsInChildren<MeshRenderer>()[1].enabled;
				}
				if (array8.Length > 0)
				{
					this.AddLog("Show RIDS: " + array8[0].GetComponentInChildren<MeshRenderer>().enabled, new Color32(0, byte.MaxValue, 0, byte.MaxValue), false);
				}
				else
				{
					this.AddLog("There are no RIDS!", new Color32(byte.MaxValue, 180, 0, byte.MaxValue), false);
				}
			}
			else if (cmd == "CLASSLIST")
			{
				string a3 = "offline";
				GameObject[] array10 = GameObject.FindGameObjectsWithTag("Player");
				foreach (GameObject gameObject5 in array10)
				{
					int num5 = 1;
					if (array.Length >= 2 && !int.TryParse(array[1], out num5))
					{
						this.AddLog("Please enter correct page number!", new Color32(byte.MaxValue, 180, 0, byte.MaxValue), false);
						return this.response;
					}
					if (gameObject5.GetComponent<NetworkIdentity>().isLocalPlayer)
					{
						a3 = "online";
						CharacterClassManager component3 = gameObject5.GetComponent<CharacterClassManager>();
						if (component3 != null)
						{
							a3 = "none";
							if (num5 < 1)
							{
								this.AddLog("Page '" + num5 + "' does not exist!", new Color32(byte.MaxValue, 180, 0, byte.MaxValue), false);
								this.RefreshConsoleScreen();
								return this.response;
							}
							Class[] klasy = component3.klasy;
							for (int num6 = 10 * (num5 - 1); num6 < 10 * num5; num6++)
							{
								if (10 * (num5 - 1) > klasy.Length)
								{
									this.AddLog("Page '" + num5 + "' does not exist!", new Color32(byte.MaxValue, 180, 0, byte.MaxValue), false);
									break;
								}
								if (num6 >= klasy.Length)
								{
									break;
								}
								this.AddLog("CLASS#" + num6.ToString("000") + " : " + klasy[num6].fullName, new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue), false);
							}
						}
					}
				}
				if (a3 != "none")
				{
					this.AddLog((!(a3 == "offline")) ? "Player inventory script couldn't be find!" : "You cannot use that command if you are not playing on any server!", new Color32(byte.MaxValue, 180, 0, byte.MaxValue), false);
				}
			}
			else if (cmd == "GOTO")
			{
				if (array.Length >= 2)
				{
					GameObject gameObject6 = null;
					GameObject[] array12 = GameObject.FindGameObjectsWithTag("RoomID");
					foreach (GameObject gameObject7 in array12)
					{
						if (gameObject7.GetComponent<Rid>().id.ToUpper() == array[1].ToUpper())
						{
							gameObject6 = gameObject7;
						}
					}
					string a4 = "offline";
					if (gameObject6 != null)
					{
						GameObject[] array14 = GameObject.FindGameObjectsWithTag("Player");
						foreach (GameObject gameObject8 in array14)
						{
							if (gameObject8.GetComponent<NetworkIdentity>().isLocalPlayer)
							{
								if (array[1].ToUpper() == "RANGE" && !gameObject8.GetComponent<ShootingRange>().isOnRange)
								{
									a4 = "range";
								}
								else
								{
									a4 = "none";
									gameObject8.transform.position = gameObject6.transform.position;
								}
							}
						}
						if (a4 == "range")
						{
							this.AddLog("<b>Shooting range is disabled!</b>", new Color32(byte.MaxValue, 180, 0, byte.MaxValue), false);
						}
						else if (a4 == "offline")
						{
							this.AddLog("You cannot use that command if you are not playing on any server!", new Color32(byte.MaxValue, 180, 0, byte.MaxValue), false);
						}
						else
						{
							this.AddLog("Teleported!", new Color32(0, byte.MaxValue, 0, byte.MaxValue), false);
						}
					}
					else
					{
						this.AddLog("Room: <i>" + array[1].ToUpper() + "</i> not found!", new Color32(byte.MaxValue, 180, 0, byte.MaxValue), false);
					}
				}
				else
				{
					this.AddLog("Second argument is missing!", new Color32(byte.MaxValue, 180, 0, byte.MaxValue), false);
				}
				this.RefreshConsoleScreen();
			}
			else if (cmd == "RANGE")
			{
				string a5 = "offline";
				GameObject[] array16 = GameObject.FindGameObjectsWithTag("Player");
				foreach (GameObject gameObject9 in array16)
				{
					if (gameObject9.GetComponent<NetworkIdentity>().isLocalPlayer)
					{
						a5 = "online";
						ShootingRange component4 = gameObject9.GetComponent<ShootingRange>();
						if (component4 != null)
						{
							a5 = "none";
							component4.isOnRange = true;
						}
					}
				}
				if (a5 == "offline" || a5 == "online")
				{
					this.AddLog((!(a5 == "offline")) ? "Player range script couldn't be find!" : "You cannot use that command if you are not playing on any server!", new Color32(byte.MaxValue, 180, 0, byte.MaxValue), false);
				}
				else
				{
					this.AddLog("<b>Shooting range</b> is now available!", new Color32(0, byte.MaxValue, 0, byte.MaxValue), false);
				}
			}
			else if (cmd == "ROUNDRESTART")
			{
				bool flag2 = false;
				foreach (GameObject gameObject10 in GameObject.FindGameObjectsWithTag("Player"))
				{
					PlayerStats component5 = gameObject10.GetComponent<PlayerStats>();
					if (component5.isLocalPlayer && component5.isServer)
					{
						flag2 = true;
						this.AddLog("The round is about to restart! Please wait..", new Color32(0, byte.MaxValue, 0, byte.MaxValue), false);
						component5.Roundrestart();
					}
				}
				if (!flag2)
				{
					this.AddLog("You're not owner of this server!", new Color32(byte.MaxValue, 180, 0, byte.MaxValue), false);
				}
			}
			else if (cmd == "CONFIG")
			{
				if (array.Length < 2)
				{
					this.TypeCommand("HELP CONFIG");
				}
				else
				{
					string text2 = array[1];
					if (text2 != null)
					{
						if (!(text2 == "RELOAD") && !(text2 == "R") && !(text2 == "RLD"))
						{
							if (!(text2 == "PATH"))
							{
								if (text2 == "VALUE")
								{
									if (array.Length < 3)
									{
										this.AddLog("Please enter key name in the third argument. (CONFIG VALUE <i>KEYNAME</i>)", new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue), false);
									}
									else
									{
										this.AddLog("The value of <i>'" + array[2] + "'</i> is: " + ConfigFile.GetString(array[2], "<color=ff0>DENIED: Entered key does not exists</color>"), new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue), false);
									}
								}
							}
							else
							{
								this.AddLog("Configuration file path: <i>" + ConfigFile.path + "</i>", new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue), false);
								this.AddLog("<i>No visible drive letter means the root game directory.</i>", new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue), false);
							}
						}
						else if (ConfigFile.singleton.ReloadConfig())
						{
							this.AddLog("Configuration file <b>successfully reloaded</b>. New settings will be applied on <b>your</b> server in <b>next</b> round.", new Color32(0, byte.MaxValue, 0, byte.MaxValue), false);
						}
						else
						{
							this.AddLog("Configuration file reload <b>failed</b> - no such file - '<i>" + ConfigFile.path + "</i>'. Loading defult settings..", new Color32(byte.MaxValue, 0, 0, byte.MaxValue), false);
							this.AddLog("Default settings have been loaded.", new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue), false);
						}
					}
				}
			}
			else if (cmd == "BAN")
			{
				if (GameObject.Find("Host").GetComponent<NetworkIdentity>().isLocalPlayer)
				{
					if (array.Length < 3)
					{
						this.AddLog("Syntax: BAN [player kick / ip] [minutes]", new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue), false);
						foreach (NetworkConnection networkConnection in NetworkServer.connections)
						{
							string text3 = string.Empty;
							GameObject gameObject11 = Console.FindConnectedRoot(networkConnection);
							if (gameObject11 != null)
							{
								text3 = gameObject11.GetComponent<NicknameSync>().myNick;
							}
							if (text3 == string.Empty)
							{
								this.AddLog("Player :: " + networkConnection.address, new Color32(160, 128, 128, byte.MaxValue), true);
							}
							else
							{
								this.AddLog("Player :: " + text3 + " :: " + networkConnection.address, new Color32(128, 160, 128, byte.MaxValue), true);
							}
						}
					}
					else
					{
						int duration = 0;
						if (int.TryParse(array[2], out duration))
						{
							bool flag3 = false;
							foreach (NetworkConnection networkConnection2 in NetworkServer.connections)
							{
								GameObject gameObject12 = Console.FindConnectedRoot(networkConnection2);
								if (networkConnection2.address.ToUpper().Contains(array[1]) || (gameObject12 != null && gameObject12.GetComponent<NicknameSync>().myNick.ToUpper().Contains(array[1])))
								{
									flag3 = true;
									BanPlayer.BanConnection(networkConnection2, duration);
									this.AddLog("Player banned.", new Color32(0, byte.MaxValue, 0, byte.MaxValue), false);
								}
							}
							if (!flag3)
							{
								this.AddLog("Player not found.", new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue), false);
							}
						}
						else
						{
							this.AddLog("Parse error: [minutes] - has to be an integer.", new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue), false);
						}
					}
				}
				else
				{
					this.AddLog("You are not the owner!.", new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue), false);
				}
			}
			else if (cmd == "BANREFRESH")
			{
				if (GameObject.Find("Host").GetComponent<NetworkIdentity>().isLocalPlayer)
				{
					BanPlayer.ReloadBans();
				}
				else
				{
					this.AddLog("You are not the owner!.", new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue), false);
				}
			}
			else
			{
				this.AddLog("Command " + cmd + " does not exist!", new Color32(byte.MaxValue, 180, 0, byte.MaxValue), false);
			}
			return this.response;
		}

		public void ProceedButton()
		{
			if (this.cmdField.text != string.Empty)
			{
				this.TypeCommand(this.cmdField.text);
			}
			this.cmdField.text = string.Empty;
			EventSystem.current.SetSelectedGameObject(this.cmdField.gameObject);
		}

		private void LateUpdate()
		{
			if (Input.GetKeyDown(KeyCode.Return))
			{
				this.ProceedButton();
			}
			if (Input.GetKeyDown(KeyCode.BackQuote))
			{
				this.ToggleConsole();
			}
			this.scrollup += Mathf.RoundToInt(Input.GetAxisRaw("Mouse ScrollWheel") * 10f);
			if (this.logs.Count > 0)
			{
				this.scrollup = Mathf.Clamp(this.scrollup, 0, this.logs.Count - 1);
			}
			else
			{
				this.scrollup = 0;
			}
			if (this.previous_scrlup != this.scrollup)
			{
				this.previous_scrlup = this.scrollup;
				this.RefreshConsoleScreen();
			}
			Scene activeScene = SceneManager.GetActiveScene();
			if (activeScene.name != this.loadedLevel)
			{
				this.loadedLevel = activeScene.name;
				this.AddLog(string.Concat(new string[]
				{
					"Scene Manager: Loaded scene '",
					activeScene.name,
					"' [",
					activeScene.path,
					"]"
				}), new Color32(0, byte.MaxValue, 0, byte.MaxValue), false);
				this.RefreshConsoleScreen();
			}
			if (this.allwaysRefreshing)
			{
				this.RefreshConsoleScreen();
			}
		}

		public void ToggleConsole()
		{
			CursorManager.consoleOpen = !this.console.activeSelf;
			this.cmdField.text = string.Empty;
			this.console.SetActive(!this.console.activeSelf);
			if (PlayerManager.singleton != null)
			{
				GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
				foreach (GameObject gameObject in array)
				{
					if (gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
					{
						FirstPersonController component = gameObject.GetComponent<FirstPersonController>();
						if (component != null)
						{
							component.usingConsole = this.console.activeSelf;
						}
					}
				}
			}
			if (this.console.activeSelf)
			{
				EventSystem.current.SetSelectedGameObject(this.cmdField.gameObject);
			}
		}

		private void QuitGame()
		{
			Application.Quit();
		}

		private bool allwaysRefreshing;

		private List<Console.Log> logs = new List<Console.Log>();

		private List<Console.Value> values = new List<Console.Value>();

		public Console.CommandHint[] hints;

		public Text txt;

		public InputField cmdField;

		public GameObject console;

		public static Console singleton;

		private int scrollup;

		private int previous_scrlup;

		private string loadedLevel;

		private string response = string.Empty;

		[Serializable]
		public class CommandHint
		{
			public CommandHint()
			{
			}

			public string name;

			public string shortDesc;

			[Multiline]
			public string fullDesc;
		}

		[Serializable]
		public class Value
		{
			public Value(string k, string v)
			{
				this.key = k;
				this.value = v;
			}

			public string key;

			public string value;
		}

		[Serializable]
		public class Log
		{
			public Log(string t, Color32 c, bool b)
			{
				this.text = t;
				this.color = c;
				this.nospace = b;
			}

			public string text;

			public Color32 color;

			public bool nospace;
		}
	}
}
