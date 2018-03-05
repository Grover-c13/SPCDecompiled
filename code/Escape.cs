using System;
using System.Collections;
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
}
