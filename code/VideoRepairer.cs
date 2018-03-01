using System;
using UnityEngine;
using UnityEngine.Video;

public class VideoRepairer : MonoBehaviour
{
	public VideoRepairer()
	{
	}

	private void Start()
	{
		base.Invoke("Repair", 5f);
	}

	private void Repair()
	{
		base.GetComponent<VideoPlayer>().targetMaterialProperty = "_EmissionMap";
	}
}
