using System;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Method)]
	internal sealed class NotifyPropertyChangedInvocatorAttribute : Attribute
	{
		public NotifyPropertyChangedInvocatorAttribute()
		{
		}

		public NotifyPropertyChangedInvocatorAttribute([NotNull] string parameterName)
		{
			this.ParameterName = parameterName;
		}

		[CanBeNull]
		public string ParameterName { get; private set; }
	}
}
