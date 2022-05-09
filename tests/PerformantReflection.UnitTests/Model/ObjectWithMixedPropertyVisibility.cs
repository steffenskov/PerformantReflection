namespace PerformantReflection.UnitTests.Model;

public class ObjectWithMixedPropertyVisibility
{
	public string Username { get; private set; } = default!;

	public Guid Id { protected get; set; }

	internal int InternalProperty { get; set; }
}
