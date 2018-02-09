using System;
using UnityEngine;

public class TextureMaterialLanguage : MonoBehaviour
{
	private void Start()
	{
		this.mat.mainTexture = ((!(PlayerPrefs.GetString("langver", "en") == "pl")) ? this.englishVersion : this.polishVersion);
	}

	public Texture polishVersion;

	public Texture englishVersion;

	public Material mat;
}
