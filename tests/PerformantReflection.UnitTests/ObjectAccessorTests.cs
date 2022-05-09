using System.Collections.Generic;
using Xunit;

namespace PerformantReflection.UnitTests;

public class ObjectAccessorTests
{
	[Fact]
	public void Properties_ContainsProperty_CanTryGet()
	{
		// Arrange
		var obj = new DummyObject("Steffen", "Some password");
		var accessor = new ObjectAccessor(obj);

		// Act
		var gotAccessor = accessor.Properties.TryGetValue(nameof(obj.Username), out _);

		// Assert
		Assert.True(gotAccessor);
	}

	[Fact]
	public void Properties_DoesNotContainProperty_CannotTryGet()
	{
		// Arrange
		var obj = new DummyObject("Steffen", "Some password");
		var accessor = new ObjectAccessor(obj);

		// Act
		var gotAccessor = accessor.Properties.TryGetValue("NonExistingProperty", out _);

		// Assert
		Assert.False(gotAccessor);
	}

	[Fact]
	public void Properties_ContainsProperty_CanGetThroughIndexer()
	{
		// Arrange
		var obj = new DummyObject("Steffen", "Some password");
		var accessor = new ObjectAccessor(obj);

		// Act
		var propertyAccessor = accessor.Properties[nameof(obj.Username)];

		// Assert
		Assert.NotNull(propertyAccessor);
	}

	[Fact]
	public void Properties_DoesNotContainProperty_ThrowsWhenUsingIndexer()
	{
		// Arrange
		var obj = new DummyObject("Steffen", "Some password");
		var accessor = new ObjectAccessor(obj);

		// Act && Assert
		Assert.Throws<KeyNotFoundException>(() => accessor.Properties["NonExistingProperty"]);
	}

	[Fact]
	public void Properties_UsingRecordAsTarget_ContainsProperties()
	{
		// Arrange
		var obj = new DummyRecord("Steffen", "Some password");
		var accessor = new ObjectAccessor(obj);

		// Act
		var propertyCount = accessor.Properties.Count;

		// Assert
		Assert.Equal(2, propertyCount);
	}
}