using System;

namespace Dissonance
{
	[BaseTypeRequired(typeof(Attribute))]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	internal sealed class BaseTypeRequiredAttribute : Attribute
	{
		public BaseTypeRequiredAttribute([NotNull] Type baseType)
		{
			this.BaseType = baseType;
		}

		[NotNull]
		public Type BaseType { get; private set; }
	}
}
