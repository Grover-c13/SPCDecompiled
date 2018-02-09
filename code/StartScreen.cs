using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour
{
	private void Start()
	{
		this.isPL = (PlayerPrefs.GetString("langver", "en") == "pl");
	}

	public void PlayAnimation(int classID)
	{
		base.StopAllCoroutines();
		base.StartCoroutine(this.Animate(classID));
	}

	private IEnumerator Animate(int classID)
	{
		this.black.gameObject.SetActive(true);
		GameObject host = GameObject.Find("Host");
		CharacterClassManager ccm = host.GetComponent<CharacterClassManager>();
		Class klasa = ccm.klasy[classID];
		this.youare.text = ((!TutorialManager.status) ? ((!this.isPL) ? "YOU ARE" : "TWOJA ROLA:") : string.Empty);
		this.wmi.text = ((!this.isPL) ? klasa.fullName : klasa.fullName_pl);
		this.wmi.color = klasa.classColor;
		this.wihtd.text = ((!this.isPL) ? klasa.description : klasa.description_pl);
		while (this.popup.transform.localScale.x < 1f)
		{
			this.popup.transform.localScale += Vector3.one * Time.deltaTime * 2f;
			if (this.popup.transform.localScale.x > 1f)
			{
				this.popup.transform.localScale = Vector3.one;
			}
			yield return new WaitForEndOfFrame();
		}
		while (this.black.color.a > 0f)
		{
			this.black.color = new Color(0f, 0f, 0f, this.black.color.a - Time.deltaTime);
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForSeconds(7f);
		CanvasRenderer c = this.youare.GetComponent<CanvasRenderer>();
		CanvasRenderer c2 = this.wmi.GetComponent<CanvasRenderer>();
		CanvasRenderer c3 = this.wihtd.GetComponent<CanvasRenderer>();
		HintManager.singleton.AddHint(0);
		while (c.GetAlpha() > 0f)
		{
			c.SetAlpha(c.GetAlpha() - Time.deltaTime / 2f);
			c2.SetAlpha(c2.GetAlpha() - Time.deltaTime / 2f);
			c3.SetAlpha(c3.GetAlpha() - Time.deltaTime / 2f);
			yield return new WaitForEndOfFrame();
		}
		this.black.gameObject.SetActive(false);
		yield break;
	}

	public GameObject popup;

	public Image black;

	public Text youare;

	public Text wmi;

	public Text wihtd;

	private bool isPL;
}
