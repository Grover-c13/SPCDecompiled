using System;
using System.Runtime.CompilerServices;

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
		public string ParameterName
		{
			[CompilerGenerated]
			get
			{
				return this.<ParameterName>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<ParameterName>k__BackingField = value;
			}
		}

		[CompilerGenerated]
		private string <ParameterName>k__BackingField;
	}
}
