using System;
using UnityEngine;

public class MaterialLanguageReplacer : MonoBehaviour
{
	public MaterialLanguageReplacer()
	{
	}

	private void Start()
	{
		base.GetComponent<Renderer>().material = this.englishVersion;
	}

	public Material englishVersion;
}
