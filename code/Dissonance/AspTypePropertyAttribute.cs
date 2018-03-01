using System;
using System.Runtime.CompilerServices;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Property)]
	internal sealed class AspTypePropertyAttribute : Attribute
	{
		public AspTypePropertyAttribute(bool createConstructorReferences)
		{
			this.CreateConstructorReferences = createConstructorReferences;
		}

		public bool CreateConstructorReferences
		{
			[CompilerGenerated]
			get
			{
				return this.<CreateConstructorReferences>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<CreateConstructorReferences>k__BackingField = value;
			}
		}

		[CompilerGenerated]
		private bool <CreateConstructorReferences>k__BackingField;
	}
}
