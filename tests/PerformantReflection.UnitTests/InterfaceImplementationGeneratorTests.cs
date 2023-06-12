using System.Text.Json;

namespace PerformantReflection.UnitTests;

public class InterfaceImplementationGeneratorTests
{
	[Fact]
	public void GenerateImplementationType_ValidInterfaceType_CanBeUsedForJsonDeserialization()
	{
		// Arrange
		var customer = new Customer
		{
			Id = Guid.NewGuid(),
			Name = "Some name"
		};
		var json = JsonSerializer.Serialize(customer); 
		
		// Act
		var implementationType = InterfaceImplementationGenerator.GenerateImplementationType<ICustomer>();
		var deserializedCustomer = (ICustomer)JsonSerializer.Deserialize(json, implementationType)!;

		// Assert
		Assert.NotNull(deserializedCustomer);
		Assert.Equal(customer.Id, deserializedCustomer!.Id);
		Assert.Equal(customer.Name, deserializedCustomer.Name);
	}
	
	[Fact]
	public void CreateInstance_ValidInterfaceType_InstanceIsCreated()
	{
		// Act
		var implementation = InterfaceImplementationGenerator.CreateInstance<IValidInterface>();
		implementation.Name = "Hello world";

		// Assert
		Assert.Equal("Hello world", implementation.Name);
		Assert.Equal(Guid.Empty, implementation.Id);
	}
	
	[Fact]
	public void CreateInstance_InterfaceImplementsOtherInterface_InstanceIsCreatedWithAllProperties()
	{
		// Act
		var implementation = InterfaceImplementationGenerator.CreateInstance<IExtendedInterface>();

		// Assert
		Assert.Null( implementation.Name);
		Assert.Equal(Guid.Empty, implementation.Id);
	}

	[Fact]
	public void CreateInstance_InternalInterfaceType_Throws()
	{
		// Act && Assert
		var ex = Assert.Throws<InvalidOperationException>(InterfaceImplementationGenerator.CreateInstance<IInternalInterface>);

		Assert.Equal("Type T must be public", ex.Message);
	}

	[Fact]
	public void CreateInstance_InterfaceContainsMethods_Throws()
	{
		// Act && Assert
		var ex = Assert.Throws<InvalidOperationException>(InterfaceImplementationGenerator.CreateInstance<IInterfaceWithMethods>);

		Assert.Equal("Cannot create an implementation for an interface that contains methods", ex.Message);
	}

	[Fact]
	public void CreateInstance_TypeIsAClass_Throws()
	{
		// Act && Assert
		var ex = Assert.Throws<InvalidOperationException>(InterfaceImplementationGenerator.CreateInstance<AClass>);

		Assert.Equal("Type T must be an interface", ex.Message);
	}

	[Fact]
	public void CreateInstance_TypeIsAStruct_Throws()
	{
		// Act && Assert
		var ex = Assert.Throws<InvalidOperationException>(() => InterfaceImplementationGenerator.CreateInstance<AStruct>());

		Assert.Equal("Type T must be an interface", ex.Message);
	}
}

public struct AStruct
{
	
}

public class AClass
{
	
}

public interface IValidInterface
{
	string Name { get; set; }
	Guid Id { get; }
}

public interface IBaseInterface
{
	Guid Id { get; }
}

public interface IExtendedInterface : IBaseInterface
{
	string Name { get; }
}

public interface IInterfaceWithMethods
{
	string Name { get; }
	
	string ToString();
}

internal interface IInternalInterface
{

}

public interface ICustomer
{
	string Name { get; }
	Guid Id { get; }
}

public class Customer : ICustomer
{
	public string Name { get; set; } = default!;
	public Guid Id { get; set;  }
}