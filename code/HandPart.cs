using System;
using UnityEngine;

public class HandPart : MonoBehaviour
{
	public HandPart()
	{
	}

	private void Start()
	{
		if (this.anim == null)
		{
			this.anim = base.GetComponentsInParent<Animator>()[0];
		}
	}

	public void UpdateItem()
	{
		this.part.SetActive(this.anim.GetInteger("CurItem") == this.id);
	}

	public GameObject part;

	public int id;

	public Animator anim;
}
