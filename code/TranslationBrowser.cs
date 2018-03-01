using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TranslationBrowser : MonoBehaviour
{
	public TranslationBrowser()
	{
	}

	private void OnEnable()
	{
		string[] directories = Directory.GetDirectories("Translations");
		foreach (GameObject obj in this.spawns)
		{
			UnityEngine.Object.Destroy(obj);
		}
		foreach (string text in directories)
		{
			Text text2 = UnityEngine.Object.Instantiate<Text>(this.instancePrefab, this.parent);
			text2.transform.localScale = Vector3.one;
			text2.text = text.Remove(0, text.IndexOf("\\") + 1);
			this.spawns.Add(text2.gameObject);
		}
	}

	public Text instancePrefab;

	public Transform parent;

	private List<GameObject> spawns = new List<GameObject>();
}
