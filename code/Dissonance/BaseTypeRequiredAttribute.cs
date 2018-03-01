using System;
using System.Runtime.CompilerServices;

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
		public Type BaseType
		{
			[CompilerGenerated]
			get
			{
				return this.<BaseType>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<BaseType>k__BackingField = value;
			}
		}

		[CompilerGenerated]
		private Type <BaseType>k__BackingField;
	}
}
