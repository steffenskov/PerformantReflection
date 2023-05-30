namespace PerformantReflection.UnitTests;

public class InstanceBuilderTests
{
	private class Fake
	{
		public Guid Id { get; private set; }
		public string? Name { get; private set; }
	}
	
	[Fact]
	public void With_PropertyName_Valid()
	{
		// Arrange
		var builder = new InstanceBuilder<Fake>();
		var id = Guid.NewGuid();
		
		// Act
		builder.With(instance => instance.Id, id);
		var result = builder.Build();

		// Assert
		Assert.Equal(id, result.Id);
	}
	
	[Fact]
	public void With_AnonymousType_Throws()
	{
		// Arrange
		var builder = new InstanceBuilder<Fake>();

		// Act && Assert
		var ex = Assert.Throws<NotSupportedException>(() =>  builder.With(instance => new { instance.Name }, null));
		
		Assert.Equal("Unsupported node type: New", ex.Message);
	}
}

