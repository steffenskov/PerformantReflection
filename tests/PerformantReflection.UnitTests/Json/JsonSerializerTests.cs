using System.Text.Json;
using PerformantReflection.Json;

namespace PerformantReflection.UnitTests.Json;

public class JsonSerializerTests
{
	[Fact]
	public void Deserialized_WithoutPrivateSupport_EmptyProperties()
	{
		// Arrange
		var user = FakeUser.Create();
		var json = JsonSerializer.Serialize(user);

		// Act
		var deserialized = JsonSerializer.Deserialize<FakeUser>(json);

		// Assert
		Assert.NotNull(deserialized);
		Assert.Equal(default, deserialized.Id);
		Assert.Equal(default, deserialized.Age);
		Assert.Equal(default, deserialized.Username);
		Assert.Equal(default, deserialized.Password);
	}

	[Fact]
	public void Deserialized_WithPrivateSupport_PropertiesDeserialized()
	{
		// Arrange
		var user = FakeUser.Create();
		var json = JsonSerializer.Serialize(user);
		var options = JsonSerializerOptionsFactory.CreateOptionsWithPrivateDeserializationSupport();

		// Act
		var deserialized = JsonSerializer.Deserialize<FakeUser>(json, options);

		// Assert
		Assert.NotNull(deserialized);
		Assert.Equal(user, deserialized);
	}
}

file record FakeUser
{
	public Guid Id { get; private set; }
	public int Age { get; private init; }
	public string? Username { get; protected set; }
	public string? Password { get; protected init; }

	public static FakeUser Create()
	{
		return new FakeUser
		{
			Id = Guid.NewGuid(),
			Age = 42,
			Username = "Steffen",
			Password = "SuperSecret"
		};
	}
}