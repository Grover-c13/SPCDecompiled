using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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

	[CompilerGenerated]
	private sealed class <Play>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <Play>c__Iterator0()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this.$PC;
			this.$PC = -1;
			switch (num)
			{
			case 0u:
				this.<first>__0 = true;
				this.$locvar0 = this.$this.logQueue;
				this.$locvar1 = 0;
				goto IL_197;
			case 1u:
				this.<i>__3++;
				break;
			case 2u:
				this.<i>__4++;
				goto IL_172;
			case 3u:
				this.$this.GetComponentInParent<MainMenuScript>().ChangeMenu(0);
				this.$PC = -1;
				return false;
			default:
				return false;
			}
			IL_DE:
			if ((float)this.<i>__3 < this.<type>__2.preTime / this.$this.speed)
			{
				this.$current = new WaitForSeconds(0.02f / this.$this.speed);
				if (!this.$disposing)
				{
					this.$PC = 1;
				}
				return true;
			}
			IL_101:
			this.$this.SpawnType(this.<type>__2, this.<item>__1.text1_en, this.<item>__1.text2_en);
			this.<i>__4 = 0;
			IL_172:
			if ((float)this.<i>__4 < this.<type>__2.postTime)
			{
				this.$current = new WaitForSeconds(0.02f / this.$this.speed);
				if (!this.$disposing)
				{
					this.$PC = 2;
				}
				return true;
			}
			this.$locvar1++;
			IL_197:
			if (this.$locvar1 >= this.$locvar0.Length)
			{
				this.$current = new WaitForSeconds(8f / this.$this.speed);
				if (!this.$disposing)
				{
					this.$PC = 3;
				}
				return true;
			}
			this.<item>__1 = this.$locvar0[this.$locvar1];
			this.<type>__2 = this.$this.logTypes[this.<item>__1.type];
			if (this.<first>__0)
			{
				this.<first>__0 = false;
				goto IL_101;
			}
			this.<i>__3 = 0;
			goto IL_DE;
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

		internal bool <first>__0;

		internal Credits.CreditLog[] $locvar0;

		internal int $locvar1;

		internal Credits.CreditLog <item>__1;

		internal Credits.CreditLogType <type>__2;

		internal int <i>__3;

		internal int <i>__4;

		internal Credits $this;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}
}
