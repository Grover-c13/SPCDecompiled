using System;
using System.IO;
using GameConsole;
using UnityEngine;

public class ConfigFile : MonoBehaviour
{
	public ConfigFile()
	{
	}

	private void Awake()
	{
		ConfigFile.singleton = this;
		ConfigFile.path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/SCP Secret Laboratory";
		try
		{
			if (!Directory.Exists(ConfigFile.path))
			{
				Directory.CreateDirectory(ConfigFile.path);
			}
		}
		catch
		{
			GameConsole.Console.singleton.AddLog("Configuration file directory creation failed.", new Color32(byte.MaxValue, 0, 0, byte.MaxValue), false);
		}
		ConfigFile.path += "/config.txt";
	}

	private void Start()
	{
		if (!this.ReloadConfig())
		{
			GameConsole.Console.singleton.AddLog("Configuration file could not be loaded - template not found! Loading default settings..", new Color32(byte.MaxValue, 0, 0, byte.MaxValue), false);
			GameConsole.Console.singleton.AddLog("Default settings have been loaded.", new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue), false);
		}
	}

	public bool ReloadConfig()
	{
		if (!File.Exists(ConfigFile.path))
		{
			try
			{
				File.Copy("config_template.txt", ConfigFile.path);
			}
			catch
			{
				return false;
			}
		}
		StreamReader streamReader = new StreamReader(ConfigFile.path);
		this.cfg = streamReader.ReadToEnd();
		streamReader.Close();
		return true;
	}

	public static string GetString(string key, string defaultValue = "")
	{
		string text = ConfigFile.singleton.cfg;
		if (text.Contains(key))
		{
			try
			{
				while (!text.ToLower().Replace(" ", string.Empty).StartsWith(key.ToLower() + "="))
				{
					if (!text.Contains(Environment.NewLine))
					{
						return defaultValue;
					}
					text = text.Remove(0, text.IndexOf(Environment.NewLine) + Environment.NewLine.Length).TrimStart(new char[]
					{
						' '
					});
				}
				text = text.Remove(0, text.IndexOf("=") + 1);
				text = text.TrimStart(new char[]
				{
					' '
				});
				return text.Remove(text.IndexOf(";"));
			}
			catch
			{
				return defaultValue;
			}
			return defaultValue;
		}
		return defaultValue;
	}

	private static string RemoveSpacesBefore(string s)
	{
		if (s[0].ToString() == " ")
		{
			s = s.Remove(0, 1);
			ConfigFile.RemoveSpacesBefore(s);
		}
		return s;
	}

	public static int GetInt(string key, int defaultValue = 0)
	{
		int result = 0;
		if (int.TryParse(ConfigFile.GetString(key, "errorInConverting"), out result))
		{
			return result;
		}
		return defaultValue;
	}

	public static ConfigFile singleton;

	public static string path;

	public string cfg;
}
