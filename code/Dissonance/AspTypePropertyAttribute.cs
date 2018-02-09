using System;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Property)]
	internal sealed class AspTypePropertyAttribute : Attribute
	{
		public AspTypePropertyAttribute(bool createConstructorReferences)
		{
			this.CreateConstructorReferences = createConstructorReferences;
		}

		public bool CreateConstructorReferences { get; private set; }
	}
}
