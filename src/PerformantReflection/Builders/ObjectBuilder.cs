namespace PerformantReflection.Builders
{
	public class ObjectBuilder<T> : BaseObjectBuilder<ObjectBuilder<T>, T>
	where T : notnull, new()
	{
		private static (T, ObjectAccessor) CreateValues()
		{
			var result = TypeInstantiator.CreateInstance<T>();
			return (result, new ObjectAccessor(result));
		}
		
		public ObjectBuilder() : base(CreateValues())
		{
		}
	}
}