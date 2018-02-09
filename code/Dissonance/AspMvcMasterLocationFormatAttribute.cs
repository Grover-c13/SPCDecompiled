using System;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
	internal sealed class AspMvcMasterLocationFormatAttribute : Attribute
	{
		public AspMvcMasterLocationFormatAttribute([NotNull] string format)
		{
			this.Format = format;
		}

		[NotNull]
		public string Format { get; private set; }
	}
}
