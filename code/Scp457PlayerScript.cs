using System;
using UnityEngine.Networking;

public class Scp457PlayerScript : NetworkBehaviour
{
	public Scp457PlayerScript()
	{
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
}
