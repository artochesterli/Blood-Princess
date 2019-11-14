namespace BehaviorDesigner.Runtime.Tasks.Unity.SharedVariables
{
	[TaskCategory("Unity/SharedVariable")]
	[TaskDescription("Sets the SharedInt variable to the specified object. Returns Success.")]
	public class AddSharedInt : Action
	{
		[RequiredField]
		[Tooltip("Target Value")]
		public SharedInt Target;
		[Tooltip("The SharedInt to add")]
		public SharedInt adder;

		public override TaskStatus OnUpdate()
		{
			Target.Value += adder.Value;
			return TaskStatus.Success;
		}
	}
}