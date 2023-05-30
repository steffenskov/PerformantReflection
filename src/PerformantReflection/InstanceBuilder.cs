namespace PerformantReflection
{
	public class InstanceBuilder<T>
		where T: notnull, new()
	{
		private T _result;
		public InstanceBuilder()
		{
			_result = TypeInstantiator.CreateInstance<T>();
		}
	}
}