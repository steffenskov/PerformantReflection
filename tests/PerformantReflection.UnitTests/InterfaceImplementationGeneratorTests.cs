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
	public void GenerateImplementationType_InterfaceHasInterfaceProperty_PropertyRemainsAnInterface()
	{
		// Act
		var implementationType = InterfaceImplementationGenerator.GenerateImplementationType<IWithInterfaceProperty>();

		// Assert
		Assert.Equal(typeof(IVatNumber), implementationType.GetProperty(nameof(IWithInterfaceProperty.Vat))!.PropertyType);
	}

	[Fact]
	public void GenerateImplementationType_WithExplicitImplementation_DoesNotGeneratePropertiesForExplicitImplementations()
	{
		// Act
		var implementationType = InterfaceImplementationGenerator.GenerateImplementationType<IWithExplicitImplementation>();

		// Assert
		Assert.Null(implementationType.GetProperty(nameof(IWithExplicitImplementation.Title)));
		Assert.NotNull(implementationType.GetProperty(nameof(IWithExplicitImplementation.Name)));
	}

	[Fact]
	public void GenerateImplementationType_WithExplicitImplementationOfImplementedInterface_DoesNotGeneratePropertiesForExplicitImplementations()
	{
		// Act
		var implementationType = InterfaceImplementationGenerator.GenerateImplementationType<IWithExplicitOtherImplementation>();

		// Assert
		Assert.Null(implementationType.GetProperty(nameof(IWithExplicitOtherImplementation.Name)));
		Assert.NotNull(implementationType.GetProperty(nameof(IWithExplicitOtherImplementation.Id)));
		Assert.NotNull(implementationType.GetProperty(nameof(IWithExplicitOtherImplementation.Title)));
	}

	[Fact]
	public void GenerateImplementationType_WithExplicitImplementationOfHiearchy_DoesNotGeneratePropertiesForExplicitImplementations()
	{
		// Act
		var implementationType = InterfaceImplementationGenerator.GenerateImplementationType<IWithExplicitImplementationHierarchy>();

		// Assert
		Assert.Null(implementationType.GetProperty(nameof(IWithExplicitImplementationHierarchy.Name)));
		Assert.Null(implementationType.GetProperty(nameof(IWithExplicitImplementationHierarchy.Id)));
		Assert.NotNull(implementationType.GetProperty(nameof(IWithExplicitImplementationHierarchy.Title)));
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
		Assert.Null(implementation.Name);
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
	public void CreateInstance_InterfaceContainsMethods_MethodsReturnsDefault()
	{
		// Act
		var implementation = InterfaceImplementationGenerator.CreateInstance<IInterfaceWithMethods>();

		// Assert - Properties
		Assert.Equal(default, implementation.Name);
		Assert.Equal(default, implementation.Count);
		Assert.Equal(default, implementation.IsActive);

		// Assert - Reference types
		Assert.Equal(default, implementation.GetString());
		Assert.Equal(default, implementation.GetObject());

		// Assert - Boolean
		Assert.Equal(default, implementation.GetBool());

		// Assert - Signed integers
		Assert.Equal(default, implementation.GetSByte());
		Assert.Equal(default, implementation.GetShort());
		Assert.Equal(default, implementation.GetInt());
		Assert.Equal(default, implementation.GetLong());

		// Assert - Unsigned integers
		Assert.Equal(default, implementation.GetByte());
		Assert.Equal(default, implementation.GetUShort());
		Assert.Equal(default, implementation.GetUInt());
		Assert.Equal(default, implementation.GetULong());

		// Assert - Floating point
		Assert.Equal(default, implementation.GetFloat());
		Assert.Equal(default, implementation.GetDouble());
		Assert.Equal(default, implementation.GetDecimal());

		// Assert - Character
		Assert.Equal(default, implementation.GetChar());

		// Assert - Pointer types
		Assert.Equal(default, implementation.GetIntPtr());
		Assert.Equal(default, implementation.GetUIntPtr());
		Assert.Equal(default, implementation.GetNInt());
		Assert.Equal(default, implementation.GetNUInt());

		// Assert - System value types
		Assert.Equal(default, implementation.GetDateTime());
		Assert.Equal(default, implementation.GetDateOnly());
		Assert.Equal(default, implementation.GetTimeOnly());
		Assert.Equal(default, implementation.GetTimeSpan());
		Assert.Equal(default, implementation.GetDateTimeOffset());
		Assert.Equal(default, implementation.GetGuid());

		// Assert - Nullable value types - primitives
		Assert.Equal(default, implementation.GetNullableBool());
		Assert.Equal(default, implementation.GetNullableSByte());
		Assert.Equal(default, implementation.GetNullableByte());
		Assert.Equal(default, implementation.GetNullableShort());
		Assert.Equal(default, implementation.GetNullableUShort());
		Assert.Equal(default, implementation.GetNullableInt());
		Assert.Equal(default, implementation.GetNullableUInt());
		Assert.Equal(default, implementation.GetNullableLong());
		Assert.Equal(default, implementation.GetNullableULong());
		Assert.Equal(default, implementation.GetNullableFloat());
		Assert.Equal(default, implementation.GetNullableDouble());
		Assert.Equal(default, implementation.GetNullableDecimal());
		Assert.Equal(default, implementation.GetNullableChar());

		// Assert - Nullable system types
		Assert.Equal(default, implementation.GetNullableDateTime());
		Assert.Equal(default, implementation.GetNullableDateOnly());
		Assert.Equal(default, implementation.GetNullableTimeOnly());
		Assert.Equal(default, implementation.GetNullableTimeSpan());
		Assert.Equal(default, implementation.GetNullableDateTimeOffset());
		Assert.Equal(default, implementation.GetNullableGuid());
		Assert.Equal(default, implementation.GetNullableIntPtr());
		Assert.Equal(default, implementation.GetNullableUIntPtr());
		Assert.Equal(default, implementation.GetNullableNInt());
		Assert.Equal(default, implementation.GetNullableNUInt());

		// Assert - Enums and nullable enums
		Assert.Equal(default, implementation.GetEnum());
		Assert.Equal(default, implementation.GetCustomEnum());
		Assert.Equal(default, implementation.GetNullableEnum());
		Assert.Equal(default, implementation.GetNullableCustomEnum());

		// Assert - Custom structs and nullable structs
		Assert.Equal(default, implementation.GetStruct());
		Assert.Equal(default, implementation.GetNullableStruct());

		// Assert - Generic value types
		Assert.Equal(default, implementation.GetKeyValuePair());
		Assert.Equal(default, implementation.GetNullableKeyValuePair());

		// Assert - Tuples
		Assert.Equal(default, implementation.GetValueTuple());
		Assert.Equal(default, implementation.GetNullableValueTuple());

		// Assert - Methods with parameters (should not throw)
		implementation.DoSomething();
		implementation.DoSomethingWithParams("test", 42);
		Assert.Equal(default, implementation.GetStringWithArguments("test", 42));
		Assert.Equal(default, implementation.ProcessData(new[] { 1, 2, 3 }, "format", DateTime.Now));

		// Assert - Generic methods (should not throw and return defaults)
		Assert.Equal(default, implementation.GenericMethod<int>());
		Assert.Equal(default, implementation.GenericMethod<string>());
		Assert.Equal(default, implementation.GenericMethod<DateTime>());
		Assert.Equal(default, implementation.GenericMethod<bool?>());

		// Assert - Implemented other interfaces
		Assert.Equal(default, implementation.GetAge());

		implementation.GenericVoidMethod(42);
		implementation.GenericVoidMethod<string>("test");

		implementation.GenericClassMethod(new CustomClass());
		implementation.GenericInterfaceMethod(new Customer());
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

public interface IOtherInterfaceWithMethods
{
	int GetAge();
}

public interface IInterfaceWithMethods : IOtherInterfaceWithMethods
{
	// Properties
	string Name { get; }
	int Count { get; }
	bool IsActive { get; }

	// Reference types
	string GetString();
	object GetObject();

	// Boolean
	bool GetBool();

	// Signed integers
	sbyte GetSByte();
	short GetShort();
	int GetInt();
	long GetLong();

	// Unsigned integers
	byte GetByte();
	ushort GetUShort();
	uint GetUInt();
	ulong GetULong();

	// Floating point
	float GetFloat();
	double GetDouble();
	decimal GetDecimal();

	// Character
	char GetChar();

	// Pointer types
	IntPtr GetIntPtr();
	UIntPtr GetUIntPtr();
	nint GetNInt();
	nuint GetNUInt();

	// System value types
	DateTime GetDateTime();
	DateOnly GetDateOnly();
	TimeOnly GetTimeOnly();
	TimeSpan GetTimeSpan();
	DateTimeOffset GetDateTimeOffset();
	Guid GetGuid();

	// Nullable value types - primitives
	bool? GetNullableBool();
	sbyte? GetNullableSByte();
	byte? GetNullableByte();
	short? GetNullableShort();
	ushort? GetNullableUShort();
	int? GetNullableInt();
	uint? GetNullableUInt();
	long? GetNullableLong();
	ulong? GetNullableULong();
	float? GetNullableFloat();
	double? GetNullableDouble();
	decimal? GetNullableDecimal();
	char? GetNullableChar();

	// Nullable system types
	DateTime? GetNullableDateTime();
	DateOnly? GetNullableDateOnly();
	TimeOnly? GetNullableTimeOnly();
	TimeSpan? GetNullableTimeSpan();
	DateTimeOffset? GetNullableDateTimeOffset();
	Guid? GetNullableGuid();
	IntPtr? GetNullableIntPtr();
	UIntPtr? GetNullableUIntPtr();
	nint? GetNullableNInt();
	nuint? GetNullableNUInt();

	// Enums and nullable enums
	DayOfWeek GetEnum();
	MyEnum GetCustomEnum();
	DayOfWeek? GetNullableEnum();
	MyEnum? GetNullableCustomEnum();

	// Custom structs and nullable structs
	MyStruct GetStruct();
	MyStruct? GetNullableStruct();

	// Generic value types
	KeyValuePair<int, string> GetKeyValuePair();
	KeyValuePair<int, string>? GetNullableKeyValuePair();

	// Tuples
	(int, string) GetValueTuple();
	(int, string)? GetNullableValueTuple();

	// Methods with parameters
	void DoSomething();
	void DoSomethingWithParams(string first, int second);
	string GetStringWithArguments(string first, int second);
	bool ProcessData(int[] data, string format, DateTime timestamp);
	T GenericMethod<T>();
	void GenericVoidMethod<T>(T value);

	void GenericClassMethod<T>(T value) where T : CustomClass;
	void GenericInterfaceMethod<T>(T value) where T : ICustomer;
}

public class CustomClass
{
}

public struct MyStruct
{
	public int Value;
	public string Name;
}

public enum MyEnum
{
	First,
	Second
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
	public Guid Id { get; set; }
}

public interface IWithInterfaceProperty
{
	IVatNumber Vat { get; }
}

public interface IVatNumber
{
}

public class WithInterfaceProperty : IWithInterfaceProperty
{
	public IVatNumber Vat { get; set; } = default!;
}

public class DKVatNumber : IVatNumber
{
	public int Value { get; set; }
}

public interface IWithExplicitImplementation
{
	string Name { get; }
	string Title => Name;
}

public interface IWithExplicitOtherImplementation : ICustomer
{
	string Title { get; }
	string ICustomer.Name => Title;
}

public interface IWithExplicitImplementationHierarchy : IWithExplicitOtherImplementation
{
	Guid ICustomer.Id => Guid.NewGuid();
}