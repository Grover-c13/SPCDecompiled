using System;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class ShootingRange : MonoBehaviour
{
	public void PrintDamage(float dmg)
	{
		if (this.isOnRange)
		{
			this.txt = GameObject.Find("ShootingText").GetComponent<Text>();
			this.curDamage = "-" + Mathf.Round(dmg * 100f) / 100f + " HP";
			this.remainingTime = 3f;
		}
	}

	private void Update()
	{
		if (!this.isOnRange)
		{
			return;
		}
		if (this.remainingTime > 0f)
		{
			this.txt.text = this.curDamage;
			this.remainingTime -= Time.deltaTime;
		}
		else if (this.txt != null)
		{
			this.txt.text = string.Empty;
		}
		Camera component = base.GetComponentInChildren<GlobalFog>().GetComponent<Camera>();
		bool flag = base.transform.position.x > 1500f & base.transform.position.y > -10f;
		base.GetComponentInChildren<GlobalFog>().startDistance = (float)((!flag) ? 0 : 200);
		base.GetComponent<FirstPersonController>().rangeSpeed = flag;
		component.farClipPlane = (float)((!flag) ? 47 : 1000);
	}

	public bool isOnRange;

	private string curDamage;

	private float remainingTime;

	private Text txt;
}
