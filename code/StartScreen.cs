using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour
{
	public StartScreen()
	{
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
		this.youare.text = ((!TutorialManager.status) ? TranslationReader.Get("Facility", 31) : string.Empty);
		this.wmi.text = klasa.fullName;
		this.wmi.color = klasa.classColor;
		this.wihtd.text = klasa.description;
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

	[CompilerGenerated]
	private sealed class <Animate>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <Animate>c__Iterator0()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this.$PC;
			this.$PC = -1;
			switch (num)
			{
			case 0u:
				this.$this.black.gameObject.SetActive(true);
				this.<host>__0 = GameObject.Find("Host");
				this.<ccm>__0 = this.<host>__0.GetComponent<CharacterClassManager>();
				this.<klasa>__0 = this.<ccm>__0.klasy[this.classID];
				this.$this.youare.text = ((!TutorialManager.status) ? TranslationReader.Get("Facility", 31) : string.Empty);
				this.$this.wmi.text = this.<klasa>__0.fullName;
				this.$this.wmi.color = this.<klasa>__0.classColor;
				this.$this.wihtd.text = this.<klasa>__0.description;
				break;
			case 1u:
				break;
			case 2u:
				goto IL_228;
			case 3u:
				this.<c1>__0 = this.$this.youare.GetComponent<CanvasRenderer>();
				this.<c2>__0 = this.$this.wmi.GetComponent<CanvasRenderer>();
				this.<c3>__0 = this.$this.wihtd.GetComponent<CanvasRenderer>();
				HintManager.singleton.AddHint(0);
				goto IL_346;
			case 4u:
				goto IL_346;
			default:
				return false;
			}
			if (this.$this.popup.transform.localScale.x < 1f)
			{
				this.$this.popup.transform.localScale += Vector3.one * Time.deltaTime * 2f;
				if (this.$this.popup.transform.localScale.x > 1f)
				{
					this.$this.popup.transform.localScale = Vector3.one;
				}
				this.$current = new WaitForEndOfFrame();
				if (!this.$disposing)
				{
					this.$PC = 1;
				}
				return true;
			}
			IL_228:
			if (this.$this.black.color.a <= 0f)
			{
				this.$current = new WaitForSeconds(7f);
				if (!this.$disposing)
				{
					this.$PC = 3;
				}
				return true;
			}
			this.$this.black.color = new Color(0f, 0f, 0f, this.$this.black.color.a - Time.deltaTime);
			this.$current = new WaitForEndOfFrame();
			if (!this.$disposing)
			{
				this.$PC = 2;
			}
			return true;
			IL_346:
			if (this.<c1>__0.GetAlpha() > 0f)
			{
				this.<c1>__0.SetAlpha(this.<c1>__0.GetAlpha() - Time.deltaTime / 2f);
				this.<c2>__0.SetAlpha(this.<c2>__0.GetAlpha() - Time.deltaTime / 2f);
				this.<c3>__0.SetAlpha(this.<c3>__0.GetAlpha() - Time.deltaTime / 2f);
				this.$current = new WaitForEndOfFrame();
				if (!this.$disposing)
				{
					this.$PC = 4;
				}
				return true;
			}
			this.$this.black.gameObject.SetActive(false);
			this.$PC = -1;
			return false;
		}

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.$current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.$current;
			}
		}

		[DebuggerHidden]
		public void Dispose()
		{
			this.$disposing = true;
			this.$PC = -1;
		}

		[DebuggerHidden]
		public void Reset()
		{
			throw new NotSupportedException();
		}

		internal GameObject <host>__0;

		internal CharacterClassManager <ccm>__0;

		internal int classID;

		internal Class <klasa>__0;

		internal CanvasRenderer <c1>__0;

		internal CanvasRenderer <c2>__0;

		internal CanvasRenderer <c3>__0;

		internal StartScreen $this;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}
}
