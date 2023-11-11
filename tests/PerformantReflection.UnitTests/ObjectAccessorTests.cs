namespace PerformantReflection.UnitTests;

public class ObjectAccessorTests
{
	[Fact]
	public void Properties_ContainsProperty_CanTryGet()
	{
		// Arrange
		var obj = new SimpleObject("Steffen", "Some password");
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
		var obj = new SimpleObject("Steffen", "Some password");
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
		var obj = new SimpleObject("Steffen", "Some password");
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
		var obj = new SimpleObject("Steffen", "Some password");
		var accessor = new ObjectAccessor(obj);

		// Act && Assert
		Assert.Throws<KeyNotFoundException>(() => accessor.Properties["NonExistingProperty"]);
	}

	[Fact]
	public void Properties_UsingRecordAsTarget_ContainsProperties()
	{
		// Arrange
		var obj = new SimpleRecord("Steffen", "Some password");
		var accessor = new ObjectAccessor(obj);

		// Act
		var propertyCount = accessor.Properties.Count;

		// Assert
		Assert.Equal(2, propertyCount);
	}

	[Fact]
	public void Properties_HasPrivateProperties_NotIncludedByDefault()
	{
		// Arrange
		var obj = new ObjectWithPrivateProperties();
		var accessor = new ObjectAccessor(obj);

		// Act
		var propertyCount = accessor.Properties.Count;

		// Assert
		Assert.Equal(1, propertyCount);
	}

	[Fact]
	public void Properties_PrivatePropertiesIncluded_AllPropertiesFound()
	{
		// Arrange
		var obj = new ObjectWithPrivateProperties();
		var accessor = new ObjectAccessor(obj, true);

		// Act
		var propertyCount = accessor.Properties.Count;

		// Assert
		Assert.Equal(3, propertyCount);
	}

	[Fact]
	public void Properties_HasMixedVisibiltyProperties_IncludedAndHasBothGetterAndSetter()
	{
		// Arrange
		var obj = new ObjectWithMixedPropertyVisibility();
		var accessor = new ObjectAccessor(obj);

		// Act
		var properties = accessor.Properties.ToDictionary(prop => prop.Name);

		// Assert
		Assert.Equal(2, properties.Count);
		Assert.Contains(nameof(obj.Username), properties.Keys);
		Assert.Contains(nameof(obj.Id), properties.Keys);
		foreach (var property in properties.Values)
		{
			Assert.True(property.HasGetter);
			Assert.True(property.HasSetter);
		}
	}

	[Fact]
	public void Properties_HasInternalProperty_IncludedWhenPrivateIsIncluded()
	{
		// Arrange
		var obj = new ObjectWithMixedPropertyVisibility();
		var accessor = new ObjectAccessor(obj, true);

		// Act
		var properties = accessor.Properties.ToDictionary(prop => prop.Name);

		// Assert
		Assert.Contains(nameof(obj.InternalInitProperty), properties.Keys);
	}

	[Fact]
	public void Properties_KeyValuePairObject_CanRead()
	{
		// Arrange
		var obj = new KeyValuePair<int, string>(42, "Hello world");

		var accessor = new ObjectAccessor(obj);

		// Act
		var oldFashionedKey = obj.GetType().GetProperty("Key")!.GetValue(obj);
		var oldFashionedValue = obj.GetType().GetProperty("Value")!.GetValue(obj);
		var key = accessor.Properties[nameof(obj.Key)].GetValue();
		var value = accessor.Properties[nameof(obj.Value)].GetValue();

		// Assert
		Assert.Equal(obj.Key, oldFashionedKey);
		Assert.Equal(obj.Value, oldFashionedValue);
		Assert.Equal(obj.Key, key);
		Assert.Equal(obj.Value, value);
	}
}