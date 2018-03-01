using System;
using System.Runtime.CompilerServices;

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
		public string Attribute
		{
			[CompilerGenerated]
			get
			{
				return this.<Attribute>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Attribute>k__BackingField = value;
			}
		}

		[CompilerGenerated]
		private string <Attribute>k__BackingField;
	}
}
