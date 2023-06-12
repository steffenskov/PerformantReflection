using PerformantReflection.Builders;

namespace PerformantReflection.UnitTests.Builders;

public class InterfaceObjectBuilderTests
{
	public interface IFake : IBaseFake
	{
		string? Name { get; }
	}

	public interface IBaseFake
	{
		Guid Id { get; }
	}

	[Fact]
	public void Build_InterfaceImplementsOtherInterface_ContainsAllProperties()
	{
		// Arrange
		var builder = new InterfaceObjectBuilder<IFake>();
		var id = Guid.NewGuid();

		// Act
		builder
			.With(instance => instance.Id, id)
			.With(instance => instance.Name, "Hello world");
		var result = builder.Build();

		// Assert
		Assert.Equal(id, result.Id);
		Assert.Equal("Hello world", result.Name);
	}
}