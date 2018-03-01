using System;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Method)]
	internal sealed class PureAttribute : Attribute
	{
		public PureAttribute()
		{
		}
	}
}
