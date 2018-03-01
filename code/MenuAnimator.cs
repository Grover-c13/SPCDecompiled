using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MenuAnimator : MonoBehaviour
{
	public MenuAnimator()
	{
	}

	private void Update()
	{
		bool flag = this.con1.activeSelf | this.con2.activeSelf | base.GetComponent<MainMenuScript>().submenus[6].activeSelf | this.dsc.activeSelf | this.lang.activeSelf;
		this.kamera.transform.position = Vector3.Lerp(this.kamera.transform.position, (!flag) ? this.unfoc.transform.position : this.foc.transform.position, Time.deltaTime * 2f);
		this.kamera.transform.rotation = Quaternion.Lerp(this.kamera.transform.rotation, (!flag) ? this.unfoc.transform.rotation : this.foc.transform.rotation, Time.deltaTime);
	}

	private void Start()
	{
		base.StartCoroutine(this.Animate());
	}

	private IEnumerator Animate()
	{
		for (;;)
		{
			int t = UnityEngine.Random.Range(2, 5);
			foreach (SignBlink signBlink in UnityEngine.Object.FindObjectsOfType<SignBlink>())
			{
				signBlink.Play(t);
			}
			yield return new WaitForSeconds((float)UnityEngine.Random.Range(3, 10));
		}
		yield break;
	}

	public GameObject kamera;

	public GameObject con1;

	public GameObject con2;

	public GameObject foc;

	public GameObject unfoc;

	public GameObject dsc;

	public GameObject lang;

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
				break;
			case 1u:
				break;
			default:
				return false;
			}
			this.<t>__1 = UnityEngine.Random.Range(2, 5);
			this.$locvar0 = UnityEngine.Object.FindObjectsOfType<SignBlink>();
			this.$locvar1 = 0;
			while (this.$locvar1 < this.$locvar0.Length)
			{
				SignBlink signBlink = this.$locvar0[this.$locvar1];
				signBlink.Play(this.<t>__1);
				this.$locvar1++;
			}
			this.$current = new WaitForSeconds((float)UnityEngine.Random.Range(3, 10));
			if (!this.$disposing)
			{
				this.$PC = 1;
			}
			return true;
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

		internal int <t>__1;

		internal SignBlink[] $locvar0;

		internal int $locvar1;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}
}
