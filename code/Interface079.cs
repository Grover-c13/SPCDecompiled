using System;
using UnityEngine;
using UnityEngine.UI;

public class Interface079 : MonoBehaviour
{
	public Interface079()
	{
	}

	private void Update()
	{
		this.teslaButton.GetComponent<Button>().interactable = (this.ability >= 40f);
		if (this.ability < 200f)
		{
			this.ability += Time.deltaTime;
		}
		DateTime now = DateTime.Now;
		this.infoText.text = string.Concat(new object[]
		{
			"CAM ▪ ",
			Screen.width,
			"x",
			Screen.height,
			" | ",
			now.Hour.ToString("00"),
			":",
			now.Minute.ToString("00"),
			":",
			now.Second.ToString("00"),
			" ▪ ",
			now.Month,
			".",
			now.Day,
			".20▮▮\n",
			(this.ability <= 40f) ? "ABILITY NOT READY" : "ABILITY READY"
		});
	}

	public void SetProgress(float f)
	{
		this.progress = f;
		this.RefreshProgress();
	}

	public void ResetProgress()
	{
		this.progress = 0f;
		this.RefreshProgress();
	}

	public void AddProgress(float f)
	{
		this.progress += f;
		this.RefreshProgress();
	}

	public float GetProgress()
	{
		return this.ClampProgress();
	}

	public bool Action()
	{
		return this.ClampProgress() == 1f;
	}

	private float ClampProgress()
	{
		this.progress = Mathf.Clamp01(this.progress);
		return this.progress;
	}

	public void RefreshProgress()
	{
		this.progress_img.fillAmount = this.ClampProgress();
	}

	public void SetConsoleScreen(int id)
	{
		this.curScreen = id;
	}

	public void UseElevator()
	{
		this.localplayer.GetComponent<Scp079PlayerScript>().UseElevator();
	}

	public void UseTesla()
	{
		this.ability = 0f;
		this.localplayer.GetComponent<Scp079PlayerScript>().UseTesla();
	}

	private float progress;

	public Image progress_img;

	public GameObject progress_obj;

	public Text infoText;

	public Image fovSlider;

	public GameObject console;

	public Text consoleScreen;

	public InputField field;

	public GameObject liftButton;

	public GameObject teslaButton;

	[HideInInspector]
	public GameObject localplayer;

	private int curScreen;

	public float ability;
}
