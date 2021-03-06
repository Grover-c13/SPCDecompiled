﻿using System;
using UnityEngine;

public class UBER_MaterialPresetCollection : ScriptableObject
{
	public UBER_MaterialPresetCollection()
	{
	}

	[HideInInspector]
	[SerializeField]
	public string currentPresetName;

	[HideInInspector]
	[SerializeField]
	public UBER_PresetParamSection whatToRestore;

	[HideInInspector]
	[SerializeField]
	public UBER_MaterialPreset[] matPresets;

	[HideInInspector]
	[SerializeField]
	public string[] names;
}
