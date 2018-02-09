using System;
using UnityEngine;

public class ConnInfoCloser : ConnInfoButton
{
	public override void UseButton()
	{
		this.objToClose.SetActive(false);
		base.UseButton();
	}

	public GameObject objToClose;
}
