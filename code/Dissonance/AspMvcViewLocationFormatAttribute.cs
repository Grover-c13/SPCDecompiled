using System;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
	internal sealed class AspMvcViewLocationFormatAttribute : Attribute
	{
		public AspMvcViewLocationFormatAttribute([NotNull] string format)
		{
			this.Format = format;
		}

		[NotNull]
		public string Format { get; private set; }
	}
}
