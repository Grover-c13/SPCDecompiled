using System;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
	public ResolutionManager()
	{
	}

	private bool FindResolution(Resolution res)
	{
		foreach (ResolutionManager.ResolutionPreset resolutionPreset in ResolutionManager.presets)
		{
			if (resolutionPreset.height == res.height && resolutionPreset.width == res.width)
			{
				return true;
			}
		}
		return false;
	}

	private void Start()
	{
		ResolutionManager.presets.Clear();
		foreach (Resolution resolution in Screen.resolutions)
		{
			if (!this.FindResolution(resolution))
			{
				ResolutionManager.presets.Add(new ResolutionManager.ResolutionPreset(resolution));
			}
		}
		ResolutionManager.preset = PlayerPrefs.GetInt("SavedResolutionSet", ResolutionManager.presets.Count - 1);
		ResolutionManager.fullscreen = (PlayerPrefs.GetInt("SavedFullscreen", 1) != 0);
		ResolutionManager.RefreshScreen();
	}

	private void OnLevelWasLoaded(int level)
	{
		ResolutionManager.RefreshScreen();
	}

	public static void RefreshScreen()
	{
		ResolutionManager.presets[ResolutionManager.preset].SetResolution();
		try
		{
			UnityEngine.Object.FindObjectOfType<ResolutionText>().txt.text = ResolutionManager.presets[ResolutionManager.preset].width + " × " + ResolutionManager.presets[ResolutionManager.preset].height;
		}
		catch
		{
		}
	}

	public static void ChangeResolution(int id)
	{
		if (id == 0)
		{
			ResolutionManager.fullscreen = !ResolutionManager.fullscreen;
			PlayerPrefs.SetInt("SavedFullscreen", (!ResolutionManager.fullscreen) ? 0 : 1);
		}
		else
		{
			ResolutionManager.preset = Mathf.Clamp(ResolutionManager.preset + id, 0, ResolutionManager.presets.Count - 1);
			PlayerPrefs.SetInt("SavedResolutionSet", ResolutionManager.preset);
		}
		ResolutionManager.RefreshScreen();
	}

	static ResolutionManager()
	{
		// Note: this type is marked as 'beforefieldinit'.
	}

	public static bool fullscreen;

	public static int preset;

	public static List<ResolutionManager.ResolutionPreset> presets = new List<ResolutionManager.ResolutionPreset>();

	[Serializable]
	public class ResolutionPreset
	{
		public ResolutionPreset(Resolution template)
		{
			this.width = template.width;
			this.height = template.height;
		}

		public void SetResolution()
		{
			Screen.SetResolution(this.width, this.height, ResolutionManager.fullscreen);
		}

		public int width;

		public int height;
	}
}
