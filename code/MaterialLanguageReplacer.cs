using System;
using UnityEngine;

public class MaterialLanguageReplacer : MonoBehaviour
{
	private void Start()
	{
		base.GetComponent<Renderer>().material = ((!(PlayerPrefs.GetString("langver", "en") == "pl")) ? this.englishVersion : this.polishVersion);
	}

	public Material polishVersion;

	public Material englishVersion;
}
