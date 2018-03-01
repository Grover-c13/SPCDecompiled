using System;
using UnityEngine;

public class ResolutionButton : MonoBehaviour
{
	public ResolutionButton()
	{
	}

	public void Click(int id)
	{
		ResolutionManager.ChangeResolution(id);
	}
}
