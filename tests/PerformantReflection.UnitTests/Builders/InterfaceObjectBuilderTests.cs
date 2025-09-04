using PerformantReflection.Builders;

namespace PerformantReflection.UnitTests.Builders;

public class InterfaceObjectBuilderTests
{
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

	[Fact]
	public void Build_TypeImplementsNestedInterface_Works()
	{
		// Arrange
		var builder = new InterfaceObjectBuilder<ICustomerAggregate>();
		var id = Guid.NewGuid();

		// Act
		builder.With(instance => instance.Id, id)
			.With(instance => instance.Deleted, true);
		var result = builder.Build();

		// Assert
		Assert.Equal(id, result.Id);
		Assert.True(result.Deleted);
	}

	[Fact]
	public void Build_InterfaceWithDefaultMethodImplementation_RetainsDefaultImplementation()
	{
		// Arrange
		var builder = new InterfaceObjectBuilder<IWithDefaultMethodImplementation>()
			.With(e => e.GivenName, "Given")
			.With(e => e.SurName, "Sur");

		// Act
		var instance = builder.Build();

		// Assert
		Assert.Equal("Given Sur", instance.BaseMethod());
		Assert.Equal("Given Sur", instance.GetDisplayName());
		Assert.Null(instance.EmptyMethod()); // auto-generated empty implementation
	}

	public interface IFake : IBaseFake
	{
		string? Name { get; }
	}

	public interface IBaseFake
	{
		Guid Id { get; }
	}

	public interface IAggregate
	{
		bool Deleted { get; }
	}

	public interface IAggregate<out TAggregateId> : IAggregate
	{
		TAggregateId Id { get; }
	}

	public interface ICustomerAggregate : IAggregate<Guid>
	{
	}

	public interface IWithDefaultMethodImplementation : IBase
	{
		string GivenName { get; }
		string SurName { get; }

		string IBase.BaseMethod()
		{
			return GetDisplayName();
		}

		string GetDisplayName()
		{
			return $"{GivenName} {SurName}";
		}

		string EmptyMethod();
	}

	public interface IBase
	{
		string BaseMethod();
	}
}