using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
	public Credits()
	{
	}

	private void SpawnType(Credits.CreditLogType l, string txt1, string txt2)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(l.preset, this.maskPosition);
		Text[] componentsInChildren = gameObject.GetComponentsInChildren<Text>();
		componentsInChildren[0].text = txt1;
		if (componentsInChildren.Length > 1)
		{
			componentsInChildren[1].text = txt2;
		}
		UnityEngine.Object.Destroy(gameObject, 12f / this.speed);
		this.spawnedLogs.Add(gameObject);
		CreditText component = gameObject.GetComponent<CreditText>();
		component.move = true;
		component.speed *= this.speed;
	}

	private void OnEnable()
	{
		base.StartCoroutine(this.Play());
	}

	private void OnDisable()
	{
		base.StopAllCoroutines();
		foreach (GameObject gameObject in this.spawnedLogs)
		{
			if (gameObject != null)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
		}
		this.spawnedLogs.Clear();
	}

	private IEnumerator Play()
	{
		bool first = true;
		foreach (Credits.CreditLog item in this.logQueue)
		{
			Credits.CreditLogType type = this.logTypes[item.type];
			if (first)
			{
				first = false;
			}
			else
			{
				int i = 0;
				while ((float)i < type.preTime / this.speed)
				{
					yield return new WaitForSeconds(0.02f / this.speed);
					i++;
				}
			}
			this.SpawnType(type, item.text1_en, item.text2_en);
			int j = 0;
			while ((float)j < type.postTime)
			{
				yield return new WaitForSeconds(0.02f / this.speed);
				j++;
			}
		}
		yield return new WaitForSeconds(8f / this.speed);
		base.GetComponentInParent<MainMenuScript>().ChangeMenu(0);
		yield break;
	}

	public Transform maskPosition;

	[Range(0.2f, 2.5f)]
	public float speed = 1f;

	public Credits.CreditLogType[] logTypes;

	public Credits.CreditLog[] logQueue;

	private List<GameObject> spawnedLogs = new List<GameObject>();

	[Serializable]
	public class CreditLogType
	{
		public CreditLogType()
		{
		}

		public float preTime;

		public float postTime;

		public GameObject preset;
	}

	[Serializable]
	public class CreditLog
	{
		public CreditLog()
		{
		}

		public string text1_en;

		public string text2_en;

		public int type;
	}
}
