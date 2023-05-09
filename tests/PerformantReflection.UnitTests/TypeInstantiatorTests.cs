namespace PerformantReflection.UnitTests;

public class TypeInstantiatorTests
{
	[Fact]
	public void CreateInstance_WithConstructor_InstanceIsCreated()
	{
		// Arrange
		var type = typeof(TypeWithDefaultConstructor);

		// Act
		var instance = TypeInstantiator.CreateInstance(type);

		// Assert
		Assert.NotNull(instance);
		Assert.IsType<TypeWithDefaultConstructor>(instance);
	}

	[Fact]
	public void CreateInstance_UsingGenerics_InstanceIsCreated()
	{
		// Act
		var instance = TypeInstantiator.CreateInstance<TypeWithDefaultConstructor>();

		// Assert
		Assert.NotNull(instance);
		Assert.IsType<TypeWithDefaultConstructor>(instance);
	}

	[Fact]
	public void CreateInstance_Twice_InstancesAreNotSame()
	{
		// Arrange
		var type = typeof(TypeWithDefaultConstructor);

		// Act
		var instance = TypeInstantiator.CreateInstance(type);
		var instanceTwo = TypeInstantiator.CreateInstance(type);

		// Assert
		Assert.NotSame(instance, instanceTwo);
	}

	[Fact]
	public void CreateInstance_TypeHasNoDefaultConstructor_Throws()
	{
		// Arrange
		var type = typeof(SimpleObject);

		// Act && Assert
		Assert.Throws<InvalidOperationException>(() => TypeInstantiator.CreateInstance(type));
	}
}