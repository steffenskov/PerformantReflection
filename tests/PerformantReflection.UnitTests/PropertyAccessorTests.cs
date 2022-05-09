using System.Security.AccessControl;
namespace PerformantReflection.UnitTests;

public class PropertyAccessorTests
{
	[Fact]
	public void GetValue_HasGetter_ValueIsFetched()
	{
		// Arrange
		var obj = new SimpleObject("Steffen", "Some password");
		var accessor = new ObjectAccessor(obj);

		// Act
		var value = accessor.Properties[nameof(obj.Username)].GetValue();

		// Assert
		Assert.Equal(obj.Username, value);
	}

	[Fact]
	public void SetValue_HasSetter_ValueIsSet()
	{
		// Arrange
		var obj = new SimpleObject("Steffen", "Some password");
		var accessor = new ObjectAccessor(obj);

		// Act
		accessor.Properties[nameof(obj.Username)].SetValue("New name");

		// Assert
		Assert.Equal("New name", obj.Username);
	}

	[Fact]
	public void SetValue_HasGetter_HasGetterIsTrue()
	{
		// Arrange
		var obj = new SimpleObject("Steffen", "Some password");
		var accessor = new ObjectAccessor(obj);

		// Assert
		Assert.True(accessor.Properties[nameof(obj.Username)].HasGetter);
	}

	[Fact]
	public void SetValue_HasSetter_HasSetterIsTrue()
	{
		// Arrange
		var obj = new SimpleObject("Steffen", "Some password");
		var accessor = new ObjectAccessor(obj);

		// Assert
		Assert.True(accessor.Properties[nameof(obj.Username)].HasSetter);
	}

	[Fact]
	public void SetValue_NoGetter_HasGetterIsFalse()
	{
		// Arrange
		var obj = new ObjectWithOnlyGetterAndSetter();
		var accessor = new ObjectAccessor(obj);

		// Assert
		Assert.False(accessor.Properties[nameof(obj.Setter)].HasGetter);
	}

	[Fact]
	public void SetValue_NoSetter_HasSetterIsFalse()
	{
		// Arrange
		var obj = new ObjectWithOnlyGetterAndSetter();
		var accessor = new ObjectAccessor(obj);

		// Assert
		Assert.False(accessor.Properties[nameof(obj.Getter)].HasSetter);
	}

	[Fact]
	public void GetValue_NoGetter_Throws()
	{
		// Arrange
		var obj = new ObjectWithOnlyGetterAndSetter();
		var accessor = new ObjectAccessor(obj);

		// Act && Assert
		Assert.Throws<InvalidOperationException>(() => accessor.Properties[nameof(obj.Setter)].GetValue());
	}

	[Fact]
	public void SetValue_NoSetter_Throws()
	{
		// Arrange
		var obj = new ObjectWithOnlyGetterAndSetter();
		var accessor = new ObjectAccessor(obj);

		// Act && Assert
		Assert.Throws<InvalidOperationException>(() => accessor.Properties[nameof(obj.Getter)].SetValue(42));
	}

	[Fact]
	public void SetValue_InvalidType_Throws()
	{
		// Arrange
		var obj = new SimpleObject("Steffen", "Some password");
		var accessor = new ObjectAccessor(obj);

		// Act && Assert
		Assert.Throws<InvalidCastException>(() => accessor.Properties[nameof(obj.Username)].SetValue(42));
	}

	[Fact]
	public void GetValue_AnonymousObject_CanGet()
	{
		// Arrange
		var obj = new
		{
			Username = "Steffen"
		};
		var accessor = new ObjectAccessor(obj);

		// Act
		var username = accessor.Properties[nameof(obj.Username)].GetValue();

		// Assert
		Assert.Equal(obj.Username, username);
	}

	[Fact]
	public void SetValue_AnonymousObject_HasNoSetter()
	{
		// Arrange
		var obj = new
		{
			Username = "Steffen"
		};
		var accessor = new ObjectAccessor(obj);

		// Assert
		Assert.False(accessor.Properties[nameof(obj.Username)].HasSetter);
	}
}
