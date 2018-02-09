using System;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Parameter)]
	internal sealed class AssertionConditionAttribute : Attribute
	{
		public AssertionConditionAttribute(AssertionConditionType conditionType)
		{
			this.ConditionType = conditionType;
		}

		public AssertionConditionType ConditionType { get; private set; }
	}
}
