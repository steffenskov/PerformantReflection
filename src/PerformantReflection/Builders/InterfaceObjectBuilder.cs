namespace PerformantReflection.Builders;

public class InterfaceObjectBuilder<T> : BaseObjectBuilder<InterfaceObjectBuilder<T>, T>
	where T : class
{
	public InterfaceObjectBuilder() : base(CreateValues())
	{
	}

	private static (T, ObjectAccessor) CreateValues()
	{
		var resultType = InterfaceImplementationGenerator.GenerateImplementationType<T>();
		var result = TypeInstantiator.CreateInstance(resultType)!;
		return ((T)result, new ObjectAccessor(result));
	}
}