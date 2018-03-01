using System;
using System.Runtime.CompilerServices;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.All)]
	internal sealed class LocalizationRequiredAttribute : Attribute
	{
		public LocalizationRequiredAttribute() : this(true)
		{
		}

		public LocalizationRequiredAttribute(bool required)
		{
			this.Required = required;
		}

		public bool Required
		{
			[CompilerGenerated]
			get
			{
				return this.<Required>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Required>k__BackingField = value;
			}
		}

		[CompilerGenerated]
		private bool <Required>k__BackingField;
	}
}
