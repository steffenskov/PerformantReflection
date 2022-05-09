namespace PerformantReflection;
/// <summary>
/// ObjectAccessor let's you get and set all properties of an object.
/// </summary>
public class ObjectAccessor
{
	public PropertyCollection Properties { get; }

	/// <summary>
	/// Create a new ObjectAccesor which exposes the properties of the target given.
	/// </summary>
	/// <param name="target">Target object to access properties of.</param>
	/// <param name="includePrivateProperties">Whether to include private properties or not.</param>
	public ObjectAccessor(object target, bool includePrivateProperties = false)
	{
		ArgumentNullException.ThrowIfNull(target);
		var propertyDatas = TypePropertyCache.GetPropertiesOfType(target.GetType(), includePrivateProperties);
		var accessors = propertyDatas
								.Select(property => new PropertyAccessor(target, property));

		Properties = new PropertyCollection(accessors);
	}
}
