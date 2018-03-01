using System;
using System.Runtime.CompilerServices;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Parameter)]
	internal sealed class AssertionConditionAttribute : Attribute
	{
		public AssertionConditionAttribute(AssertionConditionType conditionType)
		{
			this.ConditionType = conditionType;
		}

		public AssertionConditionType ConditionType
		{
			[CompilerGenerated]
			get
			{
				return this.<ConditionType>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<ConditionType>k__BackingField = value;
			}
		}

		[CompilerGenerated]
		private AssertionConditionType <ConditionType>k__BackingField;
	}
}
