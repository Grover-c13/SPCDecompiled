using System;
using System.Runtime.CompilerServices;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	internal sealed class ContractAnnotationAttribute : Attribute
	{
		public ContractAnnotationAttribute([NotNull] string contract) : this(contract, false)
		{
		}

		public ContractAnnotationAttribute([NotNull] string contract, bool forceFullStates)
		{
			this.Contract = contract;
			this.ForceFullStates = forceFullStates;
		}

		[NotNull]
		public string Contract
		{
			[CompilerGenerated]
			get
			{
				return this.<Contract>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Contract>k__BackingField = value;
			}
		}

		public bool ForceFullStates
		{
			[CompilerGenerated]
			get
			{
				return this.<ForceFullStates>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<ForceFullStates>k__BackingField = value;
			}
		}

		[CompilerGenerated]
		private string <Contract>k__BackingField;

		[CompilerGenerated]
		private bool <ForceFullStates>k__BackingField;
	}
}
