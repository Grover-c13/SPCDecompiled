using System;
using UnityEngine;

public class A_Href : MonoBehaviour
{
	public A_Href()
	{
	}

	public void Click(string url)
	{
		Application.OpenURL(url);
	}
}
