using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Escape : NetworkBehaviour
{
	public Escape()
	{
	}

	private void Start()
	{
		this.ccm = base.GetComponent<CharacterClassManager>();
		this.respawnText = GameObject.Find("Respawn Text").GetComponent<Text>();
	}

	private void Update()
	{
		if (base.isLocalPlayer && Vector3.Distance(base.transform.position, this.worldPosition) < (float)this.radius)
		{
			this.EscapeFromFacility();
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(this.worldPosition, (float)this.radius);
	}

	private void EscapeFromFacility()
	{
		if (!this.escaped)
		{
			if (this.ccm.klasy[this.ccm.curClass].team == Team.RSC)
			{
				this.escaped = true;
				this.ccm.RegisterEscape();
				base.StartCoroutine(this.EscapeAnim(TranslationReader.Get("Facility", 29)));
			}
			if (this.ccm.klasy[this.ccm.curClass].team == Team.CDP)
			{
				this.escaped = true;
				this.ccm.RegisterEscape();
				base.StartCoroutine(this.EscapeAnim(TranslationReader.Get("Facility", 30)));
			}
		}
	}

	private IEnumerator EscapeAnim(string txt)
	{
		CanvasRenderer cr = this.respawnText.GetComponent<CanvasRenderer>();
		cr.SetAlpha(0f);
		this.respawnText.text = txt;
		while (cr.GetAlpha() < 1f)
		{
			cr.SetAlpha(cr.GetAlpha() + 0.1f);
			yield return new WaitForSeconds(0.1f);
		}
		yield return new WaitForSeconds(2f);
		this.escaped = false;
		yield return new WaitForSeconds(3f);
		while (cr.GetAlpha() > 0f)
		{
			cr.SetAlpha(cr.GetAlpha() - 0.1f);
			yield return new WaitForSeconds(0.1f);
		}
		yield break;
	}

	private void UNetVersion()
	{
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		bool result;
		return result;
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
	}

	private CharacterClassManager ccm;

	private Text respawnText;

	private bool escaped;

	public Vector3 worldPosition;

	public int radius = 10;

	[CompilerGenerated]
	private sealed class <EscapeAnim>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <EscapeAnim>c__Iterator0()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this.$PC;
			this.$PC = -1;
			switch (num)
			{
			case 0u:
				this.<cr>__0 = this.$this.respawnText.GetComponent<CanvasRenderer>();
				this.<cr>__0.SetAlpha(0f);
				this.$this.respawnText.text = this.txt;
				break;
			case 1u:
				break;
			case 2u:
				this.$this.escaped = false;
				this.$current = new WaitForSeconds(3f);
				if (!this.$disposing)
				{
					this.$PC = 3;
				}
				return true;
			case 3u:
				goto IL_15C;
			case 4u:
				goto IL_15C;
			default:
				return false;
			}
			if (this.<cr>__0.GetAlpha() >= 1f)
			{
				this.$current = new WaitForSeconds(2f);
				if (!this.$disposing)
				{
					this.$PC = 2;
				}
				return true;
			}
			this.<cr>__0.SetAlpha(this.<cr>__0.GetAlpha() + 0.1f);
			this.$current = new WaitForSeconds(0.1f);
			if (!this.$disposing)
			{
				this.$PC = 1;
			}
			return true;
			IL_15C:
			if (this.<cr>__0.GetAlpha() > 0f)
			{
				this.<cr>__0.SetAlpha(this.<cr>__0.GetAlpha() - 0.1f);
				this.$current = new WaitForSeconds(0.1f);
				if (!this.$disposing)
				{
					this.$PC = 4;
				}
				return true;
			}
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

		internal CanvasRenderer <cr>__0;

		internal string txt;

		internal Escape $this;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}
}
