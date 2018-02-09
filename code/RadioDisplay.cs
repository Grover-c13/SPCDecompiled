using System;
using UnityEngine;
using UnityEngine.UI;

public class RadioDisplay : MonoBehaviour
{
	private void Start()
	{
		base.InvokeRepeating("ChangeImg", 1f, 0.5f);
	}

	private void Update()
	{
		this.normal.SetActive(RadioDisplay.battery != "0");
		this.nobattery.SetActive(RadioDisplay.battery == "0");
		this.label_t.text = RadioDisplay.label;
		this.power_t.text = RadioDisplay.power;
		this.battery_t.text = "BAT. " + RadioDisplay.battery + "%";
	}

	private void ChangeImg()
	{
		this.img.texture = ((!(this.img.texture == this.batt1)) ? this.batt1 : this.batt2);
	}

	public Text label_t;

	public Text power_t;

	public Text battery_t;

	public static string label;

	public static string power;

	public static string battery;

	public GameObject normal;

	public GameObject nobattery;

	public Texture batt1;

	public Texture batt2;

	public RawImage img;
}
