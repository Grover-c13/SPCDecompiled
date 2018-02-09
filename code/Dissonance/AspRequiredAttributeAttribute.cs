using System;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	internal sealed class AspRequiredAttributeAttribute : Attribute
	{
		public AspRequiredAttributeAttribute([NotNull] string attribute)
		{
			this.Attribute = attribute;
		}

		[NotNull]
		public string Attribute { get; private set; }
	}
}
