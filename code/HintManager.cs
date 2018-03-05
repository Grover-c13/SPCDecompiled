using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class HintManager : MonoBehaviour
{
	public HintManager()
	{
	}

	private void Awake()
	{
		this.box.canvasRenderer.SetAlpha(0f);
		HintManager.singleton = this;
		for (int i = 0; i < this.hints.Length; i++)
		{
			this.hints[i].content_en = TranslationReader.Get("Hints", i);
		}
	}

	private void Start()
	{
		this.box.canvasRenderer.SetAlpha(0f);
		base.StartCoroutine(this.ShowHints());
	}

	private IEnumerator ShowHints()
	{
		CanvasRenderer cr = this.box.canvasRenderer;
		for (;;)
		{
			while (cr.GetAlpha() > 0f)
			{
				cr.SetAlpha(cr.GetAlpha() - Time.deltaTime * 5f);
				cr.GetComponentInChildren<Text>().canvasRenderer.SetAlpha(cr.GetAlpha());
				yield return new WaitForEndOfFrame();
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
		public Hint()
		{
		}

		[Multiline]
		public string content_en;

		public string keyName;

		public float duration;
	}
}
