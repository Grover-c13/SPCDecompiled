using System;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Method)]
	internal sealed class MustUseReturnValueAttribute : Attribute
	{
		public MustUseReturnValueAttribute()
		{
		}

		public MustUseReturnValueAttribute([NotNull] string justification)
		{
			this.Justification = justification;
		}

		[CanBeNull]
		public string Justification { get; private set; }
	}
}
