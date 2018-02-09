using System;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceColorAdjuster : MonoBehaviour
{
	public void ChangeColor(Color color)
	{
		foreach (Graphic graphic in this.graphicsToChange)
		{
			if (graphic != null)
			{
				Color color2 = new Color(color.r, color.g, color.b, graphic.color.a);
				graphic.color = color2;
			}
		}
	}

	public Graphic[] graphicsToChange;
}
