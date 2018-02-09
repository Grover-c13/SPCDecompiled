using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class HintManager : MonoBehaviour
{
	private void Awake()
	{
		HintManager.singleton = this;
	}

	private void Start()
	{
		this.box.canvasRenderer.SetAlpha(0f);
		base.StartCoroutine(this.ShowHints());
	}

	private IEnumerator ShowHints()
	{
		bool usePL = PlayerPrefs.GetString("langver", "en") == "pl";
		for (;;)
		{
			if (this.hintQueue.Count > 0)
			{
				this.box.GetComponentInChildren<Text>().text = ((!usePL) ? this.hintQueue[0].content_en : this.hintQueue[0].content_pl);
				this.box.GetComponent<RectTransform>().sizeDelta = ((!usePL) ? this.hintQueue[0].size_en : this.hintQueue[0].size_pl);
				CanvasRenderer cr = this.box.canvasRenderer;
				base.GetComponent<AudioSource>().Play();
				while (cr.GetAlpha() < 1f)
				{
					cr.SetAlpha(cr.GetAlpha() + Time.deltaTime * 5f);
					cr.GetComponentInChildren<Text>().canvasRenderer.SetAlpha(cr.GetAlpha());
					yield return new WaitForEndOfFrame();
				}
				float dur = this.hintQueue[0].duration;
				while (dur > 0f)
				{
					dur -= Time.deltaTime;
					float v3 = Mathf.Sin(dur * 4f) / 15f + 0.07f;
					this.box.color = new Color(this.box.color.r, v3, this.box.color.b, this.box.color.a);
					yield return new WaitForEndOfFrame();
				}
				while (cr.GetAlpha() > 0f)
				{
					cr.SetAlpha(cr.GetAlpha() - Time.deltaTime * 5f);
					cr.GetComponentInChildren<Text>().canvasRenderer.SetAlpha(cr.GetAlpha());
					yield return new WaitForEndOfFrame();
				}
				yield return new WaitForSeconds(0.3f);
				this.hintQueue.RemoveAt(0);
			}
			yield return new WaitForEndOfFrame();
		}
		yield break;
	}

	public void AddHint(int hintID)
	{
		if (TutorialManager.status)
		{
			return;
		}
		if (PlayerPrefs.GetInt(this.hints[hintID].keyName, 0) == 0)
		{
			this.hintQueue.Add(this.hints[hintID]);
			PlayerPrefs.SetInt(this.hints[hintID].keyName, 1);
		}
	}

	public static HintManager singleton;

	[SerializeField]
	private Image box;

	public HintManager.Hint[] hints;

	public List<HintManager.Hint> hintQueue = new List<HintManager.Hint>();

	[Serializable]
	public class Hint
	{
		public Vector2 size_en;

		public Vector2 size_pl;

		[Multiline]
		public string content_en;

		[Multiline]
		public string content_pl;

		public string keyName;

		public float duration;
	}
}
