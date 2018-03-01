using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Dissonance.Integrations.UNet_HLAPI.Demo
{
	public class HlapiPlayerController : NetworkBehaviour
	{
		public HlapiPlayerController()
		{
		}

		private void Update()
		{
			if (!base.isLocalPlayer)
			{
				return;
			}
			CharacterController component = base.GetComponent<CharacterController>();
			float yAngle = Input.GetAxis("Horizontal") * Time.deltaTime * 150f;
			float d = Input.GetAxis("Vertical") * 3f;
			base.transform.Rotate(0f, yAngle, 0f);
			Vector3 a = base.transform.TransformDirection(Vector3.forward);
			component.SimpleMove(a * d);
			if (base.transform.position.y < -3f)
			{
				base.transform.position = Vector3.zero;
				base.transform.rotation = Quaternion.identity;
			}
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
}
