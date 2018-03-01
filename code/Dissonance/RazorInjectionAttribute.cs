using System;
using System.Runtime.CompilerServices;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	internal sealed class RazorInjectionAttribute : Attribute
	{
		public RazorInjectionAttribute([NotNull] string type, [NotNull] string fieldName)
		{
			this.Type = type;
			this.FieldName = fieldName;
		}

		[NotNull]
		public string Type
		{
			[CompilerGenerated]
			get
			{
				return this.<Type>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Type>k__BackingField = value;
			}
		}

		[NotNull]
		public string FieldName
		{
			[CompilerGenerated]
			get
			{
				return this.<FieldName>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<FieldName>k__BackingField = value;
			}
		}

		[CompilerGenerated]
		private string <Type>k__BackingField;

		[CompilerGenerated]
		private string <FieldName>k__BackingField;
	}
}
