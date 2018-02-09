using System;

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

		public bool Required { get; private set; }
	}
}
