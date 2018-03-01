using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.PostProcessing
{
	public abstract class PostProcessingComponent<T> : PostProcessingComponentBase where T : PostProcessingModel
	{
		protected PostProcessingComponent()
		{
		}

		public T model
		{
			[CompilerGenerated]
			get
			{
				return this.<model>k__BackingField;
			}
			[CompilerGenerated]
			internal set
			{
				this.<model>k__BackingField = value;
			}
		}

		public virtual void Init(PostProcessingContext pcontext, T pmodel)
		{
			this.context = pcontext;
			this.model = pmodel;
		}

		public override PostProcessingModel GetModel()
		{
			return this.model;
		}

		[CompilerGenerated]
		private T <model>k__BackingField;
	}
}
