using System;
using UnityEngine;
using UnityEngine.UI;

public class Embargo : MonoBehaviour
{
	private void Start()
	{
		this.txt = base.GetComponent<Text>();
		base.InvokeRepeating("ChangePosition", 3f, 3f);
	}

	private void ChangePosition()
	{
		base.GetComponent<RectTransform>().localPosition = new Vector3((float)UnityEngine.Random.Range(-500, 500), (float)UnityEngine.Random.Range(-250, 280), 0f);
	}

	private void Update()
	{
		if (this.showEmbargo)
		{
			DateTime now = DateTime.Now;
			this.txt.text = string.Concat(new object[]
			{
				"<size=30><color=#a11>EMBARGO</color></size>\n\n",
				now.Day,
				".",
				now.Month,
				".",
				now.Year,
				" ",
				now.Hour.ToString("00"),
				":",
				now.Minute.ToString("00"),
				":",
				now.Second.ToString("00"),
				"\n",
				SystemInfo.operatingSystem,
				"\n",
				SystemInfo.deviceName,
				"\n<size=18><color=#a11>DO NOT SHARE</color></size>"
			});
		}
		else
		{
			this.txt.text = string.Empty;
		}
	}

	private Text txt;

	public bool showEmbargo;
}
