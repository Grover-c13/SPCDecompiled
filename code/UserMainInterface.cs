using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UserMainInterface : MonoBehaviour
{
	private void Awake()
	{
		UserMainInterface.singleton = this;
	}

	private void Start()
	{
		ResolutionManager.RefreshScreen();
	}

	public void SearchProgress(float curProgress, float targetProgress)
	{
		this.searchProgress.maxValue = targetProgress;
		this.searchProgress.value = curProgress;
		this.searchOBJ.SetActive(curProgress != 0f);
	}

	public void SetHP(int _hp, int _maxhp)
	{
		float num = (float)_maxhp;
		this.lerpedHP = Mathf.Lerp(this.lerpedHP, (float)_hp, Time.deltaTime * this.lerpSpeed);
		this.sliderHP.value = this.lerpedHP;
		this.textHP.text = Mathf.Round(this.sliderHP.value / num * 100f) + "%";
		this.sliderHP.maxValue = num;
	}

	private void Update()
	{
		try
		{
			this.fps.text = NetworkManager.singleton.client.GetRTT() + " ms";
		}
		catch
		{
		}
	}

	public Slider sliderHP;

	public Slider searchProgress;

	public Text textHP;

	public Text specatorInfo;

	public GameObject hpOBJ;

	public GameObject searchOBJ;

	public GameObject overloadMsg;

	public GameObject summary;

	[Space]
	public Text fps;

	public static UserMainInterface singleton;

	public float lerpSpeed = 3f;

	public float lerpedHP;
}
